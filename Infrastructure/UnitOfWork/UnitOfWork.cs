using Application.IRepository;
using Application.IUnitOfWork;
using Infrastructure.Data;
using Infrastructure.Repository;
using System.Data;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly IDbConnection _db;
        private IDbTransaction _transaction;

        public IStudnetRepository Students { get; }

        public IUserRepository Users { get; }

        public IReportRepository Reports { get; }

        public IAttendanceReport AttendanceReports { get; }

        public IStudentAttendanceRepository StudentAttendances { get; }

        public IPermissionRepository Permissions { get; }

        public IPermissionMasterRepository PermissionMasters { get; }

        public IClassRepository Classes { get; }

        public UnitOfWork(DbConnectionFactory factory)
        {
            _db = factory.CreateConnection();

            Students = new StudentRepository(factory);
            Users = new UserRepository(factory);
            Reports = new ReportRepository(factory);
            AttendanceReports = new AttendanceReportRepository(factory);
            StudentAttendances = new StudentAttendanceRepository(factory);
            Permissions = new PermissionRepository(factory);
            PermissionMasters = new PermissionMasterRepository(factory);
            Classes = new ClassRepository(factory);
        }

        public async Task BeginTransactionAsync()
        {
            if (_db.State == ConnectionState.Closed)
                _db.Open();

            _transaction = _db.BeginTransaction();
        }

        public Task CommitAsync()
        {
            _transaction?.Commit();
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            _transaction?.Rollback();
            return Task.CompletedTask;
        }
    }
}
