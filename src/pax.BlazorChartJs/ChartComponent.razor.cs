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
    /// AddLastDataset
    /// </summary>
    public async Task AddLastDataset()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.AddLastDataset(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// ShowChart
    /// </summary>
    public async Task AddLastDatasToDatasets()
    {
        await ChartJsInterop.AddDataToDataset(ChartJsConfig).ConfigureAwait(false);
    }

    /// <summary>
    /// ShowChart
    /// </summary>
    public async Task RemoveLastDataset()
    {
        await ChartJsInterop.RemoveLastDataset(ChartJsConfig).ConfigureAwait(false);
    }

    /// <summary>
    /// ShowChart
    /// </summary>
    public async Task RemoveLastDataFromDatasets()
    {
        await ChartJsInterop.RemoveLastDataFromDatasets(ChartJsConfig).ConfigureAwait(false);
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
            dotNetHelper?.Dispose();
            // todo: cleanup js chart?
        }

        isDisposed = true;
    }
}
