
namespace pax.BlazorChartJs;

#pragma warning disable CA2227
public record ChartJsGrid
{
    public object? BorderColor { get; set; }
    public int? BorderWidth { get; set; }
    public ICollection<double>? BorderDash { get; set; }
    public double? BorderDashOffset { get; set; }
    public bool? Circular { get; set; }
    public object? Color { get; set; }
    public bool? Display { get; set; }
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