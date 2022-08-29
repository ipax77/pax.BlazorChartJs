using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// OnLabelClicked - reports click on chart and returns the chartConfig.Guid and the nearest label to that click
    /// </summary>
    [Parameter]
    public EventCallback<KeyValuePair<Guid, string>> OnLabelClicked { get; set; }

    /// <summary>
    /// ChartJsInterop
    /// </summary>
    [Inject]
    protected ChartJsInterop ChartJsInterop { get; set; } = default!;

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
        await ChartJsInterop.SetLabels(ChartJsConfig.ChartJsConfigGuid, e.Labels);
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
    public void ChartClicked(string label)
    {
        OnLabelClicked.InvokeAsync(new KeyValuePair<Guid, string>(ChartJsConfig.ChartJsConfigGuid, label));
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
            ChartJsConfig.DatasetAdd -= ChartJsConfig_DatasetAdd;
            ChartJsConfig.DatasetRemove -= ChartJsConfig_DatasetRemove;
            ChartJsConfig.DataAdd -= ChartJsConfig_DataAdd;
            ChartJsConfig.DataRemove -= ChartJsConfig_DataRemove;
            ChartJsConfig.DataSet -= ChartJsConfig_DataSet;
            ChartJsConfig.LabelsSet -= ChartJsConfig_LabelsSet;
            dotNetHelper?.Dispose();
            // todo: cleanup js chart?
        }

        isDisposed = true;
    }
}
