namespace pax.BlazorChartJs;

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
