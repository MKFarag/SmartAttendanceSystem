namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        bool AsNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<Result<StudentResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
}
