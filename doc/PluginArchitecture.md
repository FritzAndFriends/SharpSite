# Plugin Architecture

SharpSite should support a rich ecosystem of plugins that allow administrator to change the look, feel, and capabilities of a SharpSite application

Plugins are features that are not distributed with SharpSite but can be added after the SharpSite application is already started and deployed.


## Plugins are packages of files

A Plugin should contain a collection of files, compressed in ZIP format, and renamed with a SSPKG extension.  The version number should appear in the filename before the SSPKG extension, separated from the package name with an `@` character.

Filename format:  `NAME@VERSION.sspkg`

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
2. The package should be saved in an isolated folder, 'quarantine' or similar
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