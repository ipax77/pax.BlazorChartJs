namespace pax.BlazorChartJs;

/// <summary>
/// TimeCartesianAxis <see href="https://www.chartjs.org/docs/latest/axes/cartesian/time.htmls">ChartJs Docs</see>
/// </summary>
public record TimeCartesianAxis : LinearAxis
{
    public bool? OffsetAfterAutoskip { get; set; }
    public new TimeCartesianAxisTicks? Ticks { get; set; }
    public TimeCartesianAxisTime? Time { get; set; }
    public new StringOrDoubleValue? Min { get; set; }
    public new StringOrDoubleValue? Max { get; set; }
    public new StringOrDoubleValue? SuggestedMin { get; set; }
    public new StringOrDoubleValue? SuggestedMax { get; set; }
}
