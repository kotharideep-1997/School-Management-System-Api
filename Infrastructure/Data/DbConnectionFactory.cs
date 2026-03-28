using Domain.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace Infrastructure.Data
{
    public class DbConnectionFactory 
    {
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
        }
        //public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        //public DbSet<Student> Students { get; set; }
        ////public DbSet<Class> Classes { get; set; }

    }
}
