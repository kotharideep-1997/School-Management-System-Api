using Application.DTO;
using Domain.Models;

namespace Application.IRepository
{
    /// <summary>Authentication: login, access/refresh token issuance, and refresh flow.</summary>
    public interface IAuthRepository
    {
        Task<AuthTokenResponseDto> LoginAsync(LoginRequestDto request);

        Task<AuthTokenResponseDto> GenerateTokensAsync(User user, IReadOnlyList<string> permissionNames);

        Task<AuthTokenResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
