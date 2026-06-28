# ViewPorter

Software-only viewport switcher for Windows.

ViewPorter creates a smaller centered viewport on a monitor using black borders, window positioning, hotkeys, and local profiles. It does not touch monitor firmware, EDID, GPU drivers, custom resolutions, or display timings.

## Status

Early scaffold. The current codebase includes:

- `.NET 10` solution structure
- core viewport geometry and profile models
- JSON settings storage
- Win32 monitor enumeration
- tray-first WPF application shell
- initial unit tests

## Planned MVP

- centered aspect-fit and pixel-sized viewports
- black border overlays
- foreground window move/restore
- hotkeys
- monitor calibration for physical-size presets
- per-app profile switching

## Non-Goals

- EDID editing
- virtual display drivers
- GPU control panels or vendor APIs
- exclusive fullscreen hooking
- graphics API injection

## Build

```powershell
dotnet build ViewPorter.slnx
dotnet test tests\ViewPorter.Core.Tests\ViewPorter.Core.Tests.csproj
dotnet test tests\ViewPorter.Windows.Tests\ViewPorter.Windows.Tests.csproj
```

## Notes

ViewPorter is intended for windowed and borderless-windowed apps. Exclusive fullscreen behavior is explicitly out of scope.
