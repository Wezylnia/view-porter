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
    private string _statusMessage = "Ready";

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

    public event EventHandler<ViewportStateChangedEventArgs>? StateChanged;

    public ViewportState State { get; private set; } = ViewportState.Disabled;

    public string StatusMessage => _statusMessage;

    public string ActiveProfileName => _selectedProfile?.Name ?? "No profile";

    public async Task InitializeAsync()
    {
        var settings = await _profileStore.LoadAsync();
        _selectedProfile = settings.Profiles.FirstOrDefault(profile => profile.Id == settings.SelectedProfileId)
            ?? settings.Profiles[0];
        SetState(ViewportState.Disabled, $"Ready. Active profile: {ActiveProfileName}.");
    }

    public async Task EnableAsync(CancellationToken cancellationToken = default)
    {
        if (_selectedProfile is null || State == ViewportState.Enabled || State == ViewportState.Enabling)
        {
            return;
        }

        SetState(ViewportState.Enabling, $"Enabling {ActiveProfileName}...");

        try
        {
            var monitors = _monitorService.GetMonitors();
            var monitor = SelectMonitor(monitors, _selectedProfile.MonitorSelection);
            if (monitor is null)
            {
                SetState(ViewportState.Faulted, "No monitor was available for the selected profile.");
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
                SetState(ViewportState.Faulted, string.Join(" ", result.Errors.DefaultIfEmpty("Could not calculate the viewport.")));
                return;
            }

            await _overlayManager.ShowAsync(result.Plan, _selectedProfile.BorderColorArgb, cancellationToken);

            if (_selectedProfile.MoveForegroundWindow)
            {
                await _windowManager.MoveForegroundWindowIntoViewportAsync(result.Plan.Viewport, cancellationToken);
            }

            SetState(ViewportState.Enabled, $"Viewport enabled on {monitor.FriendlyName} with profile {ActiveProfileName}.");
        }
        catch
        {
            SetState(ViewportState.Faulted, "ViewPorter failed while enabling the viewport.");
            throw;
        }
    }

    public async Task DisableAsync(CancellationToken cancellationToken = default)
    {
        if (State == ViewportState.Disabled || State == ViewportState.Disabling)
        {
            return;
        }

        SetState(ViewportState.Disabling, "Disabling viewport...");

        await _overlayManager.HideAsync(cancellationToken);
        await _windowManager.RestoreChangedWindowsAsync(cancellationToken);

        SetState(ViewportState.Disabled, "Viewport disabled. Desktop restored.");
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

    private void SetState(ViewportState state, string message)
    {
        State = state;
        _statusMessage = message;
        StateChanged?.Invoke(this, new ViewportStateChangedEventArgs
        {
            State = state,
            Message = message
        });
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
