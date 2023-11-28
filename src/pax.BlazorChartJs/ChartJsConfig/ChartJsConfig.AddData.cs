
namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Adds data to datasets and updates the chart
    /// </summary>
    /// <param name="label"></param>
    /// <param name="data"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="borderColor"></param>
    /// <param name="atPosition"></param>
    public void AddData(string? label, object data, string? backgroundColor = null, string? borderColor = null, int? atPosition = null)
    {
        // var backgroundColors = backgroundColor == null ? null : new List<string> { backgroundColor };
        // var borderColors = borderColor == null ? null : new List<string> { borderColor };
        // AddData(label, new List<object>() { data }, backgroundColors, borderColors, atPosition);
        AddData(label, atPosition, new Dictionary<ChartJsDataset, AddDataObject>()
            {
                { Data.Datasets.First(), new AddDataObject(data, atPosition, backgroundColor, borderColor)}
            });
    }

    public void AddData(string? label, int? atPosition, Dictionary<ChartJsDataset, AddDataObject> datas)
    {
        ArgumentNullException.ThrowIfNull(datas);

        AddLabel(label, atPosition);

        Dictionary<string, AddDataObject> jsData = new();
        foreach (var data in datas)
        {
            AddDatasetData(data.Key, data.Value.Data, data.Value.AtPosition);
            if (data.Value.BackgroundColor != null)
            {
                AddDatasetBackgroundColor(data.Key, data.Value.BackgroundColor, data.Value.AtPosition);
            }
            if (data.Value.BorderColor != null)
            {
                AddDatasetBorderColor(data.Key, data.Value.BorderColor, data.Value.AtPosition);
            }
            jsData[data.Key.Id] = data.Value;
        }
        OnAddDataEvent(new AddDataEventArgs(label, atPosition, jsData));
    }

    private void AddLabel(string? label, int? atPosition)
    {
        if (label == null)
        {
            return;
        }
        if (atPosition == null)
        {
            Data.Labels.Add(label);
        }
        else
        {
            Data.Labels.Insert(atPosition.Value, label);
        }
    }

    private static void AddDatasetData(ChartJsDataset dataset, object data, int? atPosition)
    {
        if (atPosition == null)
        {
            dataset.Data.Add(data);
        }
        else
        {
            dataset.Data.Insert(atPosition.Value, data);
        }
    }

    private static void AddDatasetBorderColor(ChartJsDataset dataset, string borderColor, int? atPosition)
    {
        if (dataset is BarDataset barDataset)
        {
            if (barDataset.BorderColor != null && barDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    barDataset.BorderColor.Add(borderColor);
                }
                else
                {
                    barDataset.BorderColor.Insert(atPosition.Value, borderColor);
                }
            }
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            if (bubbleDataset.BorderColor != null && bubbleDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    bubbleDataset.BorderColor.Add(borderColor);
                }
                else
                {
                    bubbleDataset.BorderColor.Insert(atPosition.Value, borderColor);
                }
            }
        }

        else if (dataset is PieDataset pieDataset)
        {
            if (pieDataset.BorderColor != null && pieDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    pieDataset.BorderColor.Add(borderColor);
                }
                else
                {
                    pieDataset.BorderColor.Insert(atPosition.Value, borderColor);
                }
            }
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            if (polarAreaDataset.BorderColor != null && polarAreaDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    polarAreaDataset.BorderColor.Add(borderColor);
                }
                else
                {
                    polarAreaDataset.BorderColor.Insert(atPosition.Value, borderColor);
                }
            }
        }
    }


    private static void AddDatasetBackgroundColor(ChartJsDataset dataset, string backgroundColor, int? atPosition)
    {
        if (dataset is BarDataset barDataset)
        {
            if (barDataset.BackgroundColor != null && barDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    barDataset.BackgroundColor.Add(backgroundColor);
                }
                else
                {
                    barDataset.BackgroundColor.Insert(atPosition.Value, backgroundColor);
                }
            }
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            if (bubbleDataset.BackgroundColor != null && bubbleDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    bubbleDataset.BackgroundColor.Add(backgroundColor);
                }
                else
                {
                    bubbleDataset.BackgroundColor.Insert(atPosition.Value, backgroundColor);
                }
            }
        }

        else if (dataset is PieDataset pieDataset)
        {
            if (pieDataset.BackgroundColor != null && pieDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    pieDataset.BackgroundColor.Add(backgroundColor);
                }
                else
                {
                    pieDataset.BackgroundColor.Insert(atPosition.Value, backgroundColor);
                }
            }
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            if (polarAreaDataset.BackgroundColor != null && polarAreaDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    polarAreaDataset.BackgroundColor.Add(backgroundColor);
                }
                else
                {
                    polarAreaDataset.BackgroundColor.Insert(atPosition.Value, backgroundColor);
                }
            }
        }
    }
}