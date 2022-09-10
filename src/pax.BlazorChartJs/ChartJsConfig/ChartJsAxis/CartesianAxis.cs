
namespace pax.BlazorChartJs;

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
