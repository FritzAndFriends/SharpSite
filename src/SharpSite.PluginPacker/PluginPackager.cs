using System.IO.Compression;

namespace SharpSite.PluginPacker;

public static class PluginPackager
{
	public static bool PackagePlugin(string inputPath, string outputPath)
	{
		try
		{
			string tempZip = Path.GetTempFileName();
			if (File.Exists(tempZip)) File.Delete(tempZip);
			ZipFile.CreateFromDirectory(inputPath, tempZip);
			string outFile = outputPath.EndsWith(".sspkg", StringComparison.OrdinalIgnoreCase) ? outputPath : outputPath + ".sspkg";
			File.Move(tempZip, outFile);
			Console.WriteLine($"Plugin packaged successfully: {outFile}");
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Packaging failed: {ex.Message}");
			return false;
		}
	}
}
