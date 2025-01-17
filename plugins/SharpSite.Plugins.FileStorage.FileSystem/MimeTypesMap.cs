namespace SharpSite.Plugins.FileStorage.FileSystem;

public partial class FileSystemStorage
{
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
