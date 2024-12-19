using System.Text.Json.Serialization;

namespace SharpSite.Plugins;

public class PluginManifest
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }
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
	public required PluginFeatures[] Features { get; set; }

	public string IdVersionToString()
	{
		return $"{Id}@{Version}";
	}

}

public enum PluginFeatures
{
	Theme
}

