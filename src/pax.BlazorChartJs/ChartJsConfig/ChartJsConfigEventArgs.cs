namespace pax.BlazorChartJs;
public class DatasetsAddEventArgs : EventArgs
{
    public DatasetsAddEventArgs(IList<ChartJsDataset> datasets)
    {
        Datasets = datasets;
    }

    public IList<ChartJsDataset> Datasets { get; init; }
}

public class DatasetsRemoveEventArgs : EventArgs
{
    public DatasetsRemoveEventArgs(IList<string> datasetIds)
    {
        DatasetIds = datasetIds;
    }

    public IList<string> DatasetIds { get; init; }
}

public class DatasetsUpdateEventArgs : EventArgs
{
    public DatasetsUpdateEventArgs(IList<ChartJsDataset> datasets, bool smooth = false)
    {
        Datasets = datasets;
        Smooth = smooth;
    }

    public IList<ChartJsDataset> Datasets { get; init; }
    public bool Smooth { get; init; }
}

public class DatasetsSetEventArgs : EventArgs
{
    public DatasetsSetEventArgs(IList<ChartJsDataset> datasets)
    {
        Datasets = datasets;
    }
    public IList<ChartJsDataset> Datasets { get; init; }
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
    public DataRemoveEventArgs()
    {
    }
}

public class DataSetEventArgs : EventArgs
{
    public DataSetEventArgs(Dictionary<string, SetDataObject> datas, IList<string>? labels = null)
    {
        Labels = labels;
        Datas = datas;
    }
    public IList<string>? Labels { get; init; }
    public Dictionary<string, SetDataObject> Datas { get; init; }
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