namespace pax.BlazorChartJs.BlazorLegend;

public record ChartJsLegendItem
{
    public string? Text { get; init; }
    public string? FillStyle { get; init; }
    public string? FontColor { get; init; }
    public bool Hidden { get; set; }
    public string? LineCap { get; init; }
#pragma warning disable CA1002 // Do not expose generic lists
    public List<double>? LineDash { get; init; }
#pragma warning restore CA1002 // Do not expose generic lists
    public double LineDashOffset { get; init; }
    public string? LineJoin { get; init; }
    public double LineWidth { get; init; }
    public string? PointStyle { get; init; }
    public double? Rotation { get; init; }
    public string? StrokeStyle { get; init; }
    public int Index { get; init; }
    public int DatasetIndex { get; init; }
    public string? TextAlign { get; init; }
    public bool? Borderradius { get; init; }
}
