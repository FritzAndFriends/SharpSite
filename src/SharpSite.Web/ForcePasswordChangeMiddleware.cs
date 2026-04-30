namespace SharpSite.Web;

/// <summary>
/// Middleware that redirects authenticated users with the MustChangePassword claim
/// to the forced password change page. This prevents users with default seed
/// credentials from accessing the application until they set a new password.
/// </summary>
public class ForcePasswordChangeMiddleware(RequestDelegate next)
{
	private static readonly string[] AllowedPathPrefixes =
	[
		"/Account/ForceChangePassword",
		"/Account/Logout",
		"/_blazor",
		"/_framework",
		"/_content"
	];

	public async Task Invoke(HttpContext context)
	{
		if (context.User.Identity?.IsAuthenticated == true
			&& context.User.HasClaim("MustChangePassword", "true"))
		{
			var path = context.Request.Path.Value ?? string.Empty;

			bool isAllowed = Array.Exists(AllowedPathPrefixes,
				prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				|| path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
				|| path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
				|| path.Contains("/img/", StringComparison.OrdinalIgnoreCase);

			if (!isAllowed)
			{
				context.Response.Redirect("/Account/ForceChangePassword");
				return;
			}
		}

		await next(context);
	}
}
