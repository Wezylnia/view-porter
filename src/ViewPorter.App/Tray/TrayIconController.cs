using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using ViewPorter.App.Runtime;
using Application = System.Windows.Application;

namespace ViewPorter.App.Tray;

public sealed class TrayIconController : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ViewportController _viewportController;
    private readonly Window _settingsWindow;

    public TrayIconController(ViewportController viewportController, Window settingsWindow)
    {
        _viewportController = viewportController;
        _settingsWindow = settingsWindow;
        _notifyIcon = new NotifyIcon
        {
            Text = "ViewPorter",
            Icon = SystemIcons.Application,
            Visible = false,
            ContextMenuStrip = BuildMenu()
        };
    }

    public void Show()
    {
        _notifyIcon.Visible = true;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }

    private ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Enable Viewport", null, async (_, _) => await RunOnUiAsync(() => _viewportController.EnableAsync()));
        menu.Items.Add("Disable Viewport", null, async (_, _) => await RunOnUiAsync(() => _viewportController.DisableAsync()));
        menu.Items.Add("Settings", null, async (_, _) => await ShowSettingsAsync());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, async (_, _) => await RunOnUiAsync(ShutdownAsync));
        return menu;
    }

    private Task ShowSettingsAsync()
    {
        return Application.Current.Dispatcher.InvokeAsync(
            () =>
            {
                if (_settingsWindow.IsVisible)
                {
                    _settingsWindow.Activate();
                    return;
                }

                _settingsWindow.Show();
            }).Task;
    }

    private static async Task RunOnUiAsync(Func<Task> action)
    {
        await await Application.Current.Dispatcher.InvokeAsync(action);
    }

    private async Task ShutdownAsync()
    {
        await _viewportController.DisableAsync();
        Application.Current.Shutdown();
    }
}
