namespace ViewPorter.Core.Runtime;

public sealed class ViewportCalculationResult
{
    public bool IsSuccess { get; init; }

    public ViewportPlan? Plan { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static ViewportCalculationResult Success(ViewportPlan plan) =>
        new()
        {
            IsSuccess = true,
            Plan = plan
        };

    public static ViewportCalculationResult Failure(params string[] errors) =>
        new()
        {
            Errors = errors
        };
}
