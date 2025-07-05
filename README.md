# HudClock

<p align="center">
  <img src="assets/icon/hud_clock_icon_small.svg" alt="HudClock Icon" width="128" height="128">
</p>

<p align="center">
  <a href="https://github.com/lionfire/hudclock/actions/workflows/build.yml"><img src="https://img.shields.io/github/actions/workflow/status/lionfire/hudclock/build.yml?label=Build&logo=github" alt="Build Status"></a>
  <a href="https://github.com/lionfire/hudclock/releases/latest"><img src="https://img.shields.io/github/v/release/lionfire/hudclock?label=Release&logo=github" alt="Latest Release"></a>
  <a href="https://github.com/lionfire/hudclock/releases/latest"><img src="https://img.shields.io/github/downloads/lionfire/hudclock/total?label=Downloads&logo=github" alt="Total Downloads"></a>
  <a href="https://github.com/lionfire/hudclock/blob/main/LICENSE"><img src="https://img.shields.io/github/license/lionfire/hudclock?label=License" alt="License"></a>
  <img src="https://img.shields.io/badge/Platform-Windows-blue?logo=windows" alt="Platform">
  <img src="https://img.shields.io/badge/Framework-.NET%208-512BD4?logo=dotnet" alt=".NET 8">
</p>

<p align="center">
  A transparent, always-on-top clock overlay for Windows that provides unique time visualization options including 24-hour analog, metric time, and minimalist designs.
</p>

## Overview

HudClock is a WPF application designed to provide an unobtrusive yet constantly visible time display. It offers multiple clock styles including traditional analog/digital formats as well as alternative time systems like metric time (10-hour days) and 24-hour analog displays.

## Key Features

- **Multiple Clock Types**: Analog (12/24-hour), Digital (12/24-hour), Metric time
- **Transparency & Click-through**: Adjustable opacity with optional click-through mode
- **Customizable Appearance**: Various styles, shapes, and visual options
- **Always-on-top Display**: Persists above other windows
- **System Tray Integration**: Minimize to tray with configurable visibility
- **High Performance**: Configurable update rates up to 120fps for smooth animations

## Screenshots

See HudClock in action in our [features gallery](https://lionfire.github.io/hudclock/features).

## Download & Installation

Download the latest release from the [Releases page](https://github.com/lionfire/hudclock/releases/latest).

### Available Versions
- **Self-contained** (Recommended) - Single ~90MB executable with everything included
- **Framework-dependent** - Smaller ~5MB executable (requires .NET 8 Desktop Runtime)
- **Portable** - Traditional multi-file portable application

### Security
The framework-dependent and portable builds are automatically scanned with VirusTotal when released. Scan results are included in each release's notes. The self-contained build exceeds VirusTotal's free API size limit (32MB) but uses the same verified source code.

## Technical Details

- **Platform**: Windows (WPF/.NET 8.0)
- **Architecture**: MVVM pattern with data binding
- **Settings**: JSON-based configuration stored in `%APPDATA%/HudClock/`
- **UI**: Custom clock controls with hardware-accelerated rendering

## Building

```bash
dotnet build
```

## Running

```bash
dotnet run
```

## Project Structure

- `/src/wpf/` - Main WPF application
  - Clock controls (Analog, Digital, Metric variants)
  - Settings management
  - Window management with transparency/click-through support
