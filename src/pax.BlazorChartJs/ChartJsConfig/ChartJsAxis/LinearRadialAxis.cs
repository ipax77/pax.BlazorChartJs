
namespace pax.BlazorChartJs;

/// <summary>
/// LinearRadialAxis <see href="https://www.chartjs.org/docs/latest/axes/radial/linear.html">ChartJs Docs</see>
/// </summary>
public record LinearRadialAxis : ChartJsAxis
{
    public bool Animate { get; set; }
    public AngleLines? AngleLines { get; set; }
    public bool BeginAtZero { get; set; }
    public PointLabels? PointLabels { get; set; }
    public double? StartAngel { get; set; }
}
