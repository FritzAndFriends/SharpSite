using System.Reflection;
using System.Runtime.Loader;

namespace SharpSite.Plugins;

public class PluginAssemblyLoadContext : AssemblyLoadContext
{
	private readonly AssemblyDependencyResolver? _resolver;

	public PluginAssemblyLoadContext(string? mainAssemblyPath = null) : base(isCollectible: true)
	{
		if (!string.IsNullOrEmpty(mainAssemblyPath))
			_resolver = new AssemblyDependencyResolver(mainAssemblyPath);
	}

	public Assembly Load(byte[] assemblyData)
	{
		using var ms = new MemoryStream(assemblyData);
		return LoadFromStream(ms);
	}

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		if (_resolver is not null)
		{
			string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
			if (assemblyPath is not null)
			{
				return LoadFromAssemblyPath(assemblyPath);
			}
		}
		return null;
	}

	protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
	{
		if (_resolver is not null)
		{
			string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (libraryPath is not null)
			{
				return LoadUnmanagedDllFromPath(libraryPath);
			}
		}
		return IntPtr.Zero;
	}
}