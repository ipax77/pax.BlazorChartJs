using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace pax.BlazorChartJs.samplelib;

public partial class BarChartComp : ComponentBase
{
    ChartComponent? chartComponent;
    IconsChartJsConfig chartJsConfig = null!;
    private string? labelClicked;
    private string? imageString;
    private string? canvasInfo;
    private Lazy<Task<IJSObjectReference>> moduleTask = null!;
    private string chartVersion = "unknown";

    protected override void OnInitialized()
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/pax.BlazorChartJs.samplelib/customPlugin.js").AsTask());

        chartJsConfig = new()
        {
            Type = ChartType.bar,
            Data = new ChartJsData()
            {
                Labels = new List<string>()
                {
                    "Red", "Blue", "Yellow", "Green", "Purple", "Orange"
                },
                Datasets = new List<ChartJsDataset>()
                {
                    new BarDataset()
                    {
                        Label = "# of Votes",
                        Data = [ 12, 19, 3, 5, 2, 3 ],
                        BackgroundColor = [
                            "rgba(255, 99, 132, 0.2)",
                            "rgba(54, 162, 235, 0.2)",
                            "rgba(255, 206, 86, 0.2)",
                            "rgba(75, 192, 192, 0.2)",
                            "rgba(153, 102, 255, 0.2)",
                            "rgba(255, 159, 64, 0.2)",
                        ],
                        BorderColor = [
                            "rgba(255, 99, 132, 1)",
                            "rgba(54, 162, 235, 1)",
                            "rgba(255, 206, 86, 1)",
                            "rgba(75, 192, 192, 1)",
                            "rgba(153, 102, 255, 1)",
                            "rgba(255, 159, 64, 1)",
                        ],
                        BorderWidth = 1
                    }
                }
            },
            Options = new()
            {
                Responsive = true,
                MaintainAspectRatio = true,
                OnClickEvent = true,
                Plugins = new()
                {
                    Title = new Title()
                    {
                        Display = true,
                        Text = new List<string>() { "BarChart", "testing" },
                    },

                },
                Scales = new ChartJsOptionsScales()
                {
                    Y = new LinearAxis()
                    {
                        Title = new Title()
                        {
                            Display = true,
                            Text = new List<string>() { "$ y-axis title test", "€ y-axis title test" }
                        },
                        SuggestedMax = 25
                    }
                }
            }
        };
        base.OnInitialized();
    }

    private void ShowChart()
    {
        chartJsConfig.ReinitializeChart();
    }

    private void Horizontal()
    {
        if (chartJsConfig.Options != null)
        {
            chartJsConfig.Options.IndexAxis = "y";
        }
        else
        {
            chartJsConfig.Options = new()
            {
                IndexAxis = "y"
            };
        }
        chartJsConfig.ReinitializeChart();
    }

    private void ChartEventTriggered(ChartJsEvent chartJsEvent)
    {
        if (chartJsEvent is ChartJsInitEvent initEvent)
        {
            SetChartVersion();
        }

        else if (chartJsEvent is ChartJsLabelClickEvent labelClickEvent)
        {
            labelClicked = labelClickEvent.Label;
        }
    }

    private async void SetChartVersion()
    {
        chartVersion = await jsRuntime.InvokeAsync<string>("getChartVersion");
        await InvokeAsync(() => StateHasChanged());
    }

    private async Task MakeImage()
    {
        if (chartComponent != null)
        {
            imageString = await chartComponent.GetChartImage();
            await InvokeAsync(() => StateHasChanged());
        }
    }

    private async Task ShowImages()
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("registerImagePlugin", 30, 30)
            .ConfigureAwait(false);

        if (chartJsConfig.Options == null)
        {
            chartJsConfig.Options = new();
        }

        if (chartJsConfig.Options.Plugins == null)
        {
            chartJsConfig.Options.Plugins = new();
        }

        List<ChartIconsConfig> icons = new()
    {
        new()
        {
            XWidth = 30,
            YWidth = 30,
            YOffset = 0,
            ImageSrc = "_content/pax.BlazorChartJs.samplelib/images/abathur-min.png",
            Cmdr = "abathur"
        },
        new()
        {
            XWidth = 30,
            YWidth = 30,
            YOffset = 0,
            ImageSrc = "_content/pax.BlazorChartJs.samplelib/images/alarak-min.png",
            Cmdr = "alarak"
        },
        new()
        {
            XWidth = 30,
            YWidth = 30,
            YOffset = 0,
            ImageSrc = "_content/pax.BlazorChartJs.samplelib/images/artanis-min.png",
            Cmdr = "artanis"
        },
    };

        chartJsConfig.Options.Plugins.BarIcons = icons;

        chartJsConfig.ReinitializeChart();
    }

    private void ShowDataLabels()
    {
        if (chartJsConfig.Options == null)
        {
            chartJsConfig.Options = new();
        }

        if (chartJsConfig.Options.Plugins == null)
        {
            chartJsConfig.Options.Plugins = new();
        }

        chartJsConfig.Options.Plugins.Datalabels = new DataLabelsConfig()
        {
            Display = "auto",
            Color = "#0a050c",
            BackgroundColor = "#cdc7ce",
            BorderColor = "#491756",
            BorderRadius = 4,
            BorderWidth = 1,
            Anchor = "end",
            Align = "start",
            Clip = true
        };
        chartJsConfig.ReinitializeChart();
    }

    private async Task GetCanvasInfo()
    {
        if (moduleTask != null)
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            canvasInfo = await module.InvokeAsync<string>("getCanvasInfo", chartJsConfig.ChartJsConfigGuid)
                .ConfigureAwait(false);
            await InvokeAsync(() => StateHasChanged());
        }
    }

    private void AddData()
    {
        var dataAddEventArgs = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count);

        Dictionary<ChartJsDataset, AddDataObject> datas = new();
        for (int i = 0; i < chartJsConfig.Data.Datasets.Count; i++)
        {
            ChartJsDataset dataset = chartJsConfig.Data.Datasets[i];
            datas.Add(dataset,
                new AddDataObject(dataAddEventArgs.Data[i],
                                  null,
                                  dataAddEventArgs.BackgroundColors?[i],
                                  dataAddEventArgs.BorderColors?[i]));
        }
        chartJsConfig.AddData(dataAddEventArgs.Label, null, datas);
    }

    private void Randomize()
    {
        var data = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count,
         chartJsConfig.Data.Labels.Count, -100, 100);

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
        var dataset = ChartUtils
         .GetRandomDataset(chartJsConfig.Type == null ? ChartType.bar : chartJsConfig.Type.Value,
                           chartJsConfig.Data.Datasets.Count + 1,
                           chartJsConfig.Data.Labels.Count);
        chartJsConfig.AddDataset(dataset);
    }

    private void RemoveLastDataset()
    {
        if (chartJsConfig.Data.Datasets.Any())
        {
            chartJsConfig.RemoveDataset(chartJsConfig.Data.Datasets.Last());
        }
    }

    private void RemoveLastDataFromDatasets()
    {
        chartJsConfig.RemoveData();
    }

    public class IconsChartJsConfig : ChartJsConfig
    {
        public new IconsChartJsOptions? Options { get; set; }
    }

    public record IconsChartJsOptions : ChartJsOptions
    {
        public new IconsPlugins? Plugins { get; set; }
    }

    public record IconsPlugins : Plugins
    {
        public ICollection<ChartIconsConfig>? BarIcons { get; set; }
    }

    public record ChartIconsConfig
    {
        public int XWidth { get; set; }
        public int YWidth { get; set; }
        public int YOffset { get; set; }
        public string ImageSrc { get; set; } = null!;
        public string? Cmdr { get; set; }
    }
}