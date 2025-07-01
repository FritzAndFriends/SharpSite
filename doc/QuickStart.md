# Quick Start: Creating Your First Plugin

This guide shows you how to create a simple theme plugin from scratch using the SharpSite.PluginPacker.

## Step 1: Create a New Plugin Project

```bash
# Create a new directory for your plugin
mkdir MyFirstPlugin
cd MyFirstPlugin

# Create a new Razor class library project
dotnet new razorclasslib -n MyFirstPlugin --framework net9.0
cd MyFirstPlugin
```

## Step 2: Configure Project References

Edit `MyFirstPlugin.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <SupportedPlatform Include="browser" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\SharpSite.Abstractions.Theme\SharpSite.Abstractions.Theme.csproj" />
  </ItemGroup>
</Project>
```

## Step 3: Create Plugin Implementation

Create `MyTheme.cs`:

```csharp
using SharpSite.Abstractions.Theme;

namespace MyFirstPlugin;

public class MyTheme : IHasStylesheets
{
    public string[] Stylesheets => [
        "theme.css"
    ];
}
```

## Step 4: Add Theme Styles

Create `wwwroot/theme.css`:

```css
/* My First Plugin Theme */
body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: #333;
}

h1, h2, h3 {
    color: #2c3e50;
    text-shadow: 1px 1px 2px rgba(0,0,0,0.1);
}

.container {
    background: rgba(255, 255, 255, 0.95);
    border-radius: 10px;
    padding: 20px;
    margin: 20px auto;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}
```

## Step 5: Add Required Files

Create `README.md`:

```markdown
# My First Plugin

A beautiful gradient theme for SharpSite.

## Features

- Modern gradient background
- Clean typography
- Responsive design
- Professional styling

## Installation

Upload the .sspkg file through the SharpSite admin interface.
```

Create `LICENSE`:

```
MIT License

Copyright (c) 2024 Your Name

Permission is hereby granted, free of charge, to any person obtaining a copy...
```

Create `Changelog.txt`:

```
1.0.0 (2024-12-12)
- Initial release
- Beautiful gradient theme
- Responsive design
```

## Step 6: Package Your Plugin

Use the provided packaging script:

```bash
# From the SharpSite root directory
./scripts/package-plugin.sh -i ./MyFirstPlugin -o ./dist
```

Or use the PluginPacker directly:

```bash
dotnet run --project src/SharpSite.PluginPacker -- -i ./MyFirstPlugin -o ./dist
```

## Step 7: Install and Test

1. The packager will create `my.first.plugin@1.0.0.sspkg` in the dist folder
2. Upload this file through your SharpSite admin interface
3. Enable the plugin and see your theme in action!

## What You've Learned

- How to structure a plugin project
- How to implement the IHasStylesheets interface
- How to include CSS assets in your plugin
- How to use the PluginPacker to create distributable packages
- How to include proper documentation and metadata

## Next Steps

- Explore other plugin types (FileStorage, etc.)
- Add more complex styling and features
- Create Razor components for your theme
- Study the sample plugins for more advanced examples