using ViewPorter.Core.Monitors;

namespace ViewPorter.Windows.Monitors;

public interface IMonitorService
{
    IReadOnlyList<MonitorDescriptor> GetMonitors();
}
