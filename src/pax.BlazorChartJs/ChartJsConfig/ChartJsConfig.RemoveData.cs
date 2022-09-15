
namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Removes last label and last data, backgroundColor and borderColor from ALL datasets
    /// </summary>
    public void RemoveData()
    {
        RemoveLabel();

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            RemoveDatasetData(Data.Datasets[i]);
            RemoveDatasetBackgroundColor(Data.Datasets[i]);
            RemoveDatasetBorderColor(Data.Datasets[i]);
        }
        OnDataRemove(new DataRemoveEventArgs());
    }

    private void RemoveLabel()
    {
        if (!Data.Labels.Any())
        {
            return;
        }
        Data.Labels.RemoveAt(Data.Labels.Count - 1);
    }

    private static void RemoveDatasetData(ChartJsDataset dataset)
    {
        if (!dataset.Data.Any())
        {
            return;
        }
        dataset.Data.RemoveAt(dataset.Data.Count - 1);
    }

    private static void RemoveDatasetBorderColor(ChartJsDataset dataset)
    {
        if (dataset is BarDataset barDataset)
        {
            if (barDataset.BorderColor != null && barDataset.BorderColor.IsIndexed)
            {
                barDataset.BorderColor.RemoveAt(barDataset.BorderColor.Count - 1);
            }
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            if (bubbleDataset.BorderColor != null && bubbleDataset.BorderColor.IsIndexed)
            {
                bubbleDataset.BorderColor.RemoveAt(bubbleDataset.BorderColor.Count - 1);
            }
        }

        else if (dataset is PieDataset pieDataset)
        {
            if (pieDataset.BorderColor != null && pieDataset.BorderColor.IsIndexed)
            {
                pieDataset.BorderColor.RemoveAt(pieDataset.BorderColor.Count - 1);
            }
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            if (polarAreaDataset.BorderColor != null && polarAreaDataset.BorderColor.IsIndexed)
            {
                polarAreaDataset.BorderColor.RemoveAt(polarAreaDataset.BorderColor.Count - 1);
            }
        }
    }


    private static void RemoveDatasetBackgroundColor(ChartJsDataset dataset)
    {
        if (dataset is BarDataset barDataset)
        {
            if (barDataset.BackgroundColor != null && barDataset.BackgroundColor.IsIndexed)
            {
                barDataset.BackgroundColor.RemoveAt(barDataset.BackgroundColor.Count - 1);
            }
        }

        else if (dataset is BubbleDataset bubbleDataset)
        {
            if (bubbleDataset.BackgroundColor != null && bubbleDataset.BackgroundColor.IsIndexed)
            {
                bubbleDataset.BackgroundColor.RemoveAt(bubbleDataset.BackgroundColor.Count - 1);
            }
        }

        else if (dataset is PieDataset pieDataset)
        {
            if (pieDataset.BackgroundColor != null && pieDataset.BackgroundColor.IsIndexed)
            {
                pieDataset.BackgroundColor.RemoveAt(pieDataset.BackgroundColor.Count - 1);
            }
        }

        else if (dataset is PolarAreaDataset polarAreaDataset)
        {
            if (polarAreaDataset.BackgroundColor != null && polarAreaDataset.BackgroundColor.IsIndexed)
            {
                polarAreaDataset.BackgroundColor.RemoveAt(polarAreaDataset.BackgroundColor.Count - 1);
            }
        }
    }
}