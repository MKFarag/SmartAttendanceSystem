namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class PermissionService(ApplicationDbContext context) : IPermissionService
{
    private readonly ApplicationDbContext _context = context;

    #region StudentCheck

    public async Task<bool> StudentCheck(string? UserId, CancellationToken cancellationToken = default) =>
        UserId is not null && await _context.Users.AnyAsync(x => x.Id == UserId && x.IsStudent, cancellationToken);

    #endregion

    #region InstructorCheck

    public async Task<bool> InstructorCheck(string? UserId, CancellationToken cancellationToken = default) =>
        UserId is not null && await _context.Users.AnyAsync(x => x.Id == UserId && !x.IsStudent, cancellationToken);

    #endregion
}
