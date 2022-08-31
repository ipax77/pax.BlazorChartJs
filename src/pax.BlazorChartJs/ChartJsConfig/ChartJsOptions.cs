using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

# pragma warning disable CA2227

/// <summary>
/// Configuration Options
/// </summary>
public record ChartJsOptions
{
    /// <summary>
    /// Resizes the chart canvas when its container does (<see href="https://www.chartjs.org/docs/latest/configuration/responsive.html#important-note">important note...</see>).
    /// </summary>
    public bool? Responsive { get; set; }
    /// <summary>
    /// Maintain the original canvas aspect ratio (width / height) when resizing.
    /// </summary>    
    public bool? MaintainAspectRatio { get; set; }
    /// <summary>
    /// Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas). Note that this option is ignored if the height is explicitly defined either as attribute or via the style. The default value varies by chart type; Radial charts (doughnut, pie, polarArea, radar) default to 1 and others default to 2.
    /// </summary>    
    public int? AspectRatio { get; set; }
    /// <summary>
    /// Delay the resize update by the given amount of milliseconds. This can ease the resize process by debouncing the update of the elements.
    /// </summary>    
    public int? ResizeDelay { get; set; }
    /// <summary>
    /// a string with a BCP 47 language tag, leveraging on <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat/NumberFormat">INTL NumberFormat</see>
    /// </summary>        
    public string? Locale { get; set; }
    public Layout? Layout { get; set; }
    public Plugins? Plugins { get; set; }
    public ChartJsOptionsScales? Scales { get; set; }
    /// <summary>
    /// false or new Animation()
    /// </summary>
    public object? Animation { get; set; }
    public Animations? Animations { get; set; }
    /// <summary>
    /// The core transitions are 'active', 'hide', 'reset', 'resize', 'show'.
    /// A custom transition can be used by passing a custom mode to <see href="https://www.chartjs.org/docs/latest/developers/api.html#updatemode">update</see>.
    /// Transition extends the main animation configuration and <see href="https://www.chartjs.org/docs/latest/configuration/animations.html#animation-configuration">animations configuration</see>.
    /// </summary>
    public Dictionary<string, object>? Transitions { get; set; }
}

public record Animation
{
    /// <summary>
    /// The number of milliseconds an animation takes.
    /// </summary>
    public int? Duration { get; set; }
    /// <summary>
    /// Easing function <see href="https://www.chartjs.org/docs/latest/configuration/animations.html#easing">ChartJs Docs</see>
    /// </summary>
    public string? Easing { get; set; }
    /// <summary>
    /// Delay before starting the animations.
    /// </summary>
    public int? Delay { get; set; }
    /// <summary>
    /// If set to true, the animations loop endlessly.
    /// </summary>
    public bool? Loop { get; set; }
}

public record Animations
{
    /// <summary>
    /// The property names this configuration applies to. Defaults to the key name of this object.
    /// </summary>
    public IList<string>? Properties { get; set; }
    /// <summary>
    /// Type of property, determines the interpolator used. Possible values: 'number', 'color' and 'boolean'.
    /// Only really needed for 'color', because typeof does not get that right.
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// Start value for the animation. Current value is used when undefined
    /// </summary>
    public object? From { get; set; }
    /// <summary>
    /// End value for the animation. Updated value is used when undefined
    /// </summary>
    public object? To { get; set; }
    /// <summary>
    /// disables animation defined by the collection of 'colors' properties
    /// </summary>
    public bool? Colors { get; set; }
    /// <summary>
    /// disables animation defined by the 'x' property 
    /// </summary>
    public bool? X { get; set; }

}

public record Plugins
{
    public Legend? Legend { get; set; }
    public Title? Title { get; set; }
    public Title? Subtitle { get; set; }
    public Tooltip? Tooltip { get; set; }
    public ICollection<ArbitraryLineConfig>? ArbitraryLines { get; set; }
    public LabelsConfig? Labels { get; set; }
    public DataLabelsConfig? Datalabels { get; set; }
}

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
    public int? TitleSpacing { get; set; }
    public int? TitleMarginBottom { get; set; }
    public object? BodyColor { get; set; }
    public Font? BodyFont { get; set; }
    public string? BodyAlign { get; set; }
    public int? BodySpacing { get; set; }
    public object? FooterColor { get; set; }
    public Font? FooterFont { get; set; }
    public string? FooterAlign { get; set; }
    public int? FooterSpacing { get; set; }
    public int? FooterMarginTop { get; set; }
    public Padding? Padding { get; set; }
    public int? CaretPadding { get; set; }
    public int? CaretSize { get; set; }
    public int? CornerRadius { get; set; }
    public object? MultiKeyBackground { get; set; }
    public bool? DisplayColors { get; set; }
    public int? BoxWidth { get; set; }
    public int? BoxHeight { get; set; }
    public int? BoxPadding { get; set; }
    public bool? UsePointStyle { get; set; }
    public object? BorderColor { get; set; }
    public int? BorderWidth { get; set; }
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
}

