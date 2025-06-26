 For Automated Releases:

  1. Push your code to GitHub
  2. Create a tag: git tag v1.0.0 && git push origin v1.0.0
  3. GitHub Actions will automatically create a release with all packages

  For Local Testing:

  # Quick self-contained build
  dotnet publish src/wpf/MetricClock.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

  # Or use the installer script
  .\build\create-installer.ps1 -Version 1.0.0
  