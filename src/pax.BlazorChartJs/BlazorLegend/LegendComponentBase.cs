
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using pax.BlazorChartJs.BlazorLegend;

namespace pax.BlazorChartJs;

public class LegendComponentBase : ComponentBase
{
    [Parameter, EditorRequired]
    public ChartJsConfig ChartJsConfig { get; set; } = default!;

    [Parameter, EditorRequired]
    public ChartComponent? ChartComponent { get; set; }

#pragma warning disable CA1002 // Do not expose generic lists
    public virtual List<ChartJsLegendItem> LegendItems { get; private set; } = [];
#pragma warning restore CA1002 // Do not expose generic lists

    public virtual int? HoverItemIndex { get; private set; }

    public virtual async Task UpdateLegend()
    {
        if (ChartComponent is null)
        {
            return;
        }

        LegendItems = await ChartComponent.GetLegendItems()
            .ConfigureAwait(false);
        await InvokeAsync(() => StateHasChanged())
            .ConfigureAwait(false);
    }

    public virtual async Task HandleLegendItemClick(MouseEventArgs e, ChartJsLegendItem item)
    {
        if (ChartComponent is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(item);

        if (ChartJsConfig.Type == ChartType.pie || ChartJsConfig.Type == ChartType.doughnut)
        {
            await ChartComponent.ToggleDataVisibility(item.Index)
                .ConfigureAwait(false);

            item.Hidden = !item.Hidden;
        }
        else
        {
            var isVisible = await ChartComponent.IsDatasetVisible(item.DatasetIndex)
                .ConfigureAwait(false);

            await ChartComponent.SetDatasetVisibility(item.DatasetIndex, !isVisible)
                .ConfigureAwait(false);

            item.Hidden = isVisible;
        }
        await InvokeAsync(() => StateHasChanged())
            .ConfigureAwait(false);
    }

    public virtual async Task HandleLegendItemHover(MouseEventArgs e, ChartJsLegendItem item)
    {
        if (ChartComponent is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(item);

        var itemIndex = GetItemIndex(item);
        if (HoverItemIndex == itemIndex)
        {
            return;
        }

        HoverItemIndex = itemIndex;

        await ChartComponent.SetDatasetPointsActive(item.DatasetIndex)
            .ConfigureAwait(false);

        await InvokeAsync(() => StateHasChanged())
            .ConfigureAwait(false);
    }

    public virtual async Task HandleLegendItemLeave(MouseEventArgs e, ChartJsLegendItem item)
    {
        if (ChartComponent is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(item);

        if (HoverItemIndex is null)
        {
            return;
        }

        await ChartComponent.SetDatasetPointsActive(-1)
            .ConfigureAwait(false);

        HoverItemIndex = null;

        await InvokeAsync(() => StateHasChanged())
            .ConfigureAwait(false);
    }

    private int GetItemIndex(ChartJsLegendItem item)
    {
        if (ChartJsConfig.Type == ChartType.pie || ChartJsConfig.Type == ChartType.doughnut)
        {
            return item.Index;
        }
        else
        {
            return item.DatasetIndex;
        }
    }
}
