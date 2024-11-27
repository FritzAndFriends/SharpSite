using System.ComponentModel.DataAnnotations;

namespace SharpSite.Abstractions;

public class Page 
{

	[Key]
	public int Id {get; set;}

	[Required, MinLength(4), MaxLength(100)]
	public string Title {get; set;} = string.Empty;

	[Required]
	public required string Slug {get; set;} 

	public string Content {get; set;} = string.Empty;

	public required DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.Now;

}
