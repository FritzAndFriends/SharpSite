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
		if (manifest == null)
		{
			Console.WriteLine("Manifest not found or invalid.");
			return false;
		}

		// 1. Create temp build output folder
		string tempBuildDir = Path.Combine(Path.GetTempPath(), "SharpSitePluginBuild_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempBuildDir);

		// 2. Build the project in Release mode to temp build folder
		if (!BuildProject(inputPath, tempBuildDir))
		{
			Console.WriteLine("Build failed.");
			try { if (Directory.Exists(tempBuildDir)) Directory.Delete(tempBuildDir, true); } catch { }
			return false;
		}

		// 3. Create temp folder for packaging
		string tempDir = Path.Combine(Path.GetTempPath(), "SharpSitePluginPack_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			// 4. Create lib/ and copy/rename DLL
			string libDir = Path.Combine(tempDir, "lib");
			Directory.CreateDirectory(libDir);

			string projectName = new DirectoryInfo(inputPath).Name;
			string dllSource = Path.Combine(tempBuildDir, projectName + ".dll");
			string dllTarget = Path.Combine(libDir, manifest.Id + ".dll");
			if (!File.Exists(dllSource))
			{
				Console.WriteLine($"DLL not found: {dllSource}");
				return false;
			}
			File.Copy(dllSource, dllTarget, overwrite: true);

			// 4. If Theme, copy .css from wwwroot/ to web/
			if (manifest.Features.Contains(PluginFeatures.Theme))
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

			// 5. Copy manifest.json and other required files
			string[] requiredFiles = ["manifest.json", "LICENSE", "README.md", "Changelog.txt"];
			foreach (var file in requiredFiles)
			{
				string src = Path.Combine(inputPath, file);
				if (File.Exists(src))
				{
					File.Copy(src, Path.Combine(tempDir, file), overwrite: true);
				}
			}

			// 6. Zip tempDir to outputPath
			string outFile = outputPath.EndsWith(".sspkg", StringComparison.OrdinalIgnoreCase) ? outputPath : outputPath + ".sspkg";
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
			// 7. Clean up temp folder
			try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); } catch { }
			try { if (Directory.Exists(tempBuildDir)) Directory.Delete(tempBuildDir, true); } catch { }
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
		if (proc == null)
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
