using SharpSite.Abstractions;
using SharpSite.Abstractions.Theme;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Plugins;

public class ThemeManager(IPluginManager pluginManager) : IThemeManager
{
	IReadOnlyDictionary<string, IPluginManifest> IThemeManager.Themes => pluginManager.Plugins.Where(p => p.Value.Features.Contains(Enum.GetName(PluginFeatures.Theme)?.ToLowerInvariant())).ToDictionary();

	[JsonIgnore]
	public Type? ThemeType
	{
		get
		{
			if (CurrentState.CurrentTheme is null) return null;
			var themeManifest = pluginManager.Plugins.Values.FirstOrDefault(p => p.IdVersion == CurrentState.CurrentTheme.IdVersion);
			if (themeManifest is not null)
			{
				var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == themeManifest.Id);
				var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
				return themeType!;
			}

			return null;

		}
	}

	private ApplicationState CurrentState { get; set; } = new ApplicationState();

	IApplicationState IThemeManager.CurrentState => CurrentState;

	public void SetTheme(IPluginManifest manifest)
	{
		// identify the pluginAssembly in memory that's named after the manifest.Id
		var pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == manifest.Id);

		var themeType = pluginAssembly?.GetTypes().FirstOrDefault(t => typeof(IHasStylesheets).IsAssignableFrom(t));
		if (themeType is not null) CurrentState.CurrentTheme = new(manifest.IdVersion);
	}

	public async Task SaveState()
	{
		// save application state to applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		var json = JsonSerializer.Serialize((ApplicationState)CurrentState);
		await File.WriteAllTextAsync(appStateFile, json);
	}

	public async Task LoadAtStartup()
	{
		// load application state from applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		if (!File.Exists(appStateFile))
			return;

		var json = await File.ReadAllTextAsync(appStateFile);
		CurrentState = JsonSerializer.Deserialize<ApplicationState>(json) ?? new ApplicationState();
	}
}
