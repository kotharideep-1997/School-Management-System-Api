using Domain.Models;

namespace Application.IRepository
{
    public interface IPermissionMasterRepository
    {
        Task<IEnumerable<PermissionMaster>> GetAllAsync();

        Task<PermissionMaster?> GetByIdAsync(int id);

        Task<int> AddAsync(PermissionMaster permission);

        Task<bool> UpdateAsync(PermissionMaster permission);

        Task<bool> DeleteAsync(int id);
    }
}
