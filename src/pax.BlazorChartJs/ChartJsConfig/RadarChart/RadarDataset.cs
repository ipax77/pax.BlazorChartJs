namespace pax.BlazorChartJs;

/// <summary>
/// Radar chart dataset <see href="https://www.chartjs.org/docs/latest/charts/radar.html">ChartJs docs</see>
/// </summary>
public record RadarDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>   
    public string? BackgroundColor { get; set; }
    /// <summary>
    /// BorderCapStyle options: 'butt'|'round'|'square' <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineCap">MDN</see>
    /// </summary>    
    public string? BorderCapStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>   
    public string? BorderColor { get; set; }
    /// <summary>
    /// BorderDash <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/setLineDash">MDN</see>
    /// </summary>    
#pragma warning disable CA2227
    public IList<double>? BorderDash { get; set; }
    /// <summary>
    /// BorderDashOffset <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineDashOffset">MDN</see>
    /// </summary>  
    public double? BorderDashOffset { get; set; }
    /// <summary>
    /// BorderJoinStyle options: 'round'|'bevel'|'miter'
    /// </summary>    
    public string? BorderJoinStyle { get; set; }
    public double? BorderWidth { get; set; }
    public object? HoverBackgroundColor { get; set; }
    public string? HoverBorderCapStyle { get; set; }
    public object? HoverBorderColor { get; set; }
    public IList<double>? HoverBorderDash { get; set; }
#pragma warning restore CA2227    
    public double? HoverBorderDashOffset { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle options: 'round'|'bevel'|'miter'
    /// </summary>   
    public string? HoverBorderJoinStyle { get; set; }
    public double? HoverBorderWidth { get; set; }
    /// <summary>
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea.
    /// 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>
    public object? Clip { get; set; }
    /// <summary>
    /// Fill <see href="https://www.chartjs.org/docs/latest/charts/area.html">MDN</see>
    /// </summary>
    public object? Fill { get; set; }
    public string? Label { get; set; }
    public int? Order { get; set; }
    /// <summary>
    /// Bezier curve tension of the line. Set to 0 to draw straight lines.
    /// </summary>
    public double? Tension { get; set; }
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
    public IndexableOption<object>? PointStyle { get; set; }
    /// <summary>
    /// If true, lines will be drawn between points with no or null data. If false, points with null data will create a break in the line.
    /// </summary>
    public new bool? SpanGaps { get; set; }
}
