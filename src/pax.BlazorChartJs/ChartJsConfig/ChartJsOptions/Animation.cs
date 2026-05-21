namespace pax.BlazorChartJs;

public record Animation
{
    /// <summary>
    /// The number of milliseconds an animation takes.
    /// </summary>
    public double? Duration { get; set; }
    /// <summary>
    /// Easing function <see href="https://www.chartjs.org/docs/latest/configuration/animations.html#easing">ChartJs Docs</see>
    /// </summary>
    public string? Easing { get; set; }
    /// <summary>
    /// Delay before starting the animations.
    /// </summary>
    public IndexableOption<double>? Delay { get; set; }
    /// <summary>
    /// If set to true, the animations loop endlessly.
    /// </summary>
    public IndexableOption<bool>? Loop { get; set; }
    /// <summary>
    /// Start value for the animation. Current value is used when undefined.
    /// </summary>
    public object? From { get; set; }
    public object? To { get; set; }
    /// <summary>
    /// Registers OnProgress Event - ChartJsEvent type = ChartJsAnimationProgressEvent
    /// </summary>    
    public bool? OnProgressEvent { get; set; }
    public ChartJsFunction? OnProgress { get; set; }
    /// <summary>
    /// Registers OnComplete Event - ChartJsEvent type = ChartJsAnimationCompleteEvent
    /// </summary>    
    public bool? OnCompleteEvent { get; set; }
    public ChartJsFunction? OnComplete { get; set; }
}


