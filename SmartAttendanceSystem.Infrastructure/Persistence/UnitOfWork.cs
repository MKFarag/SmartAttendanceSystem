namespace SmartAttendanceSystem.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    public IGenericRepository<Attendance, int> Attendances { get; private set; }
    public ICourseRepository Courses { get; private set; }
    public IDepartmentRepository Departments { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IStudentRepository Students { get; private set; }
    public IUserRepository Users { get; private set; }

    public UnitOfWork(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;

        Attendances = new GenericRepository<Attendance, int>(_context);
        Courses = new CourseRepository(_context);
        Departments = new DepartmentRepository(_context);
        Roles = new RoleRepository(_context, roleManager);
        Students = new StudentRepository(_context);
        Users = new UserRepository(_context, userManager);
    }

    public int Complete()
        => _context.SaveChanges();

    public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                _context?.Dispose();

            _disposed = true;
        }
    }
}
