
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
    public IList<object> Data { get; set; } = new List<object> { 1, 2, 3 };
}
# pragma warning restore CA2227