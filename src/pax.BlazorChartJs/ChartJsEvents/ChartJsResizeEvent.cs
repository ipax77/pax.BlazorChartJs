namespace pax.BlazorChartJs;

public record ChartJsResizeEvent : ChartJsEvent
{
    public double Height { get; init; }
    public double Width { get; init; }
    public double WindowHeight { get; init; }
    public double WindowWidth { get; init; }
}
