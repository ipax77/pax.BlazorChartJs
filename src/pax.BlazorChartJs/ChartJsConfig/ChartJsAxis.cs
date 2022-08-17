
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
    public string? Min { get; set; }
    public string? Max { get; set; }
    public bool? Reverse { get; set; }
    public string? Stacked { get; set; }
    public double? SuggestedMax { get; set; }
    public double? SuggestedMin { get; set; }
    public object? Ticks { get; set; }
    public string? Weight { get; set; }
    public Title? Title { get; set; }
}

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

/// <summary>
/// Cartesian Axes
/// Axes that follow a cartesian grid are known as 'Cartesian Axes'. Cartesian axes are used for line, bar, and bubble charts. Four cartesian axes are included in Chart.js by default.
/// 
/// linear
/// logarithmic
/// category
/// time
/// timeseries
/// </summary>
public record CartesianAxis : ChartJsAxis
{
    /// <summary>
    /// Scale Bounds
    /// The bounds property controls the scale boundary strategy (bypassed by min/max options).
    /// 
    /// 'data': makes sure data are fully visible, labels outside are removed
    /// 'ticks': makes sure ticks are fully visible, data outside are truncated    
    /// </summary>
    public string? Bounds { get; set; }
    /// <summary>
    /// Axis Position
    /// An axis can either be positioned at the edge of the chart, at the center of the chart area, or dynamically with respect to a data value.
    ///
    /// To position the axis at the edge of the chart, set the position option to one of: 'top', 'left', 'bottom', 'right'. To position the axis at the center of the chart area, set the position option to 'center'. In this mode, either the axis option must be specified or the axis ID has to start with the letter 'x' or 'y'. This is so chart.js knows what kind of axis (horizontal or vertical) it is. To position the axis with respect to a data value, set the position option to an object such as:
    ///    {
    ///        x: -20
    ///    }
    /// This will position the axis at a value of -20 on the axis with ID "x". For cartesian axes, only 1 axis may be specified.  
    /// </summary>
    public object? Position { get; set; }
    /// <summary>
    /// Stack group. Axes at the same position with same stack are stacked.
    /// </summary>    
    public string? Stack { get; set; }
    public int? StackWeight { get; set; }
    public bool? Offset { get; set; }
}

public record CartesianAxisTick : ChartJsAxisTick
{
    /// <summary>
    /// The tick alignment along the axis. Can be 'start', 'center', 'end', or 'inner'. inner alignment means align start for first tick and end for the last tick of horizontal axis
    /// </summary>      
    public string? Align { get; set; }
    /// <summary>
    /// The tick alignment perpendicular to the axis. Can be 'near', 'center', or 'far'. See <see href="https://www.chartjs.org/docs/latest/axes/cartesian/#tick-alignment">Tick Alignment</see>
    /// </summary>      
    public string? CrossAlign { get; set; }
    /// <summary>
    /// The number of ticks to examine when deciding how many labels will fit. Setting a smaller value will be faster, but may be less accurate when there is large variability in label length.
    /// </summary>       
    public int? SampleSize { get; set; }
    /// <summary>
    /// If true, automatically calculates how many labels can be shown and hides labels accordingly. Labels will be rotated up to maxRotation before skipping any. Turn autoSkip off to show all labels no matter what.    1
    /// </summary>       
    public bool? AutoSkip { get; set; }
    /// <summary>
    /// Padding between the ticks on the horizontal axis when autoSkip is enabled.
    /// </summary>      
    public int? AutoSkipPadding { get; set; }
    /// <summary>
    /// Should the defined min and max values be presented as ticks even if they are not "nice".
    /// </summary>      
    public bool? IncludeBounds { get; set; }
    /// <summary>
    /// Distance in pixels to offset the label from the centre point of the tick (in the x-direction for the x-axis, and the y-direction for the y-axis). Note: this can cause labels at the edges to be cropped by the edge of the canvas
    /// </summary>      
    public int? LabelOffset { get; set; }
    /// <summary>
    /// Maximum rotation for tick labels when rotating to condense labels. Note: Rotation doesn't occur until necessary. Note: Only applicable to horizontal scales.
    /// </summary>      
    public int? MaxRotation { get; set; }
    /// <summary>
    /// Minimum rotation for tick labels. Note: Only applicable to horizontal scales.
    /// </summary>      
    public int? MinRotation { get; set; }
    /// <summary>
    /// Flips tick labels around axis, displaying the labels inside the chart instead of outside. Note: Only applicable to vertical scales.
    /// </summary>      
    public bool? Mirror { get; set; }
    /// <summary>
    /// Maximum number of ticks and gridlines to show.
    /// </summary>      
    public int? MaxTicksLimit { get; set; }
}

public record LinearAxis : CartesianAxis
{
    /// <summary>
    /// if true, scale will include 0 if it is not already included.
    /// </summary>      
    public bool? BeginAtZero { get; set; }
    /// <summary>
    /// Percentage (string ending with %) or amount (number) for added room in the scale range above and below data. <see href="https://www.chartjs.org/docs/latest/axes/cartesian/linear.html#grace">more...</see>
    /// </summary>      
    public object? Grace { get; set; }
}

public record LinearAxisTick : CartesianAxisTick
{
    /// <summary>
    /// The number of ticks to generate. If specified, this overrides the automatic generation.
    /// </summary>      
    public int? Count { get; set; }
    /// <summary>
    /// The <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat">Intl.NumberFormat</see> options used by the default label formatter
    /// </summary>      
    public object? Format { get; set; }
    /// <summary>
    /// if defined and stepSize is not specified, the step size will be rounded to this many decimal places.
    /// </summary>      
    public int? Precision { get; set; }
    /// <summary>
    /// User-defined fixed step size for the scale. <see href="https://www.chartjs.org/docs/latest/axes/cartesian/linear.html#step-size">more...</see>
    /// </summary>      
    public double? StepSize { get; set; }
}