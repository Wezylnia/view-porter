using ViewPorter.Core.Monitors;
using ViewPorter.Core.Profiles;

namespace ViewPorter.Core.Settings;

public sealed class SettingsDocument
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    public string? SelectedProfileId { get; init; }

    public bool AutomationEnabled { get; init; }

    public IReadOnlyList<ViewportProfile> Profiles { get; init; } = Array.Empty<ViewportProfile>();

    public IReadOnlyList<MonitorCalibration> MonitorCalibrations { get; init; } = Array.Empty<MonitorCalibration>();
}
