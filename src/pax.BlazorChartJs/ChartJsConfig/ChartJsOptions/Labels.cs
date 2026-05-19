namespace pax.BlazorChartJs;

public record Labels
{
    public double? BorderRadius { get; set; }
    public double? BoxWidth { get; set; }
    public double? BoxHeight { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>         
    public IndexableOption<string>? Color { get; set; }
    public Font? Font { get; set; }
    public double? Padding { get; set; }
    public ChartJsFunction? GenerateLabels { get; set; }
    public ChartJsFunction? Filter { get; set; }
    public ChartJsFunction? Sort { get; set; }
    /// <summary>
    /// Types
    /// The pointStyle argument accepts the following type of inputs: string, Image and HTMLCanvasElement
    /// 
    /// #Info
    /// When a string is provided, the following values are supported:
    /// 
    /// 'circle'
    /// 'cross'
    /// 'crossRot'
    /// 'dash'
    /// 'line'
    /// 'rect'
    /// 'rectRounded'
    /// 'rectRot'
    /// 'star'
    /// 'triangle'
    /// If the value is an image or a canvas element, that image or canvas element is drawn on the canvas using drawImage .
    /// </summary>    
    public string? PointStyle { get; set; }
    /// <summary>
    /// Horizontal alignment of the label text. Options are: 'left', 'right' or 'center'.
    /// </summary>
    public string? TextAlign { get; set; }
    public bool? UsePointStyle { get; set; }
    public string? PointStyleWidth { get; set; }
    /// <summary>
    /// If you have a lot of data points, it can be more performant to enable spanGaps. 
    /// This disables segmentation of the line, which can be an unneeded step.
    /// </summary>  
    public bool? SpanGaps { get; set; }
    public bool? UseBorderRadius { get; set; }
}