public record Legend
{
    public bool? Display { get; set; }
    /// <summary>
    /// Position
    /// Position of the legend. Options are:
    /// 
    /// 'top'
    /// 'left'
    /// 'bottom'
    /// 'right'
    /// 'chartArea'
    /// When using the 'chartArea' option the legend position is at the moment not configurable, it will always be on the left side of the chart in the middle.    
    /// <see href="https://www.chartjs.org/docs/latest/configuration/legend.html#position">ChartJs docs</see>
    /// </summary>
    public string? Position { get; set; }
    /// <summary>
    /// Align
    /// Alignment of the legend. Options are:
    /// 
    /// 'start'
    /// 'center'
    /// 'end'
    /// Defaults to 'center' for unrecognized values   
    /// <see href="https://www.chartjs.org/docs/latest/configuration/legend.html#align">ChartJs docs</see>
    /// </summary>    
    public string? Align { get; set; }
    public int? MaxHeight { get; set; }
    public int? MaxWidth { get; set; }
    public bool? FullSize { get; set; }
    // public object? OnClick { get; set; }
    // public object? OnHover { get; set; }
    // public object? OnLeave { get; set; }
    public bool? Reverse { get; set; }
    public Labels? Labels { get; set; }
    /// <summary>
    /// true for rendering the legends from right to left.
    /// </summary>        
    public bool? Rtl { get; set; }
    /// <summary>
    /// This will force the text direction 'rtl' or 'ltr' on the canvas for rendering the legend, regardless of the css specified on the canvas
    /// </summary>       
    public string? TextDirection { get; set; }
    public Title? Title { get; set; }
}

public record Labels
{
    public int? BoxWidth { get; set; }
    public int? BoxHeight { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>         
    public object? Color { get; set; }
    public Font? Font { get; set; }
    public int? Padding { get; set; }
    // public object? GenerateLabels { get; set; }
    // public object? Filter { get; set; }
    // public object? Sort { get; set; }
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
}

public record Layout
{
    public bool? AutoPadding { get; set; }
    public Padding? Padding { get; set; }
}

public record ChartJsOptionsScales
{
    public object? X { get; set; }
    public object? Y { get; set; }
    public object? R { get; set; }
}

public record Title
{
    /// <summary>
    /// Align
    /// Alignment of the title. Options are:
    /// 
    /// 'start'
    /// 'center'
    /// 'end'    
    /// </summary>
    public string? Align { get; set; }
    public object? Color { get; set; }
    public bool? Display { get; set; }
    public bool? FullSize { get; set; }
    /// <summary>
    /// Position
    /// Possible title position values are:
    /// 
    /// 'top'
    /// 'left'
    /// 'bottom'
    /// 'right'
    /// </summary>    
    public string? Position { get; set; }
    public Font? Font { get; set; }
    public Padding? Padding { get; set; }
    public string? Text { get; set; }
}

public record Font
{
    public string? Family { get; set; }
    public int? Size { get; set; }
    /// <summary>
    /// Default font style. Does not apply to tooltip title or footer. Does not apply to chart title. Follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
    /// </summary>
    public string? Style { get; set; }
    /// <summary>
    /// Weight - css property
    /// </summary>    
    public string? Weight { get; set; }
    /// <summary>
    /// LineHeight - css property
    /// e.g.:
    /// normal
    /// 2.5
    /// 3em
    /// 150%
    /// 32px
    /// </summary>      
    public object? LineHeight { get; set; }

}

public record Padding
{
    public Padding()
    {

    }

    public Padding(double allSides)
    {
        Left = allSides;
        Top = allSides;
        Right = allSides;
        Bottom = allSides;
    }

    public double? Left { get; set; }
    public double? Top { get; set; }
    public double? Right { get; set; }
    public double? Bottom { get; set; }
    public double? Z { get; set; }
}


