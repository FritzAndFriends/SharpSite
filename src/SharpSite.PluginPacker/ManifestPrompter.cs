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
		// Optional fields
		Console.Write("Contact: ");
		var contact = (Console.ReadLine() ?? "").Trim();
		Console.Write("ContactEmail: ");
		var contactEmail = (Console.ReadLine() ?? "").Trim();
		Console.Write("AuthorWebsite: ");
		var authorWebsite = (Console.ReadLine() ?? "").Trim();
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
			Published = published,
			SupportedVersions = supportedVersions,
			Author = author,
			Contact = contact,
			ContactEmail = contactEmail,
			AuthorWebsite = authorWebsite,
			Features = featureEnums
		};
	}
}
