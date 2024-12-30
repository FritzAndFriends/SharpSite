using SharpSite.Abstractions;
using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Web;

public static class FileApi
{

	public static WebApplication MapFileApi(this WebApplication app, PluginManager pluginManager)
	{

		var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
		if (fileProvider is null) return app;

		//{
		//	throw new InvalidOperationException("No file storage plugin found");
		//}

		var filesGroup = app.MapGroup("/api/files");
		filesGroup.MapGet("/", async (int page, int filesOnPage) =>
				{
					var files = await fileProvider.GetFiles(page, filesOnPage, out var totalFilesAvailable);

					// TODO: Add pagination metadata to the response

					return Results.Ok(files);
				});

		filesGroup.MapGet("{*path}", async (IHandleFileStorage fileProvider, string path) =>
		{
			var fileInfo = await fileProvider.GetFile(path);
			if (fileInfo != FileData.Missing)
			{
				return Results.NotFound();
			}
			return Results.File(fileInfo.File, fileInfo.Metadata.ContentType, fileInfo.Metadata.FileName, fileInfo.Metadata.CreateDate);
		});

		// Need to add a POST endpoint to upload files that is limited to members of the "Admin" role
		filesGroup.MapPost("/", async (IHandleFileStorage fileProvider, FileData file) =>
		{
			await fileProvider.AddFile(file);
			return Results.Created($"/api/files/{file.Metadata.FileName}", file.Metadata);
		}).RequireAuthorization(Constants.Roles.AllUsers);

		// need to add a PUT endpoint to update files that is limited to members of the "Admin" role
		filesGroup.MapPut("{*path}", async (IHandleFileStorage fileProvider, string path, FileData file) =>
		{
			await fileProvider.RemoveFile(path);
			await fileProvider.AddFile(file);
			return Results.Created($"/api/files/{file.Metadata.FileName}", file.Metadata);
		}).RequireAuthorization(Constants.Roles.AdminUsers);

		// need to add a DELETE endpoint to remove files that is limited to members of the "Admin" role
		filesGroup.MapDelete("{*path}", async (IHandleFileStorage fileProvider, string path) =>
		{
			await fileProvider.RemoveFile(path);
			return Results.NoContent();
		}).RequireAuthorization(Constants.Roles.AdminUsers);

		return app;

	}

}
