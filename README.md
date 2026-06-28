# ViewPorter

Software-only viewport switcher for Windows.

ViewPorter creates a smaller centered viewport on a monitor using black borders, window positioning, hotkeys, and local profiles. It does not touch monitor firmware, EDID, GPU drivers, custom resolutions, or display timings.

## v0.1 Status

`v0.1` is a working prototype with:

- tray-first WPF application shell
- monitor enumeration through Win32
- centered viewport calculation
- topmost black border overlays
- foreground window move/restore
- emergency disable and toggle hotkeys
- local JSON settings storage
- unit tests for core calculation paths

Current default hotkeys:

- `Ctrl+Alt+F9`: toggle viewport
- `Ctrl+Alt+F10`: emergency disable

## Current Scope

- aspect-fit and pixel-sized viewport foundation
- software-only overlays
- safe foreground window positioning
- local profile/settings persistence foundation

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

The current prototype is intentionally conservative. If a foreground window looks unsafe to manipulate, ViewPorter leaves it alone.
