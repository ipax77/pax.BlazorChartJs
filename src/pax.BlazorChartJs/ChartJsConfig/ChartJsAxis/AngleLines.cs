
namespace pax.BlazorChartJs;

/// <summary>
/// AngelLines <see href="https://www.chartjs.org/docs/latest/axes/radial/linear.html#angle-line-options">ChartJs Docs</see>
/// </summary>
public record AngleLines
{
    public bool? Display { get; set; }
    public object? Color { get; set; }
    public double? LineWidth { get; set; }
#pragma warning disable CA2227    
    public IList<double>? BorderDash { get; set; }
#pragma warning restore CA2227    
    public double? BorderDashOffset { get; set; }
}
