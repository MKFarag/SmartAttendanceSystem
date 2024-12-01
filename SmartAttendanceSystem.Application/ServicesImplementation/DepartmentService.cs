namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class DepartmentService(ApplicationDbContext context) :
    GenericRepository<Department, DepartmentResponse, DepartmentRequest>(context),
    ICRUDService<Department, DepartmentResponse, DepartmentRequest>
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<Result<DepartmentResponse>> AddAsync(DepartmentRequest requestEntity, CancellationToken cancellationToken = default)
    {
        if (await _context.Departments.AnyAsync(x => x.Name == requestEntity.Name, cancellationToken))
            return Result.Failure<DepartmentResponse>(GlobalErrors.DuplicatedData("Name"));

        return await base.AddAsync(requestEntity, cancellationToken);
    }

    public async Task<Result> UpdateAsync(int Id, DepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var deptResult = await GetMainAsync(Id, cancellationToken);

        if (deptResult.IsFailure)
            return Result.Failure(GlobalErrors.IdNotFound);

        if (await _context.Departments.AnyAsync(x => x.Name == request.Name && x.Id != Id, cancellationToken))
            return Result.Failure<DepartmentResponse>(GlobalErrors.DuplicatedData("Name"));

        deptResult.Value.Name = request.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
