namespace SharpSite.PluginPacker;

public static class ArgumentParser
{
	public static (string? inputPath, string? outputPath) ParseArguments(string[] args)
	{
		string? inputPath = null;
		string? outputPath = null;
		for (int i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "-i":
				case "--input":
					if (i + 1 < args.Length) inputPath = args[++i];
					break;
				case "-o":
				case "--output":
					if (i + 1 < args.Length) outputPath = args[++i];
					break;
			}
		}
		return (inputPath, outputPath);
	}
}
