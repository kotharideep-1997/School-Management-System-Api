using Application.DTO;
using Application.IRepository;
using Dapper;
using Infrastructure.Data;
using System.Data;

public class PermissionRepository : IPermissionRepository
{
    private readonly IDbConnection _db;

    public PermissionRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public Task SetUserPermissionAsync(int userId, int permissionMasterId, bool isActive)
    {
        return _db.ExecuteAsync(
            "sp_UserPermission_Set",
            new
            {
                p_UserId = userId,
                p_PermissionId = permissionMasterId,
                p_IsActive = isActive
            },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateUserPermissionActiveAsync(int userId, int permissionMasterId, bool isActive)
    {
        var p = new DynamicParameters();
        p.Add("p_UserId", userId);
        p.Add("p_PermissionId", permissionMasterId);
        p.Add("p_IsActive", isActive);
        p.Add("p_RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _db.ExecuteAsync("sp_UserPermission_UpdateActive", p, commandType: StoredProcedureHelper.Sp);
        return p.Get<int>("p_RowCount") > 0;
    }

    public Task<IEnumerable<UserPermissionRowDto>> GetUserPermissionsAsync()
    {
        return _db.QueryAsync<UserPermissionRowDto>(
            "sp_UserPermission_SelectAll",
            commandType: StoredProcedureHelper.Sp);
    }

    public Task<IEnumerable<UserPermissionRowDto>> GetUserPermissionsByUserIdAsync(int userId)
    {
        return _db.QueryAsync<UserPermissionRowDto>(
            "sp_UserPermission_SelectByUserId",
            new { p_UserId = userId },
            commandType: StoredProcedureHelper.Sp);
    }
}
