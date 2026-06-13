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

public class DatasetsBinaryDataSetEventArgs(IReadOnlyList<ChartJsBinaryDatasetPayload> payloads, string updateMode) : EventArgs
{
    public IReadOnlyList<ChartJsBinaryDatasetPayload> Payloads { get; init; } = payloads;
    public string UpdateMode { get; init; } = updateMode;
}

/// <summary>
/// Describes a batched smooth dataset update to apply to a chart.
/// </summary>
/// <remarks>
/// The change set can add datasets, smoothly update existing datasets, remove datasets,
/// reorder the final dataset collection, and optionally replace the chart labels and options
/// in one consolidated chart update.
/// </remarks>
/// <param name="desiredDatasetIds">
/// The ordered list of dataset IDs that should remain visible after the change set is applied.
/// </param>
public class DatasetsSmoothChangeSet(IList<string> desiredDatasetIds)
{
    /// <summary>
    /// Gets the ordered list of dataset IDs that should remain in the chart after the update.
    /// </summary>
    /// <remarks>
    /// This list determines the final dataset order. Dataset IDs that are not present in the
    /// existing chart data or in <see cref="DatasetsToAdd"/> are ignored unless the caller
    /// performs stricter validation before applying the change set.
    /// </remarks>
    public IList<string> DesiredDatasetIds { get; init; } = desiredDatasetIds;

    /// <summary>
    /// Gets the datasets to add before the final dataset order is applied.
    /// </summary>
    /// <remarks>
    /// A dataset is only added if its ID appears in <see cref="DesiredDatasetIds"/> and is not
    /// present in <see cref="DatasetIdsToRemove"/>.
    /// </remarks>
    public IList<ChartJsDataset>? DatasetsToAdd { get; init; }

    /// <summary>
    /// Gets the existing datasets to update using the smooth update path.
    /// </summary>
    /// <remarks>
    /// A dataset is only updated if its ID already exists after additions and removals are
    /// considered, appears in <see cref="DesiredDatasetIds"/>, and is not present in
    /// <see cref="DatasetIdsToRemove"/>.
    /// </remarks>
    public IList<ChartJsDataset>? DatasetsToUpdateSmooth { get; init; }

    /// <summary>
    /// Gets the dataset IDs to remove from the chart.
    /// </summary>
    /// <remarks>
    /// Removed dataset IDs are excluded even if matching datasets are also provided in
    /// <see cref="DatasetsToAdd"/> or <see cref="DatasetsToUpdateSmooth"/>.
    /// </remarks>
    public IList<string>? DatasetIdsToRemove { get; init; }

    /// <summary>
    /// Gets the labels to assign to the chart as part of the same update.
    /// </summary>
    /// <remarks>
    /// When this property is <see langword="null"/>, the existing chart labels are left unchanged.
    /// </remarks>
    public IList<string>? Labels { get; init; }

    /// <summary>
    /// Gets a value indicating whether the chart options should be updated as part of this change set.
    /// </summary>
    public bool UpdateOptions { get; init; }

    /// <summary>
    /// Gets the Chart.js update mode to use for the consolidated chart update.
    /// </summary>
    /// <remarks>
    /// Set to a built-in mode such as <c>none</c> or to a custom transition name defined
    /// in chart options. When <see langword="null"/>, the default Chart.js update is used.
    /// </remarks>
    public string? UpdateAnimation { get; init; }
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
