namespace pax.BlazorChartJs;

public record ChartJsLegendHoverEvent : ChartJsEvent
{
    public string? Label { get; init; }
}
