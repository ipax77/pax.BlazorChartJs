
namespace pax.BlazorChartJs;

public record ChartJsAxis
{
    public string? Type { get; set; }
    public string? AlignToPixels { get; set; }
    /// <summary>
    /// Which type of axis this is. Possible values are: 'x', 'y', 'r'. If not set, this is inferred from the first character of the ID which should be 'x' or 'y'
    /// </summary>        
    public string? Axis { get; set; }
    public string? BackgroundColor { get; set; }
    public bool? Display { get; set; }
    public object? Grid { get; set; }
    public object? Min { get; set; }
    public object? Max { get; set; }
    public bool? Reverse { get; set; }
    public string? Stacked { get; set; }
    public object? SuggestedMax { get; set; }
    public object? SuggestedMin { get; set; }
    public ChartJsAxisTick? Ticks { get; set; }
    public string? Weight { get; set; }
    public Title? Title { get; set; }
}
