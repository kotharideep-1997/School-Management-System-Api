using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

public class AttendanceReportRepository : IAttendanceReport
{
    private readonly IDbConnection _db;

    public AttendanceReportRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<IEnumerable<StudentAttendance>> GetAttendanceForStudentAsync(int studentId, DateTime from, DateTime to)
    {
        var toExclusive = to.Date.AddDays(1);

        return await _db.QueryAsync<StudentAttendance>(
            "sp_StudentAttendances_SelectByStudentAndDateRange",
            new
            {
                p_StudentId = studentId,
                p_From = from.Date,
                p_ToExclusive = toExclusive
            },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<IEnumerable<StudentAttendance>> GetAttendanceForDateAsync(DateTime date)
    {
        return await _db.QueryAsync<StudentAttendance>(
            "sp_StudentAttendances_SelectByDate",
            new { p_Date = date.Date },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetAttendanceSummaryForDateAsync(DateTime date)
    {
        var rows = await _db.QueryAsync<(string Status, int Cnt)>(
            "sp_StudentAttendances_SelectSummaryByDate",
            new { p_Date = date.Date },
            commandType: StoredProcedureHelper.Sp);

        return rows.ToDictionary(r => r.Status, r => r.Cnt);
    }
}
