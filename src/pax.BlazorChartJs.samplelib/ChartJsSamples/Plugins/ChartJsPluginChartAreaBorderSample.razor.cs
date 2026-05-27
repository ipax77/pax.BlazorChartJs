using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.PluginSamples;

public sealed partial class ChartJsPluginChartAreaBorderSample : ChartJsPluginChartAreaBorderSampleBase
{
}

public abstract class ChartJsPluginChartAreaBorderSampleBase : ChartJsPluginSampleBase
{
    private const int DataCount = 7;
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";

    private static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
    ];

    private readonly PluginSampleChartJsConfig config = CreateConfig();

    protected PluginSampleChartJsConfig Config => config;

    protected override string PluginId => "chartAreaBorder";

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new PluginSampleChartJsConfig
                {
                    Type = ChartType.line,
                    Data = data,
                    Options = new PluginSampleChartJsOptions
                    {
                        Plugins = new PluginSamplePlugins
                        {
                            ChartAreaBorder = new ChartAreaBorderOptions
                            {
                                BorderColor = "red",
                                BorderWidth = 2,
                                BorderDash = [5, 5],
                                BorderDashOffset = 2,
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
                    "chartAreaBorder");
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                var data = new ChartJsData
                {
                    Labels = ["January", "February", "March", "April", "May", "June", "July"],
                    Datasets =
                    [
                        new LineDataset
                        {
                            Label = "Dataset 1",
                            Data = RandomNumbers(7),
                            BorderColor = "rgb(255, 99, 132)",
                            BackgroundColor = "rgba(255, 99, 132, 0.5)",
                        },
                        new LineDataset
                        {
                            Label = "Dataset 2",
                            Data = RandomNumbers(7),
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
                  type: 'line',
                  data,
                  options: {
                    plugins: {
                      chartAreaBorder: {
                        borderColor: 'red',
                        borderWidth: 2,
                        borderDash: [5, 5],
                        borderDashOffset: 2
                      }
                    }
                  },
                  plugins: [chartAreaBorder]
                };
                """),
            new ChartJsDocsCodeTab(
                "plugin",
                "Plugin",
                """
                const chartAreaBorder = {
                  id: 'chartAreaBorder',
                  beforeDraw(chart, _args, options) {
                    const {ctx, chartArea: {left, top, width, height}} = chart;
                    ctx.save();
                    ctx.strokeStyle = options.borderColor;
                    ctx.lineWidth = options.borderWidth;
                    ctx.setLineDash(options.borderDash || []);
                    ctx.lineDashOffset = options.borderDashOffset;
                    ctx.strokeRect(left, top, width, height);
                    ctx.restore();
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                const data = {
                  labels: Utils.months({count: 7}),
                  datasets: [
                    {
                      label: 'Dataset 1',
                      data: Utils.numbers({count: 7, min: -100, max: 100}),
                      borderColor: Utils.CHART_COLORS.red,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5)
                    },
                    {
                      label: 'Dataset 2',
                      data: Utils.numbers({count: 7, min: -100, max: 100}),
                      borderColor: Utils.CHART_COLORS.blue,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5)
                    }
                  ]
                };
                """),
        ]);

    protected static string PluginModuleCode => ChartJsPluginCode.ChartAreaBorder;

    private static PluginSampleChartJsConfig CreateConfig()
    {
        return new PluginSampleChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = [.. MonthLabels],
                Datasets =
                [
                    CreateLineDataset("Dataset 1", Red, RedTransparent),
                    CreateLineDataset("Dataset 2", Blue, BlueTransparent),
                ],
            },
            Options = new PluginSampleChartJsOptions
            {
                Plugins = new PluginSamplePlugins
                {
                    ChartAreaBorder = new ChartAreaBorderOptions
                    {
                        BorderColor = "red",
                        BorderWidth = 2,
                        BorderDash = [5, 5],
                        BorderDashOffset = 2,
                    },
                },
            },
        };
    }

    private static LineDataset CreateLineDataset(string label, string borderColor, string backgroundColor)
    {
        return new LineDataset
        {
            Label = label,
            Data = RandomNumbers(),
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
        };
    }

    private static List<object> RandomNumbers()
    {
        List<object> values = new(DataCount);
        for (var i = 0; i < DataCount; i++)
        {
            values.Add(Random.Shared.Next(-100, 101));
        }

        return values;
    }
}
