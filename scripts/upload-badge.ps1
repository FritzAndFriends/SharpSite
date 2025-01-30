# Define variables
$storageAccountName = "fritzblog"
$storageAccountKey = $env:AZSTORAGEKEY
$containerName = "githubartifacts"
$filePath = $args[0]
$blobName = $args[1]

# Install Azure Storage module if not already installed
if (-not (Get-Module -ListAvailable -Name Az.Storage)) {
	Install-Module -Name Az.Storage -Force -AllowClobber
}

# Import the module
Import-Module Az.Storage

# Create a context for the storage account
$context = New-AzStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

# Upload the file to the storage container
Set-AzStorageBlobContent -File $filePath -Container $containerName -Blob $blobName -Context $context

Write-Output "File uploaded successfully to $containerName/$blobName"