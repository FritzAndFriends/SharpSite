using System.Text.Json;
using SharpSite.Plugins;

namespace SharpSite.PluginPacker;

public static class ManifestHandler
{
	private static readonly JsonSerializerOptions _Opts = new()  { WriteIndented = true };
	public static PluginManifest? LoadOrCreateManifest(string inputPath)
	{
		string manifestPath = Path.Combine(inputPath, "manifest.json");
		PluginManifest? manifest = null;
		if (!File.Exists(manifestPath))
		{
			Console.WriteLine($"manifest.json not found in {inputPath}.");
			Console.WriteLine("Let's create one interactively.");
			manifest = ManifestPrompter.PromptForManifest();
			var json = JsonSerializer.Serialize(manifest, _Opts);
			File.WriteAllText(manifestPath, json);
			Console.WriteLine($"Created manifest.json at {manifestPath}");
		}
		else
		{
			var json = File.ReadAllText(manifestPath);
			manifest = JsonSerializer.Deserialize<PluginManifest>(json);
			if (manifest == null)
			{
				Console.WriteLine("Failed to parse manifest.json");
				return null;
			}
		}
		return manifest;
	}
}
