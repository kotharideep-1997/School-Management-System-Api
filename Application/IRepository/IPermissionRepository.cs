using Application.DTO;

namespace Application.IRepository;

/// <summary>User–permission assignment (UserPermission table).</summary>
public interface IPermissionRepository
{
    /// <summary>Insert new link or update <c>IsActive</c> + <c>UpdatedAt</c> if (user, permission) already exists.</summary>
    Task SetUserPermissionAsync(int userId, int permissionMasterId, bool isActive);

    /// <summary>Update <c>IsActive</c> only for an existing row. Returns false if no row matched.</summary>
    Task<bool> UpdateUserPermissionActiveAsync(int userId, int permissionMasterId, bool isActive);

    /// <summary>All assignments joined to PermissionMaster.</summary>
    Task<IEnumerable<UserPermissionRowDto>> GetUserPermissionsAsync();

    /// <summary>Assignments for one user (joined to PermissionMaster).</summary>
    Task<IEnumerable<UserPermissionRowDto>> GetUserPermissionsByUserIdAsync(int userId);
}
