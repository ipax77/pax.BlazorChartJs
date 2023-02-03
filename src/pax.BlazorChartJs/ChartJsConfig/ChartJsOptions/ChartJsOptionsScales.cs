namespace pax.BlazorChartJs;

/// <summary>
/// Axes <see href="https://www.chartjs.org/docs/latest/axes/"></see>
/// </summary>
public record ChartJsOptionsScales
{
    public ChartJsAxis? X { get; set; }
    public ChartJsAxis? X1 { get; set; }
    public ChartJsAxis? Y { get; set; }
    public ChartJsAxis? Y1 { get; set; }
    public ChartJsAxis? R { get; set; }
}


