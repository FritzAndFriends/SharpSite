using System.Reflection;
using System.Runtime.Loader;

namespace SharpSite.Plugins;

public class PluginAssemblyLoadContext : AssemblyLoadContext
{
	public PluginAssemblyLoadContext() : base(isCollectible: true) { }

	public Assembly Load(byte[] assemblyData)
	{
		using (var ms = new MemoryStream(assemblyData))
		{
			return LoadFromStream(ms);
		}
	}
}