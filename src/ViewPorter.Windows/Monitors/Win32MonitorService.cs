using ViewPorter.Core.Geometry;
using ViewPorter.Core.Monitors;
using ViewPorter.Windows.Interop;

namespace ViewPorter.Windows.Monitors;

public sealed class Win32MonitorService : IMonitorService
{
    private const uint MonitorInfoPrimaryFlag = 1;

    public IReadOnlyList<MonitorDescriptor> GetMonitors()
    {
        var monitors = new List<MonitorDescriptor>();

        NativeMethods.EnumDisplayMonitors(
            hdc: 0,
            lprcClip: 0,
            (hMonitor, _, _, _) =>
            {
                var info = new NativeMethods.MonitorInfoEx
                {
                    cbSize = System.Runtime.InteropServices.Marshal.SizeOf<NativeMethods.MonitorInfoEx>(),
                    szDevice = string.Empty
                };

                if (!NativeMethods.GetMonitorInfo(hMonitor, ref info))
                {
                    return true;
                }

                var bounds = ToPixelRect(info.rcMonitor);
                var work = ToPixelRect(info.rcWork);
                var key = MonitorKeyFactory.Create(info.szDevice, bounds);

                monitors.Add(new MonitorDescriptor
                {
                    MonitorKey = key,
                    DeviceName = info.szDevice,
                    FriendlyName = info.szDevice,
                    Bounds = bounds,
                    WorkingArea = work,
                    IsPrimary = (info.dwFlags & MonitorInfoPrimaryFlag) == MonitorInfoPrimaryFlag
                });

                return true;
            },
            dwData: 0);

        return monitors;
    }

    private static PixelRect ToPixelRect(NativeMethods.Rect rect) =>
        new(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
}
