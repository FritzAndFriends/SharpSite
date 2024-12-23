namespace SharpSite.Abstractions.Base;

public interface ISharpSiteConfigurationSection
{

	/// <summary>
	/// A reference to the application configuration.
	/// </summary>
	Dictionary<string, string> Configuration { get; }
	string SectionName { get; }
}
