namespace SharpSite.Abstractions;

public interface IPlugin
{
	string Name { get; }
	ReadOnlySpan<byte> Bytes { get; }
}
