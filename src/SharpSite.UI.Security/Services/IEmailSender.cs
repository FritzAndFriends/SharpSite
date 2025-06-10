namespace SharpSite.UI.Security.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendConfirmationLinkAsync(ISharpSiteUser user, string email, string confirmationLink);
    Task SendPasswordResetLinkAsync(ISharpSiteUser user, string email, string resetLink);
    Task SendPasswordResetCodeAsync(ISharpSiteUser user, string email, string resetCode);
}
