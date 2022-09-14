
namespace pax.BlazorChartJs;

public record ChartJsAxisTick
{
    public string? BackdropColor { get; set; }
    public Padding? BackdropPadding { get; set; }
    // public object? Callback { get; set; }
    public bool? Display { get; set; }
    public string? Color { get; set; }
    public Font? Font { get; set; }
    public object? Major { get; set; }
    public int? Padding { get; set; }
    public bool? ShowLabelBackdrop { get; set; }
    public object? TextStrokeColor { get; set; }
    public int? TextStrokeWidth { get; set; }
    /// <summary>
    /// z-index of tick layer. Useful when ticks are drawn on chart area. Values &lt;= 0 are drawn under datasets, &gt; 0 on top.
    /// </summary>   
    public int? Z { get; set; }
}
