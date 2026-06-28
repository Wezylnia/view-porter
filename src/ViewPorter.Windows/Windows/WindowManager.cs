using ViewPorter.Core.Geometry;
using ViewPorter.Windows.Interop;

namespace ViewPorter.Windows.Windows;

public sealed class WindowManager : IWindowManager
{
    private static readonly HashSet<string> RejectedWindowClasses =
    [
        "Shell_TrayWnd",
        "Progman",
        "WorkerW"
    ];

    private readonly Dictionary<nint, WindowPlacementRecord> _restoreRecords = [];

    public Task<bool> MoveForegroundWindowIntoViewportAsync(PixelRect viewport, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (viewport.IsEmpty)
        {
            return Task.FromResult(false);
        }

        var window = NativeMethods.GetForegroundWindow();
        if (window == 0 || !IsCandidateWindow(window))
        {
            return Task.FromResult(false);
        }

        NativeMethods.GetWindowThreadProcessId(window, out var processId);
        if (!_restoreRecords.ContainsKey(window) && TryGetBounds(window, out var originalBounds))
        {
            _restoreRecords[window] = new WindowPlacementRecord
            {
                WindowHandle = window,
                ProcessId = processId,
                Bounds = originalBounds,
                WasMaximized = NativeMethods.IsZoomed(window)
            };
        }

        if (NativeMethods.IsIconic(window) || NativeMethods.IsZoomed(window))
        {
            NativeMethods.ShowWindow(window, NativeMethods.SwRestore);
        }

        var moved = NativeMethods.SetWindowPos(
            window,
            hWndInsertAfter: 0,
            viewport.X,
            viewport.Y,
            viewport.Width,
            viewport.Height,
            NativeMethods.SwpNoActivate | NativeMethods.SwpNoZOrder | NativeMethods.SwpShowWindow | NativeMethods.SwpAsyncWindowPos);

        return Task.FromResult(moved);
    }

    public Task RestoreChangedWindowsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var record in _restoreRecords.Values.ToArray())
        {
            if (record.WindowHandle == 0)
            {
                continue;
            }

            NativeMethods.GetWindowThreadProcessId(record.WindowHandle, out var currentProcessId);
            if (currentProcessId != record.ProcessId)
            {
                continue;
            }

            if (record.WasMaximized)
            {
                NativeMethods.ShowWindow(record.WindowHandle, NativeMethods.SwRestore);
            }

            NativeMethods.SetWindowPos(
                record.WindowHandle,
                hWndInsertAfter: 0,
                record.Bounds.X,
                record.Bounds.Y,
                record.Bounds.Width,
                record.Bounds.Height,
                NativeMethods.SwpNoActivate | NativeMethods.SwpNoZOrder | NativeMethods.SwpShowWindow | NativeMethods.SwpAsyncWindowPos);
        }

        _restoreRecords.Clear();
        return Task.CompletedTask;
    }

    private static bool IsCandidateWindow(nint window)
    {
        if (!NativeMethods.IsWindowVisible(window))
        {
            return false;
        }

        var style = NativeMethods.GetWindowLongPtr(window, NativeMethods.GwlStyle).ToInt64();
        if ((style & NativeMethods.WsChild) == NativeMethods.WsChild)
        {
            return false;
        }

        var className = GetClassName(window);
        return !RejectedWindowClasses.Contains(className);
    }

    private static string GetClassName(nint window)
    {
        var buffer = new char[256];
        var length = NativeMethods.GetClassName(window, buffer, buffer.Length);
        return length > 0 ? new string(buffer, 0, length) : string.Empty;
    }

    private static bool TryGetBounds(nint window, out PixelRect bounds)
    {
        if (NativeMethods.GetWindowRect(window, out var rect))
        {
            bounds = new PixelRect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            return true;
        }

        bounds = PixelRect.Empty;
        return false;
    }
}
