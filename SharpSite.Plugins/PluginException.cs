using System.Runtime.Serialization;

namespace SharpSite.Plugins;

public class PluginException : Exception
{
	public PluginException() { }
	public PluginException(string message) : base(message)	{ }
	public PluginException(Exception exception, string message) : base(message, exception)	{ }
}
