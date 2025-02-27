using Newtonsoft.Json;

namespace SharpSite.Abstractions;

public class ApplicationStateModel
{

	/// <summary>
	/// Indicates whether the application state has been initialized from the applicationState.json file.
	/// </summary>
	[JsonIgnore]
	public bool Initialized { get; protected set; } = false;

	public bool StartupCompleted { get; set; } = false;

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string? RobotsTxtCustomContent { get; set; }

	public string SiteName { get; set; } = "SharpSite";


	/// <summary>
	/// Maximum file upload size in megabytes.
	/// </summary>
	public long MaximumUploadSizeMB { get; set; } = 10; // 10MB

	public string PageNotFoundContent { get; set; } = string.Empty;



}
