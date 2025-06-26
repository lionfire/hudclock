# Building HudClock

## Automated Builds (GitHub Actions)

The project uses GitHub Actions to automatically build and package HudClock whenever you:
- Push to the main branch
- Create a pull request
- Create a new release tag (e.g., `v1.0.0`)

### Build Outputs

The GitHub Actions workflow creates three types of packages:

1. **Self-Contained Single EXE** (`HudClock-win-x64-self-contained.zip`)
   - All-in-one executable (~150MB)
   - No .NET installation required
   - Just extract and run

2. **Framework-Dependent Single EXE** (`HudClock-win-x64-framework-dependent.zip`)
   - Smaller executable (~1MB)
   - Requires .NET 8 Desktop Runtime
   - Good for users who already have .NET

3. **Portable Multi-File** (`HudClock-win-x64-portable.zip`)
   - Traditional portable app structure
   - Multiple DLL files
   - Smallest download size

## Local Builds

### Prerequisites
- .NET 8 SDK
- Windows 10 or later
- (Optional) Inno Setup 6 for creating installers

### Quick Build
```powershell
# Build the application
dotnet build src/wpf/MetricClock.csproj --configuration Release

# Create a single-file executable
dotnet publish src/wpf/MetricClock.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### Create Installer (Local)

Run the PowerShell script to create an installer:

```powershell
.\build\create-installer.ps1 -Version 1.0.0
```

This creates:
- `HudClock-Setup-v1.0.0.exe` - Windows installer
- `HudClock-v1.0.0-portable.zip` - Portable version

Note: Requires [Inno Setup](https://jrsoftware.org/isdl.php) to be installed.

## Creating a Release

1. Update version in `src/wpf/MetricClock.csproj`
2. Commit your changes
3. Create and push a tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
4. GitHub Actions will automatically:
   - Build all package types
   - Create a GitHub Release
   - Upload the packages as release assets

## MSI Installer

For enterprise deployments requiring MSI packages, you can use WiX Toolset:

1. Install WiX: `dotnet tool install --global wix`
2. Create WiX source files (see documentation)
3. Build MSI: `wix build HudClock.wxs`

Note: MSI creation is not included in the automated workflow due to complexity.