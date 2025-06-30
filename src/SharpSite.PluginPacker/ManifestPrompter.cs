using SharpSite.Plugins;

namespace SharpSite.PluginPacker;

public static class ManifestPrompter
{
	private static string PromptRequired(string label)
	{
		string? value;
		do
		{
			Console.Write($"{label}: ");
			value = Console.ReadLine()?.Trim();
			if (string.IsNullOrWhiteSpace(value))
			{
				Console.WriteLine($"{label} is required.");
			}
		} while (string.IsNullOrWhiteSpace(value));
		return value;
	}

	public static PluginManifest PromptForManifest()
	{
		var id = PromptRequired("Id");
		var displayName = PromptRequired("DisplayName");
		var description = PromptRequired("Description");
		var version = PromptRequired("Version");
		var published = PromptRequired("Published (yyyy-MM-dd)");
		var supportedVersions = PromptRequired("SupportedVersions");
		var author = PromptRequired("Author");
		var contact = PromptRequired("Contact");
		var contactEmail = PromptRequired("ContactEmail");
		var authorWebsite = PromptRequired("AuthorWebsite");
		
		// Optional fields
		Console.Write("Icon (URL): ");
		var icon = (Console.ReadLine() ?? "").Trim();
		Console.Write("Source (repository URL): ");
		var source = (Console.ReadLine() ?? "").Trim();
		Console.Write("KnownLicense (e.g. MIT, Apache, LGPL): ");
		var knownLicense = (Console.ReadLine() ?? "").Trim();
		Console.Write("Tags (comma separated): ");
		var tagsStr = (Console.ReadLine() ?? "").Trim();
		var tags = tagsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		
		Console.Write("Features (comma separated, e.g. Theme,FileStorage): ");
		var featuresStr = (Console.ReadLine() ?? "").Trim();
		var features = featuresStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var featureEnums = features.Length > 0 ? Array.ConvertAll(features, f => Enum.Parse<PluginFeatures>(f, true)) : [];
		return new PluginManifest
		{
			Id = id,
			DisplayName = displayName,
			Description = description,
			Version = version,
			Icon = string.IsNullOrWhiteSpace(icon) ? null : icon,
			Published = published,
			SupportedVersions = supportedVersions,
			Author = author,
			Contact = contact,
			ContactEmail = contactEmail,
			AuthorWebsite = authorWebsite,
			Source = string.IsNullOrWhiteSpace(source) ? null : source,
			KnownLicense = string.IsNullOrWhiteSpace(knownLicense) ? null : knownLicense,
			Tags = tags.Length > 0 ? tags : null,
			Features = featureEnums
		};
	}
}
