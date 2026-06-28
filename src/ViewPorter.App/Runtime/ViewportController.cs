using ViewPorter.App.Profiles;
using ViewPorter.Core.Monitors;
using ViewPorter.Core.Profiles;
using ViewPorter.Core.Runtime;
using ViewPorter.Windows.Monitors;
using ViewPorter.Windows.Overlays;
using ViewPorter.Windows.Windows;

namespace ViewPorter.App.Runtime;

public sealed class ViewportController : IAsyncDisposable
{
    private readonly ProfileStore _profileStore;
    private readonly IMonitorService _monitorService;
    private readonly IOverlayManager _overlayManager;
    private readonly IWindowManager _windowManager;
    private readonly ViewportCalculator _calculator;
    private ViewportProfile? _selectedProfile;

    public ViewportController(
        ProfileStore profileStore,
        IMonitorService monitorService,
        IOverlayManager overlayManager,
        IWindowManager windowManager,
        ViewportCalculator calculator)
    {
        _profileStore = profileStore;
        _monitorService = monitorService;
        _overlayManager = overlayManager;
        _windowManager = windowManager;
        _calculator = calculator;
    }

    public ViewportState State { get; private set; } = ViewportState.Disabled;

    public async Task InitializeAsync()
    {
        var settings = await _profileStore.LoadAsync();
        _selectedProfile = settings.Profiles.FirstOrDefault(profile => profile.Id == settings.SelectedProfileId)
            ?? settings.Profiles[0];
    }

    public async Task EnableAsync(CancellationToken cancellationToken = default)
    {
        if (_selectedProfile is null || State == ViewportState.Enabled || State == ViewportState.Enabling)
        {
            return;
        }

        State = ViewportState.Enabling;

        try
        {
            var monitors = _monitorService.GetMonitors();
            var monitor = SelectMonitor(monitors, _selectedProfile.MonitorSelection);
            if (monitor is null)
            {
                State = ViewportState.Faulted;
                return;
            }

            var calibration = (await _profileStore.LoadAsync(cancellationToken)).MonitorCalibrations
                .FirstOrDefault(item => item.MonitorKey == monitor.MonitorKey);

            var result = _calculator.Calculate(
                new ViewportCalculationRequest
                {
                    Monitor = monitor,
                    Profile = _selectedProfile,
                    Calibration = calibration
                });

            if (!result.IsSuccess || result.Plan is null)
            {
                State = ViewportState.Faulted;
                return;
            }

            await _overlayManager.ShowAsync(result.Plan, _selectedProfile.BorderColorArgb, cancellationToken);

            if (_selectedProfile.MoveForegroundWindow)
            {
                await _windowManager.MoveForegroundWindowIntoViewportAsync(result.Plan.Viewport, cancellationToken);
            }

            State = ViewportState.Enabled;
        }
        catch
        {
            State = ViewportState.Faulted;
            throw;
        }
    }

    public async Task DisableAsync(CancellationToken cancellationToken = default)
    {
        if (State == ViewportState.Disabled || State == ViewportState.Disabling)
        {
            return;
        }

        State = ViewportState.Disabling;

        await _overlayManager.HideAsync(cancellationToken);
        await _windowManager.RestoreChangedWindowsAsync(cancellationToken);

        State = ViewportState.Disabled;
    }

    public async Task ToggleAsync(CancellationToken cancellationToken = default)
    {
        if (State == ViewportState.Enabled)
        {
            await DisableAsync(cancellationToken);
            return;
        }

        await EnableAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await DisableAsync();
    }

    private static MonitorDescriptor? SelectMonitor(
        IReadOnlyList<MonitorDescriptor> monitors,
        MonitorSelection selection)
    {
        if (monitors.Count == 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(selection.MonitorKey))
        {
            var exact = monitors.FirstOrDefault(item => item.MonitorKey == selection.MonitorKey);
            if (exact is not null)
            {
                return exact;
            }
        }

        return selection.FallbackToPrimaryMonitor
            ? monitors.FirstOrDefault(item => item.IsPrimary) ?? monitors[0]
            : null;
    }
}
