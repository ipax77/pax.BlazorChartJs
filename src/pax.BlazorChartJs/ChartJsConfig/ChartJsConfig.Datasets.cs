
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

        List<string> removeDatasetIds = new();

        foreach (var dataset in datasets.ToArray())
        {
            int index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                removeDatasetIds.Add(dataset.Id);
                Data.Datasets.RemoveAt(index);
            }
        }
        OnDatasetsRemove(new DatasetsRemoveEventArgs(removeDatasetIds));
    }

    /// <summary>
    /// Updates the Chart dataset.
    /// </summary>
    /// <param name="dataset">The dataset containing the updated values.</param>
    public void UpdateDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        UpdateDatasets(new List<ChartJsDataset>() { dataset });
    }

    /// <summary>
    /// Updates the Chart datasets.
    /// </summary>
    /// <param name="datasets">The datasets containing the updated values.</param>
    public void UpdateDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        List<ChartJsDataset> updateDatasets = [];
        foreach (var dataset in datasets)
        {
            var index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                updateDatasets.Add(dataset);
            }
        }
        OnDatasetsUpdate(new DatasetsUpdateEventArgs(updateDatasets));
    }

    /// <summary>
    /// Updates the Chart with the current Datasets
    /// </summary>
    public void SetDatasets()
    {
        OnDatasetsSet(new DatasetsSetEventArgs(Data.Datasets));
    }
}