namespace SmartAttendanceSystem.Application.Interfaces;

public interface IDbContextManager
{
    DbSet<IdentityUserRole<string>> UserRoles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
