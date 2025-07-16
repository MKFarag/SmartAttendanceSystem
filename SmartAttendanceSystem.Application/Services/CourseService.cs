namespace SmartAttendanceSystem.Application.Services;

public class CourseService(IUnitOfWork unitOfWork) : ICourseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _unitOfWork.Courses.GetAllProjectionAsync<CourseResponse>(cancellationToken);

    public async Task<Result<IEnumerable<CourseResponse>>> GetAllAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Departments.ExistsAsync(departmentId, cancellationToken))
            return Result.Failure<IEnumerable<CourseResponse>>(GlobalErrors.IdNotFound(nameof(Department)));

        var courses = await _unitOfWork.Courses.GetAllAsync(departmentId, cancellationToken);

        return Result.Success(courses.Adapt<IEnumerable<CourseResponse>>());
    }

    public async Task<Result<CourseResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetAsync(id, cancellationToken);

        return course is not null
            ? Result.Success(course.Adapt<CourseResponse>())
            : Result.Failure<CourseResponse>(GlobalErrors.IdNotFound(nameof(Course)));
    }

    public async Task<Result<CourseResponse>> AddAsync(CourseRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Departments.ExistsAsync(request.DepartmentId, cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.IdNotFound(nameof(Department)));

        if (await _unitOfWork.Courses.AnyAsync(c => c.Name == request.Name || c.Code == request.Code, cancellationToken))
            return Result.Failure<CourseResponse>(GlobalErrors.DuplicatedData("Name/Code", nameof(Course)));

        var course = await _unitOfWork.Courses.AddAsync(request.Adapt<Course>(), cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _unitOfWork.Courses.AddRelationAsync(course.Id, request.DepartmentId, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(course.Adapt<CourseResponse>());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Courses.GetAsync(id, cancellationToken) is not { } course)
            return Result.Failure(GlobalErrors.IdNotFound(nameof(Course)));

        await _unitOfWork.Courses.DeleteRelationAsync(id, cancellationToken);

        _unitOfWork.Courses.Delete(course);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int id, CourseRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Departments.ExistsAsync(request.DepartmentId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound(nameof(Department)));

        if (await _unitOfWork.Courses.GetAsync(id, cancellationToken) is not { } course)
            return Result.Failure(GlobalErrors.IdNotFound(nameof(Course)));

        if (await _unitOfWork.Courses.AnyAsync(c => (c.Name == request.Name && c.Id != id) || (c.Code == request.Code && c.Id != id), cancellationToken))
            return Result.Failure(GlobalErrors.DuplicatedData("Name/Code", nameof(Course)));

        course.Name = request.Name;
        course.Code = request.Code;
        course.Level = request.Level;

        if (!await _unitOfWork.Courses.CheckDepartmentIdAsync(id, request.DepartmentId, cancellationToken))
        {
            await _unitOfWork.Courses.DeleteRelationAsync(id, cancellationToken);
            await _unitOfWork.Courses.AddRelationAsync(id, request.DepartmentId, cancellationToken);
        }

        _unitOfWork.Courses.Update(course);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
