namespace ViewPorter.Core.Geometry;

public readonly record struct PixelSize(int Width, int Height)
{
    public bool IsEmpty => Width <= 0 || Height <= 0;
}
