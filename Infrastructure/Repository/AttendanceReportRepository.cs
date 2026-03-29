using Application.DTO;
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

    public async Task<AttendanceDaySummaryDto> GetAttendanceSummaryForDateAsync(DateTime date)
    {
        var d = date.Date;
        var dExclusive = d.AddDays(1);

        var rows = (await _db.QueryAsync<AttendanceGridRowDto>(
            "sp_AttendanceReport_SelectDayRows",
            new { p_From = d, p_ToExclusive = dExclusive },
            commandType: StoredProcedureHelper.Sp)).ToList();

        var present = new List<AttendanceGridRowDto>();
        var absent = new List<AttendanceGridRowDto>();
        var other = new List<AttendanceGridRowDto>();

        foreach (var row in rows)
        {
            var status = row.Attendance?.Trim() ?? string.Empty;
            if (string.Equals(status, "Present", StringComparison.OrdinalIgnoreCase))
                present.Add(row);
            else if (string.Equals(status, "Absent", StringComparison.OrdinalIgnoreCase))
                absent.Add(row);
            else
                other.Add(row);
        }

        return new AttendanceDaySummaryDto
        {
            Date = d,
            PresentCount = present.Count,
            AbsentCount = absent.Count,
            PresentData = present,
            AbsentData = absent,
            OtherData = other
        };
    }

    public async Task<AttendanceGridResultDto> GetAttendanceGridAsync(AttendanceGridFilterDto filter)
    {
        var attFrom = filter.FromDate.Date;
        var attToExclusive = filter.ToDate.Date.AddDays(1);

        var nameTrim = string.IsNullOrWhiteSpace(filter.StudentName) ? null : filter.StudentName.Trim();
        var nameLike = nameTrim is null ? null : $"%{nameTrim}%";

        var rows = (await _db.QueryAsync<AttendanceGridRowDto>(
            "sp_AttendanceReport_SelectGridFiltered",
            new
            {
                p_AttFrom = attFrom,
                p_AttToExclusive = attToExclusive,
                p_NameLike = nameLike,
                p_RollNo = filter.RollNo
            },
            commandType: StoredProcedureHelper.Sp)).ToList();

        var present = new List<AttendanceGridRowDto>();
        var absent = new List<AttendanceGridRowDto>();
        var other = new List<AttendanceGridRowDto>();

        foreach (var row in rows)
        {
            var status = row.Attendance?.Trim() ?? string.Empty;
            if (string.Equals(status, "Present", StringComparison.OrdinalIgnoreCase))
                present.Add(row);
            else if (string.Equals(status, "Absent", StringComparison.OrdinalIgnoreCase))
                absent.Add(row);
            else
                other.Add(row);
        }

        return new AttendanceGridResultDto
        {
            FromDate = filter.FromDate.Date,
            ToDate = filter.ToDate.Date,
            PresentCount = present.Count,
            AbsentCount = absent.Count,
            PresentData = present,
            AbsentData = absent,
            OtherData = other
        };
    }
}
