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

    [JsonIgnore]
    public Guid ChartJsConfigGuid { get; private set; } = Guid.NewGuid();

    public ChartType? Type { get; set; }
    public ChartData Data { get; set; } = new();
    public ChartJsOptions? Options { get; set; }
}

public class ChartData
{
    public ChartData()
    {
        Labels = new List<string>();
        Datasets = new List<Dataset>();

    }

    public ICollection<string> Labels { get; set; }
    public ICollection<Dataset> Datasets { get; set; }

}

#pragma warning restore CA2227

public enum ChartType
{
    none = 0,
    line = 1,
    bar = 2
}