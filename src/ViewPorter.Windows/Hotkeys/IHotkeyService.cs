namespace ViewPorter.Windows.Hotkeys;

public interface IHotkeyService : IDisposable
{
    event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    void RegisterDefaults();
}
