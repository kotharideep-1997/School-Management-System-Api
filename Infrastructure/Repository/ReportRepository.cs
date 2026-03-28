using Application.DTO;
using Application.IRepository;
using Dapper;
using Infrastructure.Data;
using System.Data;

public class ReportRepository : IReportRepository
{
    private const int MaxPageSize = 100;

    private readonly IDbConnection _db;

    public ReportRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> GetActiveStudentCountAsync()
    {
        return await _db.ExecuteScalarAsync<int>(
            "sp_Report_CountActiveStudents",
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<int> GetActiveUserCountAsync()
    {
        return await _db.ExecuteScalarAsync<int>(
            "sp_Report_CountActiveUsers",
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<AttendanceRangeSummaryDto> GetAttendanceRangeSummaryAsync(DateTime fromDate, DateTime toDate)
    {
        var (from, toExclusive) = NormalizeRange(fromDate, toDate);

        var row = await _db.QueryFirstAsync<AttendanceRangeSummaryRow>(
            "sp_Report_AttendanceRangeSummary",
            new { p_From = from, p_ToExclusive = toExclusive },
            commandType: StoredProcedureHelper.Sp);

        return new AttendanceRangeSummaryDto
        {
            FromDate = fromDate.Date,
            ToDate = toDate.Date,
            PresentStudentCount = row.PresentStudentCount,
            AbsentStudentCount = row.AbsentStudentCount
        };
    }

    public Task<PagedResultDto<AttendanceReportStudentRowDto>> GetPresentStudentsForRangeAsync(
        AttendanceReportFilterDto filter)
    {
        return GetStudentsForStatusRangeAsync(filter, "present");
    }

    public Task<PagedResultDto<AttendanceReportStudentRowDto>> GetAbsentStudentsForRangeAsync(
        AttendanceReportFilterDto filter)
    {
        return GetStudentsForStatusRangeAsync(filter, "absent");
    }

    private async Task<PagedResultDto<AttendanceReportStudentRowDto>> GetStudentsForStatusRangeAsync(
        AttendanceReportFilterDto filter,
        string statusLower)
    {
        var (from, toExclusive) = NormalizeRange(filter.FromDate, filter.ToDate);
        var pageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);
        var pageNumber = Math.Max(1, filter.PageNumber);
        var offset = (pageNumber - 1) * pageSize;

        var nameTrim = string.IsNullOrWhiteSpace(filter.Name) ? null : filter.Name.Trim();

        var parameters = new
        {
            p_Status = statusLower,
            p_From = from,
            p_ToExclusive = toExclusive,
            p_Name = nameTrim,
            p_RollNo = filter.RollNo,
            p_Offset = offset,
            p_PageSize = pageSize
        };

        using var multi = await _db.QueryMultipleAsync(
            "sp_Report_AttendanceStudentsByStatusPaged",
            parameters,
            commandType: StoredProcedureHelper.Sp);

        var totalCount = await multi.ReadSingleAsync<int>();
        var rows = await multi.ReadAsync<AttendanceReportStudentRowDto>();

        return new PagedResultDto<AttendanceReportStudentRowDto>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = rows
        };
    }

    private static (DateTime From, DateTime ToExclusive) NormalizeRange(DateTime fromDate, DateTime toDate)
    {
        var from = fromDate.Date;
        var toExclusive = toDate.Date.AddDays(1);
        return (from, toExclusive);
    }

    private sealed class AttendanceRangeSummaryRow
    {
        public int PresentStudentCount { get; set; }

        public int AbsentStudentCount { get; set; }
    }
}
