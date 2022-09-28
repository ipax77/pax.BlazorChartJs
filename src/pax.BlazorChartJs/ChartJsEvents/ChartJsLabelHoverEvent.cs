namespace pax.BlazorChartJs;

public record ChartJsLabelHoverEvent : ChartJsEvent
{
    public string? Label { get; init; }
    public object? Value { get; init; }
    public double DataX { get; init; }
    public double DataY { get; init; }
}

