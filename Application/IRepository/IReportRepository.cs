using Application.DTO;

namespace Application.IRepository
{
    public interface IReportRepository
    {
        Task<int> GetActiveStudentCountAsync();

        Task<int> GetActiveUserCountAsync();

        /// <summary>Distinct active students with ≥1 Present / Absent row in [from, to] (inclusive dates).</summary>
        Task<AttendanceRangeSummaryDto> GetAttendanceRangeSummaryAsync(DateTime fromDate, DateTime toDate);

        Task<PagedResultDto<AttendanceReportStudentRowDto>> GetPresentStudentsForRangeAsync(
            AttendanceReportFilterDto filter);

        Task<PagedResultDto<AttendanceReportStudentRowDto>> GetAbsentStudentsForRangeAsync(
            AttendanceReportFilterDto filter);
    }
}
