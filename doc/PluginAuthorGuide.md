# Plugin Author Guide

This guide provides comprehensive instructions for creating, building, and packaging plugins for SharpSite using the SharpSite.PluginPacker utility.

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Plugin Project Structure](#plugin-project-structure)
4. [Creating a New Plugin](#creating-a-new-plugin)
5. [Using SharpSite.PluginPacker](#using-sharpsite-pluginpacker)
6. [Manifest.json Configuration](#manifest-json-configuration)
7. [Building and Packaging](#building-and-packaging)
8. [Plugin Types and Examples](#plugin-types-and-examples)
9. [Testing Your Plugin](#testing-your-plugin)
10. [Best Practices](#best-practices)
11. [Troubleshooting](#troubleshooting)

## Overview

SharpSite supports a rich ecosystem of plugins that allow administrators to extend the look, feel, and capabilities of a SharpSite application. Plugins are distributed as `.sspkg` files (renamed ZIP files) that contain the compiled plugin library, manifest information, and any required assets.

The **SharpSite.PluginPacker** utility automates the entire plugin packaging process, from building your project to creating the final `.sspkg` file.

## Prerequisites

- .NET 9.0 SDK or later (minimum .NET 8.0 SDK)
- Visual Studio, VS Code, or your preferred C# development environment
- SharpSite source code (to reference abstractions)
- Access to the SharpSite.PluginPacker utility

**Note**: The v0.7 branch targets .NET 9.0, but plugins can be developed for .NET 8.0 if needed for compatibility.

## Plugin Project Structure

A typical plugin project should follow this structure:

```
MyPlugin/
├── MyPlugin.csproj              # Project file
├── manifest.json                # Plugin metadata (created by PluginPacker if missing)
├── README.md                    # Plugin documentation
├── LICENSE                      # License file
├── Changelog.txt               # Version history
├── PluginClass.cs              # Main plugin implementation
├── wwwroot/                    # Web assets (for theme plugins)
│   └── theme.css              # CSS files
└── _Imports.razor             # Razor imports (if needed)
```

## Creating a New Plugin

### Step 1: Create a New Project

Create a new .NET class library or Razor class library:

```bash
dotnet new classlib -n MyAwesomePlugin
cd MyAwesomePlugin
```

For theme plugins that include Razor components:
```bash
dotnet new razorclasslib -n MyAwesomeTheme
cd MyAwesomeTheme
```

### Step 2: Add Required References

Add references to the appropriate SharpSite abstractions:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\SharpSite.Abstractions.Theme\SharpSite.Abstractions.Theme.csproj" />
    <!-- Add other abstractions as needed -->
  </ItemGroup>
</Project>
```

### Step 3: Implement Plugin Interface

Implement the appropriate interface for your plugin type. For example, a theme plugin:

```csharp
using SharpSite.Abstractions.Theme;

namespace MyAwesomeTheme;

public class MyTheme : IHasStylesheets
{
    public string[] Stylesheets => [
        "theme.css"
    ];
}
```

### Step 4: Add Required Files

Create the following files in your project root:

- **README.md**: Document your plugin's features and usage
- **LICENSE**: Include your plugin's license
- **Changelog.txt**: Track version changes

## Using SharpSite.PluginPacker

The SharpSite.PluginPacker is a command-line utility that automates the plugin packaging process.

### Building the PluginPacker

First, build the PluginPacker utility:

```bash
cd path/to/SharpSite/src/SharpSite.PluginPacker
dotnet build --configuration Release
```

### Basic Usage

```bash
dotnet run --project path/to/SharpSite/src/SharpSite.PluginPacker -- -i <input-folder> [-o <output-folder>]
```

**Parameters:**
- `-i, --input`: Input folder containing the plugin project (required)
- `-o, --output`: Output directory for the .sspkg file (optional, defaults to current directory)

### Example Usage

```bash
# Package a plugin project
dotnet run --project ../SharpSite/src/SharpSite.PluginPacker -- -i ./MyAwesomePlugin -o ./dist

# This will create: ./dist/my.awesome.plugin@1.0.0.sspkg
```

### What the PluginPacker Does

1. **Validates the input directory** exists
2. **Loads or creates manifest.json** interactively if missing
3. **Builds the project** in Release configuration
4. **Creates the package structure**:
   - Copies the compiled DLL to `lib/` folder and renames it to match the plugin ID
   - For theme plugins: copies CSS files from `wwwroot/` to `web/` folder
   - Includes required files: `manifest.json`, `LICENSE`, `README.md`, `Changelog.txt`
5. **Creates the .sspkg file** with naming format: `ID@VERSION.sspkg`

## Manifest.json Configuration

The manifest.json file contains metadata about your plugin. If it doesn't exist, the PluginPacker will create one interactively.

### Required Fields

```json
{
  "id": "my.awesome.plugin",
  "DisplayName": "My Awesome Plugin",
  "Description": "A fantastic plugin that does amazing things",
  "Version": "1.0.0",
  "Published": "2024-12-12",
  "SupportedVersions": "0.7.0-0.8.0",
  "Author": "Your Name",
  "Contact": "Your Name",
  "ContactEmail": "you@example.com",
  "AuthorWebsite": "https://yourwebsite.com",
  "Source": "https://github.com/yourusername/your-plugin",
  "KnownLicense": "MIT",
  "Tags": ["theme", "blue", "modern"],
  "Features": ["Theme"]
}
```

### Optional Fields

- `Icon`: URL to plugin icon
- `Source`: Repository URL
- `KnownLicense`: Standard license identifier (MIT, Apache, LGPL, etc.)
- `Tags`: Array of descriptive tags
- `Features`: Array of plugin features (Theme, FileStorage, etc.)

### Interactive Manifest Creation

If no manifest.json exists, the PluginPacker will prompt you for required information:

```
Id: my.awesome.plugin
DisplayName: My Awesome Plugin
Description: A fantastic plugin that does amazing things
Version: 1.0.0
Published (yyyy-MM-dd): 2024-12-12
SupportedVersions: 0.7.0-0.8.0
Author: Your Name
Contact: Your Name
ContactEmail: you@example.com
AuthorWebsite: https://yourwebsite.com
Icon (URL): 
Source (repository URL): https://github.com/yourusername/your-plugin
KnownLicense (e.g. MIT, Apache, LGPL): MIT
Tags (comma separated): theme, blue, modern
Features (comma separated, e.g. Theme,FileStorage): Theme
```

## Building and Packaging

### Step-by-Step Process

1. **Prepare your project**: Ensure all required files are present
2. **Run the PluginPacker**:
   ```bash
   dotnet run --project path/to/SharpSite/src/SharpSite.PluginPacker -- -i ./MyPlugin -o ./dist
   ```
3. **Review the output**: The packager will show progress and create `MyPlugin@1.0.0.sspkg`

### Package Contents

The generated .sspkg file contains:

```
MyPlugin@1.0.0.sspkg
├── manifest.json           # Plugin metadata
├── README.md              # Plugin documentation
├── LICENSE                # License file
├── Changelog.txt          # Version history
├── lib/
│   └── my.plugin.id.dll   # Renamed plugin DLL
└── web/                   # Web assets (theme plugins only)
    └── theme.css          # CSS files
```

## Plugin Types and Examples

### Theme Plugin Example

**Project Structure:**
```
MyTheme/
├── MyTheme.csproj
├── MyTheme.cs
├── _Imports.razor
├── wwwroot/
│   └── theme.css
├── README.md
├── LICENSE
└── Changelog.txt
```

**MyTheme.cs:**
```csharp
using SharpSite.Abstractions.Theme;

namespace MyTheme;

public class MyTheme : IHasStylesheets
{
    public string[] Stylesheets => [
        "theme.css"
    ];
}
```

**wwwroot/theme.css:**
```css
h1 {
    color: #0066cc;
    font-family: 'Segoe UI', sans-serif;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
}
```

### File Storage Plugin Example

**Project Structure:**
```
MyFileStorage/
├── MyFileStorage.csproj
├── FileSystemStorage.cs
├── README.md
├── LICENSE
└── Changelog.txt
```

**FileSystemStorage.cs:**
```csharp
using SharpSite.Abstractions.FileStorage;

namespace MyFileStorage;

public class FileSystemStorage : IHandleFileStorage
{
    // Implement IHandleFileStorage interface
    // ... implementation details
}
```

## Testing Your Plugin

### Local Testing

1. **Package your plugin** using PluginPacker
2. **Copy the .sspkg file** to your SharpSite instance
3. **Upload via Admin UI** to test installation
4. **Verify functionality** in the SharpSite application

### Validation Checklist

- [ ] Plugin compiles successfully
- [ ] Manifest.json is valid and complete
- [ ] Required files are included
- [ ] CSS files are properly copied (theme plugins)
- [ ] Plugin loads without errors
- [ ] Functionality works as expected

## Best Practices

### Project Organization

- **Use meaningful namespaces** that match your plugin ID
- **Follow C# naming conventions**
- **Include comprehensive documentation**
- **Maintain a clear changelog**

### Manifest Guidelines

- **Use semantic versioning** (e.g., 1.0.0, 1.2.3-beta1)
- **Specify accurate version ranges** for SharpSite compatibility
- **Include descriptive tags** for discoverability
- **Provide contact information** for support

### Code Quality

- **Enable nullable reference types**
- **Use dependency injection** where appropriate
- **Handle errors gracefully**
- **Follow async/await patterns** for I/O operations
- **Include unit tests** for your plugin logic

### Packaging

- **Test packaging locally** before distribution
- **Verify file structure** in the generated .sspkg
- **Include all necessary dependencies**
- **Keep packages small** by excluding unnecessary files

## Troubleshooting

### Common Issues

**"DLL not found" Error:**
- Ensure your project builds successfully
- Check that the project name matches the expected DLL name
- Verify the build output directory

**"Failed to parse manifest.json":**
- Validate JSON syntax using a JSON validator
- Ensure all required fields are present
- Check that feature names match expected values

**Missing CSS Files:**
- Verify CSS files are in the `wwwroot/` directory
- Ensure your plugin implements `IHasStylesheets`
- Check that the Features array includes "Theme"

### Debug Mode

Run the PluginPacker with detailed output to troubleshoot:

```bash
dotnet run --project path/to/SharpSite/src/SharpSite.PluginPacker -- -i ./MyPlugin -o ./dist --verbose
```

### Getting Help

- Check the [SharpSite documentation](../README.md)
- Review [sample plugins](../../plugins/) for reference
- Report issues on the [SharpSite GitHub repository](https://github.com/FritzAndFriends/SharpSite)

## Conclusion

The SharpSite.PluginPacker utility streamlines the plugin development workflow by automating the build, packaging, and validation process. By following this guide, you can create professional, distributable plugins that extend SharpSite's capabilities.

For more information about plugin architecture, see [PluginArchitecture.md](./PluginArchitecture.md).