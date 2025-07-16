namespace SmartAttendanceSystem.Application.Interfaces.Infrastructure;

public interface IEmailTemplateService
{
    void SendConfirmationLink(ApplicationUser user, string code, int expiryTimeInHours = 24);
    void SendConfirmationCode(ApplicationUser user, string code, int expiryTimeInMinutes);
    void SendResetPassword(ApplicationUser user, string code, int expiryTimeInHours = 24);
    void SendChangeEmailNotification(ApplicationUser user, string oldEmail, DateTime changeDate);
}
