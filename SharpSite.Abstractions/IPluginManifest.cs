namespace SharpSite.Abstractions;

public interface IPluginManifest
{
	public string Id { get; set; }
	public string DisplayName { get; set; }
	public string Description { get; set; }
	public string Version { get; set; }
	public string? Icon { get; set; }
	public string Published { get; set; }
	public string SupportedVersions { get; set; }
	public string Author { get; set; }
	public string Contact { get; set; }
	public string ContactEmail { get; set; }
	public string AuthorWebsite { get; set; }
	public string? Source { get; set; }
	public string? KnownLicense { get; set; }
	public string[]? Tags { get; set; }
	public string[] Features { get; set; }

	public string IdVersion => $"{Id}@{Version}";
}
