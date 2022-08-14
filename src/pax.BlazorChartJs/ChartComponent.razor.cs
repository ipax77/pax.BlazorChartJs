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
    private ElementReference? elementReference;

    /// <summary>
    /// ChartGuid - chart canvas id
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ChartJsConfig ChartJsConfig { get; set; } = default!;

    /// <summary>
    /// ChartJsInterop
    /// </summary>
    [Inject]
    protected ChartJsInterop ChartJsInterop { get; set; } = default!;

    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender"></param>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            dotNetHelper = DotNetObjectReference.Create(this);
        }
        base.OnAfterRender(firstRender);
    }

    /// <summary>
    /// ShowChart
    /// </summary>

    public async Task ShowChart()
    {
        if (elementReference != null && ChartJsInterop != null)
        {
            await ChartJsInterop.ShowChart(ChartJsConfig).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// ShowChart
    /// </summary>

    public async Task AddDataToDataset(ChartJsConfig config, Dataset dataset, string label)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dataset);

        if (elementReference != null && ChartJsInterop != null)
        {
            await ChartJsInterop.AddDataToDataset(config.ChartJsConfigGuid.ToString(), dataset.Id, label).ConfigureAwait(false);
        }
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

//        if (disposing)
//        {
//#pragma warning disable CA2012 // Use ValueTasks correctly
//            _ = ChartJsInterop?.DisposeAsync();
//#pragma warning restore CA2012 // Use ValueTasks correctly
//            dotNetHelper?.Dispose();
//        }

        isDisposed = true;
    }
}
