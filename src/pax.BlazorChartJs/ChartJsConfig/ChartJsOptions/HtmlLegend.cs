
namespace pax.BlazorChartJs;

/// <summary>
/// Create a custom HTML legend using a plugin and connect it to the chart in lieu of the default on-canvas legend.
/// </summary>
public record HtmlLegend
{
    /// <summary>
    /// ID of the container to put the legend in
    /// </summary>
    public string? ContainerID { get; set; }
}