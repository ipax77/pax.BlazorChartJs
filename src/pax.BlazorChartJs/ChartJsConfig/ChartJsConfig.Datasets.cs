
namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Adds the dataset to the config and updates the Chart
    /// </summary>
    public void AddDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        AddDatasets(new List<ChartJsDataset> { dataset });
    }

    /// <summary>
    /// Adds the datasets to the config and updates the Chart
    /// </summary>

    public void AddDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        if (!datasets.Any())
        {
            return;
        }

        foreach (var dataset in datasets)
        {
            Data.Datasets.Add(dataset);
        }
        OnDatasetsAdd(new DatasetsAddEventArgs(datasets));
    }

    /// <summary>
    /// Removes the dataset from the config and updates the Chart
    /// </summary>
    public void RemoveDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        RemoveDatasets(new List<ChartJsDataset>() { dataset });
    }

    /// <summary>
    /// Removes the datasets from the config and updates the Chart
    /// </summary>
    public void RemoveDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        foreach (var dataset in datasets)
        {
            Data.Datasets.Remove(dataset);
        }
        OnDatasetsRemove(new DatasetsRemoveEventArgs(datasets.Select(s => s.Id).ToList()));
    }

    /// <summary>
    /// Updates the dataset and Chart
    /// </summary>
    public void UpdateDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        UpdateDatasets(new List<ChartJsDataset>() { dataset });
    }

    /// <summary>
    /// Updates the datasets and Chart
    /// </summary>
    public void UpdateDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);
        foreach (var dataset in datasets)
        {
            var index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                Data.Datasets[index] = dataset;
            }
        }
        OnDatasetsUpdate(new DatasetsUpdateEventArgs(datasets));
    }

    /// <summary>
    /// Updates the Chart with the current Datasets
    /// </summary>
    public void SetDatasets()
    {
        OnDatasetsSet(new DatasetsSetEventArgs(Data.Datasets));
    }
}