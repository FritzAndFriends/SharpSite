namespace SharpSite.Abstractions.FileStorage;

public interface IHandleFileStorage
{

	/// <summary>
	/// Get a file from storage and return it with its metadata
	/// </summary>
	/// <param name="filename">Name of the file to fetch</param>
	/// <returns>the file with metadata</returns>
	Task<FileData> GetFile(string filename);


	/// <summary>
	/// Get a list of files from storage with metadata
	/// </summary>
	/// <param name="page">page number of the list of files to return</param>
	/// <param name="filesOnPage">Number of records on each page to return</param>
	/// <param name="totalFilesAvailable">The total number of files that are available</param>
	/// <returns>The selected page of file metadata</returns>
	Task<IEnumerable<FileMetaData>> GetFiles(int page, int filesOnPage, out int totalFilesAvailable);

	/// <summary>
	/// Add a file to storage
	/// </summary>
	/// <param name="file">The file to add</param>
	/// <returns>The name of the file that was added</returns>
	Task<string> AddFile(FileData file);

	/// <summary>
	/// Remove a file from storage
	/// </summary>
	Task RemoveFile(string filename);


}
