using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SharpSite.Abstractions.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Plugins.FileStorage.AzureBlobStorage;

public class AzureBlobStorageConfigurationSection : ISharpSiteConfigurationSection
{
	public string SectionName { get; } = "Azure Blob Storage";

	[DisplayName("Connection String"), Required, MaxLength(2000)]
	public string ConnectionString { get; set; } = string.Empty;

	[DisplayName("Container Name"), Required, MaxLength(63)]
	public string ContainerName { get; set; } = "sharpsite-files";

	public async Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldConfiguration, IPluginManager pluginManager)
	{
		// If this is the first time setting up the configuration, just ensure container exists
		if (oldConfiguration is not AzureBlobStorageConfigurationSection oldConfig)
		{
			await EnsureContainerExists(ConnectionString, ContainerName);
			return;
		}

		// Check if configuration has changed
		bool connectionStringChanged = oldConfig.ConnectionString != ConnectionString;
		bool containerNameChanged = oldConfig.ContainerName != ContainerName;

		if (!connectionStringChanged && !containerNameChanged)
		{
			// No changes, just ensure container exists
			await EnsureContainerExists(ConnectionString, ContainerName);
			return;
		}

		// Configuration has changed, we need to migrate files
		try
		{
			await MigrateFiles(oldConfig, connectionStringChanged, containerNameChanged);
		}
		catch (Exception ex)
		{
			// If migration fails, at least ensure the new container exists
			await EnsureContainerExists(ConnectionString, ContainerName);

			// Re-throw with more context
			throw new InvalidOperationException(
				$"Failed to migrate files from old configuration. " +
				$"Old: {oldConfig.ConnectionString}/{oldConfig.ContainerName} -> " +
				$"New: {ConnectionString}/{ContainerName}. " +
				$"Error: {ex.Message}", ex);
		}
	}

	private static async Task EnsureContainerExists(string connectionString, string containerName)
	{
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
		}

		if (string.IsNullOrWhiteSpace(containerName))
		{
			throw new ArgumentException("Container name cannot be null or empty", nameof(containerName));
		}

		try
		{
			var blobServiceClient = new BlobServiceClient(connectionString);
			var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
			await containerClient.CreateIfNotExistsAsync();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(
				$"Failed to create or access container '{containerName}' with the provided connection string. " +
				$"Please verify the connection string and container name are correct. Error: {ex.Message}", ex);
		}
	}

	private async Task MigrateFiles(AzureBlobStorageConfigurationSection oldConfig, bool connectionStringChanged, bool containerNameChanged)
	{
		try
		{
			// Set up old and new clients
			BlobServiceClient oldBlobServiceClient = new(oldConfig.ConnectionString);
			BlobContainerClient oldContainerClient = oldBlobServiceClient.GetBlobContainerClient(oldConfig.ContainerName);
			BlobServiceClient newBlobServiceClient = new(ConnectionString);
			BlobContainerClient newContainerClient = newBlobServiceClient.GetBlobContainerClient(ContainerName);

			// Ensure new container exists
			await newContainerClient.CreateIfNotExistsAsync();

			// Check if old container exists
			var oldContainerExists = await oldContainerClient.ExistsAsync();
			if (!oldContainerExists)
			{
				// Old container doesn't exist, nothing to migrate
				return;
			}

			// Get list of all blobs in the old container
			var blobsToMigrate = new List<BlobItem>();
			await foreach (var blobItem in oldContainerClient.GetBlobsAsync())
			{
				blobsToMigrate.Add(blobItem);
			}

			if (blobsToMigrate.Count == 0)
			{
				// No files to migrate
				return;
			}

			// Migrate each blob
			foreach (var blobItem in blobsToMigrate)
			{
				await MigrateBlob(oldContainerClient, newContainerClient, blobItem.Name);
			}

			// If we're moving to a different container/storage account, optionally clean up old files
			// Only delete old files if the migration was successful and we're not in the same container
			if (connectionStringChanged || containerNameChanged)
			{
				foreach (var blobItem in blobsToMigrate)
				{
					var oldBlobClient = oldContainerClient.GetBlobClient(blobItem.Name);
					await oldBlobClient.DeleteIfExistsAsync();
				}
			}
		}
		catch (Exception)
		{
			// Clean up: if migration failed, we should not leave partial state
			// The calling method will handle the exception and ensure new container exists
			throw;
		}
	}

	private static async Task MigrateBlob(BlobContainerClient oldContainer, BlobContainerClient newContainer, string blobName)
	{
		var oldBlobClient = oldContainer.GetBlobClient(blobName);
		var newBlobClient = newContainer.GetBlobClient(blobName);

		// Check if source blob exists
		var sourceExists = await oldBlobClient.ExistsAsync();
		if (!sourceExists)
		{
			return;
		}

		// Check if destination already exists
		var destinationExists = await newBlobClient.ExistsAsync();
		if (destinationExists)
		{
			// Skip if destination already exists to avoid overwriting
			return;
		}

		// For simplicity, download and re-upload the blob
		// This works across different storage accounts and is more reliable
		var downloadResponse = await oldBlobClient.DownloadContentAsync();
		var content = downloadResponse.Value.Content;

		// Get the original properties to preserve content type, etc.
		var properties = await oldBlobClient.GetPropertiesAsync();
		var blobHttpHeaders = new BlobHttpHeaders
		{
			ContentType = properties.Value.ContentType
		};

		// Upload to new location
		await newBlobClient.UploadAsync(content, new BlobUploadOptions
		{
			HttpHeaders = blobHttpHeaders
		});
	}
}
