using Domain.Models;

namespace Application.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByUserNameAsync(string userName);

        Task<IEnumerable<User>> GetAllAsync();

        Task<int> AddAsync(User user);

        Task<bool> UpdateAsync(User user);

        Task<bool> DeleteAsync(int id);
    }
}
