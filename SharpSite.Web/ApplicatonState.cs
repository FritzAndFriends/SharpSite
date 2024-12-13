﻿using SharpSite.Abstractions;

namespace SharpSite.Web;

public class ApplicationState
{
	public Type? CurrentThemeType { get; set; }

	/// <summary>
	/// List of the plugins that are currently loaded.
	/// </summary>
	public Dictionary<string, PluginManifest> Plugins { get; } = new();

	public void AddPlugin(string pluginName, PluginManifest manifest)
	{
		Plugins.Add(pluginName, manifest);
	}

}