namespace SmartAttendanceSystem.Application.Interfaces.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Attendance, int> Attendances { get; }
    ICourseRepository Courses { get; }
    IDepartmentRepository Departments { get; }
    IRoleRepository Roles { get; }
    IStudentRepository Students { get; }
    IUserRepository Users { get; }

    /// <summary>Save changes to the database</summary>
    /// <returns>The number of state entries written to the database</returns>
    int Complete();

    /// <summary>Save changes to the database</summary>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
