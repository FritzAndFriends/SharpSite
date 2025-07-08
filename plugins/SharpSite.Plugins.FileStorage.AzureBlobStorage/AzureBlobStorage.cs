using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Plugins.FileStorage.AzureBlobStorage;

[RegisterPlugin(PluginServiceLocatorScope.Singleton, PluginRegisterType.FileStorage)]
public partial class AzureBlobStorage : IHandleFileStorage
{
	private readonly AzureBlobStorageConfigurationSection _configuration;
	private readonly BlobServiceClient? _blobServiceClient;
	private readonly BlobContainerClient? _containerClient;

	public AzureBlobStorage(AzureBlobStorageConfigurationSection configuration)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

		if (!string.IsNullOrWhiteSpace(_configuration.ConnectionString) && !string.IsNullOrWhiteSpace(_configuration.ContainerName))
		{
			_blobServiceClient = new BlobServiceClient(_configuration.ConnectionString);
			_containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.ContainerName);
			// Do not create container here; defer to OnConfigurationChanged
		}
	}

	private void EnsureConfigured()
	{
		if (_blobServiceClient is null || _containerClient is null)
		{
			throw new InvalidOperationException("Azure Blob Storage plugin is not configured. Please provide a valid connection string and container name in the settings.");
		}
	}

	public async Task<string> AddFile(FileData file)
	{
		EnsureConfigured();
		ArgumentNullException.ThrowIfNull(file, nameof(file));
		if (file.File is null || file.File.Length == 0)
		{
			throw new ArgumentException("Missing file", nameof(file));
		}

		file.Metadata.ValidateFileName();

		var blobClient = _containerClient!.GetBlobClient(file.Metadata.FileName);
		
		// Set content type if provided
		var uploadOptions = new BlobUploadOptions();
		if (!string.IsNullOrWhiteSpace(file.Metadata.ContentType))
		{
			uploadOptions.HttpHeaders = new BlobHttpHeaders
			{
				ContentType = file.Metadata.ContentType
			};
		}

		// Reset stream position to beginning
		file.File.Position = 0;
		await blobClient.UploadAsync(file.File, uploadOptions, cancellationToken: default);
		return file.Metadata.FileName;
	}

	public async Task<FileData> GetFile(string filename)
	{
		EnsureConfigured();
		ArgumentException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

		var blobClient = _containerClient!.GetBlobClient(filename);
		
		// Check if blob exists
		var exists = await blobClient.ExistsAsync();
		if (!exists)
		{
			return FileData.Missing;
		}

		// Download blob content
		var response = await blobClient.DownloadContentAsync();
		var content = response.Value.Content;

		// Get blob properties for metadata
		var propertiesResponse = await blobClient.GetPropertiesAsync();
		var blobProperties = propertiesResponse.Value;

		var memoryStream = new MemoryStream(content.ToArray());
		var contentType = blobProperties.ContentType ?? MimeTypesMap.GetMimeType(Path.GetExtension(filename));
		var createDate = blobProperties.CreatedOn;

		var metadata = new FileMetaData(filename, contentType, createDate);
		return new FileData(memoryStream, metadata);
	}

	public Task<IEnumerable<FileMetaData>> GetFiles(int page, int filesOnPage, out int totalFilesAvailable)
	{
		EnsureConfigured();
		var blobs = new List<BlobItem>();
		
		// Get all blobs synchronously (we need to work with the out parameter constraint)
		var pageable = _containerClient!.GetBlobs();
		foreach (var blobItem in pageable)
		{
			blobs.Add(blobItem);
		}

		totalFilesAvailable = blobs.Count;
		
		var pagedBlobs = blobs
			.Skip((page - 1) * filesOnPage)
			.Take(filesOnPage)
			.Select(blob => new FileMetaData(
				blob.Name, 
				blob.Properties.ContentType ?? MimeTypesMap.GetMimeType(Path.GetExtension(blob.Name)),
				blob.Properties.CreatedOn ?? DateTimeOffset.UtcNow));

		return Task.FromResult(pagedBlobs);
	}

	public async Task RemoveFile(string filename)
	{
		EnsureConfigured();
		ArgumentException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

		var blobClient = _containerClient!.GetBlobClient(filename);
		await blobClient.DeleteIfExistsAsync();
	}

	private class MimeTypesMap
	{
		internal static string GetMimeType(string fileExtension)
		{
			// implement a map of file extensions to content types
			// this is a very basic implementation and should be replaced with a more comprehensive solution
			return fileExtension switch
			{
				// add basic image types
				".jpg" => "image/jpeg",
				".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".bmp" => "image/bmp",
				".svg" => "image/svg+xml",
				".webp" => "image/webp",

				// add basic text types
				".txt" => "text/plain",
				".html" => "text/html",
				".css" => "text/css",
				".js" => "text/javascript",
				".json" => "application/json",
				".xml" => "application/xml",

				_ => "application/octet-stream"
			};
		}
	}
}
