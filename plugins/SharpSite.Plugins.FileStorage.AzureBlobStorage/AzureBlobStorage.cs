using Azure.Storage.Blobs;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Plugins.FileStorage.AzureBlobStorage;

// https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
[RegisterPlugin(PluginServiceLocatorScope.Singleton, PluginRegisterType.FileStorage)]
public class AzureBlobStorage(AzureBlobStorageConfigurationSection Configuration) : IHandleFileStorage
{
	public async Task<string> AddFile(FileData file)
	{
		if (string.IsNullOrEmpty(Configuration.ConnectionString)) throw new Exception();
		BlobServiceClient blobServiceClient = new(Configuration.ConnectionString);
		BlobContainerClient blobContainerClient = await blobServiceClient.CreateBlobContainerAsync("images");
		BlobClient blobClient = blobContainerClient.GetBlobClient(file.Metadata.FileName);
		await blobClient.UploadAsync(file.File, true);
		throw new NotImplementedException();
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
