using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.PluginSamples;

public sealed partial class ChartJsPluginQuadrantsSample : ChartJsPluginQuadrantsSampleBase
{
}

public abstract class ChartJsPluginQuadrantsSampleBase : ChartJsPluginSampleBase
{
    private const int DataCount = 7;
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Green = "rgb(75, 192, 192)";
    private const string Yellow = "rgb(255, 205, 86)";

    private readonly PluginSampleChartJsConfig config = CreateConfig();

    protected PluginSampleChartJsConfig Config => config;

    protected override string PluginId => "quadrants";

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new PluginSampleChartJsConfig
                {
                    Type = ChartType.scatter,
                    Data = data,
                    Options = new PluginSampleChartJsOptions
                    {
                        Plugins = new PluginSamplePlugins
                        {
                            Quadrants = new QuadrantsOptions
                            {
                                TopLeft = "rgb(255, 99, 132)",
                                TopRight = "rgb(54, 162, 235)",
                                BottomRight = "rgb(75, 192, 192)",
                                BottomLeft = "rgb(255, 205, 86)",
                            },
                        },
                    },
                };
                """),
            new ChartJsDocsCodeTab(
                "plugin",
                "Plugin",
                """
                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/pax.BlazorChartJs.samplelib/chartJsSamplePlugins.js");

                await module.InvokeVoidAsync(
                    "registerPlugin",
                    ChartJsSetupOptions.Value.ChartJsLocation,
                    "quadrants");
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                var data = new ChartJsData
                {
                    Datasets =
                    [
                        new ScatterDataset
                        {
                            Label = "Dataset 1",
                            Data = RandomPoints(7),
                            BorderColor = "rgb(255, 99, 132)",
                            BackgroundColor = "rgba(255, 99, 132, 0.5)",
                        },
                        new ScatterDataset
                        {
                            Label = "Dataset 2",
                            Data = RandomPoints(7),
                            BorderColor = "rgb(54, 162, 235)",
                            BackgroundColor = "rgba(54, 162, 235, 0.5)",
                        },
                    ],
                };
                """),
        ]);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                const config = {
                  type: 'scatter',
                  data,
                  options: {
                    plugins: {
                      quadrants: {
                        topLeft: Utils.CHART_COLORS.red,
                        topRight: Utils.CHART_COLORS.blue,
                        bottomRight: Utils.CHART_COLORS.green,
                        bottomLeft: Utils.CHART_COLORS.yellow
                      }
                    }
                  },
                  plugins: [quadrants]
                };
                """),
            new ChartJsDocsCodeTab(
                "plugin",
                "Plugin",
                """
                const quadrants = {
                  id: 'quadrants',
                  beforeDraw(chart, _args, options) {
                    const {ctx, chartArea: {left, top, right, bottom}, scales: {x, y}} = chart;
                    const midX = x.getPixelForValue(0);
                    const midY = y.getPixelForValue(0);
                    ctx.save();
                    ctx.fillStyle = options.topLeft;
                    ctx.fillRect(left, top, midX - left, midY - top);
                    ctx.fillStyle = options.topRight;
                    ctx.fillRect(midX, top, right - midX, midY - top);
                    ctx.fillStyle = options.bottomRight;
                    ctx.fillRect(midX, midY, right - midX, bottom - midY);
                    ctx.fillStyle = options.bottomLeft;
                    ctx.fillRect(left, midY, midX - left, bottom - midY);
                    ctx.restore();
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                const data = {
                  datasets: [
                    {
                      label: 'Dataset 1',
                      data: Utils.points({count: 7, min: -100, max: 100}),
                      borderColor: Utils.CHART_COLORS.red,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5)
                    },
                    {
                      label: 'Dataset 2',
                      data: Utils.points({count: 7, min: -100, max: 100}),
                      borderColor: Utils.CHART_COLORS.blue,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5)
                    }
                  ]
                };
                """),
        ]);

    protected static string PluginModuleCode => ChartJsPluginCode.Quadrants;

    private static PluginSampleChartJsConfig CreateConfig()
    {
        return new PluginSampleChartJsConfig
        {
            Type = ChartType.scatter,
            Data = new ChartJsData
            {
                Datasets =
                [
                    CreateScatterDataset("Dataset 1", Red, RedTransparent),
                    CreateScatterDataset("Dataset 2", Blue, BlueTransparent),
                ],
            },
            Options = new PluginSampleChartJsOptions
            {
                Plugins = new PluginSamplePlugins
                {
                    Quadrants = new QuadrantsOptions
                    {
                        TopLeft = Red,
                        TopRight = Blue,
                        BottomRight = Green,
                        BottomLeft = Yellow,
                    },
                },
            },
        };
    }

    private static ScatterDataset CreateScatterDataset(string label, string borderColor, string backgroundColor)
    {
        return new ScatterDataset
        {
            Label = label,
            Data = RandomPoints(),
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
        };
    }

    private static List<object> RandomPoints()
    {
        List<object> points = new(DataCount);

        for (var i = 0; i < DataCount; i++)
        {
            points.Add(new DataPoint
            {
                X = Random.Shared.Next(-100, 101),
                Y = Random.Shared.Next(-100, 101),
            });
        }

        return points;
    }
}
