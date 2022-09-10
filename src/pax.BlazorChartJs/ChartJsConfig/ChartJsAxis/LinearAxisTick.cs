
namespace pax.BlazorChartJs;

public record LinearAxisTick : CartesianAxisTick
{
    /// <summary>
    /// The number of ticks to generate. If specified, this overrides the automatic generation.
    /// </summary>      
    public int? Count { get; set; }
    /// <summary>
    /// The <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat">Intl.NumberFormat</see> options used by the default label formatter
    /// </summary>      
    public object? Format { get; set; }
    /// <summary>
    /// if defined and stepSize is not specified, the step size will be rounded to this many decimal places.
    /// </summary>      
    public int? Precision { get; set; }
    /// <summary>
    /// User-defined fixed step size for the scale. <see href="https://www.chartjs.org/docs/latest/axes/cartesian/linear.html#step-size">more...</see>
    /// </summary>      
    public double? StepSize { get; set; }
}
