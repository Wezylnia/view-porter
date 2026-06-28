using ViewPorter.Core.Geometry;

namespace ViewPorter.Windows.Monitors;

public static class MonitorKeyFactory
{
    public static string Create(string deviceName, PixelRect bounds) =>
        $"{deviceName}|{bounds.X},{bounds.Y},{bounds.Width},{bounds.Height}";
}
