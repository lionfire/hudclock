## How to

- how to build: `"/mnt/c/Program Files/dotnet/dotnet.exe" build`
- how to run: `"/mnt/c/Program Files/dotnet/dotnet.exe" run`
- how to build installer locally: `"/mnt/d/Program Files (x86)/Inno Setup 6/ISCC.exe" ../../installer/setup.iss`

## Versioning and Release Process

1. Commit all changes
2. Bump version: Edit `../../VERSION` file
3. Commit version bump
4. Push: `"/mnt/c/Program Files/Git/bin/git.exe" push`
5. Tag: `"/mnt/c/Program Files/Git/bin/git.exe" tag v1.2.0 -m "Release v1.2.0"`
6. Push tag: `"/mnt/c/Program Files/Git/bin/git.exe" push origin v1.2.0`

Note: The tag push triggers GitHub Actions to build and create the release