namespace pax.BlazorChartJs;

public record Plugins
{
    /// <summary>
    /// The chart legend displays data about the datasets that are appearing on the chart.
    /// </summary>
    public Legend? Legend { get; set; }
    /// <summary>
    /// The chart title defines text to draw at the top of the chart.
    /// </summary>
    public Title? Title { get; set; }
    /// <summary>
    /// Subtitle is a second title placed under the main title, by default. It has exactly the same configuration options with the main title.
    /// </summary>
    public Title? Subtitle { get; set; }
    public Tooltip? Tooltip { get; set; }
#pragma warning disable CA2227
    public ICollection<ArbitraryLineConfig>? ArbitraryLines { get; set; }
#pragma warning restore CA2227
    public DataLabelsConfig? Datalabels { get; set; }
}


