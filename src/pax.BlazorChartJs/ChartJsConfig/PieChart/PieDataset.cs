
namespace pax.BlazorChartJs;

/// <summary>
/// Pie chart dataset <see href="https://www.chartjs.org/docs/latest/charts/doughnut.html">ChartJs docs</see>
/// </summary>
public record PieDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public IndexableOption<string>? BackgroundColor { get; set; }
    /// <summary>
    /// BorderAlign - 'center'|'inner'
    /// </summary>      
    public IndexableOption<string>? BorderAlign { get; set; }
    /// <summary>
    /// BorderJoinStyle - 'round'|'bevel'|'miter'
    /// </summary>
    public IndexableOption<string>? BorderJoinStyle { get; set; }
    /// <summary>
    /// Border Radius
    /// If this value is a number, it is applied to all corners of the arc (outerStart, outerEnd, innerStart, innerRight). If this value is an object, the outerStart property defines the outer-start corner's border radius. Similarly, the outerEnd, innerStart, and innerEnd properties can also be specified    
    /// <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public object? BorderRadius { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/charts/doughnut.html#border-radius">ChartJs docs</see>
    /// </summary>
    public IndexableOption<string>? BorderColor { get; set; }
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
    // todo: can be object
    public IndexableOption<double>? BorderWidth { get; set; }
    public double? Circumference { get; set; }
    // todo: can be object
    /// <summary>
    /// can be number|object|false
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea. 0 = clip at chartArea.
    /// Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>    
    public object? Clip { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public IndexableOption<string>? HoverBackgroundColor { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle - 'round'|'bevel'|'miter'
    /// </summary>          
    public IndexableOption<string>? HoverBorderJoinStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public IndexableOption<double>? HoverBorderWidth { get; set; }
    public IndexableOption<double>? HoverOffset { get; set; }
    public IndexableOption<double>? Offset { get; set; }
    public double? Rotation { get; set; }
    public double? Spacing { get; set; }
    public double? Weight { get; set; }
    /// <summary>
    /// The portion of the chart that is cut out of the middle. If string and ending with '%', percentage of the chart radius. number is considered to be pixels.
    /// </summary>  
    public StringOrDoubleValue? Cutout { get; set; }
    /// <summary>
    /// The outer radius of the chart. If string and ending with '%', percentage of the maximum radius. number is considered to be pixels.
    /// </summary>  
    public StringOrDoubleValue? Radius { get; set; }
    public PieDatasetAnimation? Animation { get; set; }
}

public record PieDatasetAnimation
{
    /// <summary>
    /// If true, the chart will animate in with a rotation animation. This property is in the options.animation object.
    /// </summary>  
    public bool? AnimateRotate { get; set; }
    /// <summary>
    /// If true, will animate scaling the chart from the center outwards.
    /// </summary>  
    public bool? AnimateScale { get; set; }
}

public record DoughnutDataset : PieDataset
{

}