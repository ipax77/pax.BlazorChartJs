namespace pax.BlazorChartJs;
public record PolarAreaDataset : ChartJsDataset
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
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/charts/doughnut.html#border-radius">ChartJs docs</see>
    /// </summary>
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
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
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public IndexableOption<string>? HoverBorderColor { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle - 'round'|'bevel'|'miter'
    /// </summary>          
    public IndexableOption<string>? HoverBorderJoinStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public IndexableOption<double>? HoverBorderWidth { get; set; }
    /// <summary>
    /// By default the Arc is curved. If circular: false the Arc will be flat.
    /// </summary>
    public IndexableOption<bool>? Circular { get; set; }
}
