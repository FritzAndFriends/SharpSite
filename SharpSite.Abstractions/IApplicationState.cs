using System.Text.Json.Serialization;

namespace SharpSite.Abstractions;

public interface IApplicationState
{
	CurrentThemeRecord? CurrentTheme { get; set; }
}
