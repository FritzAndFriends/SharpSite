using SharpSite.Abstractions;

namespace SharpSite.Plugins;

public class Plugin(MemoryStream stream, string pluginName)
{
	public readonly string Name = pluginName;

	public readonly byte[] Bytes = stream.ToArray();

	public static async Task<Plugin> LoadFromStream(Stream pluginContentStream,  string pluginName)
	{
		using var pluginMemoryStream = new MemoryStream();
		await pluginContentStream.CopyToAsync(pluginMemoryStream);
		return new Plugin(pluginMemoryStream, pluginName);
	}
}
