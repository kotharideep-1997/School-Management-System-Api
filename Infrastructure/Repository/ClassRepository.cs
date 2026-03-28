using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

namespace Infrastructure.Repository;

public class ClassRepository : IClassRepository
{
    private readonly IDbConnection _db;

    public ClassRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> AddAsync(ClassMaster classMaster)
    {
        var p = StoredProcedureHelper.InsertOutNewId(dp =>
        {
            dp.Add("p_Class", classMaster.Class);
            dp.Add("p_Strength", classMaster.Strength);
            dp.Add("p_CreatedAt", classMaster.CreatedAt);
            dp.Add("p_UpdatedAt", classMaster.UpdatedAt);
            dp.Add("p_IsActive", classMaster.IsActive);
        });

        await _db.ExecuteAsync("sp_ClassMaster_Insert", p, commandType: StoredProcedureHelper.Sp);
        return StoredProcedureHelper.ReadNewId(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM ClassMaster WHERE Id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<ClassMaster>> GetAllAsync()
    {
        return await _db.QueryAsync<ClassMaster>(
            "sp_ClassMaster_SelectAll",
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<ClassMaster?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<ClassMaster>(
            "sp_ClassMaster_SelectById",
            new { p_Id = id },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateAsync(ClassMaster classMaster)
    {
        const string sql = """
            UPDATE ClassMaster
            SET `Class` = @Class,
                Strength = @Strength,
                CreatedAt = @CreatedAt,
                UpdatedAt = @UpdatedAt,
                IsActive = @IsActive
            WHERE Id = @Id
            """;

        return await _db.ExecuteAsync(sql, classMaster) > 0;
    }
}
