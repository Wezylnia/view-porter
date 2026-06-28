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

## How To Use

### 1. Build and run

```powershell
dotnet build ViewPorter.slnx
dotnet run --project src\ViewPorter.App\ViewPorter.App.csproj
```

When the app starts, it does not open a main window. It starts in the system tray.

### 2. Find the tray icon

Look for the `ViewPorter` icon in the Windows notification area.

Right click the tray icon to open the menu:

- `Enable Viewport`
- `Disable Viewport`
- `Settings`
- `Exit`

### 3. Enable the viewport

Use either of these:

- tray menu -> `Enable Viewport`
- `Ctrl+Alt+F9`

What happens on `v0.1`:

- ViewPorter selects the active profile
- the default profile is `Centered 16:9`
- the target monitor is the primary monitor unless a saved monitor selection exists
- black borders are drawn around the centered viewport
- the current foreground window is moved into that viewport if ViewPorter considers it safe to move

### 4. Disable or recover

Use either of these:

- tray menu -> `Disable Viewport`
- `Ctrl+Alt+F10` for emergency disable

Emergency disable is the escape hatch. It hides overlays and attempts to restore any window ViewPorter changed during the current session.

### 5. Settings and saved state

The current settings window is minimal. `v0.1` does not yet provide full profile editing from the UI.

Settings are stored locally here:

`%LocalAppData%\ViewPorter\settings.json`

On first run, ViewPorter creates a default profile automatically. If the settings file becomes invalid JSON, ViewPorter quarantines it and regenerates a clean default file.

## What To Expect In v0.1

- Best results are with normal desktop apps and borderless-windowed apps.
- Some windows will be ignored on purpose if they look unsafe to manipulate.
- Exclusive fullscreen apps are not supported.
- There is no polished onboarding flow yet. The tray icon and hotkeys are the main control surface.

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
