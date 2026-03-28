using Domain.Models;

namespace Application.IRepository
{
    public interface IStudentAttendanceRepository
    {
        Task<IEnumerable<StudentAttendance>> GetAllAsync();

        Task<StudentAttendance?> GetByIdAsync(int id);

        Task<int> AddAsync(StudentAttendance attendance);

        Task<bool> UpdateAsync(StudentAttendance attendance);

        Task<bool> DeleteAsync(int id);
    }
}
