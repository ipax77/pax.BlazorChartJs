namespace pax.BlazorChartJs;

public record Interactions
{
    /// <summary>
    /// When configuring the interaction with the graph via interaction, hover or tooltips, a number of different modes are available.
    /// options.hover and options.plugins.tooltip extend from options.interaction.So if mode, intersect or any other common settings are configured only in options.interaction, both hover and tooltips obey that.
    /// The modes are detailed below and how they behave in conjunction with the intersect setting.
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


