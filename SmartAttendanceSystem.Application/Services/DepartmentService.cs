namespace SmartAttendanceSystem.Application.Services;

public class DepartmentService(IUnitOfWork unitOfWork) : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _unitOfWork.Departments.GetAllProjectionAsync<DepartmentResponse>(cancellationToken);

    public async Task<Result<DepartmentResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetAsync(id, cancellationToken);

        return department is not null
            ? Result.Success(department.Adapt<DepartmentResponse>())
            : Result.Failure<DepartmentResponse>(GlobalErrors.IdNotFound(nameof(Department)));
    }

    public async Task<Result<DepartmentResponse>> AddAsync(DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.AnyAsync(d => d.Name == request.Name, cancellationToken))
            return Result.Failure<DepartmentResponse>(GlobalErrors.DuplicatedData("Name", nameof(Department)));

        var department = await _unitOfWork.Departments.AddAsync(request.Adapt<Department>(), cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(department.Adapt<DepartmentResponse>());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.GetAsync(id, cancellationToken) is not { } department)
            return Result.Failure(GlobalErrors.IdNotFound(nameof(Department)));

        if (await _unitOfWork.Departments.RelationExistsAsync(id, cancellationToken))
            return Result.Failure(GlobalErrors.RelationError);

        _unitOfWork.Departments.Delete(department);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(int id, DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.GetAsync(id, cancellationToken) is not { } department)
            return Result.Failure(GlobalErrors.IdNotFound(nameof(Department)));

        if (await _unitOfWork.Departments.AnyAsync(d => d.Name == request.Name, cancellationToken))
            return Result.Failure(GlobalErrors.DuplicatedData("Name", nameof(Department)));

        department.Name = request.Name;

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
