namespace pax.BlazorChartJs;

/// <summary>
/// Data decimation plugin configuration.
/// <see href="https://www.chartjs.org/docs/latest/configuration/decimation.html">ChartJs docs</see>
/// </summary>
public record DecimationConfig
{
    public bool? Enabled { get; set; }
    public string? Algorithm { get; set; }
    public double? Samples { get; set; }
    public double? Threshold { get; set; }
}
