using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

public class PermissionMasterRepository : IPermissionMasterRepository
{
    private readonly IDbConnection _db;

    public PermissionMasterRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> AddAsync(PermissionMaster permission)
    {
        var p = StoredProcedureHelper.InsertOutNewId(dp =>
        {
            dp.Add("p_PermissionName", permission.PermissionName);
            dp.Add("p_CreatedAt", permission.CreatedAt);
            dp.Add("p_UpdatedAt", permission.UpdatedAt);
            dp.Add("p_IsActive", permission.IsActive);
        });

        await _db.ExecuteAsync("sp_PermissionMaster_Insert", p, commandType: StoredProcedureHelper.Sp);
        return StoredProcedureHelper.ReadNewId(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM PermissionMaster WHERE Id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<PermissionMaster>> GetAllAsync()
    {
        return await _db.QueryAsync<PermissionMaster>(
            "sp_PermissionMaster_SelectAll",
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<PermissionMaster?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<PermissionMaster>(
            "sp_PermissionMaster_SelectById",
            new { p_Id = id },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateAsync(PermissionMaster permission)
    {
        const string sql = """
            UPDATE PermissionMaster
            SET PermissionName = @PermissionName,
                CreatedAt = @CreatedAt,
                UpdatedAt = @UpdatedAt,
                IsActive = @IsActive
            WHERE Id = @Id
            """;

        return await _db.ExecuteAsync(sql, permission) > 0;
    }
}
