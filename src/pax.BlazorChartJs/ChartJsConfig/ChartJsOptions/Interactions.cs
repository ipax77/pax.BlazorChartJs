namespace pax.BlazorChartJs;

public record Interactions
{
    /// <summary>
    /// Sets which elements appear in the interaction
    /// Default: "nearest"
    /// </summary>   
    public string? Mode { get; set; }
    /// <summary>
    /// if true, the interaction mode only applies when the mouse position intersects an item on the chart.
    /// </summary>   
    public bool? Intersect { get; set; }
    /// <summary>
    /// Can be set to 'x', 'y', 'xy' or 'r' to define which directions are used in calculating distances.
    /// Defaults to 'x' for 'index' mode and 'xy' in dataset and 'nearest' modes.
    /// </summary>   
    public string? Axis { get; set; }
    /// <summary>
    /// if true, the invisible points that are outside of the chart area will also be included when evaluating interactions.
    /// </summary>   
    public bool? IncludeInvisible { get; set; }
}


