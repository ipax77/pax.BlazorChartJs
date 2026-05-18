namespace pax.BlazorChartJs;

public class DatasetsAddEventArgs(IList<ChartJsDataset> datasets) : EventArgs
{
    public IList<ChartJsDataset> Datasets { get; init; } = datasets;
}

public class DatasetsRemoveEventArgs(IList<string> datasetIds) : EventArgs
{
    public IList<string> DatasetIds { get; init; } = datasetIds;
}

public class DatasetsUpdateEventArgs(IList<ChartJsDataset> datasets, bool smooth = false) : EventArgs
{
    public IList<ChartJsDataset> Datasets { get; init; } = datasets;
    public bool Smooth { get; init; } = smooth;
}

public class DatasetsSetEventArgs(IList<ChartJsDataset> datasets) : EventArgs
{
    public IList<ChartJsDataset> Datasets { get; init; } = datasets;
}

public class DataAddEventArgs(string? label,
                        IList<object> data,
                        IList<string>? backgroundColors,
                        IList<string>? borderColors,
                        int? atPosition) : EventArgs
{
    public string? Label { get; init; } = label;
    public IList<object> Data { get; init; } = data;
    public IList<string>? BackgroundColors { get; init; } = backgroundColors;
    public IList<string>? BorderColors { get; init; } = borderColors;
    public int? AtPostion { get; init; } = atPosition;
}

public class DataRemoveEventArgs : EventArgs
{
    public DataRemoveEventArgs()
    {
    }
}

public class DataSetEventArgs(Dictionary<string, SetDataObject> datas, IList<string>? labels = null) : EventArgs
{
    public IList<string>? Labels { get; init; } = labels;
    public Dictionary<string, SetDataObject> Datas { get; init; } = datas;
}

public class LabelsSetEventArgs(IList<string> labels) : EventArgs
{
    public IList<string> Labels { get; init; } = labels;
}

public class AddDataEventArgs(string? label, int? atPosition, Dictionary<string, AddDataObject> datas) : EventArgs
{
    public string? Label { get; init; } = label;
    public int? AtPosition { get; init; } = atPosition;
    public Dictionary<string, AddDataObject> Datas { get; init; } = datas;
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

public record SetDataObject
{
    public SetDataObject(IList<object> data, IndexableOption<string>? backgroundColor = null, IndexableOption<string>? borderColor = null)
    {
        Data = data;
        BackgroundColor = backgroundColor;
        BorderColor = borderColor;
    }
    public IList<object> Data { get; init; }
    public IndexableOption<string>? BackgroundColor { get; init; }
    public IndexableOption<string>? BorderColor { get; init; }
}