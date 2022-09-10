using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace pax.BlazorChartJs;

/// <summary>
/// ChartComponent
/// </summary>
public partial class ChartComponent : ComponentBase, IDisposable
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
        ChartJsConfig.DatasetAdd += ChartJsConfig_DatasetAdd;
        ChartJsConfig.DatasetRemove += ChartJsConfig_DatasetRemove;
        ChartJsConfig.DataAdd += ChartJsConfig_DataAdd;
        ChartJsConfig.DataRemove += ChartJsConfig_DataRemove;
        ChartJsConfig.DataSet += ChartJsConfig_DataSet;
        ChartJsConfig.LabelsSet += ChartJsConfig_LabelsSet;
        base.OnInitialized();
    }

    private async void ChartJsConfig_LabelsSet(object? sender, LabelsSetEventArgs e)
    {
        await ChartJsInterop.SetLabels(ChartJsConfig.ChartJsConfigGuid, e.Labels).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DataSet(object? sender, DataSetEventArgs e)
    {
        await ChartJsInterop.SetDatasetsData(ChartJsConfig.ChartJsConfigGuid, e.Data).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DataRemove(object? sender, DataRemoveEventArgs e)
    {
        await ChartJsInterop.RemoveDataFromDatasets(ChartJsConfig.ChartJsConfigGuid, e.AtPosition).ConfigureAwait(false);
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

    private async void ChartJsConfig_DatasetRemove(object? sender, DatasetRemoveEventArgs e)
    {
        await ChartJsInterop.RemoveDataset(ChartJsConfig.ChartJsConfigGuid, e.DatasetId).ConfigureAwait(false);
    }

    private async void ChartJsConfig_DatasetAdd(object? sender, DatasetAddEventArgs e)
    {
        await ChartJsInterop.AddDataset(ChartJsConfig.ChartJsConfigGuid, e.Dataset, e.AfterDatasetId).ConfigureAwait(false);
    }

    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetHelper = DotNetObjectReference.Create(this);
            await ChartJsInterop.InitChart(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
        base.OnAfterRender(firstRender);
    }

    /// <summary>
    /// (Re-)Draws the chart
    /// </summary>
    public async Task DrawChart()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.InitChart(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Update Chart Options
    /// </summary>
    public async Task UpdateChartOptions()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.UpdateChartOptions(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Update Chart Datasets
    /// </summary>
    public async Task UpdateChartDatasets()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.UpdateChartDatasets(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Javascript call
    /// </summary>
    [JSInvokable]
    public void EventTriggered(string eventType, string eventSource, object? data)
    {
        if (Enum.TryParse(eventType, out ChartJsEventType chartJsEventType))
        {
        }
        if (Enum.TryParse(eventSource, out ChartJsEventSource chartJsEventSource))
        {
        }
        OnEventTriggered.InvokeAsync(new ChartJsEvent(ChartJsConfig.ChartJsConfigGuid, chartJsEventType, chartJsEventSource, data));
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
        await ChartJsInterop.ShowDataset(ChartJsConfig.ChartJsConfigGuid, ChartJsConfig.Data.Datasets.IndexOf(dataset), index).ConfigureAwait(false);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            dotNetHelper?.Dispose();
            ChartJsConfig.DatasetAdd -= ChartJsConfig_DatasetAdd;
            ChartJsConfig.DatasetRemove -= ChartJsConfig_DatasetRemove;
            ChartJsConfig.DataAdd -= ChartJsConfig_DataAdd;
            ChartJsConfig.DataRemove -= ChartJsConfig_DataRemove;
            ChartJsConfig.DataSet -= ChartJsConfig_DataSet;
            ChartJsConfig.LabelsSet -= ChartJsConfig_LabelsSet;
            // todo: cleanup js chart?
        }

        isDisposed = true;
    }
}
