
namespace pax.BlazorChartJs;

/// <summary>
/// Bar chart dataset <see href="https://www.chartjs.org/docs/latest/charts/bar.html">ChartJs docs</see>
/// </summary>
public record BarDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>     
    public IndexableOption<string>? BackgroundColor { get; set; }
    public int? Base { get; set; }
    public int? BarPercentage { get; set; }
    /// <summary>
    /// If this value is a number, it is applied to the width of each bar, in pixels. When this is enforced, barPercentage and categoryPercentage are ignored.
    /// If set to 'flex', the base sample widths are calculated automatically based on the previous and following samples so that they take the full available widths without overlap. Then, bars are sized using barPercentage and categoryPercentage. There is no gap when the percentage options are 1. This mode generates bars with different widths when data are not evenly spaced.
    /// If not set (default), the base sample widths are calculated using the smallest interval that prevents bar overlapping, and bars are sized using barPercentage and categoryPercentage. This mode always generates bars equally sized.
    ///<see href="https://www.chartjs.org/docs/latest/charts/bar.html#barthickness">ChartJs docs</see>
    /// </summary>
    public object? BarThickness { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? BorderColor { get; set; }
    /// <summary>
    /// This setting is used to avoid drawing the bar stroke at the base of the fill, or disable the border radius. In general, this does not need to be changed except when creating chart types that derive from a bar chart.
    ///
    ///    Note
    ///
    ///    For negative bars in a vertical chart, top and bottom are flipped. Same goes for left and right in a horizontal chart.
    ///
    ///    Options are:
    ///
    ///    'start'
    ///    'end'
    ///    'middle' (only valid on stacked bars: the borders between bars are skipped)
    ///    'bottom'
    ///    'left'
    ///    'top'
    ///    'right'
    ///    false (don't skip any borders)
    ///    true (skip all borders)
    /// <see href="https://www.chartjs.org/docs/latest/charts/bar.html#borderskipped">ChartJs docs</see>
    /// </summary>        
    public object? BorderSkipped { get; set; }
    // todo: can be object
    public int? BorderWidth { get; set; }
    // todo: can be object
    public int? BorderRadius { get; set; }
    public int? CategoryPercentage { get; set; }
    /// <summary>
    /// can be number|object|false
    /// </summary>    
    public object? Clip { get; set; }
    public bool? Grouped { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBackgroundColor { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBorderColor { get; set; }
    public int? HoverBorderWidth { get; set; }
    public int? HoverBorderRadius { get; set; }
    public string? IndexAxis { get; set; }
    /// <summary>
    /// number|'auto'
    /// This option can be used to inflate the rects that are used to draw the bars. This can be used to hide artifacts between bars when barPercentage(#barpercentage) * categoryPercentage(#categorypercentage) is 1. The default value 'auto' should work in most cases
    /// <see href="https://www.chartjs.org/docs/latest/charts/bar.html#inflateamount">ChartJs docs</see>
    /// </summary>      
    public int? InflateAmount { get; set; }
    public int? MaxBarThickness { get; set; }
    public int? MinBarLength { get; set; }
    public string? Label { get; set; }
    public int? Order { get; set; }
    /// <summary>
    /// Types
    ///    The pointStyle argument accepts the following type of inputs: string, Image and HTMLCanvasElement
    ///
    ///    #Info
    ///    When a string is provided, the following values are supported:
    ///
    ///    'circle'
    ///    'cross'
    ///    'crossRot'
    ///    'dash'
    ///    'line'
    ///    'rect'
    ///    'rectRounded'
    ///    'rectRot'
    ///    'star'
    ///    'triangle'
    ///    If the value is an image or a canvas element, that image or canvas element is drawn on the canvas usin
    /// </summary>    
    public string? PointStyle { get; set; }
    public bool? SkipNull { get; set; }
    public string? Stack { get; set; }
    public string? XAxisID { get; set; }
    public string? YAxisID { get; set; }
}