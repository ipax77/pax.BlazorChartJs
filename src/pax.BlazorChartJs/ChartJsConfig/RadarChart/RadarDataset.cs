namespace pax.BlazorChartJs;

/// <summary>
/// Radar chart dataset <see href="https://www.chartjs.org/docs/latest/charts/radar.html">ChartJs docs</see>
/// </summary>
public record RadarDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>
    public IndexableOption<string>? BackgroundColor { get; set; }
    /// <summary>
    /// BorderCapStyle options: 'butt'|'round'|'square' <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineCap">MDN</see>
    /// </summary>
    public IndexableOption<string>? BorderCapStyle { get; set; }
    public IndexableOption<bool>? CapBezierPoints { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>
    public IndexableOption<string>? BorderColor { get; set; }
    /// <summary>
    /// BorderDash <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/setLineDash">MDN</see>
    /// </summary>
#pragma warning disable CA2227
    public IndexableOption<object>? BorderDash { get; set; }
    /// <summary>
    /// BorderDashOffset <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineDashOffset">MDN</see>
    /// </summary>
    public IndexableOption<double>? BorderDashOffset { get; set; }
    /// <summary>
    /// BorderJoinStyle options: 'round'|'bevel'|'miter'
    /// </summary>    
    public IndexableOption<string>? BorderJoinStyle { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<string>? CubicInterpolationMode { get; set; }
    public IndexableOption<bool>? DrawActiveElementsOnTop { get; set; }
    public IndexableOption<string>? HoverBackgroundColor { get; set; }
    public IndexableOption<string>? HoverBorderCapStyle { get; set; }
    public IndexableOption<string>? HoverBorderColor { get; set; }
    public IndexableOption<object>? HoverBorderDash { get; set; }
#pragma warning restore CA2227    
    public IndexableOption<double>? HoverBorderDashOffset { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle options: 'round'|'bevel'|'miter'
    /// </summary>   
    public IndexableOption<string>? HoverBorderJoinStyle { get; set; }
    public IndexableOption<double>? HoverBorderWidth { get; set; }
    public IndexableOption<double>? HitRadius { get; set; }
    public IndexableOption<double>? HoverRadius { get; set; }
    public string? IndexAxis { get; set; }
    /// <summary>
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea.
    /// 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>
    public object? Clip { get; set; }
    /// <summary>
    /// Fill <see href="https://www.chartjs.org/docs/latest/charts/area.html">MDN</see>
    /// </summary>
    public IndexableOption<object>? Fill { get; set; }
    public string? Label { get; set; }
    public int? Order { get; set; }
    public IndexableOption<double>? Radius { get; set; }
    public IndexableOption<double>? Rotation { get; set; }
    public IndexableOption<object>? Segment { get; set; }
    public bool? ShowLine { get; set; }
    public string? Stack { get; set; }
    public IndexableOption<object>? Stepped { get; set; }
    /// <summary>
    /// Bezier curve tension of the line. Set to 0 to draw straight lines.
    /// </summary>
    public IndexableOption<double>? Tension { get; set; }
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
    public string? XAxisID { get; set; }
    public string? YAxisID { get; set; }
}
