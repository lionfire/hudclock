# Convert SVG to ICO using Inkscape or ImageMagick
# This script requires either Inkscape or ImageMagick to be installed

param(
    [string]$SvgPath = "..\assets\icon\hud_clock_icon.svg",
    [string]$OutputPath = "..\src\wpf\HudClock.ico"
)

$ErrorActionPreference = "Stop"

# Resolve paths
$SvgPath = Resolve-Path $SvgPath
$OutputDir = Split-Path $OutputPath -Parent
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}
$OutputPath = Join-Path (Resolve-Path $OutputDir) (Split-Path $OutputPath -Leaf)

Write-Host "Converting SVG to ICO..." -ForegroundColor Green
Write-Host "Source: $SvgPath"
Write-Host "Output: $OutputPath"

# Method 1: Try Inkscape (preferred)
$inkscapePath = @(
    "C:\Program Files\Inkscape\bin\inkscape.exe",
    "C:\Program Files\Inkscape\inkscape.exe",
    "C:\Program Files (x86)\Inkscape\bin\inkscape.exe",
    "C:\Program Files (x86)\Inkscape\inkscape.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if ($inkscapePath) {
    Write-Host "Using Inkscape..." -ForegroundColor Yellow
    
    # Create PNG files at different sizes
    $sizes = @(16, 32, 48, 64, 128, 256)
    $pngFiles = @()
    
    foreach ($size in $sizes) {
        $pngPath = Join-Path $env:TEMP "hudclock_icon_${size}.png"
        Write-Host "  Creating ${size}x${size} PNG..."
        
        & $inkscapePath `
            --export-type="png" `
            --export-filename="$pngPath" `
            --export-width=$size `
            --export-height=$size `
            $SvgPath
            
        $pngFiles += $pngPath
    }
    
    # Use ImageMagick to combine PNGs into ICO
    $magickPath = Get-Command "magick.exe" -ErrorAction SilentlyContinue
    if ($magickPath) {
        Write-Host "Combining PNGs into ICO using ImageMagick..." -ForegroundColor Yellow
        & magick.exe $pngFiles $OutputPath
        
        # Clean up temp files
        $pngFiles | ForEach-Object { Remove-Item $_ -Force }
        
        Write-Host "ICO file created successfully!" -ForegroundColor Green
        return
    }
}

# Method 2: Try ImageMagick directly
$magickPath = Get-Command "magick.exe" -ErrorAction SilentlyContinue
if ($magickPath) {
    Write-Host "Using ImageMagick..." -ForegroundColor Yellow
    
    # Convert SVG directly to ICO with multiple sizes
    & magick.exe convert `
        $SvgPath `
        -background none `
        -density 300 `
        -define icon:auto-resize="256,128,64,48,32,16" `
        $OutputPath
        
    if ($LASTEXITCODE -eq 0) {
        Write-Host "ICO file created successfully!" -ForegroundColor Green
        return
    }
}

# Method 3: Create a simple PNG-to-ICO converter script
Write-Host "Neither Inkscape nor ImageMagick found!" -ForegroundColor Red
Write-Host @"

To convert SVG to ICO, please install one of the following:

1. Inkscape (recommended):
   Download from: https://inkscape.org/release/

2. ImageMagick:
   Download from: https://imagemagick.org/script/download.php#windows
   
3. Or use an online converter:
   - https://cloudconvert.com/svg-to-ico
   - https://convertio.co/svg-ico/
   
After converting, place the ICO file at:
$OutputPath

Then uncomment the ApplicationIcon line in MetricClock.csproj
"@ -ForegroundColor Yellow

# Create a batch file for manual conversion
$batchContent = @"
@echo off
echo Please install Inkscape or ImageMagick to use this converter.
echo.
echo Inkscape: https://inkscape.org/release/
echo ImageMagick: https://imagemagick.org/script/download.php#windows
echo.
echo Or use online converters:
echo - https://cloudconvert.com/svg-to-ico
echo - https://convertio.co/svg-ico/
pause
"@

$batchPath = Join-Path (Split-Path $OutputPath -Parent) "convert-icon.bat"
$batchContent | Out-File -FilePath $batchPath -Encoding ASCII
Write-Host "Created helper batch file at: $batchPath" -ForegroundColor Cyan