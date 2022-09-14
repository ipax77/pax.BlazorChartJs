namespace pax.BlazorChartJs;

/// <summary>
/// DataLabelsConfig <see href="https://chartjs-plugin-datalabels.netlify.app/guide/">ChartJs datalabels plugin</see>
/// </summary>
public record DataLabelsConfig
{
    public string? Align { get; set; }
    public string? Anchor { get; set; }
    public object? BackgroundColor { get; set; }
    public object? BorderColor { get; set; }
    public double? BorderRadius { get; set; }
    public double? BorderWidth { get; set; }
    public bool? Clamp { get; set; }
    public bool? Clip { get; set; }
    public object? Color { get; set; }
    public object? Display { get; set; }
    public Font? Font { get; set; }
    public object? Formatter { get; set; }
    public object? Labels { get; set; }
    public object? Listeners { get; set; }
    public double? Offset { get; set; }
    public double? Opacity { get; set; }
    public Padding? Padding { get; set; }
    public double? Rotation { get; set; }
    public string? TextAlign { get; set; }
    public string? TextStrokeColor { get; set; }
    public double? TextStrokeWidth { get; set; }
    public double? TextShadowBlur { get; set; }
    public string? TextShadowColor { get; set; }
}

