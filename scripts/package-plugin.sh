#!/bin/bash

# SharpSite Plugin Packaging Script
# This script provides an easy way to package SharpSite plugins using the PluginPacker utility

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Function to show usage
show_usage() {
    cat << EOF
SharpSite Plugin Packaging Script

Usage: $0 [options]

Options:
    -i, --input DIR     Input directory containing the plugin project (required)
    -o, --output DIR    Output directory for the .sspkg file (default: ./dist)
    -s, --sharpsite DIR Path to SharpSite source directory (default: auto-detect)
    -h, --help          Show this help message

Examples:
    $0 -i ./MyPlugin
    $0 -i ./MyPlugin -o ./output
    $0 -i ./MyPlugin -o ./output -s ../SharpSite

EOF
}

# Default values
INPUT_DIR=""
OUTPUT_DIR="./dist"
SHARPSITE_DIR=""

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -i|--input)
            INPUT_DIR="$2"
            shift 2
            ;;
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        -s|--sharpsite)
            SHARPSITE_DIR="$2"
            shift 2
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Validate required parameters
if [[ -z "$INPUT_DIR" ]]; then
    print_error "Input directory is required"
    show_usage
    exit 1
fi

if [[ ! -d "$INPUT_DIR" ]]; then
    print_error "Input directory '$INPUT_DIR' does not exist"
    exit 1
fi

# Auto-detect SharpSite directory if not provided
if [[ -z "$SHARPSITE_DIR" ]]; then
    # Look for SharpSite.sln in current directory and parent directories
    CURRENT_DIR="$(pwd)"
    while [[ "$CURRENT_DIR" != "/" ]]; do
        if [[ -f "$CURRENT_DIR/SharpSite.sln" ]]; then
            SHARPSITE_DIR="$CURRENT_DIR"
            break
        fi
        CURRENT_DIR="$(dirname "$CURRENT_DIR")"
    done
    
    if [[ -z "$SHARPSITE_DIR" ]]; then
        print_error "Could not auto-detect SharpSite directory. Please specify with -s option."
        exit 1
    fi
fi

# Validate SharpSite directory
PLUGINPACKER_DIR="$SHARPSITE_DIR/src/SharpSite.PluginPacker"
if [[ ! -d "$PLUGINPACKER_DIR" ]]; then
    print_error "SharpSite.PluginPacker not found at '$PLUGINPACKER_DIR'"
    print_error "Please ensure you have the correct SharpSite source directory"
    exit 1
fi

# Create output directory if it doesn't exist
if [[ ! -d "$OUTPUT_DIR" ]]; then
    print_info "Creating output directory: $OUTPUT_DIR"
    mkdir -p "$OUTPUT_DIR"
fi

# Get absolute paths
INPUT_DIR="$(cd "$INPUT_DIR" && pwd)"
OUTPUT_DIR="$(cd "$OUTPUT_DIR" && pwd)"
PLUGINPACKER_DIR="$(cd "$PLUGINPACKER_DIR" && pwd)"

print_info "Starting plugin packaging..."
print_info "Input directory: $INPUT_DIR"
print_info "Output directory: $OUTPUT_DIR"
print_info "PluginPacker: $PLUGINPACKER_DIR"

# Check if manifest.json exists
if [[ ! -f "$INPUT_DIR/manifest.json" ]]; then
    print_warning "No manifest.json found in input directory"
    print_warning "The PluginPacker will prompt you to create one interactively"
fi

# Build and run the PluginPacker
print_info "Building PluginPacker..."
cd "$PLUGINPACKER_DIR"
if ! dotnet build --configuration Release > /dev/null 2>&1; then
    print_error "Failed to build PluginPacker"
    exit 1
fi

print_success "PluginPacker built successfully"

print_info "Packaging plugin..."
if dotnet run --configuration Release --no-build -- -i "$INPUT_DIR" -o "$OUTPUT_DIR"; then
    print_success "Plugin packaged successfully!"
    
    # Find and display the generated package
    PACKAGE_FILE=$(find "$OUTPUT_DIR" -name "*.sspkg" -type f -newer "$PLUGINPACKER_DIR" | head -n 1)
    if [[ -n "$PACKAGE_FILE" ]]; then
        PACKAGE_SIZE=$(du -h "$PACKAGE_FILE" | cut -f1)
        print_success "Generated package: $(basename "$PACKAGE_FILE") ($PACKAGE_SIZE)"
        print_info "Location: $PACKAGE_FILE"
    fi
else
    print_error "Plugin packaging failed"
    exit 1
fi

print_success "Plugin packaging completed successfully!"