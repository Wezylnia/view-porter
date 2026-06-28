using System.Drawing;
using System.Windows.Forms;

namespace ViewPorter.Windows.Overlays;

internal sealed class OverlayForm : Form
{
    private const int HtTransparent = -1;
    private const int WmNcHitTest = 0x0084;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;
    private const int WsExTransparent = 0x00000020;

    public OverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
    }

    public void Apply(Rectangle bounds, Color color)
    {
        BackColor = color;
        Bounds = bounds;
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            parameters.ExStyle |= WsExToolWindow | WsExNoActivate | WsExTransparent;
            return parameters;
        }
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == WmNcHitTest)
        {
            message.Result = new nint(HtTransparent);
            return;
        }

        base.WndProc(ref message);
    }
}
