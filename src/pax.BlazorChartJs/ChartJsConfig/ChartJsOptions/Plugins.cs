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
    public FillerOptions? Filler { get; set; }
    public Tooltip? Tooltip { get; set; }
    /// <summary>
    /// Options for an externally registered Chart.js plugin with the id <c>htmlLegend</c>.
    /// </summary>
    public HtmlLegendOptions? HtmlLegend { get; set; }
#pragma warning disable CA2227
    public ICollection<ArbitraryLineConfig>? ArbitraryLines { get; set; }
#pragma warning restore CA2227
    public DataLabelsConfig? Datalabels { get; set; }
    public DecimationConfig? Decimation { get; set; }
}

public record FillerOptions
{
    public bool? Propagate { get; set; }
    public string? DrawTime { get; set; }
}

public record HtmlLegendOptions
{
    /// <summary>
    /// ID of the DOM element that the external HTML legend plugin uses as its container.
    /// </summary>
    public string? ContainerID { get; set; }
}
