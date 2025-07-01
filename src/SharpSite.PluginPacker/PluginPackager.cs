using System.Diagnostics;
using System.IO.Compression;
using SharpSite.Plugins;

namespace SharpSite.PluginPacker;

public static class PluginPackager
{
	public static bool PackagePlugin(string inputPath, string outputPath)
	{
		// Load manifest
		var manifest = ManifestHandler.LoadOrCreateManifest(inputPath);
		if (manifest is null)
		{
			Console.WriteLine("Manifest not found or invalid.");
			return false;
		}

		// Create temp build output folder
		string tempBuildDir = Path.Combine(Path.GetTempPath(), "SharpSitePluginBuild_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempBuildDir);

		// Build the project in Release mode to temp build folder
		if (!BuildProject(inputPath, tempBuildDir))
		{
			Console.WriteLine("Build failed.");
			try { if (Directory.Exists(tempBuildDir)) Directory.Delete(tempBuildDir, true); } catch { }
			return false;
		}

		// Create temp folder for packaging
		string tempDir = Path.Combine(Path.GetTempPath(), "SharpSitePluginPack_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			// Copy DLL to lib/ and rename
			CopyAndRenameDll(inputPath, tempBuildDir, tempDir, manifest);

			// If Theme, copy .css from wwwroot/ to web/
			if (manifest.Features.Contains(PluginFeatures.Theme))
			{
				CopyThemeCssFiles(inputPath, tempDir);
			}
			// Copy manifest.json and other required files
			CopyRequiredFiles(inputPath, tempDir);
		// Zip tempDir to outputPath - use proper naming convention ID@VERSION.sspkg
		// outputPath is always a directory, generate the filename from manifest
		string outFile = Path.Combine(outputPath, $"{manifest.IdVersionToString()}.sspkg");

		// Ensure the output directory exists
		if (!Directory.Exists(outputPath))
		{
			Directory.CreateDirectory(outputPath);
		}

			if (File.Exists(outFile)) File.Delete(outFile);
			ZipFile.CreateFromDirectory(tempDir, outFile);
			Console.WriteLine($"Plugin packaged successfully: {outFile}");
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Packaging failed: {ex.Message}");
			return false;
		}
		finally
		{
			// Clean up temp folder
			try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); } catch { }
			try { if (Directory.Exists(tempBuildDir)) Directory.Delete(tempBuildDir, true); } catch { }
		}
	}

	private static void CopyAndRenameDll(string inputPath, string tempBuildDir, string tempDir, PluginManifest manifest)
	{
		string libDir = Path.Combine(tempDir, "lib");
		Directory.CreateDirectory(libDir);
		string projectName = new DirectoryInfo(inputPath).Name;
		string dllSource = Path.Combine(tempBuildDir, projectName + ".dll");
		string dllTarget = Path.Combine(libDir, manifest.Id + ".dll");
		if (!File.Exists(dllSource))
		{
			throw new FileNotFoundException($"DLL not found: {dllSource}");
		}
		File.Copy(dllSource, dllTarget, overwrite: true);
	}

	private static void CopyThemeCssFiles(string inputPath, string tempDir)
	{
		string webSrc = Path.Combine(inputPath, "wwwroot");
		string webDst = Path.Combine(tempDir, "web");
		if (Directory.Exists(webSrc))
		{
			Directory.CreateDirectory(webDst);
			foreach (var css in Directory.GetFiles(webSrc, "*.css", SearchOption.AllDirectories))
			{
				string dest = Path.Combine(webDst, Path.GetFileName(css));
				File.Copy(css, dest, overwrite: true);
			}
		}
	}

	private static void CopyRequiredFiles(string inputPath, string tempDir)
	{
		string[] requiredFiles = ["manifest.json", "LICENSE", "README.md", "Changelog.txt"];
		foreach (var file in requiredFiles)
		{
			string src = Path.Combine(inputPath, file);
			if (File.Exists(src))
			{
				File.Copy(src, Path.Combine(tempDir, file), overwrite: true);
			}
		}
	}

	private static bool BuildProject(string inputPath, string outputPath)
	{
		var psi = new ProcessStartInfo
		{
			FileName = "dotnet",
			Arguments = $"build --configuration Release --output \"{outputPath}\"",
			WorkingDirectory = inputPath,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
		using var proc = Process.Start(psi);
		if (proc is null)
		{
			Console.WriteLine("Failed to start build process.");
			return false;
		}
		proc.WaitForExit();
		if (proc.ExitCode != 0)
		{
			Console.WriteLine(proc.StandardError.ReadToEnd());
			return false;
		}
		return true;
	}
}
