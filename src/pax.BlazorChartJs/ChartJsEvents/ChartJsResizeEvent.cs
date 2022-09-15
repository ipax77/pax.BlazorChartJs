namespace pax.BlazorChartJs;

public record ChartJsResizeEvent : ChartJsEvent
{
    public double Height { get; init; }
    public double Width { get; init; }
}