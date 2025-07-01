using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.DataStorage;
using SharpSite.Abstractions.FileStorage;
using SharpSite.Abstractions.Security;

namespace SharpSite.Web;

/// <summary>
/// Provides mapping between PluginRegisterType enum values and their corresponding interface types.
/// </summary>
public static class PluginTypeMapping
{
	/// <summary>
	/// Dictionary that maps PluginRegisterType to their corresponding interface types.
	/// </summary>
	private static readonly Dictionary<PluginRegisterType, Type?> _TypeMap = new()
	{
		{ PluginRegisterType.FileStorage, typeof(IHandleFileStorage) },
		{ PluginRegisterType.DataStorage_Configuration, typeof(IConfigureDataStorage) },
		{ PluginRegisterType.DataStorage_EfContext, null }, // Special case - uses the actual type
		{ PluginRegisterType.DataStorage_PageRepository, typeof(IPageRepository) },
		{ PluginRegisterType.DataStorage_PostRepository, typeof(IPostRepository) },
		{ PluginRegisterType.Security_SignInManager, typeof(ISignInManager<ISharpSiteUser>) },
		{ PluginRegisterType.Security_UserManager, typeof(IUserManager<ISharpSiteUser>) }
	};

	/// <summary>
	/// Gets the interface type for a given PluginRegisterType.
	/// </summary>
	/// <param name="registerType">The plugin register type to look up.</param>
	/// <returns>The corresponding interface type, or null if not found or for special cases like DataStorage_EfContext.</returns>
	public static Type? GetInterfaceType(PluginRegisterType registerType) => 
		_TypeMap.GetValueOrDefault(registerType);
}
