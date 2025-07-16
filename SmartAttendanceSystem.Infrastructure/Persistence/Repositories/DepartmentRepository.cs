namespace SmartAttendanceSystem.Infrastructure.Persistence.Repositories;

public class DepartmentRepository(ApplicationDbContext context) : GenericRepository<Department, int>(context), IDepartmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> RelationExistsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.DepartmentCourses
            .AsNoTracking()
            .AnyAsync(x => x.DepartmentId == id, cancellationToken);
}
