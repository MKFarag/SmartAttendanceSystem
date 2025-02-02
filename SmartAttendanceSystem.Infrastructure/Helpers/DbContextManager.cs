namespace SmartAttendanceSystem.Infrastructure.Helpers;

public class DbContextManager

    #region Initialize Fields

    (ApplicationDbContext context) : IDbContextManager
{
    private readonly ApplicationDbContext _context = context;

    #endregion


}
