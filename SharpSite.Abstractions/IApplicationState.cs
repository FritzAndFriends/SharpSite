using System.Text.Json.Serialization;

namespace SharpSite.Abstractions;

public interface IApplicationState
{
	long? MaximumUploadSizeMB { get; set; }
	CurrentThemeRecord? CurrentTheme { get; set; }
}
