using System.Data;
using System;
using System.Text.Json.Serialization;
using pax.BlazorChartJs;
using System.Reflection.Emit;

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

    internal event EventHandler<DatasetAddEventArgs>? DatasetAdd;

    internal event EventHandler<DatasetRemoveEventArgs>? DatasetRemove;
    internal event EventHandler<DataAddEventArgs>? DataAdd;
    internal event EventHandler<DataRemoveEventArgs>? DataRemove;
    internal event EventHandler<DataSetEventArgs>? DataSet;
    internal event EventHandler<LabelsSetEventArgs>? LabelsSet;

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
    public void AddDataset(object dataset, int? atPosition = null)
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
                OnDatasetAdd(new DatasetAddEventArgs(afterDataset, ((ChartJsDataset)afterDataset).Id));
            }
        }
    }

    /// <summary>
    /// Removes the dataset and updates the chart
    /// </summary>
    /// <param name="dataset"></param>
    public void RemoveDataset(object dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        if (Data.Datasets.Contains(dataset))
        {
            Data.Datasets.Remove(dataset);
            OnDatasetRemove(new DatasetRemoveEventArgs(((ChartJsDataset)dataset).Id));
        }
    }

    /// <summary>
    /// Adds data to datasets and updates the chart
    /// </summary>
    /// <param name="label"></param>
    /// <param name="data"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="borderColor"></param>
    /// <param name="atPosition"></param>
    public void AddData(string label, object data, string? backgroundColor = null, string? borderColor = null, int? atPosition = null)
    {
        var backgroundColors = backgroundColor == null ? null : new List<string> { backgroundColor };
        var borderColors = borderColor == null ? null : new List<string> { borderColor };
        AddData(label, new List<object>() { data }, backgroundColors, borderColors, atPosition);
    }

    /// <summary>
    /// Adds data to datasets and updates the chart
    /// </summary>
    /// <param name="label"></param>
    /// <param name="data"></param>
    /// <param name="backgroundColors"></param>
    /// <param name="borderColors"></param>
    /// <param name="atPosition"></param>
    public void AddData(string label, IList<object> data, IList<string>? backgroundColors = null, IList<string>? borderColors = null, int? atPosition = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        int pos = atPosition == null ? -1 : atPosition.Value;

        if (pos < 0)
        {
            Data.Labels.Add(label);
        }
        else
        {
            Data.Labels.Insert(pos, label);
        }

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            if (data.Count >= i)
            {
                if (pos < 0)
                {
                    ((ChartJsDataset)Data.Datasets[i]).Data.Add(data[i]);
                }
                else
                {
                    ((ChartJsDataset)Data.Datasets[i]).Data.Insert(pos, data[i]);
                }
            }

            if (backgroundColors != null && backgroundColors.Count >= i)
            {
                if (Data.Datasets[i].GetType() == typeof(BarDataset))
                {
                    BarDataset dataset = (BarDataset)Data.Datasets[i];
                    if (dataset.BackgroundColor != null && dataset.BackgroundColor.GetType() == typeof(List<string>))
                    {
                        if (pos < 0)
                        {
                            ((List<string>)dataset.BackgroundColor).Add(backgroundColors[i]);
                        }
                        else
                        {
                            ((List<string>)dataset.BackgroundColor).Insert(pos, backgroundColors[i]);
                        }
                    }
                }
            }

            if (borderColors != null && borderColors.Count >= i)
            {
                if (Data.Datasets[i].GetType() == typeof(BarDataset))
                {
                    BarDataset dataset = (BarDataset)Data.Datasets[i];
                    if (dataset.BorderColor != null && dataset.BorderColor.GetType() == typeof(List<string>))
                    {
                        if (pos < 0)
                        {
                            ((List<string>)dataset.BorderColor).Add(borderColors[i]);
                        }
                        else
                        {
                            ((List<string>)dataset.BorderColor).Insert(pos, borderColors[i]);
                        }
                    }
                }
            }
        }
        OnDataAdd(new DataAddEventArgs(label, data, backgroundColors, borderColors, atPosition));
    }

    /// <summary>
    /// Removes label and data from ALL datasets at last or given position
    /// </summary>
    /// <param name="atPosition"></param>
    public void RemoveData(int? atPosition = null)
    {
        int pos = atPosition == null ? Data.Labels.Count - 1 : atPosition.Value;

        if (pos < 0)
        {
            return;
        }

        Data.Labels.RemoveAt(pos);

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            ((ChartJsDataset)Data.Datasets[i]).Data.RemoveAt(pos);


            if (Data.Datasets[i].GetType() == typeof(BarDataset))
            {
                BarDataset dataset = (BarDataset)Data.Datasets[i];
                if (dataset.BackgroundColor != null && dataset.BackgroundColor.GetType() == typeof(List<string>))
                {
                    ((List<string>)dataset.BackgroundColor).RemoveAt(pos);
                }
            }

            if (Data.Datasets[i].GetType() == typeof(BarDataset))
            {
                BarDataset dataset = (BarDataset)Data.Datasets[i];
                if (dataset.BorderColor != null && dataset.BorderColor.GetType() == typeof(List<string>))
                {
                    ((List<string>)dataset.BorderColor).RemoveAt(pos);
                }
            }
        }
        OnDataRemove(new DataRemoveEventArgs(atPosition));
    }

    /// <summary>
    /// Sets the data and updates the chart
    /// </summary>
    /// <param name="dataset"></param>
    /// <param name="data"></param>
    public void SetData(object dataset, IList<object> data)
    {
        SetData(new Dictionary<object, IList<object>>() { { dataset, data } });
    }

    /// <summary>
    /// Sets the dataset (=key) data (=value) and updates the chart
    /// </summary>
    /// <param name="data"></param>
    public void SetData(Dictionary<object, IList<object>> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        foreach (var ent in data)
        {
            var dataset = Data.Datasets.FirstOrDefault(f => f.Equals(ent.Key)) as ChartJsDataset;
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


public class ChartJsData
{
    public ChartJsData()
    {
        Labels = new List<string>();
        Datasets = new List<object>();

    }

    public IList<string> Labels { get; set; }
    public virtual IList<object> Datasets { get; set; }

}

#pragma warning restore CA2227

public enum ChartType
{
    None = 0,
    line = 1,
    bar = 2,
    doughnut = 3,
    pie = 4,
    radar = 5,
    polarArea = 6,
}