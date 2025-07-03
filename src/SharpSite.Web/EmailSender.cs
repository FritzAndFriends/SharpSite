using Microsoft.AspNetCore.Identity.UI.Services;
using SSS = SharpSite.Abstractions.Security;

namespace SharpSite.Web;

public class EmailSender : SSS.IEmailSender
{
    private readonly IEmailSender _emailSender;

    public EmailSender(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendConfirmationLinkAsync(SSS.ISharpSiteUser user, string email, string confirmationLink)
    {
        return _emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetLinkAsync(SSS.ISharpSiteUser user, string email, string resetLink)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetCodeAsync(SSS.ISharpSiteUser user, string email, string resetCode)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Your password reset code is: {resetCode}");
    }
}
