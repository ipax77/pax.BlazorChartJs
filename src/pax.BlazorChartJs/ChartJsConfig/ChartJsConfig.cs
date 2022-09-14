using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// ChartJs v3.9.1 wrapper class <see href="https://www.chartjs.org/docs/latest/configuration/">ChartJs docs</see>
/// NULL values are ignored (=> ChartJs default)
/// </summary>
public partial class ChartJsConfig
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

    internal event EventHandler<DatasetAddEventArgs>? DatasetAdd;

    internal event EventHandler<DatasetRemoveEventArgs>? DatasetRemove;
    internal event EventHandler<DataAddEventArgs>? DataAdd;
    internal event EventHandler<DataRemoveEventArgs>? DataRemove;
    internal event EventHandler<DataSetEventArgs>? DataSet;
    internal event EventHandler<LabelsSetEventArgs>? LabelsSet;
    internal event EventHandler<AddDataEventArgs>? AddDataEvent;

    internal virtual void OnAddDataEvent(AddDataEventArgs e)
    {
        EventHandler<AddDataEventArgs>? handler = AddDataEvent;
        handler?.Invoke(this, e);
    }

    internal virtual void OnLabelsSet(LabelsSetEventArgs e)
    {
        EventHandler<LabelsSetEventArgs>? handler = LabelsSet;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDatasetAdd(DatasetAddEventArgs e)
    {
        EventHandler<DatasetAddEventArgs>? handler = DatasetAdd;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDatasetRemove(DatasetRemoveEventArgs e)
    {
        EventHandler<DatasetRemoveEventArgs>? handler = DatasetRemove;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDataAdd(DataAddEventArgs e)
    {
        EventHandler<DataAddEventArgs>? handler = DataAdd;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDataRemove(DataRemoveEventArgs e)
    {
        EventHandler<DataRemoveEventArgs>? handler = DataRemove;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDataSet(DataSetEventArgs e)
    {
        EventHandler<DataSetEventArgs>? handler = DataSet;
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// AddDataset - Adds the dataset and updates the chart
    /// <paramref name="dataset"/>
    /// <paramref name="atPosition"/>
    /// </summary>
    public void AddDataset(ChartJsDataset dataset, int? atPosition = null)
    {
        if (atPosition == null)
        {
            Data.Datasets.Add(dataset);
            OnDatasetAdd(new DatasetAddEventArgs(dataset, null));
        }
        else
        {
            var afterDataset = Data.Datasets.ElementAt(atPosition.Value);
            if (afterDataset != null)
            {
                Data.Datasets.Insert(atPosition.Value, dataset);
                OnDatasetAdd(new DatasetAddEventArgs(dataset, afterDataset.Id));
            }
        }
    }

    /// <summary>
    /// Removes the dataset and updates the chart
    /// </summary>
    /// <param name="dataset"></param>
    public void RemoveDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        if (Data.Datasets.Contains(dataset))
        {
            Data.Datasets.Remove(dataset);
            OnDatasetRemove(new DatasetRemoveEventArgs(dataset.Id));
        }
    }

    /// <summary>
    /// Sets the data and updates the chart
    /// </summary>
    /// <param name="dataset"></param>
    /// <param name="data"></param>
    public void SetData(ChartJsDataset dataset, IList<object> data)
    {
        SetData(new Dictionary<ChartJsDataset, IList<object>>() { { dataset, data } });
    }

    /// <summary>
    /// Sets the dataset (=key) data (=value) and updates the chart
    /// </summary>
    /// <param name="data"></param>
    public void SetData(Dictionary<ChartJsDataset, IList<object>> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        foreach (var ent in data)
        {
            var dataset = Data.Datasets.FirstOrDefault(f => f.Equals(ent.Key));
            if (dataset != null)
            {
                dataset.Data = ent.Value;
            }
        }
        OnDataSet(new DataSetEventArgs(data));
    }

    /// <summary>
    /// Sets the chart labels
    /// </summary>
    /// <param name="labels"></param>
    public void SetLabels(IList<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        Data.Labels = labels;
        OnLabelsSet(new(labels));
    }
}


public enum ChartType
{
    None = 0,
    line = 1,
    bar = 2,
    doughnut = 3,
    pie = 4,
    radar = 5,
    polarArea = 6,
    scatter = 7,
    bubble = 8
}