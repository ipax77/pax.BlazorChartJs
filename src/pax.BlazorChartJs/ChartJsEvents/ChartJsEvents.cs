
namespace pax.BlazorChartJs;

public record ChartJsEvent
{
    public Guid ChartJsConfigGuid { get; set; }
}

public enum ChartJsEventType
{
    None = 0,
    click = 1,
    hover = 2,
    leave = 3,
    progress = 4,
    complete = 5,
    resize = 6,
}

public enum ChartJsEventSource
{
    None = 0,
    legend = 1,
    animation = 2,
    label = 3,
    chart = 4,
}

/// <summary>
/// Event raised after a chart has been successfully initialized.
/// </summary>
public record ChartJsInitEvent : ChartJsEvent
{
    /// <summary>
    /// Chart height after initialization, in CSS pixels.
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Chart width after initialization, in CSS pixels.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Browser viewport height at initialization time, from window.innerHeight.
    /// </summary>
    public double WindowHeight { get; init; }

    /// <summary>
    /// Browser viewport width at initialization time, from window.innerWidth.
    /// </summary>
    public double WindowWidth { get; init; }
}

public sealed record ChartJsInitResult
{
    public bool Success { get; init; }

    public double Height { get; init; }

    public double Width { get; init; }

    public double WindowHeight { get; init; }

    public double WindowWidth { get; init; }
}