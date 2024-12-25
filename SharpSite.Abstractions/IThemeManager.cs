using System.Text.Json.Serialization;

namespace SharpSite.Abstractions;

public interface IThemeManager: IStartupManager
{
	IReadOnlyDictionary<string, IPluginManifest> Themes { get; }

	IApplicationState CurrentState { get; }

	public Type? ThemeType { get; }

	Task SaveState();

	void SetTheme(IPluginManifest manifest);
}