namespace SmartAttendanceSystem.Core.Services;

public interface IHardDelete
{
    Task<Result> HardDeleteAsync(int id, CancellationToken cancellationToken = default);
}
