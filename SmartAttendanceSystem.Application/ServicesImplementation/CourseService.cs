namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class CourseService(ApplicationDbContext context) : GenericRepository<Course, CourseResponse, CourseRequest>(context),
    ICourseService
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<Result<CourseResponse>> AddAsync(CourseRequest request, CancellationToken cancellationToken = default)
    {
        if (await AnyAsync(x => (x.Name == request.Name) || (x.Code == request.Code), cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.DuplicatedData("Name/Code"));

        return await base.AddAsync(request, cancellationToken);
    }
    
    public async Task<Result> UpdateAsync(int Id, CourseRequest request, CancellationToken cancellationToken = default)
    {
        var courseResult = await GetMainAsync(Id, cancellationToken);

        if (courseResult.IsFailure)
            return Result.Failure(GlobalErrors.IdNotFound("Courses"));

        if (await AnyAsync(x => (x.Name == request.Name && x.Id != Id) || (x.Code == request.Code && x.Id != Id), cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.DuplicatedData("Name/Code"));

        courseResult.Value.Name = request.Name;
        courseResult.Value.Code = request.Code;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
