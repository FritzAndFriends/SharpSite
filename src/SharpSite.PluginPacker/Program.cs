using SharpSite.PluginPacker;

(string? inputPath, string? outputPath) = ArgumentParser.ParseArguments(args);

if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
{
	Console.WriteLine("Usage: SharpSite.PluginPacker -i <input-folder> -o <output-file>");
	return 1;
}

if (!Directory.Exists(inputPath))
{
	Console.WriteLine($"Input directory '{inputPath}' does not exist.");
	return 1;
}

if (File.Exists(outputPath))
{
	Console.WriteLine($"Output file '{outputPath}' already exists. Please choose a different output path.");
	return 1;
}

var manifest = ManifestHandler.LoadOrCreateManifest(inputPath);
if (manifest == null)
{
	Console.WriteLine("Failed to load or create manifest.");
	return 1;
}
Console.WriteLine($"Loaded manifest for {manifest.DisplayName} ({manifest.Id})");

if (!PluginPackager.PackagePlugin(inputPath, outputPath))
{
	Console.WriteLine("Packaging failed.");
	return 1;
}

return 0;
