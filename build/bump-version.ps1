#!/usr/bin/env pwsh
# Script to bump version numbers for HudClock
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch")]
    [string]$BumpType,
    
    [switch]$CreateTag,
    [switch]$Push
)

$ErrorActionPreference = "Stop"

# Read current version
$versionFile = Join-Path $PSScriptRoot "..\VERSION"
$currentVersion = (Get-Content $versionFile).Trim()
$versionParts = $currentVersion.Split('.')

if ($versionParts.Count -ne 3) {
    throw "Invalid version format. Expected X.Y.Z"
}

$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

# Bump version
switch ($BumpType) {
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "patch" {
        $patch++
    }
}

$newVersion = "$major.$minor.$patch"

Write-Host "Bumping version from $currentVersion to $newVersion" -ForegroundColor Green

# Update VERSION file
$newVersion | Out-File -FilePath $versionFile -NoNewline -Encoding UTF8

# Test build to ensure version is valid
Write-Host "Testing build with new version..." -ForegroundColor Yellow
$buildResult = & dotnet build "$PSScriptRoot\..\src\wpf\MetricClock.csproj" --configuration Release 2>&1
if ($LASTEXITCODE -ne 0) {
    # Revert on failure
    $currentVersion | Out-File -FilePath $versionFile -NoNewline -Encoding UTF8
    throw "Build failed with new version. Version reverted to $currentVersion"
}

Write-Host "Build successful!" -ForegroundColor Green

# Git operations
if ($CreateTag -or $Push) {
    # Commit version change
    git add "$versionFile"
    git commit -m "Bump version to $newVersion"
    
    if ($CreateTag) {
        Write-Host "Creating tag v$newVersion..." -ForegroundColor Yellow
        
        # Generate release notes (you can customize this)
        $releaseNotes = @"
Release v$newVersion

[Changes since v$currentVersion](https://github.com/lionfire/hudclock/compare/v$currentVersion...v$newVersion)
"@
        
        git tag -a "v$newVersion" -m "$releaseNotes"
        Write-Host "Tag v$newVersion created" -ForegroundColor Green
    }
    
    if ($Push) {
        Write-Host "Pushing to remote..." -ForegroundColor Yellow
        git push
        if ($CreateTag) {
            git push origin "v$newVersion"
        }
        Write-Host "Pushed successfully!" -ForegroundColor Green
    }
}

Write-Host @"

Version bumped to $newVersion

Next steps:
"@ -ForegroundColor Cyan

if (-not $CreateTag) {
    Write-Host "  - Create tag: git tag -a v$newVersion -m 'Release v$newVersion'" -ForegroundColor Yellow
}
if (-not $Push) {
    Write-Host "  - Push changes: git push" -ForegroundColor Yellow
    if ($CreateTag) {
        Write-Host "  - Push tag: git push origin v$newVersion" -ForegroundColor Yellow
    }
}

Write-Host @"
  - Monitor CI/CD: https://github.com/lionfire/hudclock/actions
  - View releases: https://github.com/lionfire/hudclock/releases
"@ -ForegroundColor Yellow
