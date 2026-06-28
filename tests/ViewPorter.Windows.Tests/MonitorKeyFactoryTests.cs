using ViewPorter.Core.Geometry;
using ViewPorter.Windows.Monitors;

namespace ViewPorter.Windows.Tests;

public sealed class MonitorKeyFactoryTests
{
    [Fact]
    public void Create_ReturnsStableKey()
    {
        var key = MonitorKeyFactory.Create(@"\\.\DISPLAY2", new PixelRect(-1920, 0, 1920, 1080));

        Assert.Equal(@"\\.\DISPLAY2|-1920,0,1920,1080", key);
    }
}
