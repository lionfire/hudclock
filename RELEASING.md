# Releasing HudClock

## Version Management

HudClock uses a centralized version management system:
- Version is stored in `/VERSION` file (e.g., `1.0.0`)
- `Directory.Build.props` reads this version for all projects
- No need to update version in multiple places

### Current Version
Check the current version:
```bash
cat VERSION
```

## Creating a Release

### Option 1: Automated Release (Recommended)

Use the version bump script:
```powershell
# Bump patch version (1.0.0 -> 1.0.1)
.\build\bump-version.ps1 -BumpType patch -CreateTag -Push

# Bump minor version (1.0.0 -> 1.1.0)
.\build\bump-version.ps1 -BumpType minor -CreateTag -Push

# Bump major version (1.0.0 -> 2.0.0)
.\build\bump-version.ps1 -BumpType major -CreateTag -Push
```

This will:
1. Update the VERSION file
2. Test build with new version
3. Commit the change
4. Create an annotated tag (e.g., `v1.0.1`)
5. Push everything to GitHub
6. Trigger GitHub Actions to create the release

### Option 2: Manual Release

1. **Update version**:
   ```bash
   echo "1.0.1" > VERSION
   ```

2. **Commit version change**:
   ```bash
   git add VERSION
   git commit -m "Bump version to 1.0.1"
   ```

3. **Create and push tag**:
   ```bash
   git tag -a v1.0.1 -m "Release v1.0.1"
   git push
   git push origin v1.0.1
   ```

## GitHub Actions Workflow

When you push a tag starting with `v`, the workflow will:
1. Build the application
2. Create multiple package types:
   - Self-contained single EXE (~150MB, no .NET required)
   - Framework-dependent single EXE (~1MB, requires .NET 8)
   - Portable ZIP with multiple files
3. Create a GitHub Release
4. Upload all packages as release assets

## Local Testing

### Build single-file executable:
```powershell
# Self-contained (no .NET required)
dotnet publish src/wpf/MetricClock.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Framework-dependent (requires .NET 8)
dotnet publish src/wpf/MetricClock.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true
```

### Create installer:
```powershell
.\build\create-installer.ps1 -Version $(Get-Content VERSION)
```

## Release Checklist

Before releasing:
- [ ] All changes committed and pushed
- [ ] Application builds without errors
- [ ] Tested on Windows 10/11
- [ ] Updated documentation if needed
- [ ] Decided on version bump type (major/minor/patch)

## Version Numbering

HudClock follows [Semantic Versioning](https://semver.org/):
- **Major** (X.0.0): Breaking changes
- **Minor** (1.X.0): New features, backward compatible
- **Patch** (1.0.X): Bug fixes, backward compatible

## Monitoring Releases

- **GitHub Actions**: https://github.com/lionfire/hudclock/actions
- **Releases Page**: https://github.com/lionfire/hudclock/releases
- **Latest Release**: https://github.com/lionfire/hudclock/releases/latest

## Troubleshooting

If a release fails:
1. Check GitHub Actions logs
2. Ensure version format is correct (X.Y.Z)
3. Verify tag starts with 'v' (e.g., v1.0.0)
4. Check file permissions and .NET SDK version