namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class CourseService(ApplicationDbContext context) : GenericRepository<Course, CourseResponse, CourseRequest>(context),
    ICourseService<Course, CourseResponse, CourseRequest>
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<Result<CourseResponse>> AddAsync(CourseRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Courses.AnyAsync(x => (x.Name == request.Name) || (x.Code == request.Code), cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.DuplicatedData);

        return await base.AddAsync(request, cancellationToken);
    }
    
    public async Task<Result> UpdateAsync(int Id, CourseRequest request, CancellationToken cancellationToken = default)
    {
        var courseResult = await GetMainAsync(Id, cancellationToken);

        if (courseResult.IsFailure)
            return Result.Failure(GlobalErrors.IdNotFound);

        if (await _context.Courses.AnyAsync(x => (x.Name == request.Name && x.Id != Id) || (x.Code == request.Code && x.Id != Id), cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.DuplicatedData);

        courseResult.Value.Name = request.Name;
        courseResult.Value.Code = request.Code;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    //TODO: After Create a studentTable should delete a try catch block and add custom check on the relation
    public async Task<Result> HardDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await GetMainAsync(id, cancellationToken);

        if (course.IsFailure)
            return Result.Failure(GlobalErrors.IdNotFound);

        try
        {
            _context.Courses.Remove(course.Value);
        }
        catch
        {
            return Result.Failure(GlobalErrors.RelationError);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
