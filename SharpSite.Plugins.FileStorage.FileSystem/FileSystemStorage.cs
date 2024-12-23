using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Plugins.FileStorage.FileSystem;

public class FileSystemStorage(FileSystemConfigurationSection configuration) : IHandleFileStorage
{

	public async Task AddFile(FileData file)
	{

		ArgumentNullException.ThrowIfNull(file, nameof(file));
		if (file.File is null || file.File.Length == 0)
		{
			throw new ArgumentException("Missing file", nameof(file));
		}

		file.Metadata.ValidateFileName();

		var path = Path.Combine(configuration.BaseFolderName, file.Metadata.FileName);
		using var fileStream = File.Create(path);
		await file.File.CopyToAsync(fileStream);

	}

	public Task<FileData> GetFile(string filename)
	{

		// get the file from disk and return it with metadata
		var path = Path.Combine(configuration.BaseFolderName, filename);

		var memoryStream = new MemoryStream();
		using (var file = File.Open(path, FileMode.Open))
		{
			file.CopyTo(memoryStream);
			memoryStream.Position = 0;
		}
		var metadata = new FileMetaData(filename, File.GetCreationTime(path));
		return Task.FromResult(new FileData(memoryStream, metadata));
	}

	public Task<IEnumerable<FileMetaData>> GetFiles(int page, int filesOnPage, out int totalFilesAvailable)
	{

		var files = Directory.GetFiles(configuration.BaseFolderName);
		totalFilesAvailable = files.Length;
		var filesOnPageArray = files.Skip((page - 1) * filesOnPage).Take(filesOnPage).Select(f => new FileMetaData(Path.GetFileName(f), File.GetCreationTime(f)));
		return Task.FromResult(filesOnPageArray);

	}

	public Task RemoveFile(string filename)
	{

		var path = Path.Combine(configuration.BaseFolderName, filename);
		File.Delete(path);
		return Task.CompletedTask;

	}
}
