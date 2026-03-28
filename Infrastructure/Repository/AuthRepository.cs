using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTO;
using Dapper;
using Application.Helpers;
using Application.IRepository;
using Domain.Models;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repository;

public class AuthRepository : IAuthRepository
{
    private const string RefreshTokenType = "refresh";
    private readonly DbConnectionFactory _factory;
    private readonly JwtSettings _jwt;

    public AuthRepository(DbConnectionFactory factory, IOptions<JwtSettings> jwtOptions)
    {
        _factory = factory;
        _jwt = jwtOptions.Value;
        if (string.IsNullOrEmpty(_jwt.Secret) || Encoding.UTF8.GetByteCount(_jwt.Secret) < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 bytes (UTF-8) for HMAC-SHA256.");
    }

    public async Task<AuthTokenResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (request is null
            || string.IsNullOrWhiteSpace(request.UserName)
            || string.IsNullOrEmpty(request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        using var conn = _factory.CreateConnection();
        var user = await conn.QueryFirstOrDefaultAsync<User>(
            "sp_Users_SelectByUserName",
            new { p_UserName = request.UserName.Trim() },
            commandType: StoredProcedureHelper.Sp);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");
        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var perms = await GetPermissionNamesAsync(conn, user.Id);
        return BuildTokenResponse(user, perms);
    }

    public Task<AuthTokenResponseDto> GenerateTokensAsync(User user, IReadOnlyList<string> permissionNames)
    {
        ArgumentNullException.ThrowIfNull(user);
        permissionNames ??= Array.Empty<string>();

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is inactive.");

        return Task.FromResult(BuildTokenResponse(user, permissionNames));
    }

    public async Task<AuthTokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Invalid token.");

        int userId;
        try
        {
            userId = ValidateRefreshTokenAndGetUserId(refreshToken);
        }
        catch (SecurityTokenException)
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        using var conn = _factory.CreateConnection();
        var user = await conn.QueryFirstOrDefaultAsync<User>(
            "sp_Users_SelectById",
            new { p_Id = userId },
            commandType: StoredProcedureHelper.Sp);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid token.");

        var perms = await GetPermissionNamesAsync(conn, user.Id);
        return BuildTokenResponse(user, perms);
    }

    private static async Task<IReadOnlyList<string>> GetPermissionNamesAsync(IDbConnection conn, int userId)
    {
        var names = await conn.QueryAsync<string>(
            "sp_Auth_SelectPermissionNamesByUserId",
            new { p_UserId = userId },
            commandType: StoredProcedureHelper.Sp);

        return names.ToList();
    }

    private AuthTokenResponseDto BuildTokenResponse(User user, IReadOnlyList<string> permissionNames)
    {
        var accessExpires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);
        var refreshExpires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);
        var access = CreateAccessToken(user, permissionNames, accessExpires);
        var refresh = CreateRefreshToken(user.Id, refreshExpires);

        return new AuthTokenResponseDto
        {
            AccessToken = access,
            RefreshToken = refresh,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenExpiresAtUtc = refreshExpires,
            UserId = user.Id,
            UserName = user.UserName,
            Permissions = permissionNames
        };
    }

    private string CreateAccessToken(User user, IReadOnlyList<string> permissionNames, DateTime expiresUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };
        foreach (var p in permissionNames)
            claims.Add(new Claim("permission", p));

        return CreateJwt(claims, expiresUtc);
    }

    private string CreateRefreshToken(int userId, DateTime expiresUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString(CultureInfo.InvariantCulture)),
            new("token_type", RefreshTokenType),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return CreateJwt(claims, expiresUtc);
    }

    private string CreateJwt(IEnumerable<Claim> claims, DateTime expiresUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            expires: expiresUtc,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int ValidateRefreshTokenAndGetUserId(string refreshToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidIssuer = _jwt.Issuer,
            ValidAudience = _jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(refreshToken, parameters, out _);

        if (principal.FindFirst("token_type")?.Value != RefreshTokenType)
            throw new SecurityTokenException("Not a refresh token.");

        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, CultureInfo.InvariantCulture, out var userId))
            throw new SecurityTokenException("Invalid subject.");

        return userId;
    }
}
