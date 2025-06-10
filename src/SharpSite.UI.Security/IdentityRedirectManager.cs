using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace SharpSite.UI.Security;

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        navigationManager.NavigateTo(uri);
    }

    [DoesNotReturn]
    public void RedirectTo(string page, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = NavigationManager.ToAbsoluteUri(page).GetLeftPart(UriPartial.Path);
        navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters));
    }

    [DoesNotReturn]
    public void RedirectToCurrentPage() => RedirectTo(NavigationManager.Uri);

    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
    {
        var currentUriWithoutQuery = NavigationManager.Uri.GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(currentUriWithoutQuery, new Dictionary<string, object?>
        {
            ["message"] = message
        });
        navigationManager.NavigateTo(newUri);
    }
}
