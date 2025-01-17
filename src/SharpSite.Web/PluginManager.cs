using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.FileStorage;
using SharpSite.Plugins;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Web;

public class PluginManager(
	PluginAssemblyManager pluginAssemblyManager,
	ApplicationState AppState,
	ILogger<PluginManager> logger) : IPluginManager, IDisposable
{
	private Plugin? plugin;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	private readonly static IServiceCollection _ServiceDescriptors = new ServiceCollection();

	private static IServiceProvider? _ServiceProvider;

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

		_ServiceProvider = _ServiceDescriptors.BuildServiceProvider();

		CleanupCurrentUploadedPlugin();
	}

	public async Task LoadPluginsAtStartup()
	{

		AppState.ConfigurationSectionChanged += (sender, e) =>
		{
			// Update the registered ConfigurationSection in the service locator
			if (_ServiceDescriptors.Any(descriptor => descriptor.ServiceType == e.GetType()))
			{
				var oldSectionDescriptor = _ServiceDescriptors.First(descriptor => descriptor.ServiceType == e.GetType());
				var oldSection = (ISharpSiteConfigurationSection)oldSectionDescriptor.ImplementationInstance!;
				e.OnConfigurationChanged(oldSection, this);
				_ServiceDescriptors.Remove(oldSectionDescriptor);
			}

			var serviceDescriptor = new ServiceDescriptor(e.GetType(), e);
			_ServiceDescriptors.Add(serviceDescriptor);
			_ServiceProvider = _ServiceDescriptors.BuildServiceProvider();
		};

		_ServiceDescriptors.AddSingleton<IPluginManager>(this);

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

				await RegisterWithServiceLocator(pluginAssembly);

			}

			AppState.AddPlugin(key, manifest!);
			logger.LogInformation("Plugin {PluginName} loaded at startup.", pluginName);

		}

		_ServiceProvider = _ServiceDescriptors.BuildServiceProvider();

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

				var knownInterface = pluginAttribute.RegisterType switch
				{
					PluginRegisterType.FileStorage => typeof(IHandleFileStorage),
					_ => null
				};

				var serviceDescriptor = new ServiceDescriptor(knownInterface!, type, pluginAttribute.Scope switch
				{
					PluginServiceLocatorScope.Singleton => ServiceLifetime.Singleton,
					PluginServiceLocatorScope.Scoped => ServiceLifetime.Scoped,
					_ => ServiceLifetime.Transient
				});
				_ServiceDescriptors.Add(serviceDescriptor);
			}
			else if (typeof(ISharpSiteConfigurationSection).IsAssignableFrom(type))
			{
				var configurationSection = (ISharpSiteConfigurationSection)Activator.CreateInstance(type)!;

				// we should only add the configuration section if it is not already present
				if (!AppState.ConfigurationSections.ContainsKey(configurationSection.SectionName))
				{
					AppState.ConfigurationSections.Add(configurationSection.SectionName, configurationSection);
				}

				_ServiceDescriptors.Add(new ServiceDescriptor(type, configurationSection));

				if (AppState.Initialized)
				{
					await configurationSection.OnConfigurationChanged(null!, this);
				}

			}

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

		using var pluginMemoryStream = new MemoryStream(plugin.Bytes);
		archive = new ZipArchive(pluginMemoryStream, ZipArchiveMode.Read, true);

		// Create the plugins/_wwwroot folder if it doesn't exist
		var hasWebContent = archive.Entries.Any(entry => entry.FullName.StartsWith("web/"));
		DirectoryInfo? pluginWwwRootFolder = null;

		if (hasWebContent)
		{
			pluginWwwRootFolder = Directory.CreateDirectory(Path.Combine("plugins", "_wwwroot", $"{pluginManifest!.Id}@{pluginManifest.Version}"));
		}

		foreach (var entry in archive.Entries)
		{
			// skip directory entries in the archive
			if (string.IsNullOrEmpty(entry.Name)) continue;

			string entryPath = entry.FullName switch
			{
				"manifest.json" => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("lib/") => Path.Combine(pluginLibFolder.FullName, entry.Name),
				var s when s.StartsWith("web/") => Path.Combine(pluginWwwRootFolder!.FullName, entry.Name),
				_ => string.Empty
			};

			if (string.IsNullOrEmpty(entryPath)) continue;

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
		return Task.FromResult(Directory.CreateDirectory(Path.Combine("plugins", "_" + name)));
	}

	public T? GetPluginProvidedService<T>()
	{
		return _ServiceProvider!.GetService<T>();
	}

	public Task<DirectoryInfo> MoveDirectoryInPluginsFolder(string oldName, string newName)
	{

		// check if the oldName directory exists
		if (!Directory.Exists(Path.Combine("plugins", "_" + oldName)))
		{
			throw new DirectoryNotFoundException($"Directory {oldName} not found in plugins folder.");
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
}
