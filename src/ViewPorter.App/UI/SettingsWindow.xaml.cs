using System.Windows;
using ViewPorter.App.Runtime;
using ViewPorter.Core.Runtime;

namespace ViewPorter.App.UI;

public partial class SettingsWindow : Window
{
    private bool _allowClose;
    private readonly ViewportController _viewportController;

    public SettingsWindow(ViewportController viewportController)
    {
        _viewportController = viewportController;
        InitializeComponent();
        Closing += OnClosing;
        StateChanged += OnWindowStateChanged;
        _viewportController.StateChanged += OnViewportStateChanged;
        RefreshState(_viewportController.State, _viewportController.StatusMessage);
    }

    public void AllowClose() => _allowClose = true;

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        e.Cancel = true;
        Hide();
    }

    private async void EnableButton_OnClick(object sender, RoutedEventArgs e)
    {
        await _viewportController.EnableAsync();
    }

    private async void DisableButton_OnClick(object sender, RoutedEventArgs e)
    {
        await _viewportController.DisableAsync();
    }

    private void MinimizeToTrayButton_OnClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void ExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void OnViewportStateChanged(object? sender, ViewportStateChangedEventArgs e)
    {
        Dispatcher.Invoke(() => RefreshState(e.State, e.Message));
    }

    private void RefreshState(ViewportState state, string message)
    {
        StatusTextBlock.Text = message;
        ProfileTextBlock.Text = $"Active profile: {_viewportController.ActiveProfileName}";

        EnableButton.IsEnabled = state is ViewportState.Disabled or ViewportState.Faulted;
        DisableButton.IsEnabled = state is ViewportState.Enabled or ViewportState.Enabling or ViewportState.Disabling;
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }
}
