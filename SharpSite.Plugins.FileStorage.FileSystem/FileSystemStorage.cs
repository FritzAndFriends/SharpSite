using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Plugins.FileStorage.FileSystem;

public class FileSystemStorage(FileSystemConfigurationSection configuration) : IHandleFileStorage
{

	public async Task AddFile(FileData file)
	{

		var path = Path.Combine(configuration.BaseFolderName, file.Metadata.FileName);
		using var fileStream = File.Create(path);
		await file.File.CopyToAsync(fileStream);

	}

	public Task<FileData> GetFile(string filename)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<FileMetaData>> GetFiles(int page, int filesOnPage, out int totalFilesAvailable)
	{
		throw new NotImplementedException();
	}

	public Task RemoveFile(string filename)
	{
		throw new NotImplementedException();
	}
}
