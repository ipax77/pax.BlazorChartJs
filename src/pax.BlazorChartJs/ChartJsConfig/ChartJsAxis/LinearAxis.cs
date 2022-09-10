
namespace pax.BlazorChartJs;

public record LinearAxis : CartesianAxis
{
    /// <summary>
    /// if true, scale will include 0 if it is not already included.
    /// </summary>      
    public bool? BeginAtZero { get; set; }
    /// <summary>
    /// Percentage (string ending with %) or amount (number) for added room in the scale range above and below data. <see href="https://www.chartjs.org/docs/latest/axes/cartesian/linear.html#grace">more...</see>
    /// </summary>      
    public object? Grace { get; set; }
}
