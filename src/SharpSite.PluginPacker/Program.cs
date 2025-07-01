using SharpSite.PluginPacker;

(string? inputPath, string? outputPath) = ArgumentParser.ParseArguments(args);

if (string.IsNullOrWhiteSpace(inputPath))
{
	Console.WriteLine("Usage: SharpSite.PluginPacker -i <input-folder> [-o <output-folder>]");
	Console.WriteLine("  -i, --input   Input folder containing the plugin project");
	Console.WriteLine("  -o, --output  Output directory (optional, defaults to current directory)");
	Console.WriteLine();
	Console.WriteLine("The output filename will be automatically generated as: ID@VERSION.sspkg");
	return 1;
}

// Default to current directory if no output path specified
outputPath = string.IsNullOrWhiteSpace(outputPath) ? Directory.GetCurrentDirectory() : outputPath;

if (!Directory.Exists(inputPath))
{
	Console.WriteLine($"Input directory '{inputPath}' does not exist.");
	return 1;
}

// Validate that output path is a directory, not a file
if (File.Exists(outputPath))
{
	Console.WriteLine($"Error: Output path '{outputPath}' points to a file. Please specify a directory.");
	return 1;
}

var manifest = ManifestHandler.LoadOrCreateManifest(inputPath);
if (manifest is null)
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
