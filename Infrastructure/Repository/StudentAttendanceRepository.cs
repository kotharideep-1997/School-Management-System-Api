using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

public class StudentAttendanceRepository : IStudentAttendanceRepository
{
    private readonly IDbConnection _db;

    public StudentAttendanceRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> AddAsync(StudentAttendance attendance)
    {
        var now = DateTime.UtcNow;
        attendance.Created_At = now;
        attendance.Updated_At = now;

        // Procedure uses 3 IN + OUT; DB sets Created_At/Updated_At (matches connector argument count).
        var p = StoredProcedureHelper.InsertOutNewId(dp =>
        {
            dp.Add("p_StudentId", attendance.StudentId, DbType.Int32);
            dp.Add("p_AttendanceDate", attendance.AttendanceDate, DbType.DateTime);
            dp.Add("p_Status", attendance.Status);
        });

        await _db.ExecuteAsync("sp_StudentAttendances_Insert", p, commandType: StoredProcedureHelper.Sp);
        return StoredProcedureHelper.ReadNewId(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM StudentAttendances WHERE Id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<StudentAttendance>> GetAllAsync()
    {
        return await _db.QueryAsync<StudentAttendance>(
            "sp_StudentAttendances_SelectAll",
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<StudentAttendance?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<StudentAttendance>(
            "sp_StudentAttendances_SelectById",
            new { p_Id = id },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateAsync(StudentAttendance attendance)
    {
        attendance.Updated_At = DateTime.UtcNow;

        const string sql = """
            UPDATE StudentAttendances
            SET StudentId = @StudentId,
                AttendanceDate = @AttendanceDate,
                Status = @Status,
                Created_At = @Created_At,
                Updated_At = @Updated_At
            WHERE Id = @Id
            """;

        return await _db.ExecuteAsync(sql, attendance) > 0;
    }
}
