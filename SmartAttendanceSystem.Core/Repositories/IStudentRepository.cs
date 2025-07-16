namespace SmartAttendanceSystem.Core.Repositories;

public interface IStudentRepository : IGenericRepository<Student, int>, IPaginatedRepository<Student>
{
    Task<TProjection?> FindProjectionAsync<TProjection>(Expression<Func<Student, bool>> predicate, Expression<Func<Student, TProjection>> selector, bool singleQuery, CancellationToken cancellationToken = default);
}
