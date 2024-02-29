
namespace pax.BlazorChartJs;

#pragma warning disable CA2227
/// <summary>
/// Line chart dataset <see href="https://www.chartjs.org/docs/latest/charts/line.html">ChartJs docs</see>
/// </summary>
public record LineDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public string? BackgroundColor { get; set; }
    public string? BorderCapStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public string? BorderColor { get; set; }
    public IList<double>? BorderDash { get; set; }
    public double? BorderDashOffset { get; set; }
    /// <summary>
    /// BorderJoinStyle options: 'round'|'bevel'|'miter' - default 'miter'
    /// </summary>    
    public string? BorderJoinStyle { get; set; }
    // todo: can be object
    public double? BorderWidth { get; set; }
    /// <summary>
    /// Clip - number|object|false
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea. 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>    
    public object? Clip { get; set; }
    /// <summary>
    /// cubicInterpolationMode
    /// The following interpolation modes are supported.
    /// 
    /// 'default'
    /// 'monotone'
    /// The 'default' algorithm uses a custom weighted cubic interpolation, which produces pleasant curves for all types of datasets.
    /// 
    /// The 'monotone' algorithm is more suited to y = f(x) datasets: it preserves monotonicity (or piecewise monotonicity) of the dataset being interpolated, and ensures local extremums (if any) stay at input data points.
    /// 
    /// If left untouched (undefined), the global options.elements.line.cubicInterpolationMode property is used
    /// </summary>        
    public string? CubicInterpolationMode { get; set; }
    public IndexableOption<bool>? DrawActiveElementsOnTop { get; set; }
    /// <summary>
    /// Fill - boolean|string
    /// <see href="https://www.chartjs.org/docs/latest/charts/area.html">ChartJs docs</see>
    /// </summary>       
    public object? Fill { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBackgroundColor { get; set; }
    /// <summary>
    ///  The CanvasRenderingContext2D.lineCap property of the Canvas 2D API determines the shape used to draw the end points of lines.
    /// 
    /// Note: Lines can be drawn with the stroke(), strokeRect(), and strokeText() methods.
    /// 
    /// Value
    /// One of the followings:
    /// 
    /// "butt"
    /// The ends of lines are squared off at the endpoints. Default value.
    /// 
    /// "round"
    /// The ends of lines are rounded.
    /// 
    /// "square"
    /// The ends of lines are squared off by adding a box with an equal width and half the height of the line's thickness.
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineCap">Canvas docs</see>
    /// </summary>
    public string? HoverBorderCapStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBorderColor { get; set; }
    public IList<double>? HoverBorderDash { get; set; }
    public double? HoverBorderDashOffset { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle - 'round'|'bevel'|'miter'
    /// </summary>          
    public string? HoverBorderJoinStyle { get; set; }
    public double? HoverBorderWidth { get; set; }
    public string? IndexAxis { get; set; }
    /// <summary>
    /// number|'auto'
    /// This option can be used to inflate the rects that are used to draw the bars. This can be used to hide artifacts between bars when barPercentage(#barpercentage) * categoryPercentage(#categorypercentage) is 1. The default value 'auto' should work in most cases
    /// <see href="https://www.chartjs.org/docs/latest/charts/bar.html#inflateamount">ChartJs docs</see>
    /// </summary>      
    public string? Label { get; set; }
    public double? Order { get; set; }
    public IndexableOption<string>? PointBackgroundColor { get; set; }
    public IndexableOption<string>? PointBorderColor { get; set; }
    public IndexableOption<double>? PointBorderWidth { get; set; }
    public IndexableOption<double>? PointHitRadius { get; set; }
    public IndexableOption<string>? PointHoverBackgroundColor { get; set; }
    public IndexableOption<string>? PointHoverBorderColor { get; set; }
    public IndexableOption<double>? PointHoverBorderWidth { get; set; }
    public IndexableOption<double>? PointHoverRadius { get; set; }
    public IndexableOption<double>? PointRadius { get; set; }
    public IndexableOption<double>? PointRotation { get; set; }
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
    public IndexableOption<string>? PointStyle { get; set; }
    /// <summary>
    /// Segment
    /// Line segment styles can be overridden by scriptable options in the segment object. Currently all of the border* and backgroundColor options are supported. The segment styles are resolved for each section of the line between each point. undefined fallbacks to main line styles.
    /// 
    /// TIP
    /// 
    /// To be able to style gaps, you need the spanGaps option enabled.
    /// 
    /// Context for the scriptable segment contains the following properties:
    /// 
    /// type: 'segment'
    /// p0: first point element
    /// p1: second point element
    /// p0DataIndex: index of first point in the data array
    /// p1DataIndex: index of second point in the data array
    /// datasetIndex: dataset index
    /// <see href="https://www.chartjs.org/docs/latest/charts/line.html#segment">ChartJs docs</see>
    /// </summary>        
    public object? Segment { get; set; }
    public bool? ShowLine { get; set; }
    /// <summary>
    /// number|'auto'
    /// If true, lines will be drawn between points with no or null data. If false, points with null data will create a break in the line.
    /// Can also be a number specifying the maximum gap length to span. The unit of the value depends on the scale used.
    /// <see href="https://www.chartjs.org/docs/latest/charts/line.html#line-styling">ChartJs docs</see>
    /// </summary>       
    public new object? SpanGaps { get; set; }
    public string? Stack { get; set; }
    /// <summary>
    /// Stepped
    /// The following values are supported for stepped.
    /// 
    /// false: No Step Interpolation (default)
    /// true: Step-before Interpolation (eq. 'before')
    /// 'before': Step-before Interpolation
    /// 'after': Step-after Interpolation
    /// 'middle': Step-middle Interpolation
    /// If the stepped value is set to anything other than false, tension will be ignored  
    /// <see href="https://www.chartjs.org/docs/latest/charts/line.html#stepped">ChartJs docs</see>
    /// </summary>    
    public object? Stepped { get; set; }
    /// <summary>
    /// Bezier curve tension of the line. Set to 0 to draw straightlines. This option is ignored if monotone cubic interpolation is used.
    /// </summary>
    public double? Tension { get; set; }
    public string? XAxisID { get; set; }
    public string? YAxisID { get; set; }
}
#pragma warning restore CA2227