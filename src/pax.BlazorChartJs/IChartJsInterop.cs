using Microsoft.JSInterop;

namespace pax.BlazorChartJs;

public interface IChartJsInterop
{
    IJSRuntime JsRuntime { get; }

    ValueTask AddDataset(Guid configGuid, object dataset, string? afterDatasetId);
    ValueTask AddDataToDataset(Guid configGuid, string? label, IList<object> data, IList<string>? backgroundColors, IList<string>? borderColors, int? atPosition);
    ValueTask DisposeAsync();
    ValueTask<string> GetChartImage(Guid configGuid, string? imageType = null, int? quality = null, double? width = null, double? height = null);
    ValueTask<bool> GetDataVisibility(Guid configGuid, int index);
    ValueTask HideDataset(Guid configGuid, ChartJsDataset dataset, int? index);
    ValueTask<bool> InitChart(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef);
    ValueTask<bool> IsDatasetVisible(Guid configGuid, int datasetIndex);
    ValueTask RemoveDataFromDatasets(Guid configGuid, int? atPosition);
    ValueTask RemoveDataset(Guid configGuid, string datasetId);
    ValueTask RenderChart(Guid configGuid);
    ValueTask ResetChart(Guid configGuid);
    ValueTask ResizeChart(Guid configGuid, double? width = null, double? height = null);
    ValueTask SetDatasetPointsActive(Guid configGuid, int datasetIndex);
    ValueTask SetDatasetsData(Guid configGuid, Dictionary<ChartJsDataset, IList<object>> data);
    ValueTask SetDatasetVisibility(Guid configGuid, int datasetIndex, bool value);
    ValueTask SetLabels(Guid configGuid, IList<string> labels);
    ValueTask ShowDataset(Guid configGuid, int datasetIndex, int? index);
    ValueTask StopChart(Guid configGuid);
    ValueTask ToggleDataVisibility(Guid configGuid, int index);
    ValueTask UpdateChartOptions(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef);
}