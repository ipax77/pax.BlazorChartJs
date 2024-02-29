using Microsoft.AspNetCore.Components;

namespace pax.BlazorChartJs.samplelib;

public partial class HtmlLegendChartComp : ComponentBase
{
    ChartComponent? chartComponent;
    ChartJsConfig chartJsConfig = null!;
    LegendComponent? legendComponent;

    protected override void OnInitialized()
    {
        chartJsConfig = new()
        {
            Type = ChartType.line,
            Data = new ChartJsData()
            {
                Labels = new List<string>()
                {
                    "Red", "Blue", "Yellow", "Green", "Purple", "Orange"
                },
                Datasets = new List<ChartJsDataset>()
                {
                    new LineDataset()
                    {
                        Label = "Team 1",
                        Data = new List<object>() { 1, 2, 3, 4, 5, 6 },
                        BackgroundColor = "lightblue",
                        BorderColor = "blue",
                        BorderWidth = 5,
                        PointRadius = 6,
                        PointBorderWidth = 6,
                        PointHitRadius = 6,
                        PointHoverRadius = 10,
                        PointHoverBorderWidth = 10,
                    },
                    new LineDataset()
                    {
                        Label = "Team 2",
                        Data = new List<object>() { 6, 5, 4, 3, 2, 1 },
                        BackgroundColor = "lightgreen",
                        BorderColor = "green",
                        BorderWidth = 5,
                        PointRadius = 6,
                        PointBorderWidth = 6,
                        PointHitRadius = 6,
                        PointHoverRadius = 10,
                        PointHoverBorderWidth = 10,
                    }
                }
            },
            Options = new ChartJsOptions()
            {
                Responsive = true,
                OnHoverEvent = true,
                Plugins = new Plugins()
                {
                    ArbitraryLines = new List<ArbitraryLineConfig>(),
                    Legend = new Legend()
                    {
                        Display = false
                    }
                },
                Animation = new Animation()
                {
                    OnCompleteEvent = true,
                }
            }
        };

        base.OnInitialized();
    }

    private void ChartEventTriggered(ChartJsEvent chartJsEvent)
    {
        if (chartJsEvent is ChartJsInitEvent)
        {
            InvokeAsync(() => StateHasChanged()).Wait();
            UpdateLegend();
        }
        else if (chartJsEvent is ChartJsAnimationCompleteEvent)
        {
            // UpdateLegend();
        }
        else if (chartJsEvent is ChartJsLabelHoverEvent labelHoverEvent)
        {
            legendComponent?.HighlightDataset(labelHoverEvent.DatasetIndex);
        }
    }

    public void UpdateLegend()
    {
        legendComponent?.UpdateLegend();
    }

    private void Randomize()
    {
        var data = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count, chartJsConfig.Data.Labels.Count, -100, 100);

        Dictionary<ChartJsDataset, SetDataObject> chartData = new();

        for (int i = 0; i < chartJsConfig.Data.Datasets.Count; i++)
        {
            var dataset = chartJsConfig.Data.Datasets.ElementAt(i);
            var dataList = data.ElementAt(i);
            chartData.Add(dataset, new SetDataObject(dataList));
        }
        chartJsConfig.SetData(chartData);
    }

    private void AddDataset()
    {
        var dataset = ChartUtils.GetRandomDataset(chartJsConfig.Type == null ? ChartType.bar : chartJsConfig.Type.Value, chartJsConfig.Data.Datasets.Count + 1, chartJsConfig.Data.Labels.Count);
        if (dataset is LineDataset lineDataset)
        {
            lineDataset.BorderWidth = 3;
            lineDataset.PointHitRadius = 6;
            lineDataset.PointHitRadius = 6;
            lineDataset.PointHoverRadius = 10;
            lineDataset.PointHoverBorderWidth = 10;
        }
        chartJsConfig.AddDataset(dataset);
        UpdateLegend();
    }

    private void RemoveLastDataset()
    {
        if (chartJsConfig.Data.Datasets.Count > 0)
        {
            chartJsConfig.RemoveDataset(chartJsConfig.Data.Datasets.Last());
            UpdateLegend();
        }
    }
}
