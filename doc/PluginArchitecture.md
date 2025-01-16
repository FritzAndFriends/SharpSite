# Plugin Architecture

SharpSite should support a rich ecosystem of plugins that allow administrator to change the look, feel, and capabilities of a SharpSite application

Plugins are features that are not distributed with SharpSite but can be added after the SharpSite application is already started and deployed.


## Plugins are packages of files

A Plugin should contain a collection of files, compressed in ZIP format, and renamed with a SSPKG extension.  The version number should appear in the filename before the SSPKG extension, separated from the package name with an `@` character.

Filename format:  `ID@VERSION.sspkg`

The collection should include at a minimum the following for a package called `MyPlugin@1.0.0.sspkg`:

```
 manifest.json
 README.md
 Changelog.txt
 LICENSE
 lib
 - MyPlugin.dll
 web
 - theme.css
```

The name of the entry DLL must match the name of the package.  SharpSite will attempt to load this DLL

## Manifest features and schema

The manifest should include information Like the following:

- unique id of the package
- display name of the package
- short description of the plugin for display on implemented sites
- version number of the package
- icon
- Published Date
- range of supported versions of SharpSite
- list of SharpSite packages this depends on
- list of NuGet packages this depends on
- Author name
- Contact name (optional)
- Contact email address
- Author website (optional)
- Source repository (optional)
- Tags
- Required SharpSite feature access

### Licenses

A package is required to either have a `LICENSE` file embedded or provide an entry in the manifest that references a known license such as LGPL, MIT, Apache, etc

### Sample manifest for the 'Foo Theme' plugin

```json
{ 
  "id": "foo.theme",
  "DisplayName": "Foo Theme",
  "Description": "This is a theme that looks like foo and uses the foo.css framework",
  "Version": "1.0.0-preview1",
  "Icon": "https://footheme.com/icon.png",
  "Published": "2024-12-12",
  "SupportedVersions": "0.4.0-0.5.0",
  "Author": "Foo Industries Inc.",
  "Contact": "John Foo",
  "ContactEmail": "john@footheme.com",
  "AuthorWebsite": "https://footheme.com",
  "Source": "https://github.com/footheme",
  "KnownLicense": "MIT",
  "Tags": ["theme", "foo", "bar"],
  "Features": ["theme"]
}
```

### Plugin Install Process

1. A plugin package should be uploaded to SharpSite using an Site Admin UI.  
2. The package should be saved in an isolated folder, `_uploaded`
3. Extract the manifest from the package and display the content on screen for the admin to review and grant permissions for the plugin
4. If approved, 
   1. Move the lib files into a `Plugins` child folder named after the plugin 
   2. Copy the `manifest.json` for each plugin into its `Plugins` folder
   3. Dynamically load the initial assembly 
   4. Load the manifest into the 'LoadedPlugins' application state information
   5. Move the web files into the `wwwroot/Plugins/PLUGIN_NAME/` folder

We need to enhance the website startup so that it loads the libraries and manifests from the `Plugins` folder.

### Enable / Disable plugins

We will want a way to have plugins downloaded, but not enabled

## Plugin Dependencies

We need to understand and provide a capability for plugins to define that they depend on other types of plugins.  This means that a payment processor plugin requires a GDPR cookie compliance plugin enabled as well.

What does this type of plugin relationship look like?  How do we enforce these requirements?

## System Plugins

Another class of plugins provides various system features like the following:

- Database storage for text-based content
- Database storage for security
- security system configuration (Entra, Keycloak, openid, etc)
- File storage for images and binary content

Each of these types of plugin's that support the architecture of the frameworkneed some sort of a contract that defines how the framework interacts with them, how other plugins interact with them, and how they're presented to the public on the website

### File Storage

Users may want to store their images, sound bytes, videos in several different mediums.We should be able to support storing with one of the public cloud services like Azure Blob storage, S3, or some mix of other capabilities. We should also be able to support storing data directly on disk, or as an embedded resource in a database

Should we providea storage mechanism for the metadata that goes along with the files that are being stored?

We should have a PluginFeatures enumerable value for file storage.

#### IHandleFiles interface

We should enable the standard crud operations with our interface. Instead of an update it should be a replace method.

```csharp

public record FileData(Stream File, FileMetaData Metadata);

public record FileMetaData(string FileName, DateTimeOffset CreateDate);

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
  Task AddFile(FileData file);

  /// <summary>
  /// Remove a file from storage
  /// </summary>
  Task RemoveFile(string filename);

}
```