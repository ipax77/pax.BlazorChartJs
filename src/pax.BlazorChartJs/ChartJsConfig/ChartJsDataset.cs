
namespace pax.BlazorChartJs;

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
    public object Data { get; set; } = new List<int> { 1, 2, 3 };
}