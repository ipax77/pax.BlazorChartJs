
namespace pax.BlazorChartJs;

public record TimeCartesianAxisTime
{
    public string? Source { get; set; }
    /// <summary>
    /// DisplayFormats <see href="https://www.chartjs.org/docs/latest/axes/cartesian/time.html#display-formats">ChartJs Docs</see>
    /// </summary>
    public object? DisplayFormats { get; set; }
    /// <summary>
    /// If boolean and true and the unit is set to 'week', then the first day of the week will be Monday.
    /// Otherwise, it will be Sunday. If number, the index of the first day of the week (0 - Sunday, 6 - Saturday)
    /// </summary>
    public object? IsoWeekday { get; set; }
    public string? Parser { get; set; }
    public string? Round { get; set; }
    public string? TooltipFormat { get; set; }
    public string? Unit { get; set; }
    public string? MinUnit { get; set; }
}
