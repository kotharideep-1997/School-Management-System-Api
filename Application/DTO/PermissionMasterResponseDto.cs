namespace Application.DTO;

/// <summary>Permission row for API responses (no userPermissions navigation).</summary>
public class PermissionMasterResponseDto
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}
