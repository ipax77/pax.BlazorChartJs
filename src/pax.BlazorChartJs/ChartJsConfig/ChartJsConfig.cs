using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// ChartJs wrapper class <see href="https://www.chartjs.org/docs/latest/configuration/">ChartJs docs</see>
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

    internal event EventHandler<DatasetsAddEventArgs>? DatasetsAdd;

    internal event EventHandler<DatasetsRemoveEventArgs>? DatasetsRemove;
    internal event EventHandler<DatasetsUpdateEventArgs>? DatasetsUpdate;
    internal event EventHandler<DatasetsSetEventArgs>? DatasetsSet;
    internal event EventHandler<DataAddEventArgs>? DataAdd;
    internal event EventHandler<DataRemoveEventArgs>? DataRemove;
    internal event EventHandler<DataSetEventArgs>? DataSet;
    internal event EventHandler<LabelsSetEventArgs>? LabelsSet;
    internal event EventHandler<AddDataEventArgs>? AddDataEvent;
    internal event EventHandler? ChartOptionsUpdate;
    internal event EventHandler? ChartRedraw;

    internal virtual void OnChartRedraw()
    {
        EventHandler? handler = ChartRedraw;
        handler?.Invoke(this, new());
    }

    internal virtual void OnUpdateChartOptions()
    {
        EventHandler? handler = ChartOptionsUpdate;
        handler?.Invoke(this, new());
    }

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

    internal virtual void OnDatasetsAdd(DatasetsAddEventArgs e)
    {
        EventHandler<DatasetsAddEventArgs>? handler = DatasetsAdd;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDatasetsRemove(DatasetsRemoveEventArgs e)
    {
        EventHandler<DatasetsRemoveEventArgs>? handler = DatasetsRemove;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDatasetsUpdate(DatasetsUpdateEventArgs e)
    {
        EventHandler<DatasetsUpdateEventArgs>? handler = DatasetsUpdate;
        handler?.Invoke(this, e);
    }

    internal virtual void OnDatasetsSet(DatasetsSetEventArgs e)
    {
        EventHandler<DatasetsSetEventArgs>? handler = DatasetsSet;
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
    /// Sets the chart labels
    /// </summary>
    /// <param name="labels"></param>
    public void SetLabels(IList<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        Data.Labels = labels;
        OnLabelsSet(new(labels));
    }

    /// <summary>
    /// Updates chart options
    /// </summary>
    public void UpdateChartOptions()
    {
        OnUpdateChartOptions();
    }

    /// <summary>
    /// Reinitializes the chart and triggers a new ChartJsInitEvent
    /// </summary>
    public void ReinitializeChart()
    {
        OnChartRedraw();
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