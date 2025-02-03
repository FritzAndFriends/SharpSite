using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.Theme;
using SharpSite.Plugins;

namespace SharpSite.Web;

public class ApplicationState
{

	/// <summary>
	/// Indicates whether the application state has been initialized from the applicationState.json file.
	/// </summary>
	[JsonIgnore]
	public bool Initialized { get; private set; } = false;

	public record CurrentThemeRecord(string IdVersion);

	public record LocalizationRecord(string? DefaultCulture, string[]? SupportedCultures);

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public CurrentThemeRecord? CurrentTheme { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public LocalizationRecord? Localization { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string? RobotsTxtCustomContent { get; set; }

	public Dictionary<string, ISharpSiteConfigurationSection> ConfigurationSections { get; private set; } = new();

	public event Func<ApplicationState, ISharpSiteConfigurationSection, Task>? ConfigurationSectionChanged;

	/// <summary>
	/// Maximum file upload size in megabytes.
	/// </summary>
	public long MaximumUploadSizeMB { get; set; } = 10; // 10MB

	public string PageNotFoundContent { get; set; } = string.Empty;

	[JsonIgnore]
	public Type? ThemeType
	{
		get
		{
			if (CurrentTheme is null) return null;
			var themeManifest = Plugins.Values.FirstOrDefault(p => p.IdVersionToString() == CurrentTheme.IdVersion);
			if (themeManifest is not null)
			{
				var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == themeManifest.Id);
				var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
				return themeType!;
			}

			return null;

		}
	}

	/// <summary>
	/// List of the plugins that are currently loaded.
	/// </summary>
	[JsonIgnore]
	public Dictionary<string, PluginManifest> Plugins { get; } = new();

	public void AddPlugin(string pluginName, PluginManifest manifest)
	{
		if (!Plugins.ContainsKey(pluginName))
		{
			Plugins.Add(pluginName, manifest);
		}
		else
		{
			Plugins[pluginName] = manifest;
		}
	}

	public void SetTheme(PluginManifest manifest)
	{
		// identify the pluginAssembly in memory that's named after the manifest.Id
		var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == manifest.Id);

		var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
		if (themeType is not null) CurrentTheme = new(manifest.IdVersionToString());
	}

	private string GetApplicationStateFileContents()
	{
		// read the applicationState.json file in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");
		if (File.Exists(appStateFile))
		{
			return File.ReadAllText(appStateFile);
		}
		return string.Empty;
	}

	public async Task Load(IServiceProvider services, Func<string>? getApplicationStateContents = null)
	{
		// load application state from applicationState.json in the root of the plugins folder
		var appStateContents = getApplicationStateContents is null ? GetApplicationStateFileContents() : getApplicationStateContents();

		if (!string.IsNullOrEmpty(appStateContents))
		{

			// use Newtonsoft.json to deserialize the json string into the ApplicationState object
			var state = JsonConvert.DeserializeObject<ApplicationState>(appStateContents,
			 new JsonSerializerSettings
			 {
				 TypeNameHandling = TypeNameHandling.Auto,
			 });

			if (state is not null)
			{
				ConfigurationSections = state.ConfigurationSections;
				CurrentTheme = state.CurrentTheme;
				MaximumUploadSizeMB = state.MaximumUploadSizeMB;
				Localization = state.Localization;
				RobotsTxtCustomContent = state.RobotsTxtCustomContent;
				PageNotFoundContent = state.PageNotFoundContent;
			}

			Initialized = true;

			// This shouldn't be called while initializing
			//if (ConfigurationSectionChanged is not null)
			//{
			//	foreach (var section in ConfigurationSections)
			//	{
			//		ConfigurationSectionChanged.Invoke(this, section.Value);
			//	}
			//}

			await PostLoadApplicationState(services);

		}
	}

	public async Task SetConfigurationSection(ISharpSiteConfigurationSection section)
	{

		// add a null check for the section argument
		ArgumentNullException.ThrowIfNull(section, nameof(section));

		if (ConfigurationSections.ContainsKey(section.SectionName))
		{
			ConfigurationSections[section.SectionName] = section;
		}
		else
		{
			ConfigurationSections.Add(section.SectionName, section);
		}

		if (ConfigurationSectionChanged is not null)
		{
			try
			{
				await ConfigurationSectionChanged.Invoke(this, section);
			}
			catch (Exception)
			{
				throw;
			}
		}

	}

	private Task PostLoadApplicationState(IServiceProvider services)
	{

		// Set the max upload size
		var hubOptions = services.GetRequiredService<IOptions<HubOptions>>();
		hubOptions.Value.MaximumReceiveMessageSize = 1024 * 1024 * MaximumUploadSizeMB;

		// TODO: Provide an event that Plugins can register for to provide some additional actions to be taken after they are loaded

		return Task.CompletedTask;

	}

	public async Task Save()
	{
		// save application state to applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		var json = JsonConvert.SerializeObject(this,
			 new JsonSerializerSettings
			 {
				 TypeNameHandling = TypeNameHandling.Auto,
			 });
		await File.WriteAllTextAsync(appStateFile, json);
	}
}
