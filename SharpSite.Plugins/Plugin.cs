using SharpSite.Abstractions;

namespace SharpSite.Plugins;

public class Plugin(MemoryStream stream, string pluginName): IPlugin
{
	public string Name { get; } = pluginName;

	private readonly byte[] Bytes = stream.ToArray();

	ReadOnlySpan<byte> IPlugin.Bytes => Bytes;

	public static async Task<Plugin> LoadFromStream(Stream pluginContentStream,  string pluginName)
	{
		using var pluginMemoryStream = new MemoryStream();
		await pluginContentStream.CopyToAsync(pluginMemoryStream);
		return new Plugin(pluginMemoryStream, pluginName);
	}
}
