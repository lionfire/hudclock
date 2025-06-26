## How to

- how to build: `"/mnt/c/Program Files/dotnet/dotnet.exe" build`
- how to run: `"/mnt/c/Program Files/dotnet/dotnet.exe" run`

## Versioning

- For future releases, use the version bump script:
  # For a patch release (1.0.0 -> 1.0.1)
  .\build\bump-version.ps1 -BumpType patch -CreateTag -Push