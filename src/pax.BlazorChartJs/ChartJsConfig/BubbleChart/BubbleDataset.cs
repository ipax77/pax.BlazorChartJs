namespace pax.BlazorChartJs;
public record BubbleDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public IndexableOption<string>? BackgroundColor { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    /// <summary>
    /// Clip - number|object|false
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea. 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>    
    public object? Clip { get; set; }
    /// <summary>
    /// Draw the active bubbles of a dataset over the other bubbles of the dataset
    /// </summary>
    public IndexableOption<bool>? DrawActiveElementsOnTop { get; set; }
    public IndexableOption<string>? HoverBackgroundColor { get; set; }
    public IndexableOption<string>? HoverBorderColor { get; set; }
    public IndexableOption<double>? HoverBorderWidth { get; set; }
    public IndexableOption<double>? HoverRadius { get; set; }
    public IndexableOption<double>? HitRadius { get; set; }
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
    public IndexableOption<string>? PointStyle { get; set; }
    public IndexableOption<double>? Rotation { get; set; }
    public IndexableOption<double>? Radius { get; set; }

}

public record BubbleDataPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double R { get; set; }
}