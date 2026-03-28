using Application.IRepository;

namespace Application.IUnitOfWork
{
    public interface IUnitOfWork
    {
        IStudnetRepository Students { get; }

        IUserRepository Users { get; }

        IReportRepository Reports { get; }

        IAttendanceReport AttendanceReports { get; }

        IStudentAttendanceRepository StudentAttendances { get; }

        IPermissionRepository Permissions { get; }

        IPermissionMasterRepository PermissionMasters { get; }

        IClassRepository Classes { get; }

        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}