namespace pax.BlazorChartJs;

#pragma warning disable CA2227
/// <summary>
/// chartjs-plugin-labels - <see href="https://github.com/emn178/chartjs-plugin-labels">GitHub</see> and <see href="https://github.com/DavideViolante/chartjs-plugin-labels">GitHub</see>
/// </summary>
public record LabelsConfig
{
    /// <summary>
    /// // render 'label', 'value', 'percentage', 'image' or custom function, default is 'percentage'
    /// </summary>
    public string? Render { get; set; }
    public double? Precision { get; set; }
    /// <summary>
    /// identifies whether or not labels of value 0 are displayed, default is false 
    /// </summary>
    public bool? ShowZero { get; set; }

    /// <summary>
    /// font size, default is defaultFontSize
    /// </summary>
    public double? FontSize { get; set; }
    /// <summary>
    /// font color, can be color array for each data or function for dynamic color, default is defaultFontColor
    /// </summary>
    public object? FontColor { get; set; }
    /// <summary>
    /// font style, default is defaultFontStyle
    /// </summary>
    public string? FontStyle { get; set; }
    /// <summary>
    /// font family, default is defaultFontFamily
    /// </summary>
    public string? FontFamily { get; set; }
    /// <summary>
    /// draw text shadows under labels, default is false
    /// </summary>
    public bool? TextShadow { get; set; }
    /// <summary>
    /// text shadow intensity, default is 6
    /// </summary>
    public double? ShadowBlur { get; set; }
    /// <summary>
    /// text shadow X offset, default is 3
    /// </summary>
    public double? ShadowOffsetX { get; set; }
    /// <summary>
    /// text shadow Y offset, default is 3 
    /// </summary>
    public double? ShadowOffsetY { get; set; }
    /// <summary>
    /// text shadow color, default is 'rgba(0,0,0,0.3)'
    /// </summary>
    public string? ShadowColor { get; set; }
    /// <summary>
    /// draw label in arc, default is false
    /// bar chart ignores this
    /// </summary>
    public bool? Arc { get; set; }
    /// <summary>
    /// 
    /// position to draw label, available value is 'default', 'border' and 'outside'
    /// bar chart ignores this
    /// default is 'default'
    /// </summary>
    public string? Position { get; set; }
    /// <summary>
    /// draw label even it's overlap, default is true
    /// bar chart ignores this
    /// </summary>
    public bool? Overlap { get; set; }
    /// <summary>
    /// show the real calculated percentages from the values and don't apply the additional logic to fit the percentages to 100 in total, default is false
    /// </summary>
    public bool? ShowActualPercentages { get; set; }
    /// <summary>
    /// set images when `render` is 'image' 
    /// </summary>
    public ICollection<LabelsConfigImage>? Images { get; set; }
    /// <summary>
    /// add padding when position is `outside`
    /// default is 2
    /// </summary>

    public double? OutsidePadding { get; set; }
    /// <summary>
    /// add margin of text when position is `outside` or `border`
    /// default is 2
    /// </summary>
    public double? TextMargin { get; set; }
}
#pragma warning restore CA2227

public record LabelsConfigImage
{
    public string? Src { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
}