namespace pax.BlazorChartJs;

public record ChartJsLegendClickEvent : ChartJsEvent
{
    public string? Label { get; init; }
}
