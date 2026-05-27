
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
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// can be object|object[]| number[]|string[]
    /// </summary>    
    public IList<object> Data { get; set; } = [];
    public ChartType? Type { get; set; }
    /// <summary>
    /// If you have a lot of data points, it can be more performant to enable spanGaps. 
    /// This disables segmentation of the line, which can be an unneeded step.
    /// </summary>  
    public object? SpanGaps { get; set; }
    /// <summary>
    /// How Chart.js parses dataset data. Set to false for better performance when data is already in the internal format.
    /// </summary>
    public object? Parsing { get; set; }
    /// <summary>
    /// Per-dataset animation options. Set to false to disable animations for this dataset.
    /// </summary>
    public object? Animation { get; set; }
    public Animations? Animations { get; set; }
    /// <summary>
    /// Per-dataset transition options keyed by transition mode.
    /// </summary>
    public Dictionary<string, object>? Transitions { get; set; }
    /// <summary>
    /// Set to true when data indices are unique, sorted, and consistent across datasets for better performance.
    /// </summary>
    public bool? Normalized { get; set; }
    public bool? Hidden { get; set; }
    /// <summary>
    /// DataLabelsConfig per Dataset <see href="https://chartjs-plugin-datalabels.netlify.app/guide/">ChartJs datalabels plugin</see>
    /// </summary>
    public DataLabelsConfig? Datalabels { get; set; }
    public ChartJsDatasetTooltip? Tooltip { get; set; }

}

public record ChartJsDatasetTooltip
{
    public TooltipCallbacks? Callbacks { get; set; }
}
#pragma warning restore CA2227
