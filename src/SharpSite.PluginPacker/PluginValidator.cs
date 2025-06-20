using SharpSite.Plugins;

namespace SharpSite.PluginPacker;

public static class PluginValidator
{
	public static bool ValidatePlugin(string inputPath, PluginManifest manifest)
	{
		bool valid = true;
		foreach (var feature in manifest.Features)
		{
			switch (feature)
			{
				case PluginFeatures.Theme:
					if (!Directory.Exists(Path.Combine(inputPath, "web")))
					{
						Console.WriteLine("Error: Theme plugin must contain a 'web' folder.");
						valid = false;
					}
					break;
				case PluginFeatures.FileStorage:
					// Add FileStorage-specific validation if needed
					break;
			}
		}
		string dllName = manifest.Id.Split('.').Last() + ".dll";
		string libPath = Path.Combine(inputPath, "lib");
		if (!Directory.Exists(libPath) || !File.Exists(Path.Combine(libPath, dllName)))
		{
			Console.WriteLine($"Error: 'lib/{dllName}' is required.");
			valid = false;
		}
		if (!File.Exists(Path.Combine(inputPath, "LICENSE")))
		{
			Console.WriteLine("Warning: LICENSE file is missing.");
		}
		return valid;
	}
}
