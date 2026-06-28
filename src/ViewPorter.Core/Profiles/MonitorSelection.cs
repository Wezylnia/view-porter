namespace ViewPorter.Core.Profiles;

public sealed class MonitorSelection
{
    public string? MonitorKey { get; init; }

    public bool FallbackToPrimaryMonitor { get; init; } = true;
}
