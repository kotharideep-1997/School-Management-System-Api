using Application.DTO;
using Domain.Models;

namespace Application.IRepository
{
    public interface IStudnetRepository
    {
        Task<int> AddAsync(Student student);

        Task<bool> UpdateAsync(Student student);

        Task<bool> DeleteAsync(int id);

        Task<Student?> GetByIdAsync(int id);

        Task<int> CountActiveStudentsInClassAsync(int classId, int? excludeStudentId = null);

        Task<bool> ExistsRollNoInClassAsync(int classId, int rollNo, int? excludeStudentId = null);

        Task<IEnumerable<Student>> GetAllAsync();

        Task<IEnumerable<Student>> SearchAsync(StudentSearchRequestDto request);

        Task<PagedResultDto<Student>> GetPagedAsync(PagedRequestDto request);
    }
}
