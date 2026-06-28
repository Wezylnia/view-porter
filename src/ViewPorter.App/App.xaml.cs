using ViewPorter.App.Bootstrap;

namespace ViewPorter.App;

public partial class App : System.Windows.Application, IAsyncDisposable
{
    private AppBootstrapper? _bootstrapper;

    protected override async void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

        _bootstrapper = new AppBootstrapper();
        await _bootstrapper.StartAsync();
    }

    protected override async void OnExit(System.Windows.ExitEventArgs e)
    {
        await DisposeAsync();

        base.OnExit(e);
    }

    public async ValueTask DisposeAsync()
    {
        if (_bootstrapper is null)
        {
            return;
        }

        await _bootstrapper.DisposeAsync();
        _bootstrapper = null;
        GC.SuppressFinalize(this);
    }
}
