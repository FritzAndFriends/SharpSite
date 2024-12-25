namespace SharpSite.Abstractions;

public interface IPluginManifest
{
	public string Id { get; }
	public string DisplayName { get; }
	public string Description { get; }
	public string Version { get; }
	public string? Icon { get; }
	public string Published { get; }
	public string SupportedVersions { get; }
	public string Author { get; }
	public string Contact { get; }
	public string ContactEmail { get; }
	public string AuthorWebsite { get; }
	public string? Source { get; }
	public string? KnownLicense { get; }
	public string[]? Tags { get; }
	public string[] Features { get; }

	public string IdVersion => $"{Id}@{Version}";
}
