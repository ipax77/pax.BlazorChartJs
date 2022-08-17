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

    public async Task UpdateChartOptions()
    {
        if (dotNetHelper != null)
        {
            await ChartJsInterop.UpdateChartOptions(ChartJsConfig, dotNetHelper).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// ShowChart
    /// </summary>
    public async Task AddDataToDataset(ChartJsConfig config, object dataset, string label)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dataset);

        var chartDataset = dataset as ChartJsDataset;
        ArgumentNullException.ThrowIfNull(chartDataset);

        await ChartJsInterop.AddDataToDataset(config.ChartJsConfigGuid.ToString(), chartDataset.Id, label).ConfigureAwait(false);
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
