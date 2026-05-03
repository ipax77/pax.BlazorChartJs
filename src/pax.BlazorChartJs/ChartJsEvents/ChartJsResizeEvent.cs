namespace pax.BlazorChartJs;

/// <summary>
/// Event raised when Chart.js reports that the chart has resized.
/// </summary>
public record ChartJsResizeEvent : ChartJsEvent
{
    /// <summary>
    /// Chart canvas height after the resize, in CSS pixels.
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Chart canvas width after the resize, in CSS pixels.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Browser viewport height at the time of the resize, from window.innerHeight.
    /// </summary>
    public double WindowHeight { get; init; }

    /// <summary>
    /// Browser viewport width at the time of the resize, from window.innerWidth.
    /// </summary>
    public double WindowWidth { get; init; }
}
