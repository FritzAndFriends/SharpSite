namespace SharpSite.Abstractions.Base;

/// <summary>
/// An interface for the application configuration and to access the configuration.
/// </summary>
public interface ISharpSiteConfiguration
{

	/// <summary>
	/// A reference to the application configuration.
	/// </summary>
	Dictionary<string, Dictionary<string, string>> Configuration { get; }

	/// <summary>
	/// Get the configuration for a specific section.
	/// </summary>
	/// <param name="sectionName">The section of the application to get configuration settings for </param>
	/// <returns>Collection of configuration settings</returns>
	Dictionary<string, string> GetConfigurationForSection(string sectionName);

}
