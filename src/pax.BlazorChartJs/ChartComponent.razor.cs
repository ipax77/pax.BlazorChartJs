using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using pax.BlazorChartJs.BlazorLegend;
using System.Text.Json;

namespace pax.BlazorChartJs;

/// <summary>
/// ChartComponent
/// </summary>
public partial class ChartComponent : ComponentBase, IAsyncDisposable
{
    private bool isDisposed;
    private DotNetObjectReference<ChartComponent>? dotNetHelper;

#pragma warning disable CA1024
    public DotNetObjectReference<ChartComponent>? GetTestReference()
    {
        return dotNetHelper;
    }
#pragma warning restore CA1024    

    /// <summary>
    /// ChartGuid - chart canvas id
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ChartJsConfig ChartJsConfig { get; set; } = default!;

    /// <summary>
    /// ChartJsEvent - set required events to true to trigger
    /// e.g. config.Options.Plugins.Legend.OnClickEvent = true
    /// </summary>
    [Parameter]
    public EventCallback<ChartJsEvent> OnEventTriggered { get; set; }

    /// <summary>
    /// ChartJsInterop
    /// </summary>
    [Inject]
    protected ChartJsInterop ChartJsInterop { get; set; } = default!;

    public ElementReference? CanvasElement { get; private set; }

    protected override void OnInitialized()
    {
        dotNetHelper = DotNetObjectReference.Create(this);

        ChartJsConfig.DatasetsAdd += ChartJsConfig_DatasetsAdd;
        ChartJsConfig.DatasetsRemove += ChartJsConfig_DatasetsRemove;
        ChartJsConfig.DatasetsUpdate += ChartJsConfig_DatasetsUpdate;
        ChartJsConfig.DatasetsSet += ChartJsConfig_DatasetsSet;
        ChartJsConfig.DataAdd += ChartJsConfig_DataAdd;
        ChartJsConfig.DataRemove += ChartJsConfig_DataRemove;
        ChartJsConfig.DataSet += ChartJsConfig_DataSet;
        ChartJsConfig.LabelsSet += ChartJsConfig_LabelsSet;
        ChartJsConfig.AddDataEvent += ChartJsConfig_AddDataEvent;
        ChartJsConfig.ChartOptionsUpdate += ChartJsConfig_ChartOptionsUpdate;
        ChartJsConfig.ChartRedraw += ChartJsConfig_ChartRedraw;
        base.OnInitialized();
    }

