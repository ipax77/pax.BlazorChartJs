
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

#pragma warning disable CA2227
/// <summary>
/// ChartJs v3.9.1 wrapper class
/// </summary>
public class ChartJsConfig
{
    /// <summary>
    /// ChartJs
    /// </summary>      
    public ChartJsConfig()
    {
    }

    /// <summary>
    /// ChartJsConfigGuid
    /// </summary>    
    [JsonIgnore]
    public Guid ChartJsConfigGuid { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// ChartType
    /// </summary>    
    public string Type { get; set; } = "bar";
    /// <summary>
    /// Data
    /// </summary>       
    public ChartData Data { get; set; } = new();
    /// <summary>
    /// Datasets
    /// </summary>     
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ChartJsOptions? Options { get; set; }
}

/// <summary>
/// ChartData
/// </summary>   
public class ChartData
{
    /// <summary>
    /// ChartData
    /// </summary>      
    public ChartData()
    {
        Labels = new List<string>();
        Datasets = new List<Dataset>();

    }

    /// <summary>
    /// Labels
    /// </summary>       
    public ICollection<string> Labels { get; set; }
    /// <summary>
    /// Datasets
    /// </summary>       
    public ICollection<Dataset> Datasets { get; set; }

}

#pragma warning restore CA2227

/// <summary>
/// ChartType
/// </summary>
public enum ChartType
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Bar
    /// </summary>
    Bar = 1
}