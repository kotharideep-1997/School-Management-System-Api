using Domain.Models;

namespace Application.IRepository
{
    public interface IClassRepository
    {
        Task<IEnumerable<ClassMaster>> GetAllAsync();

        Task<ClassMaster?> GetByIdAsync(int id);

        Task<int> AddAsync(ClassMaster classMaster);

        Task<bool> UpdateAsync(ClassMaster classMaster);

        Task<bool> DeleteAsync(int id);
    }
}
