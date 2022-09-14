namespace pax.BlazorChartJs;

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


