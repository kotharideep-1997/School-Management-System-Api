using Domain.Models;

namespace Application.IRepository
{
    public interface IAttendanceReport
    {
        Task<IEnumerable<StudentAttendance>> GetAttendanceForStudentAsync(int studentId, DateTime from, DateTime to);

        Task<IEnumerable<StudentAttendance>> GetAttendanceForDateAsync(DateTime date);

        Task<IReadOnlyDictionary<string, int>> GetAttendanceSummaryForDateAsync(DateTime date);
    }
}
