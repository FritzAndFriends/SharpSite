using Microsoft.AspNetCore.Components.Forms;
using SharpSite.Abstractions;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SharpSite.Web;

public class PluginManager(ApplicationState AppState, ILogger<PluginManager> logger) : IDisposable
{
	private MemoryStream? CurrentUploadedPlugin;
	private string CurrentUploadedPluginName = string.Empty;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	public static void Initialize()
	{
		Directory.CreateDirectory("plugins");
		Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot"));
	}

	public async Task HandleUploadedPlugin(IBrowserFile uploadedFile)
	{
		ArgumentNullException.ThrowIfNull(uploadedFile);

		ValidatePlugin(logger, uploadedFile);

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

		Manifest = ReadManifest(manifestStream);

		ValidateManifest();

		// Add your logic to process the manifest content here
		logger.LogInformation("Plugin {PluginName} uploaded and manifest processed.", CurrentUploadedPluginName);
	}


	private PluginManifest? ReadManifest(string manifestPath)
	{
		using var manifestStream = File.OpenRead(manifestPath);
		return ReadManifest(manifestStream);
	}

	private PluginManifest? ReadManifest(Stream manifestStream)
	{
		var options = new JsonSerializerOptions
		{
			Converters = { new JsonStringEnumConverter() }
		};
		return JsonSerializer.Deserialize<PluginManifest>(manifestStream, options);
	}

	public async Task SavePlugin()
	{
		if (CurrentUploadedPlugin is null || Manifest is null)
		{
			var exception = new Exception("No plugin uploaded.");
			logger.LogError(exception, "Attempted to save plugin without uploading.");
			throw exception;
		}

		FileStream fileStream;
		DirectoryInfo pluginLibFolder;
		ZipArchive archive;
		(fileStream, pluginLibFolder, archive) = await ExtractAndInstallPlugin(logger);

		// By convention it is a package_name of (<package_name>@<package_vesrson>.(sspkg|.dll)
		var key = Manifest.id;
		// if there is a DLL in the pluginLibFolder with the same base name as the plugin file, reflection load that DLL
		var pluginDll = Directory.GetFiles(pluginLibFolder.FullName, $"{key}*.dll").FirstOrDefault();
		Assembly? pluginAssembly = null;
		if (!string.IsNullOrEmpty(pluginDll))
		{
			// Soft load of package without taking ownership for the process .dll
			var assemblyData = await File.ReadAllBytesAsync(pluginDll);
			pluginAssembly = Assembly.Load(assemblyData);
			logger.LogInformation("Assembly {AssemblyName} loaded at runtime.", pluginDll);
		}
		// Add plugin to the list of plugins in ApplicationState
		AppState.AddPlugin(Manifest.id, Manifest);
		logger.LogInformation("Plugin {PluginName} loaded at runtime.", CurrentUploadedPluginName);

		if (Manifest.Features.Contains(PluginFeatures.Theme))
		{
			AppState.SetTheme(Manifest);
		}

		// Add your logic to save the plugin here
		CleanupCurrentUploadedPlugin();

		logger.LogInformation("Plugin {PluginName} saved and registered.", CurrentUploadedPluginName);
	}

	public void LoadPluginsAtStartup()
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
			var key = manifest!.id;

			var pluginDll = Directory.GetFiles(pluginFolder, $"{key}*.dll").FirstOrDefault();
			Assembly? pluginAssembly = null;
			if (!string.IsNullOrEmpty(pluginDll))
			{
				// Soft load of package without taking ownership for the process .dll
				var assemblyData = File.ReadAllBytes(pluginDll);
				pluginAssembly = Assembly.Load(assemblyData);
				logger.LogInformation("Assembly {AssemblyName} loaded at startup.", pluginDll);
			}

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

		}
	}

	private async Task<(FileStream, DirectoryInfo, ZipArchive)> ExtractAndInstallPlugin(ILogger<PluginManager> logger)
	{

		FileStream fileStream;
		DirectoryInfo pluginLibFolder;
		ZipArchive archive;

		var pluginFolder = Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		var filePath = Path.Combine(pluginFolder.FullName, $"{Manifest!.id}@{Manifest.Version}.sspkg");

		CurrentUploadedPlugin!.Position = 0;
		fileStream = new FileStream(filePath, FileMode.Create);
		await CurrentUploadedPlugin.CopyToAsync(fileStream);
		logger.LogInformation("Plugin saved to {FilePath}", filePath);

		// Create a folder named after the plugin name under /plugins
		pluginLibFolder = Directory.CreateDirectory(Path.Combine("plugins", $"{Manifest!.id}@{Manifest.Version}"));

		// Create the plugins/_wwwroot folder if it doesn't exist
		var pluginWwwRootFolder = Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot", $"{Manifest!.id}@{Manifest.Version}"));
		archive = new ZipArchive(CurrentUploadedPlugin, ZipArchiveMode.Read, true);
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

		return (fileStream, pluginLibFolder, archive);

	}

	private void CleanupCurrentUploadedPlugin()
	{
		CurrentUploadedPlugin?.Dispose();
		CurrentUploadedPlugin = null;
		CurrentUploadedPluginName = string.Empty;
		Manifest = null;
	}

	private static void ValidatePlugin(ILogger<PluginManager> logger, IBrowserFile uploadedFile)
	{
		if (uploadedFile.Name.StartsWith("_"))
		{
			var exception = new Exception("Plugin filenames are not allowed to start with an underscore '_'");
			logger.LogError(exception, "Invalid plugin filename: {FileName}", uploadedFile.Name);
			throw exception;
		}
	}

	private void ValidateManifest()
	{
		// check for a valid version number, valid plugin id, etc
		if (string.IsNullOrEmpty(Manifest!.id))
		{
			var exception = new Exception("Plugin manifest is missing a valid id.");
			logger.LogError(exception, "Invalid plugin manifest: {FileName}", CurrentUploadedPluginName);
			throw exception;
		}

		// manifest id should only contain letters, numbers, period, hyphen, and underscore
		if (!Regex.IsMatch(Manifest.id, @"^[a-zA-Z0-9\.\-_]+$"))
		{
			var exception = new Exception("Plugin manifest id contains invalid characters.");
			logger.LogError(exception, "Invalid plugin manifest: {FileName}", CurrentUploadedPluginName);
			throw exception;
		}

		// manifest version should only contain letters, numbers, period, hyphen, and underscore
		if (!Regex.IsMatch(Manifest.Version, @"^[a-zA-Z0-9\.\-_]+$"))
		{
			var exception = new Exception("Plugin manifest version contains invalid characters.");
			logger.LogError(exception, "Invalid plugin manifest: {FileName}", CurrentUploadedPluginName);
			throw exception;
		}

		if (string.IsNullOrEmpty(Manifest.DisplayName))
		{
			var exception = new Exception("Plugin manifest is missing a valid DisplayName.");
			logger.LogError(exception, "Invalid plugin manifest: {FileName}", CurrentUploadedPluginName);
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
