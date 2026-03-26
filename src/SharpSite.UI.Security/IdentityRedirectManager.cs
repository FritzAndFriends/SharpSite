using Microsoft.AspNetCore.Http;
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

    public void RedirectTo(string page, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(page).GetLeftPart(UriPartial.Path);
        navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters));
    }

    public void RedirectToCurrentPage() => RedirectTo(navigationManager.Uri);

    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
    {
        var currentUri = new Uri(navigationManager.Uri);
        var currentUriWithoutQuery = currentUri.GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(currentUriWithoutQuery, new Dictionary<string, object?>
        {
            ["message"] = message
        });
        navigationManager.NavigateTo(newUri);
    }

    public void RedirectToWithStatus(string page, string message, HttpContext context)
    {
        var uri = navigationManager.GetUriWithQueryParameters(
            navigationManager.ToAbsoluteUri(page).GetLeftPart(UriPartial.Path),
            new Dictionary<string, object?> { ["message"] = message });
        navigationManager.NavigateTo(uri);
    }
}
