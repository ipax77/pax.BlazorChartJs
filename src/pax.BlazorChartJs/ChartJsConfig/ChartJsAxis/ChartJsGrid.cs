
namespace pax.BlazorChartJs;

#pragma warning disable CA2227
public record ChartJsGrid
{
    [Obsolete("In ChartJs v4.x use Border.Color instead")]
    public object? BorderColor { get; set; }
    [Obsolete("In ChartJs v4.x use Border.Width instead")]
    public int? BorderWidth { get; set; }
    [Obsolete("In ChartJs v4.x use Border.Dash instead")]
    public ICollection<double>? BorderDash { get; set; }
    [Obsolete("In ChartJs v4.x use Border.DashOffset instead")]
    public double? BorderDashOffset { get; set; }
    public bool? Circular { get; set; }
    public object? Color { get; set; }
    public bool? Display { get; set; }
    [Obsolete("In ChartJs v4.x use Border.Display instead")]
    public bool? DrawBorder { get; set; }
    public bool? DrawOnChartArea { get; set; }
    public bool? DrawTicks { get; set; }
    public double? LineWidth { get; set; }
    public bool? Offset { get; set; }
    public ICollection<double>? TickBorderDash { get; set; }
    public double? TickBorderDashOffset { get; set; }
    public object? TickColor { get; set; }
    public double? TickLength { get; set; }
    public double? TickWidth { get; set; }
    public int? Z { get; set; }
}
#pragma warning restore CA2227