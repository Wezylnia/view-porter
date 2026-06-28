namespace ViewPorter.Windows.Hotkeys;

public sealed class HotkeyPressedEventArgs : EventArgs
{
    public required int HotkeyId { get; init; }
}
