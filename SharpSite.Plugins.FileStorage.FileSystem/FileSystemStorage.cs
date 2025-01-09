using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Plugins.FileStorage.FileSystem;

// NOTE: This is a naive and insecure implementation of file storage. It is not recommended to use this in a production environment.

[RegisterPlugin(PluginServiceLocatorScope.Singleton, PluginRegisterType.FileStorage)]
public partial class FileSystemStorage : IHandleFileStorage
{

	private readonly DirectoryInfo _BaseFolder;

	public FileSystemConfigurationSection Configuration { get; }

	public FileSystemStorage(
		FileSystemConfigurationSection configuration,
		IPluginManager pluginManager)
	{
		Configuration = configuration;
		_BaseFolder = pluginManager.GetDirectoryInPluginsFolder(Configuration.BaseFolderName);
	}

	public async Task<string> AddFile(FileData file)
	{

		ArgumentNullException.ThrowIfNull(file, nameof(file));
		if (file.File is null || file.File.Length == 0)
		{
			throw new ArgumentException("Missing file", nameof(file));
		}

		file.Metadata.ValidateFileName();

		// Create a new file in the BaseFolder with the filename submitted
		var path = Path.Combine(_BaseFolder.FullName, file.Metadata.FileName);
		using var fileStream = File.Create(path);
		await file.File.CopyToAsync(fileStream);

		return file.Metadata.FileName;

	}

	public Task<FileData> GetFile(string filename)
	{

		// get the file from disk and return it with metadata
		var path = Path.Combine(_BaseFolder.FullName, filename);

		// handle a missing file by returning a placeholder
		if (!File.Exists(path)) return Task.FromResult(FileData.Missing);

		var memoryStream = new MemoryStream();
		using (var file = File.Open(path, FileMode.Open))
		{
			file.CopyTo(memoryStream);
			memoryStream.Position = 0;
		}

		// Get the content type from the file extension
		var contentType = MimeTypesMap.GetMimeType(Path.GetExtension(path));
		var metadata = new FileMetaData(filename, contentType, File.GetCreationTime(path));
		return Task.FromResult(new FileData(memoryStream, metadata));
	}

	public Task<IEnumerable<FileMetaData>> GetFiles(int page, int filesOnPage, out int totalFilesAvailable)
	{

		var files = Directory.GetFiles(_BaseFolder.FullName);
		totalFilesAvailable = files.Length;
		var filesOnPageArray = files.Skip((page - 1) * filesOnPage)
			.Take(filesOnPage)
			.Select(f => new FileMetaData(Path.GetFileName(f), MimeTypesMap.GetMimeType(f), File.GetCreationTime(f)));
		return Task.FromResult(filesOnPageArray);

	}

	public Task RemoveFile(string filename)
	{

		var path = Path.Combine(_BaseFolder.FullName, filename);
		File.Delete(path);
		return Task.CompletedTask;

	}
}