    private async void ChartJsConfig_ChartRedraw(object? sender, EventArgs e)
    {
        if (dotNetHelper != null)
        {
            var initResult = await ChartJsInterop.InitChart(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
            if (initResult == true)
            {
                await InvokeAsync(() =>
                    OnEventTriggered.InvokeAsync(new ChartJsInitEvent()
                    {
                        ChartJsConfigGuid = ChartJsConfig.ChartJsConfigGuid
                    }))
                .ConfigureAwait(false);
            }
        }
    }

    private async void ChartJsConfig_ChartOptionsUpdate(object? sender, EventArgs e)
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.UpdateChartOptions(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    private async void ChartJsConfig_DatasetsSet(object? sender, DatasetsSetEventArgs e)
    {
        await ChartJsInterop.SetDatasets(ChartJsConfig.ChartJsConfigGuid, e.Datasets).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DatasetsUpdate(object? sender, DatasetsUpdateEventArgs e)
    {
        await ChartJsInterop.UpdateDatasets(ChartJsConfig.ChartJsConfigGuid, e.Datasets, e.Smooth)
            .ConfigureAwait(false);
    }

    private async void ChartJsConfig_AddDataEvent(object? sender, AddDataEventArgs e)
    {
        await ChartJsInterop.AddData(ChartJsConfig.ChartJsConfigGuid, e.Label, e.AtPosition, e.Datas)
            .ConfigureAwait(false);
    }

    private async void ChartJsConfig_LabelsSet(object? sender, LabelsSetEventArgs e)
    {
        await ChartJsInterop.SetLabels(ChartJsConfig.ChartJsConfigGuid, e.Labels).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DataSet(object? sender, DataSetEventArgs e)
    {
        await ChartJsInterop.SetData(ChartJsConfig.ChartJsConfigGuid, e.Labels, e.Datas).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DataRemove(object? sender, DataRemoveEventArgs e)
    {
        await ChartJsInterop.RemoveData(ChartJsConfig.ChartJsConfigGuid).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DataAdd(object? sender, DataAddEventArgs e)
    {
        await ChartJsInterop.AddDataToDataset(
            ChartJsConfig.ChartJsConfigGuid,
            e.Label,
            e.Data,
            e.BackgroundColors,
            e.BorderColors,
            e.AtPostion).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DatasetsRemove(object? sender, DatasetsRemoveEventArgs e)
    {
        await ChartJsInterop.RemoveDatasets(ChartJsConfig.ChartJsConfigGuid, e.DatasetIds).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DatasetsAdd(object? sender, DatasetsAddEventArgs e)
    {
        await ChartJsInterop.AddDatasets(ChartJsConfig.ChartJsConfigGuid, e.Datasets).ConfigureAwait(false);
    }

    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && dotNetHelper != null)
        {
            try
            {
                var initResult = await ChartJsInterop.InitChart(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
                if (initResult == true)
                {
                    await InvokeAsync(() => OnEventTriggered
                        .InvokeAsync(new ChartJsInitEvent() { ChartJsConfigGuid = ChartJsConfig.ChartJsConfigGuid }))
                    .ConfigureAwait(false);
                }
            }
            catch (ObjectDisposedException) { }
            catch (JSException) { }
        }
        base.OnAfterRender(firstRender);
    }

    /// <summary>
    /// (Re-)Draws the chart
    /// </summary>
    [Obsolete($"Please use {nameof(ChartJsConfig)}.{nameof(ChartJsConfig.ReinitializeChart)} instead")]
    public async Task DrawChart()
    {
        if (dotNetHelper != null)
        {
            var initResult = await ChartJsInterop.InitChart(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
            if (initResult == true)
            {
                await InvokeAsync(() =>
                    OnEventTriggered.InvokeAsync(new ChartJsInitEvent()
                    {
                        ChartJsConfigGuid = ChartJsConfig.ChartJsConfigGuid
                    }))
                .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Update Chart Options
    /// </summary>
    [Obsolete($"Please use {nameof(ChartJsConfig)}.{nameof(ChartJsConfig.UpdateChartOptions)} instead")]
    public async Task UpdateChartOptions()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.UpdateChartOptions(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Javascript call
    /// </summary>
    [JSInvokable]
    public void EventTriggered(string eventType, string eventSource, object? data)
    {
        if (Enum.TryParse(eventType, out ChartJsEventType chartJsEventType)
         && Enum.TryParse(eventSource, out ChartJsEventSource chartJsEventSource))
        {
            var jsonElement = data == null ? new JsonElement() : (JsonElement)data;
            ChartJsEvent? chartJsEvent = (chartJsEventSource, chartJsEventType) switch
            {
                (ChartJsEventSource.label, ChartJsEventType.click) => JsonSerializer.Deserialize<ChartJsLabelClickEvent>(jsonElement),
                (ChartJsEventSource.label, ChartJsEventType.hover) => JsonSerializer.Deserialize<ChartJsLabelHoverEvent>(jsonElement),
                (ChartJsEventSource.legend, ChartJsEventType.click) => JsonSerializer.Deserialize<ChartJsLegendClickEvent>(jsonElement),
                (ChartJsEventSource.legend, ChartJsEventType.hover) => JsonSerializer.Deserialize<ChartJsLegendHoverEvent>(jsonElement),
                (ChartJsEventSource.legend, ChartJsEventType.leave) => JsonSerializer.Deserialize<ChartJsLegendLeaveEvent>(jsonElement),
                (ChartJsEventSource.animation, ChartJsEventType.progress) => JsonSerializer.Deserialize<ChartJsAnimationProgressEvent>(jsonElement),
                (ChartJsEventSource.animation, ChartJsEventType.complete) => JsonSerializer.Deserialize<ChartJsAnimationCompleteEvent>(jsonElement),
                (ChartJsEventSource.chart, ChartJsEventType.resize) => JsonSerializer.Deserialize<ChartJsResizeEvent>(jsonElement),
                _ => null
            };
            if (chartJsEvent != null)
            {
                chartJsEvent.ChartJsConfigGuid = ChartJsConfig.ChartJsConfigGuid;
                OnEventTriggered.InvokeAsync(chartJsEvent);
            }
        }
    }

    /// <summary>
    /// Use this to manually resize the canvas element. This is run each time the canvas container is resized,
    /// but you can call this method manually if you change the size of the canvas nodes container element.
    /// You can call.resize() with no parameters to have the chart take the size of its container element,
    /// or you can pass explicit dimensions (e.g., for printing).
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public async Task ResizeChart(double? width, double? height)
    {
        await ChartJsInterop.ResizeChart(ChartJsConfig.ChartJsConfigGuid, width, height).ConfigureAwait(false);
    }

    public async Task<string> GetChartImage(string? imageType = null, int? imageQuality = null, double? width = null, double? height = null)
    {
        return await ChartJsInterop.GetChartImage(ChartJsConfig.ChartJsConfigGuid, imageType, imageQuality, width, height).ConfigureAwait(false);
    }

    /// <summary>
    /// Reset the chart to its state before the initial animation. A new animation can then be triggered using update.
    /// </summary>
    public async ValueTask ResetChart()
    {
        await ChartJsInterop.ResetChart(ChartJsConfig.ChartJsConfigGuid).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers a redraw of all chart elements. Note, this does not update elements for new data. Use .update() in that case.
    /// </summary>
    public async ValueTask RenderChart()
    {
        await ChartJsInterop.RenderChart(ChartJsConfig.ChartJsConfigGuid).ConfigureAwait(false);
    }

    /// <summary>
    /// Use this to stop any current animation. This will pause the chart during any current animation frame. Call .render() to re-animate.
    /// </summary>
    public async ValueTask StopChart()
    {
        await ChartJsInterop.StopChart(ChartJsConfig.ChartJsConfigGuid).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the visibility for a given dataset. This can be used to build a chart legend in HTML.
    /// During click on one of the HTML items, you can call setDatasetVisibility to change the appropriate dataset.
    /// </summary>
    public async ValueTask SetDatasetVisibility(ChartJsDataset dataset, bool value)
    {
        await ChartJsInterop.SetDatasetVisibility(ChartJsConfig.ChartJsConfigGuid, ChartJsConfig.Data.Datasets.IndexOf(dataset), value).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the visibility for a given dataset. This can be used to build a chart legend in HTML.
    /// During click on one of the HTML items, you can call setDatasetVisibility to change the appropriate dataset.
    /// </summary>
    public async ValueTask SetDatasetVisibility(int datasetIndex, bool value)
    {
        await ChartJsInterop.SetDatasetVisibility(ChartJsConfig.ChartJsConfigGuid, datasetIndex, value).ConfigureAwait(false);
    }

    /// <summary>
    /// Toggles the visibility of an item in all datasets. A dataset needs to explicitly support this feature for it to have an effect.
    /// From internal chart types, doughnut / pie, polar area, and bar use this.
    /// </summary>
    public async ValueTask ToggleDataVisibility(int index)
    {
        await ChartJsInterop.ToggleDataVisibility(ChartJsConfig.ChartJsConfigGuid, index).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the stored visibility state of an data index for all datasets. Set by toggleDataVisibility.
    /// A dataset controller should use this method to determine if an item should not be visible.
    /// </summary>
    public async ValueTask<bool> GetDataVisibility(int index)
    {
        return await ChartJsInterop.GetDataVisibility(ChartJsConfig.ChartJsConfigGuid, index).ConfigureAwait(false);
    }

    /// <summary>
    /// If dataIndex is not specified, sets the visibility for the given dataset to false. Updates the chart and animates the dataset with 'hide' mode.
    /// This animation can be configured under the hide key in animation options. Please see animations docs for more details.
    /// If dataIndex is specified, sets the hidden flag of that element to true and updates the chart.
    /// </summary>
    public async ValueTask HideDataset(ChartJsDataset dataset, int? index)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        await ChartJsInterop.HideDataset(ChartJsConfig.ChartJsConfigGuid, dataset, index).ConfigureAwait(false);
    }

    /// <summary>
    /// If dataIndex is not specified, sets the visibility for the given dataset to true. Updates the chart and animates the dataset with 'show' mode.
    /// This animation can be configured under the show key in animation options. Please see animations docs for more details.
    /// If dataIndex is specified, sets the hidden flag of that element to false and updates the chart.
    /// </summary>
    public async ValueTask ShowDataset(ChartJsDataset dataset, int? index)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        await ChartJsInterop.ShowDataset(ChartJsConfig.ChartJsConfigGuid, ChartJsConfig.Data.Datasets.IndexOf(dataset), index)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the current chart legend Items
    /// </summary>
    /// <returns>ChartJsLegendItems</returns>
    public async ValueTask<List<ChartJsLegendItem>> GetLegendItems()
    {
        return await ChartJsInterop.GetLegendItems(ChartJsConfig.ChartJsConfigGuid)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the current dataset visibility
    /// </summary>
    /// <param name="datasetIndex"></param>
    /// <returns>True if the dataset is visible.</returns>
    public async ValueTask<bool> IsDatasetVisible(int datasetIndex)
    {
        return await ChartJsInterop.IsDatasetVisible(ChartJsConfig.ChartJsConfigGuid, datasetIndex)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the datset points active. Use -1 to clear all active points.
    /// </summary>
    /// <param name="datasetIndex"></param>
    public async ValueTask SetDatasetPointsActive(int datasetIndex)
    {
        await ChartJsInterop.SetDatasetPointsActive(ChartJsConfig.ChartJsConfigGuid, datasetIndex)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true).ConfigureAwait(false);
        dotNetHelper?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            await ChartJsInterop.DisposeChart(ChartJsConfig.ChartJsConfigGuid).ConfigureAwait(false);
            ChartJsConfig.DatasetsAdd -= ChartJsConfig_DatasetsAdd;
            ChartJsConfig.DatasetsRemove -= ChartJsConfig_DatasetsRemove;
            ChartJsConfig.DatasetsUpdate -= ChartJsConfig_DatasetsUpdate;
            ChartJsConfig.DatasetsSet -= ChartJsConfig_DatasetsSet;
            ChartJsConfig.DataAdd -= ChartJsConfig_DataAdd;
            ChartJsConfig.DataRemove -= ChartJsConfig_DataRemove;
            ChartJsConfig.DataSet -= ChartJsConfig_DataSet;
            ChartJsConfig.LabelsSet -= ChartJsConfig_LabelsSet;
            ChartJsConfig.AddDataEvent -= ChartJsConfig_AddDataEvent;
            ChartJsConfig.ChartOptionsUpdate -= ChartJsConfig_ChartOptionsUpdate;
            ChartJsConfig.ChartRedraw -= ChartJsConfig_ChartRedraw;
        }

        isDisposed = true;
    }
}
