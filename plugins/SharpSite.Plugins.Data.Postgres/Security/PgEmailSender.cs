using AbsSecurity = SharpSite.Abstractions.Security;
using MsEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

namespace SharpSite.Plugins.Data.Postgres.Security;

public class PgEmailSender : AbsSecurity.IEmailSender
{
    private readonly MsEmailSender _emailSender;

    public PgEmailSender(MsEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendConfirmationLinkAsync(AbsSecurity.ISharpSiteUser user, string email, string confirmationLink)
    {
        return _emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetLinkAsync(AbsSecurity.ISharpSiteUser user, string email, string resetLink)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetCodeAsync(AbsSecurity.ISharpSiteUser user, string email, string resetCode)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Your password reset code is: {resetCode}");
    }

    public Task SendChangeEmailConfirmationLinkAsync(AbsSecurity.ISharpSiteUser user, string email, string confirmationLink)
    {
        return _emailSender.SendEmailAsync(email, "Confirm your email change",
            $"Please confirm your email change by <a href='{confirmationLink}'>clicking here</a>.");
    }
}
