using ViewPorter.Core.Geometry;

namespace ViewPorter.Windows.Windows;

internal sealed class WindowPlacementRecord
{
    public required nint WindowHandle { get; init; }

    public required uint ProcessId { get; init; }

    public required PixelRect Bounds { get; init; }

    public required bool WasMaximized { get; init; }
}
