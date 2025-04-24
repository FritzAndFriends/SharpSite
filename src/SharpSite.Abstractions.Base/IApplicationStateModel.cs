namespace SharpSite.Abstractions.Base;

public interface IApplicationStateModel
{

	/// <summary>
	/// Get configuration options from Application State for the specified name
	/// </summary>
	/// <param name="name">The configuration key sought</param>
	/// <param name="defaultValue">Default value if not present</param>
	/// <returns></returns>
	string GetConfigurationByName(string name, string defaultValue = "");

	/// <summary>
	/// Sets a configuration option using a specified name and value.
	/// </summary>
	/// <param name="name">The identifier for the configuration setting to be modified.</param>
	/// <param name="value">The new value to assign to the specified configuration setting.</param>
	void SetConfigurationByName(string name, string value);

}

public class ApplicationStateKeys
{
	public const string ContentConnectionString = "ContentConnectionString";
}