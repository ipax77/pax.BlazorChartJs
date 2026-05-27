namespace pax.BlazorChartJs;

public record Tooltip
{
    public IndexableOption<bool>? Enabled { get; set; }
    public ChartJsFunction? External { get; set; }
    /// <summary>
    /// false or new Animation()
    /// </summary>
    public object? Animation { get; set; }
    public object? Animations { get; set; }
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
    public string? Axis { get; set; }
    public bool? IncludeInvisible { get; set; }
    public IndexableOption<string>? Position { get; set; }
    public TooltipCallbacks? Callbacks { get; set; }
    public ChartJsFunction? ItemSort { get; set; }
    public ChartJsFunction? Filter { get; set; }
    public IndexableOption<string>? BackgroundColor { get; set; }
    public IndexableOption<string>? TitleColor { get; set; }
    public IndexableOption<Font>? TitleFont { get; set; }
    public IndexableOption<string>? TitleAlign { get; set; }
    public IndexableOption<double>? TitleSpacing { get; set; }
    public IndexableOption<double>? TitleMarginBottom { get; set; }
    public IndexableOption<string>? BodyColor { get; set; }
    public IndexableOption<Font>? BodyFont { get; set; }
    public IndexableOption<string>? BodyAlign { get; set; }
    public IndexableOption<double>? BodySpacing { get; set; }
    public IndexableOption<string>? FooterColor { get; set; }
    public IndexableOption<Font>? FooterFont { get; set; }
    public IndexableOption<string>? FooterAlign { get; set; }
    public IndexableOption<double>? FooterSpacing { get; set; }
    public IndexableOption<double>? FooterMarginTop { get; set; }
    public Padding? Padding { get; set; }
    public IndexableOption<double>? CaretPadding { get; set; }
    public IndexableOption<double>? CaretSize { get; set; }
    public IndexableOption<double>? CornerRadius { get; set; }
    public IndexableOption<string>? MultiKeyBackground { get; set; }
    public IndexableOption<bool>? DisplayColors { get; set; }
    public IndexableOption<double>? BoxWidth { get; set; }
    public IndexableOption<double>? BoxHeight { get; set; }
    public double? BoxPadding { get; set; }
    public IndexableOption<bool>? UsePointStyle { get; set; }
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<bool>? Rtl { get; set; }
    public IndexableOption<string>? TextDirection { get; set; }
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
    public IndexableOption<string>? XAlign { get; set; }
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
    public IndexableOption<string>? YAlign { get; set; }
    /// <summary>
    /// The events option defines the browser events that the chart should listen to for.
    /// Each of these events trigger hover and are passed to plugins. <see href="https://www.chartjs.org/docs/latest/configuration/interactions.html#event-option">more...</see>
    /// </summary>
#pragma warning disable CA2227
    public IList<string>? Events { get; set; }
#pragma warning restore CA2227      
}

public record TooltipCallbacks
{
    public ChartJsFunction? BeforeTitle { get; set; }
    public ChartJsFunction? Title { get; set; }
    public ChartJsFunction? AfterTitle { get; set; }
    public ChartJsFunction? BeforeBody { get; set; }
    public ChartJsFunction? BeforeLabel { get; set; }
    public ChartJsFunction? Label { get; set; }
    public ChartJsFunction? LabelColor { get; set; }
    public ChartJsFunction? LabelTextColor { get; set; }
    public ChartJsFunction? LabelPointStyle { get; set; }
    public ChartJsFunction? AfterLabel { get; set; }
    public ChartJsFunction? AfterBody { get; set; }
    public ChartJsFunction? BeforeFooter { get; set; }
    public ChartJsFunction? Footer { get; set; }
    public ChartJsFunction? AfterFooter { get; set; }
}


