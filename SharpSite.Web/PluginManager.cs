using Microsoft.AspNetCore.Components.Forms;
using SharpSite.Abstractions;
using System.IO.Compression;
using System.Text.Json;

namespace SharpSite.Web;

public class PluginManager(ApplicationState AppState) : IDisposable
// , IStringLocalizer Localizer
{

	private MemoryStream? CurrentUploadedPlugin;
	private string CurrentUploadedPluginName = string.Empty;
	private bool disposedValue;

	public PluginManifest? Manifest { get; private set; }

	public async Task HandleUploadedPlugin(IBrowserFile uploadedFile)
	{

		ArgumentNullException.ThrowIfNull(uploadedFile);

		if (uploadedFile.Name.StartsWith("_"))
		{
			throw new Exception("Plugin filenames are not allowed to start with an underscore '_'");
		}

		CurrentUploadedPluginName = uploadedFile.Name;


		using var stream = uploadedFile.OpenReadStream();
		CurrentUploadedPlugin = new MemoryStream();

		await stream.CopyToAsync(CurrentUploadedPlugin);
		var fileContent = CurrentUploadedPlugin.ToArray();

		using var archive = new ZipArchive(CurrentUploadedPlugin, ZipArchiveMode.Read, true);
		var manifestEntry = archive.GetEntry("manifest.json");

		if (manifestEntry is null)
		{
			throw new Exception("manifest.json not found in the ZIP file.");
		}

		using var manifestStream = manifestEntry.Open();

		Manifest = JsonSerializer.Deserialize<PluginManifest>(manifestStream);

		// Add your logic to process the manifest content here

	}

	public async Task SavePlugin()
	{
		if (CurrentUploadedPlugin is null || Manifest is null)
		{
			throw new Exception("No plugin uploaded.");
		}


		var pluginFolder = Directory.CreateDirectory(Path.Combine("plugins", "_uploaded"));

		var filePath = Path.Combine(pluginFolder.FullName, CurrentUploadedPluginName);

		CurrentUploadedPlugin.Position = 0;
		using var fileStream = new FileStream(filePath, FileMode.Create);

		await CurrentUploadedPlugin.CopyToAsync(fileStream);

		Console.WriteLine($"Plugin saved to {filePath}");

		// Add plugin to the list of plugins in ApplicationState
		AppState.AddPlugin(CurrentUploadedPluginName, Manifest);

		// Add your logic to save the plugin here
		CurrentUploadedPlugin.Dispose();
		CurrentUploadedPlugin = null;
		CurrentUploadedPluginName = string.Empty;
		Manifest = null;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				CurrentUploadedPlugin = null;
				CurrentUploadedPluginName = string.Empty;
				Manifest = null;
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
