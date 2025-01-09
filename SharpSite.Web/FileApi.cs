using SharpSite.Abstractions;
using SharpSite.Abstractions.FileStorage;

namespace SharpSite.Web;

public static class FileApi
{

	public static WebApplication MapFileApi(this WebApplication app, PluginManager pluginManager)
	{

		// Don't add the File API if there is no file storage plugin configured
		var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
		if (fileProvider is null) return app;
		fileProvider = null;

		//{
		//	throw new InvalidOperationException("No file storage plugin found");
		//}

		var filesGroup = app.MapGroup("/api/files");
		filesGroup.MapGet("/", async (int page, int filesOnPage) =>
				{

					var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
					var files = await fileProvider!.GetFiles(page, filesOnPage, out var totalFilesAvailable);

					// TODO: Add pagination metadata to the response

					return Results.Ok(files);
				});

		filesGroup.MapGet("{*path}", async (string path) =>
		{

			var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
			var fileInfo = await fileProvider!.GetFile(path);
			if (fileInfo != FileData.Missing)
			{
				return Results.NotFound();
			}
			return Results.File(fileInfo.File, fileInfo.Metadata.ContentType, fileInfo.Metadata.FileName, fileInfo.Metadata.CreateDate);
		});

		// Need to add a POST endpoint to upload files that is limited to members of the "Admin" role
		filesGroup.MapPost("/", async (FileData file, HttpContext context) =>
		{
			var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
			await fileProvider!.AddFile(file);

			// generate the base of the URL using HttpContextAccessor to get the host and port
			var path = $"{context.Request.Scheme}://{context.Request.Host}/api/files/{file.Metadata.FileName}";
			return Results.Ok(path);
		}).RequireAuthorization(Constants.Roles.AllUsers);

		// need to add a PUT endpoint to update files that is limited to members of the "Admin" role
		filesGroup.MapPut("{*path}", async (string path, FileData file) =>
		{
			var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
			await fileProvider!.RemoveFile(path);
			await fileProvider.AddFile(file);
			return Results.Created($"/api/files/{file.Metadata.FileName}", file.Metadata);
		}).RequireAuthorization(Constants.Roles.AdminUsers);

		// need to add a DELETE endpoint to remove files that is limited to members of the "Admin" role
		filesGroup.MapDelete("{*path}", async (string path) =>
		{
			var fileProvider = pluginManager.GetPluginProvidedService<IHandleFileStorage>();
			await fileProvider!.RemoveFile(path);
			return Results.NoContent();
		}).RequireAuthorization(Constants.Roles.AdminUsers);

		return app;

	}

}
