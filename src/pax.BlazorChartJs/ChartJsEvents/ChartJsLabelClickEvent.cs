namespace pax.BlazorChartJs;

public record ChartJsLabelClickEvent : ChartJsEvent
{
    public string? Label { get; init; }
    public object? Value { get; init; }
    public double DataX { get; init; }
    public double DataY { get; init; }
    public string? DatasetLabel { get; init; }
    public int? DatasetIndex { get; init; }
}
