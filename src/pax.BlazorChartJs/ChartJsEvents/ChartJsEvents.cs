
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

public record ChartJsInitEvent : ChartJsEvent
{
}