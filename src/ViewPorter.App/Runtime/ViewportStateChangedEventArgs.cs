using ViewPorter.Core.Runtime;

namespace ViewPorter.App.Runtime;

public sealed class ViewportStateChangedEventArgs : EventArgs
{
    public required ViewportState State { get; init; }

    public required string Message { get; init; }
}
