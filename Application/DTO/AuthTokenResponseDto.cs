namespace Application.DTO;

public class AuthTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public DateTime RefreshTokenExpiresAtUtc { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
}
