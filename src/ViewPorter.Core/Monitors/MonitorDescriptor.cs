using ViewPorter.Core.Geometry;

namespace ViewPorter.Core.Monitors;

public sealed class MonitorDescriptor
{
    public string MonitorKey { get; init; } = string.Empty;

    public string DeviceName { get; init; } = string.Empty;

    public string FriendlyName { get; init; } = string.Empty;

    public PixelRect Bounds { get; init; }

    public PixelRect WorkingArea { get; init; }

    public bool IsPrimary { get; init; }

    public uint DpiX { get; init; } = 96;

    public uint DpiY { get; init; } = 96;
}
