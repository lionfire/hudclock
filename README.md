# HudClock

<p align="center">
  <img src="assets/icon/hud_clock_icon_small.svg" alt="HudClock Icon" width="128" height="128">
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
