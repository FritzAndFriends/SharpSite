namespace SharpSite.Abstractions;


public class PluginManifest
{
	public required string id { get; set; }
	public required string DisplayName { get; set; }
	public required string Description { get; set; }
	public required string Version { get; set; }
	public string? Icon { get; set; }
	public required string Published { get; set; }
	public required string SupportedVersions { get; set; }
	public required string Author { get; set; }
	public required string Contact { get; set; }
	public required string ContactEmail { get; set; }
	public required string AuthorWebsite { get; set; }
	public string? Source { get; set; }
	public string? KnownLicense { get; set; }
	public string[]? Tags { get; set; }
	public required string[] Features { get; set; }

	public string IdVersionToString()
	{
		return $"{id}@{Version}";
	}

}

