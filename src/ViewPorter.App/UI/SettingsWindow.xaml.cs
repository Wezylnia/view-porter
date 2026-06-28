using System.Windows;

namespace ViewPorter.App.UI;

public partial class SettingsWindow : Window
{
    private bool _allowClose;

    public SettingsWindow()
    {
        InitializeComponent();
        Closing += OnClosing;
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
}
