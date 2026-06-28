using ViewPorter.App.Profiles;
using ViewPorter.App.Runtime;
using ViewPorter.App.Settings;
using ViewPorter.App.Tray;
using ViewPorter.App.UI;
using ViewPorter.Core.Runtime;
using ViewPorter.Core.Settings;
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

    public AppBootstrapper()
    {
        _settingsStore = new JsonSettingsStore(new SettingsValidator());

        var profileStore = new ProfileStore(_settingsStore);
        var monitorService = new Win32MonitorService();
        var overlayManager = new OverlayManager();
        var windowManager = new WindowManager();
        var calculator = new ViewportCalculator();

        _viewportController = new ViewportController(profileStore, monitorService, overlayManager, windowManager, calculator);
        _settingsWindow = new SettingsWindow();
        _trayIconController = new TrayIconController(_viewportController, _settingsWindow);
    }

    public async Task StartAsync()
    {
        await _viewportController.InitializeAsync();
        _trayIconController.Show();
    }

    public async ValueTask DisposeAsync()
    {
        _trayIconController.Dispose();
        _settingsWindow.AllowClose();
        _settingsWindow.Close();
        await _viewportController.DisposeAsync();
    }
}
