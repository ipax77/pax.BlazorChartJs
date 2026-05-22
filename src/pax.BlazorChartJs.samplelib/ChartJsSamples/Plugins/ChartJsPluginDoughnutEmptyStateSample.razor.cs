using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.PluginSamples;

public sealed partial class ChartJsPluginDoughnutEmptyStateSample : ChartJsPluginDoughnutEmptyStateSampleBase
{
}

public abstract class ChartJsPluginDoughnutEmptyStateSampleBase : ChartJsPluginSampleBase
{
    private readonly PluginSampleChartJsConfig config = CreateConfig();

    protected PluginSampleChartJsConfig Config => config;

    protected override string PluginId => "emptyDoughnut";

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new PluginSampleChartJsConfig
                {
                    Type = ChartType.doughnut,
                    Data = data,
                    Options = new PluginSampleChartJsOptions
                    {
                        Plugins = new PluginSamplePlugins
                        {
                            EmptyDoughnut = new EmptyDoughnutOptions
                            {
                                Color = "rgba(255, 128, 0, 0.5)",
                                Width = 2,
                                RadiusDecrease = 20,
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
                    "emptyDoughnut");
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                var data = new ChartJsData
                {
                    Labels = [],
                    Datasets =
                    [
                        new DoughnutDataset
                        {
                            Label = "Dataset 1",
                            Data = [],
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
                  type: 'doughnut',
                  data,
                  options: {
                    plugins: {
                      emptyDoughnut: {
                        color: 'rgba(255, 128, 0, 0.5)',
                        width: 2,
                        radiusDecrease: 20
                      }
                    }
                  },
                  plugins: [emptyDoughnut]
                };
                """),
            new ChartJsDocsCodeTab(
                "plugin",
                "Plugin",
                """
                const emptyDoughnut = {
                  id: 'emptyDoughnut',
                  afterDraw(chart, _args, options) {
                    const {datasets} = chart.data;
                    let hasData = false;
                    for (let i = 0; i < datasets.length; i += 1) {
                      hasData |= datasets[i].data.length > 0;
                    }

                    if (!hasData) {
                      const {chartArea: {left, top, right, bottom}, ctx} = chart;
                      const centerX = (left + right) / 2;
                      const centerY = (top + bottom) / 2;
                      const radius = Math.min(right - left, bottom - top) / 2;
                      ctx.beginPath();
                      ctx.lineWidth = options.width || 2;
                      ctx.strokeStyle = options.color || 'rgba(255, 128, 0, 0.5)';
                      ctx.arc(centerX, centerY, radius - (options.radiusDecrease || 0), 0, 2 * Math.PI);
                      ctx.stroke();
                    }
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                const data = {
                  labels: [],
                  datasets: [
                    {
                      label: 'Dataset 1',
                      data: []
                    }
                  ]
                };
                """),
        ]);

    protected static string PluginModuleCode => ChartJsPluginCode.EmptyDoughnut;

    private static PluginSampleChartJsConfig CreateConfig()
    {
        return new PluginSampleChartJsConfig
        {
            Type = ChartType.doughnut,
            Data = new ChartJsData
            {
                Labels = [],
                Datasets =
                [
                    new DoughnutDataset
                    {
                        Label = "Dataset 1",
                        Data = [],
                    },
                ],
            },
            Options = new PluginSampleChartJsOptions
            {
                Plugins = new PluginSamplePlugins
                {
                    EmptyDoughnut = new EmptyDoughnutOptions
                    {
                        Color = "rgba(255, 128, 0, 0.5)",
                        Width = 2,
                        RadiusDecrease = 20,
                    },
                },
            },
        };
    }
}
