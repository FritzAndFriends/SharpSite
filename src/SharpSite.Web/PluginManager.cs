using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.DataStorage;
using SharpSite.Abstractions.FileStorage;
using SharpSite.Abstractions.Security;
using SharpSite.Plugins;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Web;

public class PluginManager(
	PluginAssemblyManager pluginAssemblyManager,
	PluginAssemblyValidator assemblyValidator,
	ApplicationState AppState,
	ILogger<PluginManager> logger) : IPluginManager, IDisposable
{
	private Plugin? plugin;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	private readonly static IServiceCollection _ServiceDescriptors = new ServiceCollection();
	private static IServiceProvider? _ServiceProvider;
	private static readonly object _ServiceLock = new();

	private const long MaxTotalExtractedSize = 100L * 1024 * 1024; // 100MB
	private const long MaxSingleFileSize = 50L * 1024 * 1024;      // 50MB
	private const double MaxCompressionRatio = 100.0;               // 100:1

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
		EnsurePluginNotInstalled(Manifest, logger);
		ValidateArchiveSecurity(archive);

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
			// Validate DLL integrity before loading
			assemblyValidator.VerifyOrStoreHash(key, pluginDll);

			// Soft load of package without taking ownership for the process .dll
			using var pluginAssemblyFileStream = File.OpenRead(pluginDll);
			plugin = await Plugin.LoadFromStream(pluginAssemblyFileStream, key);
			var pluginAssembly = new PluginAssembly(Manifest, plugin);
			pluginAssemblyManager.AddAssembly(pluginAssembly);

			// Validate assembly name matches manifest ID
			if (pluginAssembly.Assembly is not null)
			{
				assemblyValidator.ValidateAssemblyName(pluginAssembly.Assembly, key);
			}

			await RegisterWithServiceLocator(pluginAssembly);
			await AppState.Save();

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

		lock (_ServiceLock)
		{
			Interlocked.Exchange(ref _ServiceProvider, _ServiceDescriptors.BuildServiceProvider());
		}

		CleanupCurrentUploadedPlugin();
	}

	public async Task LoadPluginsAtStartup()
	{

		AppState.ConfigurationSectionChanged += async (sender, e) =>
		{
			ServiceDescriptor? oldSectionDescriptor = null;
			ISharpSiteConfigurationSection? oldSection = null;

			lock (_ServiceLock)
			{
				oldSectionDescriptor = _ServiceDescriptors.FirstOrDefault(descriptor => descriptor.ServiceType == e.GetType());
				if (oldSectionDescriptor is not null)
				{
					oldSection = (ISharpSiteConfigurationSection)oldSectionDescriptor.ImplementationInstance!;
				}
			}

			if (oldSection is not null)
			{
				await e.OnConfigurationChanged(oldSection, this);
			}

			lock (_ServiceLock)
			{
				if (oldSectionDescriptor is not null)
				{
					_ServiceDescriptors.Remove(oldSectionDescriptor);
				}
				_ServiceDescriptors.Add(new ServiceDescriptor(e.GetType(), e));
				Interlocked.Exchange(ref _ServiceProvider, _ServiceDescriptors.BuildServiceProvider());
			}
		};

		lock (_ServiceLock)
		{
			_ServiceDescriptors.AddSingleton<IPluginManager>(this);
			_ServiceDescriptors.AddSingleton<IApplicationStateModel>(AppState);
			_ServiceDescriptors.AddMemoryCache();
		}

		foreach (var pluginFolder in Directory.GetDirectories("plugins"))
		{
			var pluginName = Path.GetFileName(pluginFolder);
			if (pluginName.StartsWith("_")) continue;

			var manifestPath = Path.Combine(pluginFolder, "manifest.json");
			if (!File.Exists(manifestPath)) continue;

			// Add plugin to the list of plugins in ApplicationState
			var manifest = ReadManifest(manifestPath);

			// By convention it is a package_name of (<package_name>@<package_version>.(sspkg|.dll)
			var key = manifest!.Id;

			var pluginDll = Directory.GetFiles(pluginFolder, $"{key}*.dll").FirstOrDefault();
			if (!string.IsNullOrEmpty(pluginDll))
			{
				// Validate DLL integrity before loading
				try
				{
					assemblyValidator.VerifyOrStoreHash(key, pluginDll);
				}
				catch (PluginException ex)
				{
					logger.LogError(ex, "Plugin '{PluginName}' failed integrity validation at startup. Skipping.", key);
					continue;
				}

				// Soft load of package without taking ownership for the process .dll
				using var pluginAssemblyFileStream = File.OpenRead(pluginDll);
				plugin = await Plugin.LoadFromStream(pluginAssemblyFileStream, key);
				var pluginAssembly = new PluginAssembly(manifest, plugin);
				pluginAssemblyManager.AddAssembly(pluginAssembly);

				// Validate assembly name matches manifest ID
				if (pluginAssembly.Assembly is not null)
				{
					try
					{
						assemblyValidator.ValidateAssemblyName(pluginAssembly.Assembly, key);
					}
					catch (PluginException ex)
					{
						logger.LogError(ex, "Plugin '{PluginName}' assembly name mismatch at startup. Unloading.", key);
						pluginAssemblyManager.RemoveAssembly(pluginAssembly);
						continue;
					}
				}

				logger.LogInformation("Assembly {AssemblyName} loaded at startup.", pluginDll);

				await RegisterWithServiceLocator(pluginAssembly);

			}

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

		}

		lock (_ServiceLock)
		{
			Interlocked.Exchange(ref _ServiceProvider, _ServiceDescriptors.BuildServiceProvider());
		}

	}


	private async Task RegisterWithServiceLocator(PluginAssembly pluginAssembly)
	{

		var types = pluginAssembly.Assembly!.GetTypes();

		// TODO: is there a way to do this without reflection or analyzing every type?

		foreach (var type in types)
		{
			// analyze the assembly for classes that are decorated with the PluginAttribute
			// and register them with the service locator
			var pluginAttributes = type.GetCustomAttributes(typeof(RegisterPluginAttribute), false);

			// if pluginAttributes has a value, then the class is to be registered with the service locator
			if (pluginAttributes.Length > 0)
			{
				var pluginAttribute = (RegisterPluginAttribute)pluginAttributes[0]!;

				var knownInterface = pluginAttribute.RegisterType == PluginRegisterType.DataStorage_EfContext 
					? type 
					: PluginTypeMapping.GetInterfaceType(pluginAttribute.RegisterType);

				var serviceDescriptor = new ServiceDescriptor(knownInterface!, type, pluginAttribute.Scope switch
				{
					PluginServiceLocatorScope.Singleton => ServiceLifetime.Singleton,
					PluginServiceLocatorScope.Scoped => ServiceLifetime.Scoped,
					_ => ServiceLifetime.Transient
				});
				lock (_ServiceLock)
				{
					_ServiceDescriptors.Add(serviceDescriptor);
				}
			}
			else if (typeof(ISharpSiteConfigurationSection).IsAssignableFrom(type))
			{
				var configurationSection = (ISharpSiteConfigurationSection)Activator.CreateInstance(type)!;

				// we should only add the configuration section if it is not already present
				if (!AppState.ConfigurationSections.ContainsKey(configurationSection.SectionName))
				{
					AppState.ConfigurationSections.Add(configurationSection.SectionName, configurationSection);
				}

				lock (_ServiceLock)
				{
					_ServiceDescriptors.Add(new ServiceDescriptor(type, configurationSection));
				}

				if (AppState.Initialized)
				{
					await configurationSection.OnConfigurationChanged(null!, this);
				}

			}

		}


	}


	private void ValidateArchiveSecurity(ZipArchive archive)
	{
		long totalExtractedSize = 0;

		foreach (var entry in archive.Entries)
		{
			if (string.IsNullOrEmpty(entry.Name)) continue;

			// Path traversal protection: reject any entry containing ".." (normalized for both slash styles)
			var normalizedName = entry.FullName.Replace('\\', '/');
			if (normalizedName.Contains("..", StringComparison.Ordinal))
			{
				var ex = new PluginException($"Path traversal detected in ZIP entry: {entry.FullName}");
				logger.LogError(ex, "Rejected plugin: path traversal in entry '{EntryName}'", entry.FullName);
				throw ex;
			}

			// Per-file size limit
			if (entry.Length > MaxSingleFileSize)
			{
				var ex = new PluginException(
					$"ZIP entry '{entry.FullName}' exceeds maximum file size of {MaxSingleFileSize / (1024 * 1024)}MB " +
					$"(actual: {entry.Length / (1024 * 1024)}MB).");
				logger.LogError(ex, "Rejected plugin: entry '{EntryName}' is {SizeMB}MB, max is {MaxMB}MB",
					entry.FullName, entry.Length / (1024 * 1024), MaxSingleFileSize / (1024 * 1024));
				throw ex;
			}

			// Compression ratio check (ZIP bomb detection)
			if (entry.CompressedLength > 0)
			{
				double ratio = (double)entry.Length / entry.CompressedLength;
				if (ratio > MaxCompressionRatio)
				{
					var ex = new PluginException(
						$"ZIP entry '{entry.FullName}' has suspicious compression ratio of {ratio:F1}:1 " +
						$"(max allowed: {MaxCompressionRatio}:1).");
					logger.LogError(ex, "Rejected plugin: entry '{EntryName}' compression ratio {Ratio}:1 exceeds limit of {MaxRatio}:1",
						entry.FullName, ratio, MaxCompressionRatio);
					throw ex;
				}
			}

			totalExtractedSize += entry.Length;
		}

		// Total extracted size limit
		if (totalExtractedSize > MaxTotalExtractedSize)
		{
			var ex = new PluginException(
				$"Total extracted size of {totalExtractedSize / (1024 * 1024)}MB exceeds maximum of " +
				$"{MaxTotalExtractedSize / (1024 * 1024)}MB.");
			logger.LogError(ex, "Rejected plugin: total extracted size {SizeMB}MB exceeds max {MaxMB}MB",
				totalExtractedSize / (1024 * 1024), MaxTotalExtractedSize / (1024 * 1024));
			throw ex;
		}
	}


	private static async Task<(FileStream, DirectoryInfo, ZipArchive)> ExtractAndInstallPlugin(ILogger<PluginManager> logger, Plugin plugin, PluginManifest pluginManifest)
	{
		DirectoryInfo pluginLibFolder;
		ZipArchive archive;

		var pluginFolder = Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));
		var filePath = Path.Combine(pluginFolder.FullName, $"{pluginManifest.IdVersionToString()}.sspkg");

		using var pluginAssemblyFileStream = File.OpenWrite(filePath);
		await pluginAssemblyFileStream.WriteAsync(plugin.Bytes);
		logger.LogInformation("Plugin saved to {FilePath}", filePath);

		// Create a folder named after the plugin name under /plugins
		pluginLibFolder = Directory.CreateDirectory(Path.Combine("plugins", pluginManifest.IdVersionToString()));

		using var pluginMemoryStream = new MemoryStream(plugin.Bytes);
		archive = new ZipArchive(pluginMemoryStream, ZipArchiveMode.Read, true);

		// Create the plugins/_wwwroot folder if it doesn't exist
		var hasWebContent = archive.Entries.Any(entry => entry.FullName.StartsWith("web/"));
		DirectoryInfo? pluginWwwRootFolder = null;

		if (hasWebContent)
		{
			pluginWwwRootFolder = Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot", pluginManifest.IdVersionToString()));
		}

		foreach (var entry in archive.Entries)
		{
			// skip directory entries in the archive
			if (string.IsNullOrEmpty(entry.Name)) continue;

			// Defense-in-depth: reject path traversal during extraction
			var normalizedEntryName = entry.FullName.Replace('\\', '/');
			if (normalizedEntryName.Contains("..", StringComparison.Ordinal))
			{
				throw new PluginException($"Path traversal detected in ZIP entry: {entry.FullName}");
			}

			string entryPath = entry.FullName switch
			{
				"manifest.json" => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("lib/") => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("web/") => Path.Combine(pluginWwwRootFolder!.FullName, entry.Name),
				_ => string.Empty
			};

			if (string.IsNullOrEmpty(entryPath)) continue;

			// Defense-in-depth: verify extracted file resolves within allowed directories
			var resolvedPath = Path.GetFullPath(entryPath);
			var libFullPath = Path.GetFullPath(pluginLibFolder.FullName) + Path.DirectorySeparatorChar;
			var wwwFullPath = pluginWwwRootFolder is not null
				? Path.GetFullPath(pluginWwwRootFolder.FullName) + Path.DirectorySeparatorChar
				: null;

			if (!resolvedPath.StartsWith(libFullPath, StringComparison.OrdinalIgnoreCase) &&
				(wwwFullPath is null || !resolvedPath.StartsWith(wwwFullPath, StringComparison.OrdinalIgnoreCase)))
			{
				throw new PluginException($"ZIP entry '{entry.FullName}' resolves outside allowed directories.");
			}

			using var entryStream = entry.Open();
			using var entryFileStream = new FileStream(entryPath, FileMode.Create);
			await entryStream.CopyToAsync(entryFileStream);

		}

		return (pluginAssemblyFileStream, pluginLibFolder, archive);

	}

	public void CleanupCurrentUploadedPlugin()
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

	public Task<DirectoryInfo> CreateDirectoryInPluginsFolder(string name)
	{
		if (!IsValidDirectory(name))
		{
			throw new InvalidFolderException($"Invalid path for folder: {name}");
		}
		return Task.FromResult(Directory.CreateDirectory(Path.Combine("plugins", "_" + name)));
	}

	public T? GetPluginProvidedService<T>() where T : class
	{
		lock (_ServiceLock)
		{
			if (_ServiceProvider is null)
			{
				throw new InvalidOperationException("Service provider is not initialized. Call LoadPluginsAtStartup first.");
			}

			if (!_ServiceDescriptors.Any(descriptor => descriptor.ServiceType == typeof(T)))
			{
				return null;
			}
			return _ServiceProvider.GetService<T>();
		}
	}

	public Task<DirectoryInfo> MoveDirectoryInPluginsFolder(string oldName, string newName)
	{

		// check if the oldName directory exists
		if (!Directory.Exists(Path.Combine("plugins", "_" + oldName)))
		{
			throw new DirectoryNotFoundException($"Directory {oldName} not found in plugins folder.");
		}
		if (!IsValidDirectory(newName))
		{
			throw new InvalidFolderException($"Invalid path for folder: {newName}");
		}

		// move the directory specified, which is prefixed with an underscore, to a new name
		Directory.Move(
			Path.Combine("plugins", "_" + oldName),
			Path.Combine("plugins", "_" + newName)
		);

		return Task.FromResult(new DirectoryInfo(Path.Combine("plugins", "_" + newName)));

	}

	public DirectoryInfo GetDirectoryInPluginsFolder(string name)
	{
		return new DirectoryInfo(Path.Combine("plugins", "_" + name));
	}

	private static readonly char[] _InvalidChars = Path.GetInvalidPathChars();
	private static readonly string[] _InvalidPathSegments = ["~", "..", "/", "\\"];
	private static readonly string[] _ReservedNames = ["CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"];

	private static bool IsValidDirectory(string name)
	{

		if (string.IsNullOrWhiteSpace(name))
		{
			return false;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && _ReservedNames.Contains(name, StringComparer.OrdinalIgnoreCase))
		{
			return false;
		}

		if (new DirectoryInfo(name).FullName.Length > 255) // Common maximum path length for cross-platform
		{
			return false;
		}

		foreach (char c in _InvalidChars)
		{
			if (name.Contains(c))
			{
				return false;
			}
		}

		foreach (string s in _InvalidPathSegments)
		{
			if (name.Contains(s, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}

		if (name.EndsWith(' ') || name.EndsWith('.'))
		{
			return false;
		}

		return true;

	}

	private static void EnsurePluginNotInstalled(PluginManifest? manifest, ILogger logger)
	{

		if (manifest is not null && Directory.Exists(Path.Combine("plugins", manifest.IdVersionToString())))
		{
			var errMsg = string.Format(Locales.SharedResource.sharpsite_plugin_exists, manifest.IdVersionToString());
			PluginException ex = new(errMsg);
			logger.LogError(ex, "Plugin '{Plugin}' is already installed.", manifest.IdVersionToString());
			throw ex;
		}

	}

	public async Task InstallDefaultPlugins()
	{

		var defaultPluginFolder = new DirectoryInfo("defaultplugins");
		if (!defaultPluginFolder.Exists) return;

		foreach (var file in defaultPluginFolder.GetFiles("*.sspkg"))
		{

			using var stream = File.OpenRead(file.FullName);
			var plugin = await Plugin.LoadFromStream(stream, file.Name);

			try
			{
				HandleUploadedPlugin(plugin);
				logger.LogInformation("Plugin {0} loaded from default plugins.", file.Name);
				await SavePlugin();
			}
			catch (PluginException ex)
			{
				logger.LogError(ex, "Plugin {0} failed to load from default plugins.", file.Name);
			}
			finally
			{
				// Cleanup the plugin after processing
				CleanupCurrentUploadedPlugin();
			}

		}

	}

}
