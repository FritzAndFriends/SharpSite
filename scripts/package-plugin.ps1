# SharpSite Plugin Packaging Script (PowerShell)
# This script provides an easy way to package SharpSite plugins using the PluginPacker utility

param(
    [Parameter(Mandatory=$true, HelpMessage="Input directory containing the plugin project")]
    [Alias("i")]
    [string]$InputDir,
    
    [Parameter(HelpMessage="Output directory for the .sspkg file (default: ./dist)")]
    [Alias("o")]
    [string]$OutputDir = "./dist",
    
    [Parameter(HelpMessage="Path to SharpSite source directory (default: auto-detect)")]
    [Alias("s")]
    [string]$SharpSiteDir = "",
    
    [Parameter(HelpMessage="Show help message")]
    [Alias("h")]
    [switch]$Help
)

# Function to show usage
function Show-Usage {
    Write-Host @"
SharpSite Plugin Packaging Script

Usage: .\package-plugin.ps1 [options]

Options:
    -InputDir, -i DIR       Input directory containing the plugin project (required)
    -OutputDir, -o DIR      Output directory for the .sspkg file (default: ./dist)
    -SharpSiteDir, -s DIR   Path to SharpSite source directory (default: auto-detect)
    -Help, -h               Show this help message

Examples:
    .\package-plugin.ps1 -InputDir ./MyPlugin
    .\package-plugin.ps1 -i ./MyPlugin -o ./output
    .\package-plugin.ps1 -i ./MyPlugin -o ./output -s ../SharpSite

"@
}

# Function to print colored output
function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

# Show help if requested
if ($Help) {
    Show-Usage
    exit 0
}

# Validate required parameters
if (-not $InputDir) {
    Write-Error "Input directory is required"
    Show-Usage
    exit 1
}

if (-not (Test-Path $InputDir -PathType Container)) {
    Write-Error "Input directory '$InputDir' does not exist"
    exit 1
}

# Auto-detect SharpSite directory if not provided
if (-not $SharpSiteDir) {
    $CurrentDir = Get-Location
    while ($CurrentDir -and $CurrentDir.Path -ne $CurrentDir.Root) {
        $SolutionFile = Join-Path $CurrentDir.Path "SharpSite.sln"
        if (Test-Path $SolutionFile) {
            $SharpSiteDir = $CurrentDir.Path
            break
        }
        $CurrentDir = $CurrentDir.Parent
    }
    
    if (-not $SharpSiteDir) {
        Write-Error "Could not auto-detect SharpSite directory. Please specify with -SharpSiteDir option."
        exit 1
    }
}

# Validate SharpSite directory
$PluginPackerDir = Join-Path $SharpSiteDir "src\SharpSite.PluginPacker"
if (-not (Test-Path $PluginPackerDir -PathType Container)) {
    Write-Error "SharpSite.PluginPacker not found at '$PluginPackerDir'"
    Write-Error "Please ensure you have the correct SharpSite source directory"
    exit 1
}

# Create output directory if it doesn't exist
if (-not (Test-Path $OutputDir -PathType Container)) {
    Write-Info "Creating output directory: $OutputDir"
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Get absolute paths
$InputDir = Resolve-Path $InputDir
$OutputDir = Resolve-Path $OutputDir
$PluginPackerDir = Resolve-Path $PluginPackerDir

Write-Info "Starting plugin packaging..."
Write-Info "Input directory: $InputDir"
Write-Info "Output directory: $OutputDir"
Write-Info "PluginPacker: $PluginPackerDir"

# Check if manifest.json exists
$ManifestPath = Join-Path $InputDir "manifest.json"
if (-not (Test-Path $ManifestPath)) {
    Write-Warning "No manifest.json found in input directory"
    Write-Warning "The PluginPacker will prompt you to create one interactively"
}

# Build and run the PluginPacker
Write-Info "Building PluginPacker..."
Push-Location $PluginPackerDir
try {
    $BuildResult = dotnet build --configuration Release 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build PluginPacker"
        Write-Host $BuildResult
        exit 1
    }
    
    Write-Success "PluginPacker built successfully"
    
    Write-Info "Packaging plugin..."
    $PackageResult = dotnet run --configuration Release --no-build -- -i "$InputDir" -o "$OutputDir" 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Plugin packaged successfully!"
        
        # Find and display the generated package
        $PackageFiles = Get-ChildItem -Path $OutputDir -Filter "*.sspkg" | Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-1) }
        if ($PackageFiles) {
            $PackageFile = $PackageFiles[0]
            $PackageSize = [math]::Round($PackageFile.Length / 1KB, 2)
            Write-Success "Generated package: $($PackageFile.Name) ($PackageSize KB)"
            Write-Info "Location: $($PackageFile.FullName)"
        }
    } else {
        Write-Error "Plugin packaging failed"
        Write-Host $PackageResult
        exit 1
    }
} finally {
    Pop-Location
}

Write-Success "Plugin packaging completed successfully!"