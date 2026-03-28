namespace Application.DTO;

public class UpdateUserRequest
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    /// <summary>If null or empty, the existing password is kept.</summary>
    public string? Password { get; set; }

    public bool IsActive { get; set; }
}
