using System.Windows.Forms;
using ViewPorter.Windows.Interop;

namespace ViewPorter.Windows.Hotkeys;

public sealed class HotkeyManager : IHotkeyService
{
    private readonly HotkeyWindow _window = new();
    private bool _registered;

    public HotkeyManager()
    {
        _window.HotkeyPressed += (_, args) => HotkeyPressed?.Invoke(this, args);
    }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public void RegisterDefaults()
    {
        if (_registered)
        {
            return;
        }

        RegisterOrThrow(HotkeyIds.ToggleViewport, NativeMethods.ModControl | NativeMethods.ModAlt | NativeMethods.ModNoRepeat, (uint)Keys.F9);
        RegisterOrThrow(HotkeyIds.EmergencyDisable, NativeMethods.ModControl | NativeMethods.ModAlt | NativeMethods.ModNoRepeat, (uint)Keys.F10);
        _registered = true;
    }

    public void Dispose()
    {
        if (_registered)
        {
            NativeMethods.UnregisterHotKey(_window.Handle, HotkeyIds.ToggleViewport);
            NativeMethods.UnregisterHotKey(_window.Handle, HotkeyIds.EmergencyDisable);
            _registered = false;
        }

        _window.Dispose();
    }

    private void RegisterOrThrow(int hotkeyId, uint modifiers, uint key)
    {
        if (!NativeMethods.RegisterHotKey(_window.Handle, hotkeyId, modifiers, key))
        {
            throw new InvalidOperationException($"Could not register hotkey {hotkeyId}.");
        }
    }

    private sealed class HotkeyWindow : NativeWindow, IDisposable
    {
        public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

        public HotkeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WmHotkey)
            {
                HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs { HotkeyId = m.WParam.ToInt32() });
                return;
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            DestroyHandle();
        }
    }
}
