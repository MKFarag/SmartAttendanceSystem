namespace SmartAttendanceSystem.Core.Services;

public interface ISoftDelete
{
    Task<Result> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
