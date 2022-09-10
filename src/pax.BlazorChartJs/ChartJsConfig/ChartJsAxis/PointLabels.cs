
namespace pax.BlazorChartJs;

/// <summary>
/// PointLabels <see href="https://www.chartjs.org/docs/latest/axes/radial/linear.html#point-label-options">ChartJs Docs</see>
/// </summary>
public record PointLabels
{
    public object? BackdropColor { get; set; }
    public Padding? BackdropPadding { get; set; }
    public object? BorderRadius { get; set; }
    public bool? Display { get; set; }
    public string? Color { get; set; }
    public Font? Font { get; set; }
    public Padding? Padding { get; set; }
    public bool? CenterPointLabels { get; set; }
}
