using System.ComponentModel.DataAnnotations;

namespace SharpSite.Data.Postgres;

/// <summary>
/// A postgres specific implementation of a post.
/// </summary>
public class PgPost
{

	[Required, Key, MaxLength(300)]
	public required string Slug { get; set; }

	[Required, MaxLength(200)]
	public required string Title { get; set; }

	[MaxLength(500)]
	public string? Description { get; set; }

	[Required]
	public required string Content { get; set; } = string.Empty;

	[Required]
	public required DateTimeOffset Published { get; set; } = DateTimeOffset.MaxValue;

	[Required]
	public required DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.Now;

	[Required, MaxLength(11)]
	public string LanguageCode { get; set; } = "en";

	public static explicit operator PgPost(SharpSite.Abstractions.Post post)
	{

		return new PgPost
		{
			Slug = post.Slug,
			Title = post.Title,
			Description = post.Description,
			Content = post.Content,
			Published = post.PublishedDate,
			LastUpdate = post.LastUpdate,
			LanguageCode = post.LanguageCode,
		};

	}

	public static explicit operator SharpSite.Abstractions.Post(PgPost post)
	{

		return new SharpSite.Abstractions.Post
		{
			Slug = post.Slug,
			Title = post.Title,
			Description = post.Description,
			Content = post.Content,
			PublishedDate = post.Published,
			LastUpdate = post.LastUpdate,
			LanguageCode = post.LanguageCode,
		};

	}

}