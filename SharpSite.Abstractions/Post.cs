using System.ComponentModel.DataAnnotations;

namespace SharpSite.Abstractions;

/// <summary>
/// A blog post.
/// </summary>
public class Post
{

	[Key, Required, MaxLength(300)]
	public required string Slug { get; set; } = string.Empty;

	[Required, MaxLength(200)]
	public required string Title { get; set; } = string.Empty;

	[MaxLength(500)]
	public string? Description { get; set; }

	[Required]
	public required string Content { get; set; } = string.Empty;

	/// <summary>
	/// The date the article will be published.  If the date is in the future, the article will not be displayed.
	/// </summary>
	/// <value></value>
	[Required]
	public DateTimeOffset PublishedDate { get; set; } = DateTimeOffset.MaxValue;

	[Required]
	public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.Now;

	[Required, MaxLength(11)]
	public string LanguageCode { get; set; } = "en";

	public static string GetSlug(string title)
	{
		var slug = title.ToLower().Replace(" ", "-");
		// urlencode the slug
		slug = System.Web.HttpUtility.UrlEncode(slug);
		return slug;
	}

	public Uri ToUrl()
	{
		return new Uri($"/{PublishedDate.UtcDateTime.ToString("yyyyMMdd")}/{Slug}", UriKind.Relative);
	}

}
