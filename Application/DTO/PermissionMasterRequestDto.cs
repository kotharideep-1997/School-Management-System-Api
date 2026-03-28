namespace Application.DTO;

public class CreatePermissionMasterRequestDto
{
    public string PermissionName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class UpdatePermissionMasterRequestDto
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
