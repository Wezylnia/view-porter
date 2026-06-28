namespace ViewPorter.Core.Profiles;

public readonly record struct AspectRatio(int Width, int Height)
{
    public static readonly AspectRatio Ratio16By9 = new(16, 9);
    public static readonly AspectRatio Ratio16By10 = new(16, 10);
    public static readonly AspectRatio Ratio21By9 = new(21, 9);
    public static readonly AspectRatio Ratio4By3 = new(4, 3);
    public static readonly AspectRatio Ratio5By4 = new(5, 4);

    public bool IsValid => Width > 0 && Height > 0;

    public double Value => IsValid ? (double)Width / Height : 0d;

    public override string ToString() => $"{Width}:{Height}";
}
