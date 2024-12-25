using SharpSite.Abstractions;

namespace SharpSite.Plugins;

public class ApplicationState : IApplicationState
{
	public CurrentThemeRecord? CurrentTheme { get; set; } = null;
}
