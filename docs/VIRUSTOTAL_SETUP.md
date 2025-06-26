# VirusTotal Integration Setup

## Prerequisites

1. Create a free VirusTotal account at https://www.virustotal.com
2. Get your API key from: https://www.virustotal.com/gui/my-apikey
3. Add the API key to your GitHub repository secrets:
   - Go to Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `VT_API_KEY`
   - Value: Your VirusTotal API key

## How It Works

When you push a new tag (e.g., `v1.0.1`), the GitHub Actions workflow will:

1. Build three versions of HudClock:
   - Framework-dependent (~5MB)
   - Portable (~10MB)
   - Self-contained (~90MB)

2. Scan the smaller builds with VirusTotal (free API limit is 32MB)

3. Create a GitHub release with:
   - All three build artifacts
   - VirusTotal scan links in the release notes
   - Download statistics and version badges

## API Limitations

- **Free API**: 
  - 4 requests per minute
  - 500 requests per day
  - 32MB file size limit
  - Public scan results

- **Premium API** (optional):
  - Higher rate limits
  - Larger file sizes
  - Private scans

## Manual Scanning

For the self-contained build or if automated scanning fails:

1. Download the release file
2. Upload to https://www.virustotal.com
3. Copy the scan URL
4. Add to release notes manually

## Troubleshooting

If VirusTotal scanning fails:
- Check the API key is correct
- Verify file sizes are under 32MB
- Check API rate limits haven't been exceeded
- The workflow continues even if scanning fails (continue-on-error: true)

## Badge Updates

The README includes status badges that update automatically:
- Build status
- Latest release version
- Total downloads
- License

No manual updates needed for these badges.