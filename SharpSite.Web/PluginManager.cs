using Microsoft.AspNetCore.Components.Forms;
using SharpSite.Abstractions;
using SharpSite.Plugins;
using SharpSite.Security.Postgres.Account.Pages;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SharpSite.Web;

public class PluginManager(PluginAssemblyManager pluginAssemblyManager, ApplicationState AppState, ILogger<PluginManager> logger) : IDisposable
{
	private Plugin? plugin;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	public static void Initialize()
	{
		Directory.CreateDirectory("plugins");
		Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot"));
	}

	public void HandleUploadedPlugin(Plugin plugin)
	{
		ArgumentNullException.ThrowIfNull(plugin);

		this.plugin = plugin;

		using var currentUploadedPlugin = new MemoryStream(plugin.Bytes);
		using var archive = new ZipArchive(currentUploadedPlugin, ZipArchiveMode.Read, true);
		var manifestEntry = archive.GetEntry("manifest.json");

		if (manifestEntry is null)
		{
			var exception = new Exception("manifest.json not found in the ZIP file.");
			logger.LogError(exception, "Manifest file missing in plugin: {FileName}", plugin.Name);
			throw exception;
		}

		using var manifestStream = manifestEntry.Open();

		Manifest = ReadManifest(manifestStream);
		Manifest.ValidateManifest(logger, plugin);

		// Add your logic to process the manifest content here
		logger.LogInformation("Plugin {PluginName} uploaded and manifest processed.", Manifest);
	}


	private PluginManifest? ReadManifest(string manifestPath)
	{
		using var manifestStream = File.OpenRead(manifestPath);
		return ReadManifest(manifestStream);
	}

	private PluginManifest ReadManifest(Stream manifestStream)
	{
		var options = new JsonSerializerOptions
		{
			Converters = { new JsonStringEnumConverter() }
		};
		return JsonSerializer.Deserialize<PluginManifest>(manifestStream, options)!;
	}

	public async Task SavePlugin()
	{
		if (plugin is null || Manifest is null)
		{
			var exception = new Exception("No plugin uploaded.");
			logger.LogError(exception, "Attempted to save plugin without uploading.");
			throw exception;
		}

		FileStream fileStream;
		DirectoryInfo pluginLibFolder;
		ZipArchive archive;
		(fileStream, pluginLibFolder, archive) = await ExtractAndInstallPlugin(logger, plugin, Manifest);

		// By convention it is a package_name of (<package_name>@<package_vesrson>.(sspkg|.dll)
		var key = Manifest.Id;
		// if there is a DLL in the pluginLibFolder with the same base name as the plugin file, reflection load that DLL
		var pluginDll = Directory.GetFiles(pluginLibFolder.FullName, $"{key}*.dll").FirstOrDefault();
		if (!string.IsNullOrEmpty(pluginDll))
		{
			// Soft load of package without taking ownership for the process .dll
			using var pluginAssemblyFileStream = File.OpenRead(pluginDll);
			plugin = await Plugin.LoadFromStream(pluginAssemblyFileStream, key);
			var pluginAssembly = new PluginAssembly(Manifest, plugin);
			pluginAssemblyManager.AddAssembly(pluginAssembly);
			logger.LogInformation("Assembly {AssemblyName} loaded at runtime.", pluginDll);
		}
		// Add plugin to the list of plugins in ApplicationState
		AppState.AddPlugin(Manifest.Id, Manifest);
		logger.LogInformation("Plugin {PluginName} loaded at runtime.", Manifest);

		if (Manifest.Features.Contains(PluginFeatures.Theme))
		{
			AppState.SetTheme(Manifest);
		}

		logger.LogInformation("Plugin {PluginName} saved and registered.", plugin.Name);
		// Add your logic to save the plugin here

		CleanupCurrentUploadedPlugin();
	}

	public async Task LoadPluginsAtStartup()
	{

		foreach (var pluginFolder in Directory.GetDirectories("plugins"))
		{
			var pluginName = Path.GetFileName(pluginFolder);
			if (pluginName.StartsWith("_")) continue;

			var manifestPath = Path.Combine(pluginFolder, "manifest.json");
			if (!File.Exists(manifestPath)) continue;

			// Add plugin to the list of plugins in ApplicationState
			var manifest = ReadManifest(manifestPath);

			// By convention it is a package_name of (<package_name>@<package_vesrson>.(sspkg|.dll)
			var key = manifest!.Id;

			var pluginDll = Directory.GetFiles(pluginFolder, $"{key}*.dll").FirstOrDefault();
			if (!string.IsNullOrEmpty(pluginDll))
			{
				// Soft load of package without taking ownership for the process .dll
				using var pluginAssemblyFileStream = File.OpenRead(pluginDll);
				plugin = await Plugin.LoadFromStream(pluginAssemblyFileStream, key);
				var pluginAssembly = new PluginAssembly(manifest, plugin);
				pluginAssemblyManager.AddAssembly(pluginAssembly);
				logger.LogInformation("Assembly {AssemblyName} loaded at startup.", pluginDll);
			}

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

		}
	}

	private static async Task<(FileStream, DirectoryInfo, ZipArchive)> ExtractAndInstallPlugin(ILogger<PluginManager> logger, Plugin plugin, PluginManifest pluginManifest)
	{
		DirectoryInfo pluginLibFolder;
		ZipArchive archive;

		var pluginFolder = Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		var filePath = Path.Combine(pluginFolder.FullName, $"{pluginManifest!.Id}@{pluginManifest.Version}.sspkg");

		using var pluginAssemblyFileStream = File.OpenWrite(filePath);
		await pluginAssemblyFileStream.WriteAsync(plugin.Bytes);
		logger.LogInformation("Plugin saved to {FilePath}", filePath);

		// Create a folder named after the plugin name under /plugins
		pluginLibFolder = Directory.CreateDirectory(Path.Combine("plugins", $"{pluginManifest!.Id}@{pluginManifest.Version}"));

		// Create the plugins/_wwwroot folder if it doesn't exist
		var pluginWwwRootFolder = Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot", $"{pluginManifest!.Id}@{pluginManifest.Version}"));
		using var pluginMemoryStream = new MemoryStream(plugin.Bytes);
		archive = new ZipArchive(pluginMemoryStream, ZipArchiveMode.Read, true);
		foreach (var entry in archive.Entries)
		{

			// skip directory entries in the archive
			if (string.IsNullOrEmpty(entry.Name)) continue;

			string entryPath = entry.FullName switch
			{
				"manifest.json" => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("lib/") => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("web/") => Path.Combine(pluginWwwRootFolder.FullName, entry.Name),
				_ => string.Empty
			};

			if (string.IsNullOrEmpty(entryPath)) continue;

			using var entryStream = entry.Open();
			using var entryFileStream = new FileStream(entryPath, FileMode.Create);
			await entryStream.CopyToAsync(entryFileStream);

		}

		return (pluginAssemblyFileStream, pluginLibFolder, archive);

	}

	private void CleanupCurrentUploadedPlugin()
	{
		plugin = null;
		Manifest = null;
	}

	public void ValidatePlugin(string pluginName)
	{
		if (pluginName.StartsWith("_"))
		{
			var exception = new Exception("Plugin filenames are not allowed to start with an underscore '_'");
			logger.LogError(exception, "Invalid plugin filename: {FileName}", pluginName);
			throw exception;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				CleanupCurrentUploadedPlugin();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			//pluginAssemblyManager.Dispose();
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
