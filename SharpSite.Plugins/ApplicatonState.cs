using SharpSite.Abstractions;

namespace SharpSite.Plugins;

public class ApplicationState : IApplicationState
{
	public long? MaximumUploadSizeMB { get; set; } = 10;
	public CurrentThemeRecord? CurrentTheme { get; set; } = null;
}
