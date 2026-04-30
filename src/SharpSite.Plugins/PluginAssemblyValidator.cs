using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SharpSite.Plugins;

/// <summary>
/// Validates plugin assemblies by checking assembly name against manifest ID
/// and verifying SHA-256 file hashes to detect tampering.
/// </summary>
public class PluginAssemblyValidator(ILogger<PluginAssemblyValidator> logger)
{
	private static readonly object _hashFileLock = new();

	/// <summary>
	/// Computes the SHA-256 hash of a file on disk.
	/// </summary>
	public static string ComputeFileHash(string filePath)
	{
		using var stream = File.OpenRead(filePath);
		var hashBytes = SHA256.HashData(stream);
		return Convert.ToHexStringLower(hashBytes);
	}

	/// <summary>
	/// Validates that the loaded assembly's simple name matches the expected manifest ID.
	/// Throws <see cref="PluginException"/> if validation fails.
	/// </summary>
	public void ValidateAssemblyName(Assembly assembly, string manifestId)
	{
		var assemblyName = assembly.GetName().Name;
		if (!string.Equals(assemblyName, manifestId, StringComparison.OrdinalIgnoreCase))
		{
			var ex = new PluginException(
				$"Assembly name mismatch: expected '{manifestId}' but loaded assembly is '{assemblyName}'. " +
				"The plugin DLL does not match its manifest ID.");
			logger.LogError(ex,
				"Plugin rejected: assembly name '{AssemblyName}' does not match manifest ID '{ManifestId}'",
				assemblyName, manifestId);
			throw ex;
		}

		logger.LogDebug("Assembly name validation passed for '{ManifestId}'", manifestId);
	}

	/// <summary>
	/// Verifies the SHA-256 hash of a plugin DLL. On first load, stores the hash.
	/// On subsequent loads, verifies the hash matches the stored value.
	/// Throws <see cref="PluginException"/> if hash verification fails.
	/// </summary>
	public void VerifyOrStoreHash(string manifestId, string dllPath)
	{
		var currentHash = ComputeFileHash(dllPath);
		var registry = LoadHashRegistry();

		if (registry.TryGetValue(manifestId, out var storedHash))
		{
			if (!string.Equals(currentHash, storedHash, StringComparison.OrdinalIgnoreCase))
			{
				var ex = new PluginException(
					$"Plugin integrity check failed for '{manifestId}': " +
					$"DLL hash has changed since initial installation. " +
					$"Expected: {storedHash}, Actual: {currentHash}. " +
					"The plugin file may have been tampered with.");
				logger.LogError(ex,
					"Plugin rejected: SHA-256 hash mismatch for '{ManifestId}'. Expected '{ExpectedHash}', got '{ActualHash}'",
					manifestId, storedHash, currentHash);
				throw ex;
			}

			logger.LogDebug("Hash verification passed for '{ManifestId}'", manifestId);
		}
		else
		{
			registry[manifestId] = currentHash;
			SaveHashRegistry(registry);
			logger.LogInformation(
				"Stored SHA-256 hash for new plugin '{ManifestId}': {Hash}",
				manifestId, currentHash);
		}
	}

	/// <summary>
	/// Removes a stored hash entry for a plugin (e.g., when uninstalling).
	/// </summary>
	public void RemoveStoredHash(string manifestId)
	{
		var registry = LoadHashRegistry();
		if (registry.Remove(manifestId))
		{
			SaveHashRegistry(registry);
			logger.LogInformation("Removed stored hash for plugin '{ManifestId}'", manifestId);
		}
	}

	private Dictionary<string, string> LoadHashRegistry()
	{
		lock (_hashFileLock)
		{
			if (!File.Exists(HashRegistryPath))
			{
				return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}

			try
			{
				var json = File.ReadAllText(HashRegistryPath);
				return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
					?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Failed to read hash registry at '{Path}'. Starting with empty registry.", HashRegistryPath);
				return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
		}
	}

	private void SaveHashRegistry(Dictionary<string, string> registry)
	{
		lock (_hashFileLock)
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			var json = JsonSerializer.Serialize(registry, options);
			var directory = Path.GetDirectoryName(HashRegistryPath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
			File.WriteAllText(HashRegistryPath, json);
		}
	}

	private static string HashRegistryPath => SharpSitePathProvider.AssemblyHashRegistryPath;
}
