namespace Application.DTO;

/// <summary>UserPermission row with permission master name (for API / lists).</summary>
public class UserPermissionRowDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class SetUserPermissionRequestDto
{
    public int UserId { get; set; }

    public int PermissionMasterId { get; set; }

    public bool IsActive { get; set; }
}

public class UpdateUserPermissionActiveRequestDto
{
    public int UserId { get; set; }

    public int PermissionMasterId { get; set; }

    public bool IsActive { get; set; }
}
