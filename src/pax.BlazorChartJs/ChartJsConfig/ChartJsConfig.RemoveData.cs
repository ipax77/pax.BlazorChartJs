
namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Removes label and data from ALL datasets at last or given position
    /// </summary>
    /// <param name="atPosition"></param>
    public void RemoveData(int? atPosition = null)
    {
        RemoveLabel(atPosition);

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            RemoveDatasetData(Data.Datasets[i], atPosition);
            RemoveDatasetBackgroundColor(Data.Datasets[i], atPosition);
            RemoveDatasetBorderColor(Data.Datasets[i], atPosition);
        }
        OnDataRemove(new DataRemoveEventArgs(atPosition));
    }

    private void RemoveLabel(int? atPosition)
    {
        if (!Data.Labels.Any())
        {
            return;
        }

        if (atPosition == null)
        {
            Data.Labels.RemoveAt(Data.Labels.Count - 1);
        }
        else
        {
            Data.Labels.RemoveAt(atPosition.Value);
        }
    }

    private static void RemoveDatasetData(ChartJsDataset dataset, int? atPosition)
    {
        if (!dataset.Data.Any())
        {
            return;
        }

        if (atPosition == null)
        {
            dataset.Data.RemoveAt(dataset.Data.Count - 1);
        }
        else
        {
            dataset.Data.RemoveAt(atPosition.Value);
        }
    }

    private static void RemoveDatasetBorderColor(ChartJsDataset dataset, int? atPosition)
    {
        if (dataset.GetType() == typeof(BarDataset))
        {
            BarDataset barDataset = (BarDataset)dataset;
            if (barDataset.BorderColor != null && barDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    barDataset.BorderColor.RemoveAt(barDataset.BorderColor.Count - 1);
                }
                else
                {
                    barDataset.BorderColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(BubbleDataset))
        {
            BubbleDataset bubbleDataset = (BubbleDataset)dataset;
            if (bubbleDataset.BorderColor != null && bubbleDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    bubbleDataset.BorderColor.RemoveAt(bubbleDataset.BorderColor.Count - 1);
                }
                else
                {
                    bubbleDataset.BorderColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(PieDataset))
        {
            PieDataset pieDataset = (PieDataset)dataset;
            if (pieDataset.BorderColor != null && pieDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    pieDataset.BorderColor.RemoveAt(pieDataset.BorderColor.Count - 1);
                }
                else
                {
                    pieDataset.BorderColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(PolarAreaDataset))
        {
            PolarAreaDataset polarAreaDataset = (PolarAreaDataset)dataset;
            if (polarAreaDataset.BorderColor != null && polarAreaDataset.BorderColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    polarAreaDataset.BorderColor.RemoveAt(polarAreaDataset.BorderColor.Count - 1);
                }
                else
                {
                    polarAreaDataset.BorderColor.RemoveAt(atPosition.Value);
                }
            }
        }
    }


    private static void RemoveDatasetBackgroundColor(ChartJsDataset dataset, int? atPosition)
    {
        if (dataset.GetType() == typeof(BarDataset))
        {
            BarDataset barDataset = (BarDataset)dataset;
            if (barDataset.BackgroundColor != null && barDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    barDataset.BackgroundColor.RemoveAt(barDataset.BackgroundColor.Count - 1);
                }
                else
                {
                    barDataset.BackgroundColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(BubbleDataset))
        {
            BubbleDataset bubbleDataset = (BubbleDataset)dataset;
            if (bubbleDataset.BackgroundColor != null && bubbleDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    bubbleDataset.BackgroundColor.RemoveAt(bubbleDataset.BackgroundColor.Count - 1);
                }
                else
                {
                    bubbleDataset.BackgroundColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(PieDataset))
        {
            PieDataset pieDataset = (PieDataset)dataset;
            if (pieDataset.BackgroundColor != null && pieDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    pieDataset.BackgroundColor.RemoveAt(pieDataset.BackgroundColor.Count - 1);
                }
                else
                {
                    pieDataset.BackgroundColor.RemoveAt(atPosition.Value);
                }
            }
        }

        if (dataset.GetType() == typeof(PolarAreaDataset))
        {
            PolarAreaDataset polarAreaDataset = (PolarAreaDataset)dataset;
            if (polarAreaDataset.BackgroundColor != null && polarAreaDataset.BackgroundColor.IsIndexed)
            {
                if (atPosition == null)
                {
                    polarAreaDataset.BackgroundColor.RemoveAt(polarAreaDataset.BackgroundColor.Count - 1);
                }
                else
                {
                    polarAreaDataset.BackgroundColor.RemoveAt(atPosition.Value);
                }
            }
        }
    }
}