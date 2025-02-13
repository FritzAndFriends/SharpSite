namespace SharpSite.Abstractions.Base;

public class RegisterPluginAttribute : Attribute
{
	public RegisterPluginAttribute(PluginServiceLocatorScope scope, PluginRegisterType registerType)
	{
		Scope = scope;
		RegisterType = registerType;
	}
	public PluginServiceLocatorScope Scope { get; }
	public PluginRegisterType RegisterType { get; }

}

public enum PluginServiceLocatorScope
{
	Transient,
	Singleton,
	Scoped
}

public enum PluginRegisterType
{
	FileStorage
}