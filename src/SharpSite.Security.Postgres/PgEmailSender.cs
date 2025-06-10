using SharpSite.Abstractions.Security;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SharpSite.Security.Postgres;

/// <summary>
/// Implementation of IEmailSender for PostgreSQL using ASP.NET Core Identity
/// </summary>
public class PgEmailSender : IEmailSender<ISharpSiteUser>
{
    private readonly IEmailSender _emailSender;

    public PgEmailSender(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendConfirmationLinkAsync(ISharpSiteUser user, string email, string confirmationLink)
    {
        return _emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetLinkAsync(ISharpSiteUser user, string email, string resetLink)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetCodeAsync(ISharpSiteUser user, string email, string resetCode)
    {
        return _emailSender.SendEmailAsync(email, "Reset Password",
            $"Your password reset code is: {resetCode}");
    }
}
