namespace ViewPorter.Core.Geometry;

public readonly record struct PixelRect(int X, int Y, int Width, int Height)
{
    public int Left => X;
    public int Top => Y;
    public int Right => X + Width;
    public int Bottom => Y + Height;

    public bool IsEmpty => Width <= 0 || Height <= 0;

    public static PixelRect Empty => new(0, 0, 0, 0);

    public PixelRect Deflate(int margin)
    {
        if (margin <= 0)
        {
            return this;
        }

        var width = Math.Max(0, Width - (margin * 2));
        var height = Math.Max(0, Height - (margin * 2));
        return new PixelRect(X + margin, Y + margin, width, height);
    }

    public PixelRect Center(PixelSize size)
    {
        var x = X + Math.Max(0, (Width - size.Width) / 2);
        var y = Y + Math.Max(0, (Height - size.Height) / 2);
        return new PixelRect(x, y, Math.Max(0, size.Width), Math.Max(0, size.Height));
    }

    public PixelRect ClampInside(PixelRect outer)
    {
        var width = Math.Min(Width, outer.Width);
        var height = Math.Min(Height, outer.Height);
        var x = Math.Max(outer.Left, Math.Min(X, outer.Right - width));
        var y = Math.Max(outer.Top, Math.Min(Y, outer.Bottom - height));
        return new PixelRect(x, y, width, height);
    }
}
