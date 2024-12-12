# Plugin Architecture

SharpSite should support a rich ecosystem of plugins that allow administrator to change the look, feel, and capabilities of a SharpSite application

Plugins are features that are not distributed with SharpSite but can be added after the SharpSite application is already started and deployed.


## Plugins are packages of files

A Plugin should contain a collection of files, compressed in ZIP format, and renamed with a SSPKG extension.  The version number should appear in the filename before the SSPKG extension, separated from the package name with an `@` character.

Filename format:  `NAME@VERSION.sspkg`

The collection should include at a minimum the following for a package called `MyPlugin@1.0.0.sspkg`:

```
 manifest.json
 MyPlugin.dll
 README.md
 Changelog.txt
 LICENSE
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