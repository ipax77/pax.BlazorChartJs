using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

#pragma warning disable CA2227
/// <summary>
/// ChartJs v3.9.1 wrapper class <see href="https://www.chartjs.org/docs/latest/configuration/">ChartJs docs</see>
/// NULL values are ignored (=> ChartJs default)
/// </summary>
public class ChartJsConfig
{
    public ChartJsConfig()
    {
    }

    /// <summary>
    /// ChartJsConfigGuid - used for canvas id and to track the dotnetobjectreference
    /// </summary>
    [JsonIgnore]
    public Guid ChartJsConfigGuid { get; private set; } = Guid.NewGuid();

    public ChartType? Type { get; set; }
    public ChartJsData Data { get; set; } = new();
    public ChartJsOptions? Options { get; set; }
}


public class ChartJsData
{
    public ChartJsData()
    {
        Labels = new List<string>();
        Datasets = new List<object>();

    }

    public ICollection<string> Labels { get; set; }
    public virtual ICollection<object> Datasets { get; set; }

}

#pragma warning restore CA2227

public enum ChartType
{
    none = 0,
    line = 1,
    bar = 2,
    doughnut = 3,
    pie = 4,
}