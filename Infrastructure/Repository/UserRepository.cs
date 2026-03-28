using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> AddAsync(User user)
    {
        var p = StoredProcedureHelper.InsertOutNewId(dp =>
        {
            dp.Add("p_UserName", user.UserName);
            dp.Add("p_PasswordHash", user.PasswordHash);
            dp.Add("p_PasswordSalt", user.PasswordSalt);
            dp.Add("p_CreatedDate", user.CreatedDate);
            dp.Add("p_IsActive", user.IsActive);
        });

        await _db.ExecuteAsync("sp_Users_Insert", p, commandType: StoredProcedureHelper.Sp);
        return StoredProcedureHelper.ReadNewId(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Users WHERE Id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.QueryAsync<User>("sp_Users_SelectAll", commandType: StoredProcedureHelper.Sp);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<User>(
            "sp_Users_SelectById",
            new { p_Id = id },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _db.QueryFirstOrDefaultAsync<User>(
            "sp_Users_SelectByUserName",
            new { p_UserName = userName },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = """
            UPDATE Users
            SET UserName = @UserName,
                PasswordHash = @PasswordHash,
                PasswordSalt = @PasswordSalt,
                CreatedDate = @CreatedDate,
                IsActive = @IsActive
            WHERE Id = @Id
            """;

        return await _db.ExecuteAsync(sql, user) > 0;
    }
}
