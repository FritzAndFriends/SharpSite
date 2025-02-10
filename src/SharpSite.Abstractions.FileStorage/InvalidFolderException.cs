namespace SharpSite.Abstractions.FileStorage;
public class InvalidFolderException : Exception
{
	public InvalidFolderException() : base("Invalid folder location.") { }

	public InvalidFolderException(string message) : base(message) { }

	public InvalidFolderException(string message, Exception innerException) : base(message, innerException) { }
}
