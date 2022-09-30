
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
    public ChartJsGrid? Grid { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public bool? Reverse { get; set; }
    /// <summary>
    /// By default data is not stacked. If the stacked option of the value scale (y-axis on horizontal chart) is true,
    /// positive and negative values are stacked separately. Additionally a stack option can be defined per dataset to 
    /// further divide into stack groups more.... For some charts, you might want to stack positive and negative values together.
    ///  That can be achieved by specifying stacked: 'single'   
    /// </summary>
    public object? Stacked { get; set; }
    public double? SuggestedMax { get; set; }
    public double? SuggestedMin { get; set; }
    public ChartJsAxisTick? Ticks { get; set; }
    public double? Weight { get; set; }
    public Title? Title { get; set; }
}
