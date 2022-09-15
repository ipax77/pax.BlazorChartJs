

namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Sets the data and optional labels and updates the Chart
    /// </summary>
    public void SetData(Dictionary<ChartJsDataset, SetDataObject> datas, IList<string>? labels = null)
    {
        ArgumentNullException.ThrowIfNull(datas);

        if (labels != null)
        {
            Data.Labels = labels;
        }

        Dictionary<string, SetDataObject> jsData = new();
        foreach (var data in datas)
        {
            data.Key.Data = data.Value.Data;

            if (data.Value.BackgroundColor != null)
            {
                SetDatasetBackgroundColor(data.Key, data.Value.BackgroundColor);
            }
            if (data.Value.BorderColor != null)
            {
                SetDatasetBorderColor(data.Key, data.Value.BorderColor);
            }
            jsData[data.Key.Id] = data.Value;
        }
        OnDataSet(new DataSetEventArgs(jsData, labels));
    }

    private static void SetDatasetBorderColor(ChartJsDataset dataset, IndexableOption<string> borderColor)
    {
        if (dataset is LineDataset lineDataset)
        {
            lineDataset.BorderColor = borderColor.SingleValue;
        }

        else if (dataset is RadarDataset radarDataset)
        {
            radarDataset.BorderColor = borderColor.SingleValue;
        }

        else if (dataset is ScatterDataset scatterDataset)
        {
            scatterDataset.BorderColor = borderColor.SingleValue;
        }

        else if (dataset is BarDataset barDataset)
        {
            barDataset.BorderColor = borderColor;
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            bubbleDataset.BorderColor = borderColor;
        }

        else if (dataset is PieDataset pieDataset)
        {
            pieDataset.BorderColor = borderColor;
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            polarAreaDataset.BorderColor = borderColor;
        }
    }


    private static void SetDatasetBackgroundColor(ChartJsDataset dataset, IndexableOption<string> backgroundColor)
    {
        if (dataset is LineDataset lineDataset)
        {
            lineDataset.BackgroundColor = backgroundColor.SingleValue;
        }

        else if (dataset is RadarDataset radarDataset)
        {
            radarDataset.BackgroundColor = backgroundColor.SingleValue;
        }

        else if (dataset is ScatterDataset scatterDataset)
        {
            scatterDataset.BackgroundColor = backgroundColor.SingleValue;
        }

        else if (dataset is BarDataset barDataset)
        {
            barDataset.BackgroundColor = backgroundColor;
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            bubbleDataset.BackgroundColor = backgroundColor;
        }

        else if (dataset is PieDataset pieDataset)
        {
            pieDataset.BackgroundColor = backgroundColor;
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            polarAreaDataset.BackgroundColor = backgroundColor;
        }
    }
}