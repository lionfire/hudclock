# PowerShell script to create HudClock installer locally
# This script can be run locally if you have Inno Setup installed

param(
    [string]$Version = "1.0.0",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "Creating HudClock Installer v$Version" -ForegroundColor Green

# Paths
$projectPath = Join-Path $PSScriptRoot "..\src\wpf\MetricClock.csproj"
$outputPath = Join-Path $PSScriptRoot "output"
$publishPath = Join-Path $outputPath "publish"
$installerPath = Join-Path $outputPath "installer"

# Clean and create output directories
if (Test-Path $outputPath) {
    Remove-Item $outputPath -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $publishPath | Out-Null
New-Item -ItemType Directory -Force -Path $installerPath | Out-Null

# Build and publish the application
Write-Host "Building application..." -ForegroundColor Yellow
dotnet publish $projectPath `
    --configuration $Configuration `
    --runtime win-x64 `
    --no-self-contained `
    --output $publishPath

# Create Inno Setup script
$innoScript = @"
[Setup]
AppId={{A8B3F4E2-6C7D-4E5F-9A1B-3D2E5F7A9B1C}
AppName=HudClock
AppVersion=$Version
AppVerName=HudClock $Version
AppPublisher=LionFire
AppPublisherURL=https://github.com/lionfire/hudclock
AppSupportURL=https://github.com/lionfire/hudclock/issues
AppUpdatesURL=https://github.com/lionfire/hudclock/releases
DefaultDirName={autopf}\HudClock
DisableProgramGroupPage=yes
LicenseFile=$PSScriptRoot\..\LICENSE
OutputDir=$installerPath
OutputBaseFilename=HudClock-Setup-v$Version
SetupIconFile=$PSScriptRoot\..\src\wpf\HudClock.ico
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
UninstallDisplayIcon={app}\HudClock.exe
UninstallDisplayName=HudClock
VersionInfoVersion=$Version.0

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "Start HudClock when Windows starts"; GroupDescription: "Additional options:"; Flags: unchecked

[Files]
Source: "$publishPath\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\HudClock"; Filename: "{app}\HudClock.exe"
Name: "{autodesktop}\HudClock"; Filename: "{app}\HudClock.exe"; Tasks: desktopicon
Name: "{userstartup}\HudClock"; Filename: "{app}\HudClock.exe"; Tasks: startupicon

[Run]
Filename: "{app}\HudClock.exe"; Description: "{cm:LaunchProgram,HudClock}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\HudClock"
"@

$innoScriptPath = Join-Path $outputPath "HudClock.iss"
$innoScript | Out-File -FilePath $innoScriptPath -Encoding UTF8

# Check if Inno Setup is installed
$innoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $innoSetupPath)) {
    Write-Host "Inno Setup not found. Please install it from: https://jrsoftware.org/isdl.php" -ForegroundColor Red
    Write-Host "Installer script created at: $innoScriptPath" -ForegroundColor Yellow
    exit 1
}

# Compile the installer
Write-Host "Creating installer..." -ForegroundColor Yellow
& $innoSetupPath $innoScriptPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Installer created successfully!" -ForegroundColor Green
    Write-Host "Location: $installerPath\HudClock-Setup-v$Version.exe" -ForegroundColor Cyan
} else {
    Write-Host "Failed to create installer" -ForegroundColor Red
    exit 1
}

# Also create a portable ZIP
Write-Host "Creating portable ZIP..." -ForegroundColor Yellow
$zipPath = Join-Path $installerPath "HudClock-v$Version-portable.zip"
Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath -Force
Write-Host "Portable ZIP created at: $zipPath" -ForegroundColor Cyan