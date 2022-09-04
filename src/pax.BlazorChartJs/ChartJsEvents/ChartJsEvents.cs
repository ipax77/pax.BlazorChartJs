
namespace pax.BlazorChartJs;

public enum ChartJsEventType
{
    None = 0,
    click = 1,
    hover = 2,
    leave = 3,
    progress = 4,
    complete = 5
}

public enum ChartJsEventSource
{
    None = 0,
    legend = 1,
    animation = 2
}

public record ChartJsEvent
{
    public ChartJsEventType Type { get; init; }
    public ChartJsEventSource Source { get; init; }
    public object? Data { get; init; }
}