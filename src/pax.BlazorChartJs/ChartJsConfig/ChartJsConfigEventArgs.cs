namespace pax.BlazorChartJs;
public class DatasetAddEventArgs : EventArgs
{
    public DatasetAddEventArgs(object dataset, string? afterDatasetId)
    {
        Dataset = dataset;
        AfterDatasetId = afterDatasetId;
    }

    public object Dataset { get; init; }
    public string? AfterDatasetId { get; init; }
}

public class DatasetRemoveEventArgs : EventArgs
{
    public DatasetRemoveEventArgs(string datasetId)
    {
        DatasetId = datasetId;
    }

    public string DatasetId { get; init; }
}

public class DataAddEventArgs : EventArgs
{
    public DataAddEventArgs(string label,
                            IList<object> data,
                            IList<string>? backgroundColors,
                            IList<string>? borderColors,
                            int? atPosition)
    {
        Label = label;
        Data = data;
        BackgroundColors = backgroundColors;
        BorderColors = borderColors;
        AtPostion = atPosition;
    }

    public string Label { get; init; }
    public IList<object> Data { get; init; }
    public IList<string>? BackgroundColors { get; init; }
    public IList<string>? BorderColors { get; init; }
    public int? AtPostion { get; init; }
}

public class DataRemoveEventArgs : EventArgs
{
    public DataRemoveEventArgs(int? atPosition)
    {
        AtPosition = atPosition;
    }
    public int? AtPosition { get; init; }
}

public class DataSetEventArgs : EventArgs
{
    public DataSetEventArgs(Dictionary<ChartJsDataset, IList<object>> data)
    {
        Data = data;
    }
    public Dictionary<ChartJsDataset, IList<object>> Data { get; init; }
}

public class LabelsSetEventArgs : EventArgs
{
    public LabelsSetEventArgs(IList<string> labels)
    {
        Labels = labels;
    }

    public IList<string> Labels { get; init; }
}