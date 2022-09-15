namespace pax.BlazorChartJs;

public record ChartJsAnimationProgressEvent : ChartJsEvent
{
    public double CurrentStep { get; init; }
    public double NumSteps { get; init; }
}
