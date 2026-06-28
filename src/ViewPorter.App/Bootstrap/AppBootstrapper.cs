using ViewPorter.App.Profiles;
using ViewPorter.App.Runtime;
using ViewPorter.App.Settings;
using ViewPorter.App.Tray;
using ViewPorter.App.UI;
using ViewPorter.Core.Runtime;
using ViewPorter.Core.Settings;
using ViewPorter.Windows.Hotkeys;
using ViewPorter.Windows.Monitors;
using ViewPorter.Windows.Overlays;
using ViewPorter.Windows.Windows;

namespace ViewPorter.App.Bootstrap;

public sealed class AppBootstrapper : IAsyncDisposable
{
    private readonly JsonSettingsStore _settingsStore;
    private readonly ViewportController _viewportController;
    private readonly TrayIconController _trayIconController;
    private readonly SettingsWindow _settingsWindow;
    private readonly HotkeyManager _hotkeyService;

    public AppBootstrapper()
    {
        _settingsStore = new JsonSettingsStore(new SettingsValidator());

        var profileStore = new ProfileStore(_settingsStore);
        var monitorService = new Win32MonitorService();
        var overlayManager = new OverlayManager();
        var windowManager = new WindowManager();
        _hotkeyService = new HotkeyManager();
        var calculator = new ViewportCalculator();

        _viewportController = new ViewportController(profileStore, monitorService, overlayManager, windowManager, calculator);
        _settingsWindow = new SettingsWindow();
        _trayIconController = new TrayIconController(_viewportController, _settingsWindow);
        _hotkeyService.HotkeyPressed += OnHotkeyPressed;
    }

    public async Task StartAsync()
    {
        await _viewportController.InitializeAsync();
        _hotkeyService.RegisterDefaults();
        _trayIconController.Show();
    }

    public async ValueTask DisposeAsync()
    {
        _trayIconController.Dispose();
        _hotkeyService.HotkeyPressed -= OnHotkeyPressed;
        _hotkeyService.Dispose();
        _settingsWindow.AllowClose();
        _settingsWindow.Close();
        await _viewportController.DisposeAsync();
    }

    private async void OnHotkeyPressed(object? sender, HotkeyPressedEventArgs args)
    {
        switch (args.HotkeyId)
        {
            case HotkeyIds.ToggleViewport:
                await _viewportController.ToggleAsync();
                break;
            case HotkeyIds.EmergencyDisable:
                await _viewportController.DisableAsync();
                break;
        }
    }
}
