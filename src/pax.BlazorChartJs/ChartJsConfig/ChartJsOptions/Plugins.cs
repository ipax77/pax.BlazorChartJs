namespace pax.BlazorChartJs;

public record Plugins
{
    public Legend? Legend { get; set; }
    public Title? Title { get; set; }
    public Title? Subtitle { get; set; }
    public Tooltip? Tooltip { get; set; }
#pragma warning disable CA2227
    public ICollection<ArbitraryLineConfig>? ArbitraryLines { get; set; }
#pragma warning restore CA2227
    public DataLabelsConfig? Datalabels { get; set; }
    public HtmlLegend? HtmlLegend { get; set; }
}


