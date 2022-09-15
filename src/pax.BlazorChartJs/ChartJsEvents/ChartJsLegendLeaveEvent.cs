
namespace pax.BlazorChartJs;

public record ChartJsLegendLeaveEvent : ChartJsEvent
{
    public string? Label { get; init; }
}
