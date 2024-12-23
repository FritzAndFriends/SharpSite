namespace SharpSite.Abstractions.FileStorage;

public record FileData(Stream File, FileMetaData Metadata);

public record FileMetaData(string FileName, DateTimeOffset CreateDate);