using System.ComponentModel.DataAnnotations;

namespace SharpSite.Abstractions;

/// <summary>
/// A blog post.
/// </summary>
public class Post
{

		[Key, Required]
		public required string Slug { get; set; } = string.Empty;

		[Required, MaxLength(200)]
		public required string Title { get; set; } = string.Empty;

		[Required]
		public required string Content { get; set; } = string.Empty;

		/// <summary>
		/// The date the article will be published.  If the date is in the future, the article will not be displayed.
		/// </summary>
		/// <value></value>
		[Required]
		public DateTimeOffset PublishedDate { get; set; } = DateTimeOffset.MaxValue;

}
