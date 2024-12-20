using SharpSite.Abstractions;
using SharpSite.Abstractions.Theme;
using SharpSite.Plugins;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpSite.Web;

public class ApplicationState
{
	public record CurrentThemeRecord(string IdVersion);

	public CurrentThemeRecord? CurrentTheme { get; set; }

	/// <summary>
	/// Maximum file upload size in megabytes.
	/// </summary>
	public long MaximumUploadSizeMB { get; set; } = 10; // 10MB

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

	public async Task Load()
	{
		// load application state from applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		if (File.Exists(appStateFile))
		{
			var json = await File.ReadAllTextAsync(appStateFile);
			var state = JsonSerializer.Deserialize<ApplicationState>(json);
			if (state is not null)
			{
				CurrentTheme = state.CurrentTheme;
				MaximumUploadSizeMB = state.MaximumUploadSizeMB;
			}
		}
	}

	public async Task Save()
	{
		// save application state to applicationState.json in the root of the plugins folder
		var appStateFile = Path.Combine("plugins", "applicationState.json");

		var json = JsonSerializer.Serialize(this);
		await File.WriteAllTextAsync(appStateFile, json);
	}
}
