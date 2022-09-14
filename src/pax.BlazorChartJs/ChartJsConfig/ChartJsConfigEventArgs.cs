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
    public DataAddEventArgs(string? label,
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

    public string? Label { get; init; }
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

public class AddDataEventArgs : EventArgs
{
    public AddDataEventArgs(string? label, int? atPosition, Dictionary<string, AddDataObject> datas)
    {
        Label = label;
        Datas = datas;
        AtPosition = atPosition;
    }
    public string? Label { get; init; }
    public int? AtPosition { get; init; }
    public Dictionary<string, AddDataObject> Datas { get; init; }
}

public record AddDataObject
{
    public AddDataObject(object data, int? atPosition = null, string? backgroundColor = null, string? borderColor = null)
    {
        Data = data;
        BackgroundColor = backgroundColor;
        BorderColor = borderColor;
        AtPosition = atPosition;
    }
    public object Data { get; init; }
    public string? BackgroundColor { get; init; }
    public string? BorderColor { get; init; }
    public int? AtPosition { get; init; }
}