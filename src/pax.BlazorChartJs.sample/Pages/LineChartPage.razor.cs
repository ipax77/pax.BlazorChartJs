
using Microsoft.AspNetCore.Components;

namespace pax.BlazorChartJs.sample.Pages;

public partial class LineChartPage : ComponentBase
{
    ChartComponent? chartComponent;
    ChartJsConfig chartJsConfig = null!;

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
                Datasets = new List<object>()
                {
                    new LineDataset()
                    {
                        Label = "Team 1",
                        Data = new List<object>() { 1, 2, 3, 4, 5, 6 },
                        BackgroundColor = "lightblue",
                        BorderColor = new List<string>() { "lightblue" },
                        BorderWidth = 5,
                        Fill = false,
                        PointBackgroundColor = "blue",
                        PointBorderColor = "blue",
                        PointRadius = 6,
                        PointBorderWidth = 6,
                        PointHitRadius = 6,
                        Tension = 0
                    },
                    new LineDataset()
                    {
                        Label = "Team 2",
                        Data = new List<object>() { 6, 5, 4, 3, 2, 1 },
                        BackgroundColor = "lightgreen",
                        BorderColor = new List<string>() { "lightgreen" },
                        BorderWidth = 5,
                        Fill = false,
                        PointBackgroundColor = "green",
                        PointBorderColor = "green",
                        PointRadius = 6,
                        PointBorderWidth = 6,
                        PointHitRadius = 6,
                        Tension = 0
                    }
                }
            },
            Options = new ChartJsOptions()
            {
                Responsive = true,
                Plugins = new Plugins()
                {
                    ArbitraryLines = new List<ArbitraryLineConfig>()
                },
                Scales = new ChartJsOptionsScales()
                {
                    X = new LinearAxis()
                    {
                        Display = true,
                        Position = "bottom",
                        BeginAtZero = true,
                        Title = new Title()
                        {
                            Display = true,
                            Text = "GameTime",
                            Color = "black",
                            Font = new()
                            {
                                Size = 16
                            },
                            Padding = new()
                            {
                                Top = 4,
                                Bottom = 4
                            }
                        },
                        Ticks = new LinearAxisTick()
                        {
                            Color = "red",
                            Padding = 3,
                            AutoSkipPadding = 3,
                            BackdropColor = "rgba(255, 255, 255, 0.75)",
                            Align = "center",
                            CrossAlign = "near",
                            ShowLabelBackdrop = false,
                            BackdropPadding = new Padding(2)
                        },
                        Grid = new ChartJsGrid()
                        {
                            Display = true,
                            Color = "lightgrey",
                            LineWidth = 1,
                            DrawBorder = true,
                            DrawOnChartArea = true,
                            TickLength = 8,
                            TickWidth = 1,
                            TickColor = "red",
                            Offset = false,
                            BorderWidth = 1,
                            BorderColor = "rgba(0,0,0,0.1)"
                        }
                    },
                    Y = new LinearAxis()
                    {
                        Display = true,
                        BeginAtZero = true,
                        Title = new Title()
                        {
                            Display = true,
                            Text = "%",
                            Color = "red",
                            Font = new()
                            {
                                Size = 16
                            },
                            Padding = new()
                            {
                                Top = 4,
                                Bottom = 4
                            }
                        },
                        Ticks = new LinearAxisTick()
                        {
                            Color = "red",
                            Padding = 3,
                            AutoSkipPadding = 3,
                            BackdropColor = "rgba(255, 255, 255, 0.75)",
                            Align = "center",
                            CrossAlign = "near",
                            ShowLabelBackdrop = false,
                            BackdropPadding = new Padding(2)
                        },
                        Grid = new ChartJsGrid()
                        {
                            Display = true,
                            Color = "lightgrey",
                            LineWidth = 1,
                            DrawBorder = true,
                            DrawOnChartArea = true,
                            TickLength = 8,
                            TickWidth = 1,
                            TickColor = "red",
                            Offset = false,
                            BorderWidth = 1,
                            BorderColor = "rgba(0,0,0,0.1)"
                        }
                    }
                }
            }
        };
            
        base.OnInitialized();
    }

    public void ShowChart()
    {
        chartComponent?.DrawChart();
    }

    public void AddArbitraryLine()
    {
        if (chartJsConfig.Options == null)
        {
            chartJsConfig.Options = new();
        }

        if (chartJsConfig.Options.Plugins == null)
        {
            chartJsConfig.Options.Plugins = new();
        }

        chartJsConfig.Options.Plugins.ArbitraryLines = new List<ArbitraryLineConfig>()
        {
            new ArbitraryLineConfig()
            {
                ArbitraryLineColor = "blue",
                XPosition = 2,
                XWidth = 3,
                Text = "Plugin Test"
            }
        };
        chartComponent?.UpdateChartOptions();
    }

    public async Task AddData()
    {
    }    
}