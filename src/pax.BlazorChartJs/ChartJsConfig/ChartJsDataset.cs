
namespace pax.BlazorChartJs;

# pragma warning disable CA2227
/// <summary>
/// ChartJsDataset
/// </summary>
public record ChartJsDataset
{
    /// <summary>
    /// Id
    /// </summary>        
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// can be object|object[]| number[]|string[]
    /// </summary>    
    public IList<object> Data { get; set; } = new List<object>();
    public ChartType? Type { get; set; }
    /// <summary>
    /// If you have a lot of data points, it can be more performant to enable spanGaps. 
    /// This disables segmentation of the line, which can be an unneeded step.
    /// </summary>  
    public object? SpanGaps { get; set; }
    public bool? Hidden { get; set; }
    /// <summary>
    /// DataLabelsConfig per Dataset <see href="https://chartjs-plugin-datalabels.netlify.app/guide/">ChartJs datalabels plugin</see>
    /// </summary>
    public DataLabelsConfig? Datalabels { get; set; }

}
#pragma warning restore CA2227