namespace SharpSite.UI.Security.Services;

public class NoOpEmailSender : IEmailSender
{
    public Task SendConfirmationLinkAsync(ISharpSiteUser user, string email, string confirmationLink)
    {
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ISharpSiteUser user, string email, string resetCode)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ISharpSiteUser user, string email, string resetLink)
    {
        return Task.CompletedTask;
    }

    public Task SendChangeEmailConfirmationLinkAsync(ISharpSiteUser user, string email, string confirmationLink)
    {
        return Task.CompletedTask;
    }
}
