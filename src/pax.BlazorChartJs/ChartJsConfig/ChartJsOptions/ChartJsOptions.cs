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
    public double? AspectRatio { get; set; }
    /// <summary>
    /// Delay the resize update by the given amount of milliseconds. This can ease the resize process by debouncing the update of the elements.
    /// </summary>    
    public double? ResizeDelay { get; set; }
    /// <summary>
    /// a string with a BCP 47 language tag, leveraging on <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat/NumberFormat">INTL NumberFormat</see>
    /// </summary>        
    public string? Locale { get; set; }
    public ChartJsLayout? Layout { get; set; }
    /// <summary>
    /// Plugins must define a unique id in order to be configurable. <see href="https://www.chartjs.org/docs/latest/developers/plugins.html#plugin-options">INTL NumberFormat</see>
    /// Use 'new' in a derived ChartJsConfig for custom plugins
    /// </summary>
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
    /// <summary>
    /// Override the window's default devicePixelRatio.
    /// </summary>
    public double? DevicePixelRatio { get; set; }
    /// <summary>
    /// options.interaction, the global interaction configuration is at Chart.defaults.interaction. To configure which events trigger chart interactions
    /// </summary>
    public Interactions? Interaction { get; set; }
    public bool? Stacked { get; set; }
    /// <summary>
    /// The events option defines the browser events that the chart should listen to for.
    /// Each of these events trigger hover and are passed to plugins. <see href="https://www.chartjs.org/docs/latest/configuration/interactions.html#event-option">more...</see>
    /// </summary>
    public IList<string>? Events { get; set; }
    /// <summary>
    /// Registers OnHover Event - ChartJsEvent type = ChartJsLabelHoverEvent
    /// </summary>   
    public bool? OnHoverEvent { get; set; }
    /// <summary>
    /// Registers OnClick Event - ChartJsEvent type = ChartJsLabelClickEvent
    /// </summary>   
    public bool? OnClickEvent { get; set; }
    /// <summary>
    /// Registers OnResize Event - ChartJsEvent type = ChartJsResizeEvent
    /// </summary>   
    public bool? OnResizeEvent { get; set; }
    public string? IndexAxis { get; set; }
}
