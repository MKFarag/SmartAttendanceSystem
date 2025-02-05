namespace SmartAttendanceSystem.Application.Interfaces;

public interface IDbContextManager
{
    DbSet<ApplicationRole> Roles { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<IdentityUserRole<string>> UserRoles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
