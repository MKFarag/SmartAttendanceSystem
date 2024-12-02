namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class StudentService(ApplicationDbContext context) : IStudentService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        bool AsNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Student> query = _context.Students;

        if (AsNoTracking)
            query = query.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        return await query
            .ProjectToType<StudentResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<StudentResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Students.FindAsync([id], cancellationToken: cancellationToken);

        return response is not null
            ? Result.Success(response.Adapt<StudentResponse>())
            : Result.Failure<StudentResponse>(GlobalErrors.IdNotFound);
    }

    public async Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
    {
        predicate ??= x => true;

        return await _context.Students.AnyAsync(predicate, cancellationToken);
    }
}
