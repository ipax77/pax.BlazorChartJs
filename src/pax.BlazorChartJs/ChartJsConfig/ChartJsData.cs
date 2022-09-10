namespace pax.BlazorChartJs;
#pragma warning disable CA2227
public record ChartJsData
{
    public ChartJsData()
    {
        Labels = new List<string>();
        Datasets = new List<ChartJsDataset>();

    }

    public IList<string> Labels { get; set; }
    public virtual IList<ChartJsDataset> Datasets { get; set; }

}
#pragma warning restore CA2227