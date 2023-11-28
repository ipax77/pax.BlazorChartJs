namespace pax.BlazorChartJs;

public record Tooltip
{
    public bool? Enabled { get; set; }
    // public object? External { get; set; }
    /// <summary>
    /// Modes
    /// When configuring the interaction with the graph via interaction, hover or tooltips, a number of different modes are available.
    /// 
    /// options.hover and options.plugins.tooltip extend from options.interaction. So if mode, intersect or any other common settings are configured only in options.interaction, both hover and tooltips obey that.
    /// 
    /// The modes are detailed below and how they behave in conjunction with the intersect setting.
    /// 
    /// See how different modes work with the tooltip in <see href="https://www.chartjs.org/docs/latest/samples/tooltip/interactions.html">tooltip interactions sample</see> 
    /// </summary>      
    public string? Mode { get; set; }
    public bool? Intersect { get; set; }
    public string? Position { get; set; }
    // public object? Callbacks { get; set; }
    // public object? ItemSort { get; set; }
    // public object? Filter { get; set; }
    public object? BackgroundColor { get; set; }
    public object? TitleColor { get; set; }
    public Font? TitleFont { get; set; }
    public string? TitleAlign { get; set; }
    public double? TitleSpacing { get; set; }
    public double? TitleMarginBottom { get; set; }
    public object? BodyColor { get; set; }
    public Font? BodyFont { get; set; }
    public string? BodyAlign { get; set; }
    public double? BodySpacing { get; set; }
    public object? FooterColor { get; set; }
    public Font? FooterFont { get; set; }
    public string? FooterAlign { get; set; }
    public double? FooterSpacing { get; set; }
    public double? FooterMarginTop { get; set; }
    public Padding? Padding { get; set; }
    public double? CaretPadding { get; set; }
    public double? CaretSize { get; set; }
    public double? CornerRadius { get; set; }
    public object? MultiKeyBackground { get; set; }
    public bool? DisplayColors { get; set; }
    public double? BoxWidth { get; set; }
    public double? BoxHeight { get; set; }
    public double? BoxPadding { get; set; }
    public bool? UsePointStyle { get; set; }
    public object? BorderColor { get; set; }
    public double? BorderWidth { get; set; }
    public bool? Rtl { get; set; }
    public string? TextDirection { get; set; }
    /// <summary>
    /// Tooltip Alignment
    /// The xAlign and yAlign options define the position of the tooltip caret. If these parameters are unset, the optimal caret position is determined.
    /// 
    /// The following values for the xAlign setting are supported.
    /// 
    /// 'left'
    /// 'center'
    /// 'right'
    /// </summary>   
    public string? XAlign { get; set; }
    /// <summary>
    /// Tooltip Alignment
    /// The xAlign and yAlign options define the position of the tooltip caret. If these parameters are unset, the optimal caret position is determined.
    /// 
    /// The following values for the yAlign setting are supported.
    /// 
    /// 'top'
    /// 'center'
    /// 'bottom' 
    /// </summary>     
    public string? YAlign { get; set; }
    /// <summary>
    /// The events option defines the browser events that the chart should listen to for.
    /// Each of these events trigger hover and are passed to plugins. <see href="https://www.chartjs.org/docs/latest/configuration/interactions.html#event-option">more...</see>
    /// </summary>
#pragma warning disable CA2227
    public IList<string>? Events { get; set; }
#pragma warning restore CA2227      
}


