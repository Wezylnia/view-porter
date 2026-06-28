using ViewPorter.Core.Runtime;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace ViewPorter.Windows.Overlays;

public sealed class OverlayManager : IOverlayManager
{
    private readonly List<OverlayForm> _forms = [];

    public Task ShowAsync(ViewportPlan plan, int borderColorArgb, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plan);

        cancellationToken.ThrowIfCancellationRequested();

        EnsureForms();
        var color = Color.FromArgb(borderColorArgb);
        var rectangles =
            new[]
            {
                plan.TopBorder,
                plan.BottomBorder,
                plan.LeftBorder,
                plan.RightBorder
            };

        for (var index = 0; index < _forms.Count; index++)
        {
            var rect = rectangles[index];
            var form = _forms[index];

            if (rect.IsEmpty)
            {
                form.Hide();
                continue;
            }

            form.Apply(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), color);
            if (!form.Visible)
            {
                form.Show();
            }
        }

        return Task.CompletedTask;
    }

    public Task HideAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var form in _forms)
        {
            form.Hide();
            form.Dispose();
        }

        _forms.Clear();
        return Task.CompletedTask;
    }

    private void EnsureForms()
    {
        if (_forms.Count > 0)
        {
            return;
        }

        for (var index = 0; index < 4; index++)
        {
            _forms.Add(new OverlayForm());
        }
    }
}
