namespace SmartAttendanceSystem.Infrastructure.Persistence.Repositories;

public class CourseRepository(ApplicationDbContext context) : GenericRepository<Course, int>(context), ICourseRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Course>> GetAllAsync(int departmentId, CancellationToken cancellationToken = default)
        => await (from c in _context.Courses
                  join dc in _context.DepartmentCourses
                  on c.Id equals dc.CourseId
                  where dc.DepartmentId == departmentId
                  select dc.Course)
                  .AsNoTracking()
                  .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(int departmentId, Expression<Func<Course, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default)
    {
        var query = (from c in _context.Courses
                     join dc in _context.DepartmentCourses
                     on c.Id equals dc.CourseId
                     where departmentId == dc.DepartmentId
                     select c)
                     .AsNoTracking()
                     .Select(selector);
        if (distinct)
            query = query.Distinct();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckDepartmentIdAsync(int courseId, int departmentId, CancellationToken cancellationToken = default)
        => await _context.DepartmentCourses
            .AsNoTracking()
            .AnyAsync(x => x.CourseId == courseId && x.DepartmentId == departmentId, cancellationToken);

    public async Task AddRelationAsync(int courseId, int departmentId, CancellationToken cancellationToken = default)
        => await _context.DepartmentCourses.AddAsync
            (new DepartmentCourses
            { CourseId = courseId, DepartmentId = departmentId },
            cancellationToken);

    public async Task DeleteRelationAsync(int courseId, CancellationToken cancellationToken = default)
        => await _context.DepartmentCourses
            .Where(x => x.CourseId == courseId)
            .ExecuteDeleteAsync(cancellationToken);
}
