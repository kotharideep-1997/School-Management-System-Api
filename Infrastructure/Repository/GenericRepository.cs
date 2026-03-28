
using Application.IRepository;
using Dapper;
using System.Data;

namespace Infrastructure.Repository
{

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IDbConnection _db;

        public GenericRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var table = typeof(T).Name + "s"; // Students, Users
            var sql = $"SELECT * FROM {table}";
            return await _db.QueryAsync<T>(sql);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var table = typeof(T).Name + "s";
            var sql = $"SELECT * FROM {table} WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(T entity)
        {
            // ⚠️ Generic insert is complex → usually handled per repo
            throw new NotImplementedException("Use specific repository for Insert");
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            throw new NotImplementedException("Use specific repository for Update");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var table = typeof(T).Name + "s";
            var sql = $"DELETE FROM {table} WHERE Id = @Id";
            return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}