using System.Text.RegularExpressions;

namespace SharpSite.Abstractions.FileStorage;

public record FileData(Stream File, FileMetaData Metadata)
{

	/// <summary>
	/// A placeholder for a missing file
	/// </summary>
	public static FileData Missing => new(Stream.Null, new FileMetaData(string.Empty, string.Empty, DateTimeOffset.MinValue));

}

public record FileMetaData(string FileName, string ContentType, DateTimeOffset CreateDate)
{

	public void ValidateFileName()
	{
		if (string.IsNullOrEmpty(FileName))
		{
			throw new ArgumentException("Missing file name", nameof(FileName));
		}

		// run a regular expression check to ensure the file name is valid - no slashes or other special characters
		var reValidFileName = new Regex(@"^[a-zA-Z0-9_\-\.]+$");
		if (!reValidFileName.IsMatch(FileName))
		{
			throw new ArgumentException("Invalid file name", nameof(FileName));
		}


	}

}