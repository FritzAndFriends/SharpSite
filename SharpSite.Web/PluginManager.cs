using Microsoft.AspNetCore.Components.Forms;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Theme;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace SharpSite.Web;

public class PluginManager(ApplicationState AppState, ILogger<PluginManager> logger) : IDisposable
{
	private MemoryStream? CurrentUploadedPlugin;
	private string CurrentUploadedPluginName = string.Empty;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	public async Task HandleUploadedPlugin(IBrowserFile uploadedFile)
	{
		ArgumentNullException.ThrowIfNull(uploadedFile);

		if (uploadedFile.Name.StartsWith("_"))
		{
			var exception = new Exception("Plugin filenames are not allowed to start with an underscore '_'");
			logger.LogError(exception, "Invalid plugin filename: {FileName}", uploadedFile.Name);
			throw exception;
		}

		CurrentUploadedPluginName = uploadedFile.Name;

		using var stream = uploadedFile.OpenReadStream();
		CurrentUploadedPlugin = new MemoryStream();

		await stream.CopyToAsync(CurrentUploadedPlugin);
		var fileContent = CurrentUploadedPlugin.ToArray();

		using var archive = new ZipArchive(CurrentUploadedPlugin, ZipArchiveMode.Read, true);
		var manifestEntry = archive.GetEntry("manifest.json");

		if (manifestEntry is null)
		{
			var exception = new Exception("manifest.json not found in the ZIP file.");
			logger.LogError(exception, "Manifest file missing in plugin: {FileName}", CurrentUploadedPluginName);
			throw exception;
		}

		using var manifestStream = manifestEntry.Open();

		Manifest = JsonSerializer.Deserialize<PluginManifest>(manifestStream);

		// Add your logic to process the manifest content here
		logger.LogInformation("Plugin {PluginName} uploaded and manifest processed.", CurrentUploadedPluginName);
	}

	public async Task SavePlugin()
	{
		if (CurrentUploadedPlugin is null || Manifest is null)
		{
			var exception = new Exception("No plugin uploaded.");
			logger.LogError(exception, "Attempted to save plugin without uploading.");
			throw exception;
		}

		var pluginFolder = Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		var filePath = Path.Combine(pluginFolder.FullName, CurrentUploadedPluginName);

		CurrentUploadedPlugin.Position = 0;
		using var fileStream = new FileStream(filePath, FileMode.Create);

		await CurrentUploadedPlugin.CopyToAsync(fileStream);
		logger.LogInformation("Plugin saved to {FilePath}", filePath);

		// Create a folder named after the plugin name under /plugins
		var pluginLibFolder = Directory.CreateDirectory(Path.Combine("plugins", CurrentUploadedPluginName.Split('@')[0]));

		// Create the plugins/_wwwroot folder if it doesn't exist
		var pluginWwwRootFolder = Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot", CurrentUploadedPluginName.Split('@')[0]));

		// Extract the contents of the uploaded plugin's lib folder to the pluginLibFolder
		using var archive = new ZipArchive(CurrentUploadedPlugin, ZipArchiveMode.Read, true);
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

		// By convention it is a package_name of (<package_name>@<package_vesrson>.(sspkg|.dll)
		var key = CurrentUploadedPluginName.Split('@')[0];
		// if there is a DLL in the pluginLibFolder with the same base name as the plugin file, reflection load that DLL
		var pluginDll = Directory.GetFiles(pluginLibFolder.FullName, $"{key}*.dll").FirstOrDefault();
		Assembly? pluginAssembly = null;
		if (!string.IsNullOrEmpty(pluginDll))
		{
			// Soft load of package without taking ownership for the process .dll
			var assembylData = await File.ReadAllBytesAsync(pluginDll);
			pluginAssembly = Assembly.Load(assembylData);
			logger.LogInformation("Assembly {AssemblyName} loaded at runtime.", pluginDll);
		}
		// Add plugin to the list of plugins in ApplicationState
		AppState.AddPlugin(key, Manifest);
		logger.LogInformation("Plugin {PluginName} loaded at runtime.", CurrentUploadedPluginName);

		if (Manifest.Features.Contains("theme", StringComparer.InvariantCultureIgnoreCase))
		{
			// identify a type in the pluginAssembly that implements IHasStylesheets
			var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
			AppState.CurrentThemeType = themeType;
		}

		// Add your logic to save the plugin here
		CleanupCurrentUploadedPlugin();

		logger.LogInformation("Plugin {PluginName} saved and registered.", CurrentUploadedPluginName);
	}

	public void LoadPluginsAtStartup()
	{
		// create the plugins folder if it doesn't exist
		Directory.CreateDirectory("plugins");
		Directory.CreateDirectory("plugins/_wwwroot");

		foreach (var pluginFolder in Directory.GetDirectories("plugins"))
		{

			var pluginName = Path.GetFileName(pluginFolder);
			if (pluginName.StartsWith("_")) continue;

			var manifestPath = Path.Combine(pluginFolder, "manifest.json");
			if (!File.Exists(manifestPath)) continue;

			// By convention it is a package_name of (<package_name>@<package_vesrson>.(sspkg|.dll)
			var key = pluginName.Split('@')[0];

			// Add plugin to the list of plugins in ApplicationState
			var manifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(manifestPath));

			var pluginDll = Directory.GetFiles(pluginFolder, $"{key}*.dll").FirstOrDefault();
			Assembly? pluginAssembly = null;
			if (!string.IsNullOrEmpty(pluginDll))
			{
				// Soft load of package without taking ownership for the process .dll
				var assembylData = File.ReadAllBytes(pluginDll);
				pluginAssembly = Assembly.Load(assembylData);
				logger.LogInformation("Assembly {AssemblyName} loaded at startup.", pluginDll);
			}

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

			if (manifest!.Features.Contains("theme", StringComparer.InvariantCultureIgnoreCase))
			{
				// identify a type in the pluginAssembly that implements IHasStylesheets
				var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
				AppState.CurrentThemeType = themeType;
			}

		}
	}

	private void CleanupCurrentUploadedPlugin()
	{
		CurrentUploadedPlugin?.Dispose();
		CurrentUploadedPlugin = null;
		CurrentUploadedPluginName = string.Empty;
		Manifest = null;
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
