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

## Plugin Install Process

The current implementation follows this process:

1. Plugin package is uploaded and handled by `HandleUploadedPlugin`:
   - Validates the uploaded package
   - Extracts and validates the manifest
   - Ensures plugin is not already installed
   - Stores manifest information temporarily

2. When `SavePlugin` is called:
   - Creates required plugin directories if they don't exist
   - Extracts the plugin package to appropriate folders:
     - Library files go to `plugins/{pluginId}@{version}/`
     - Web content goes to `plugins/_wwwroot/{pluginId}@{version}/`
     - Manifest is copied to the plugin folder
   - Loads plugin assembly dynamically
   - Registers plugin services and configuration
   - Updates application state
   - Applies theme if plugin contains theme features

3. At application startup, `LoadPluginsAtStartup`:
   - Scans the plugins directory
   - Loads manifests and assemblies
   - Registers all plugin services and configurations
   - Updates application state

## Plugin Storage Structure

The implementation uses the following directory structure:

```
plugins/
├── _uploaded/         # Temporary storage for uploaded plugins
├── _wwwroot/         # Web content from plugins
│   └── {pluginId}@{version}/
└── {pluginId}@{version}/  # Plugin library files
    ├── manifest.json
    └── lib/
```

## Plugin Services and Configuration

### Service Registration

The PluginManager supports automatic service registration through:

1. `RegisterPluginAttribute` for plugin features:
```csharp
[RegisterPlugin(PluginRegisterType.FileStorage, PluginServiceLocatorScope.Singleton)]
public class MyFileStorageHandler : IHandleFileStorage { }
```

Supported registration types:
- FileStorage → IHandleFileStorage
- DataStorage_Configuration → IConfigureDataStorage
- DataStorage_EfContext → Direct type registration
- DataStorage_PageRepository → IPageRepository
- DataStorage_PostRepository → IPostRepository

### Configuration Sections

Plugins can provide configuration sections by implementing `ISharpSiteConfigurationSection`:

```csharp
public interface ISharpSiteConfigurationSection 
{
    string SectionName { get; }
    Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldSection, IPluginManager pluginManager);
}
```

Configuration sections are:
- Automatically discovered and registered
- Added to ApplicationState.ConfigurationSections
- Notified of configuration changes via OnConfigurationChanged
- Available through dependency injection

### Plugin Service Access

Services provided by plugins can be accessed using:

```csharp
T? service = pluginManager.GetPluginProvidedService<T>();
```

## Plugin Features

Plugins declare their features in the manifest through the Features array. Current supported features:
- Theme: Allows the plugin to provide custom styling and layout

## Security and Validation

The implementation includes several security measures:

1. Path validation for plugin directories:
   - Prevents usage of invalid characters
   - Blocks reserved names
   - Validates path lengths
   - Prevents directory traversal

2. Plugin validation:
   - Ensures unique plugin IDs
   - Validates manifest contents
   - Prevents duplicate installations

3. Secure file handling:
   - Isolated plugin directories
   - Protected system directories (prefixed with '_')
   - Safe file extraction from packages

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

## Automatic Service Registration

The PluginManager will automatically register services from your plugin when specific attributes and interfaces are detected. This enables a plugin to seamlessly integrate with the SharpSite framework without manual registration code.

### Service Registration via Attributes

Classes decorated with the `RegisterPluginAttribute` will be automatically registered with the service locator. The following plugin types are supported:

- `FileStorage` - Registers as `IHandleFileStorage`
- `DataStorage_Configuration` - Registers as `IConfigureDataStorage`
- `DataStorage_EfContext` - Registers the class itself
- `DataStorage_PageRepository` - Registers as `IPageRepository`
- `DataStorage_PostRepository` - Registers as `IPostRepository`

Example usage:

```csharp
[RegisterPlugin(PluginRegisterType.FileStorage, PluginServiceLocatorScope.Singleton)]
public class MyFileStorageImplementation : IHandleFileStorage
{
    // Implementation
}
```

The second parameter of the RegisterPlugin attribute defines the service lifetime:

- `Singleton` - One instance for the entire application
- `Scoped` - One instance per scope (typically per request)
- `Transient` - New instance each time requested

### Configuration Section Registration

Classes that implement `ISharpSiteConfigurationSection` are automatically registered as configuration sections. These sections are:

1. Added to the `ApplicationState.ConfigurationSections` dictionary
2. Registered with the service locator for dependency injection
3. Have their `OnConfigurationChanged` method called when configuration changes occur

Example:

```csharp
public class MyPluginConfig : ISharpSiteConfigurationSection
{
    public string SectionName => "MyPlugin";
    
    public async Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldSection, IPluginManager pluginManager)
    {
        // Handle configuration changes
    }
}
```

### Startup Service Registration Process

The PluginManager handles service registration during application startup through a well-defined process:

1. **Initial Setup**
   - The PluginManager and ApplicationState are registered as singleton services
   - Memory cache services are added
   - Event handlers for configuration changes are set up

2. **Plugin Discovery and Loading**
   - The "plugins" directory is scanned for installed plugins
   - Each plugin's manifest.json is read and validated
   - Matching DLL files are loaded using `Plugin.LoadFromStream`
   - Plugin assemblies are added to the PluginAssemblyManager

3. **Service Registration**
   - Each plugin assembly is scanned using reflection
   - Classes with `RegisterPluginAttribute` are identified
   - Services are registered based on the PluginRegisterType:
     - File Storage (`IHandleFileStorage`)
     - Data Storage Configuration (`IConfigureDataStorage`)
     - Entity Framework Contexts
     - Page Repository (`IPageRepository`)
     - Post Repository (`IPostRepository`)
   - Configuration sections (`ISharpSiteConfigurationSection`) are discovered and registered

4. **Service Provider Creation**
   - After all services are registered, a service provider is built
   - The service provider is used to resolve dependencies throughout the application
   - When configuration changes occur, the service provider is rebuilt

5. **Dynamic Updates**
   - Configuration section changes trigger event handlers
   - Old configuration sections are replaced with new ones
   - Service provider is rebuilt to reflect changes

This process ensures that:

- Plugins are loaded in a predictable order
- Services are properly scoped (Singleton, Scoped, or Transient)
- Configuration changes are properly propagated
- Dependencies are correctly resolved through the service provider
