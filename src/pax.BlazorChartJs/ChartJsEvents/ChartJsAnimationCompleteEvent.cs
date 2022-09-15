namespace pax.BlazorChartJs;

public record ChartJsAnimationCompleteEvent : ChartJsEvent
{
    public bool Initial { get; init; }
}
