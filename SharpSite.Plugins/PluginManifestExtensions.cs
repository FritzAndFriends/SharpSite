using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace SharpSite.Plugins;

public static class PluginManifestExtensions
{
	/// <summary>
	/// Provides PluginManifest extensions for mainfest validaion
	/// </summary>
	/// <param name="manifest"></param>
	/// <param name="plugin"></param>
	/// <exception cref="PluginException">Raises </exception>
	public static void ValidateManifest(this PluginManifest manifest, ILogger logger, Plugin plugin)
	{
		if (manifest == null) return;

		// check for a valid version number, valid plugin Id, etc
		if (string.IsNullOrEmpty(manifest.Id))
		{
			logger.LogError("Invalid plugin manifest: {FileName}", plugin.Name);
			throw new PluginException("Plugin manifest is missing a valid Id.");
		}

		// manifest Id should only contain letters, numbers, period, hyphen, and underscore
		if (!Regex.IsMatch(manifest.Id, @"^[a-zA-Z0-9\.\-_]+$"))
		{
			logger.LogError("Invalid plugin manifest: {FileName}", plugin.Name);
			throw new PluginException("Plugin manifest Id contains invalid characters.");
		}

		// manifest version should only contain letters, numbers, period, hyphen, and underscore
		if (!Regex.IsMatch(manifest.Version, @"^[a-zA-Z0-9\.\-_]+$"))
		{
			logger.LogError("Invalid plugin manifest: {FileName}", plugin.Name);
			throw new PluginException("Plugin manifest version contains invalid characters.");
		}

		if (string.IsNullOrEmpty(manifest.DisplayName))
		{
			logger.LogError("Invalid plugin manifest: {FileName}", plugin.Name);
			throw new PluginException("Plugin manifest is missing a valid DisplayName.");
		}
	}
}

