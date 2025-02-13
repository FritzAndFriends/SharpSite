using SharpSite.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Data.Postgres;

public class PgPage
{

	[Key]
	public int Id {get; set;}

	[Required, MinLength(4), MaxLength(100)]
	public string Title {get; set;} = string.Empty;

	[Required]
	public required string Slug {get; set;} 

	public string Content {get; set;} = string.Empty;

	public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.Now;


	[Required, MaxLength(11)]
	public string LanguageCode { get; set; } = "en";

	public static explicit operator PgPage(Page page)
	{

		return new PgPage
		{
			Id = page.Id,
			Title = page.Title,
			Slug = page.Slug,
			Content = page.Content,
			LastUpdate = page.LastUpdate,
			LanguageCode = page.LanguageCode
		};

	}

	public static explicit operator Page(PgPage page)
	{
		return new Page
		{
			Id = page.Id,
			Title = page.Title,
			Slug = page.Slug,
			Content = page.Content,
			LastUpdate = page.LastUpdate,
			LanguageCode = page.LanguageCode
		};
	}

}
