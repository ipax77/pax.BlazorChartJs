using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedRadialGradientSample : ChartJsAdvancedRadialGradientSampleBase
{
}

public abstract class ChartJsAdvancedRadialGradientSampleBase : ChartJsDocsBaseComponent
{
    private const int DataCount = 5;
    private const int RandomMax = 100;

    private static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
    ];

    private readonly ChartJsConfig config = CreateRadialGradientConfig();
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected ChartJsConfig Config => config;

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override void OnInitialized()
    {
        actions =
        [
            CreateAction("randomize", "Randomize", Randomize),
        ];
    }

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                "create-radial-gradient-3",
                "createRadialGradient3",
                """
                // The gradient helper lives in chartJsCallbacks.js so it can return
                // CanvasGradient values without an extra Blazor interop round trip.
                BackgroundColor = ChartJsFunction.FromName("radialGradientArcBackgroundColor");
                """),
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new ChartJsConfig
                {
                    Type = ChartType.polarArea,
                    Data = data,
                    Options = new ChartJsOptions
                    {
                        Plugins = new Plugins
                        {
                            Legend = new Legend { Display = false },
                            Tooltip = new Tooltip { Enabled = false },
                        },
                        Elements = new ChartJsElementsOptions
                        {
                            Arc = new ChartJsArcElementOptions
                            {
                                BackgroundColor = ChartJsFunction.FromName("radialGradientArcBackgroundColor"),
                            },
                        },
                    },
                };
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                var data = new ChartJsData
                {
                    Labels = ["January", "February", "March", "April", "May"],
                    Datasets =
                    [
                        new PolarAreaDataset
                        {
                            Data = GenerateData(),
                        },
                    ],
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                const int DataCount = 5;

                static List<object> GenerateData()
                {
                    List<object> data = new(DataCount);
                    for (var i = 0; i < DataCount; i++)
                    {
                        data.Add(Random.Shared.Next(0, 101));
                    }

                    return data;
                }
                """),
            new ChartJsDocsCodeTab(
                ActionsTab,
                "Actions",
                """
                void Randomize()
                {
                    Dictionary<ChartJsDataset, SetDataObject> data = new(config.Data.Datasets.Count);
                    foreach (var dataset in config.Data.Datasets)
                    {
                        data[dataset] = new SetDataObject(GenerateData());
                    }

                    config.SetData(data);
                }
                """),
        ]);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                "create-radial-gradient-3",
                "createRadialGradient3",
                """
                function createRadialGradient3(context, c1, c2, c3) {
                  const chartArea = context.chart.chartArea;
                  if (!chartArea) {
                    return;
                  }

                  const chartWidth = chartArea.right - chartArea.left;
                  const chartHeight = chartArea.bottom - chartArea.top;
                  if (width !== chartWidth || height !== chartHeight) {
                    cache.clear();
                  }

                  let gradient = cache.get(c1 + c2 + c3);
                  if (!gradient) {
                    width = chartWidth;
                    height = chartHeight;
                    const centerX = (chartArea.left + chartArea.right) / 2;
                    const centerY = (chartArea.top + chartArea.bottom) / 2;
                    const r = Math.min(chartWidth / 2, chartHeight / 2);
                    gradient = context.chart.ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, r);
                    gradient.addColorStop(0, c1);
                    gradient.addColorStop(0.5, c2);
                    gradient.addColorStop(1, c3);
                    cache.set(c1 + c2 + c3, gradient);
                  }

                  return gradient;
                }
                """),
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                const config = {
                  type: 'polarArea',
                  data,
                  options: {
                    plugins: { legend: false, tooltip: false },
                    elements: {
                      arc: {
                        backgroundColor(context) {
                          let c = colors[context.dataIndex];
                          if (!c) {
                            return;
                          }
                          if (context.active) {
                            c = helpers.getHoverColor(c);
                          }

                          const mid = helpers.color(c).desaturate(0.2).darken(0.2).rgbString();
                          const start = helpers.color(c).lighten(0.2).rotate(270).rgbString();
                          const end = helpers.color(c).lighten(0.1).rgbString();
                          return createRadialGradient3(context, start, mid, end);
                        }
                      }
                    }
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                "data",
                "Data",
                """
                function generateData() {
                  return Utils.numbers({count: DATA_COUNT, min: 0, max: 100});
                }

                const data = {
                  labels: Utils.months({count: DATA_COUNT}),
                  datasets: [{data: generateData()}]
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                const DATA_COUNT = 5;
                Utils.srand(110);
                const chartColors = Utils.CHART_COLORS;
                const colors = [
                  chartColors.red,
                  chartColors.orange,
                  chartColors.yellow,
                  chartColors.green,
                  chartColors.blue
                ];
                const cache = new Map();
                let width = null;
                let height = null;
                """),
            new ChartJsDocsCodeTab(
                ActionsTab,
                "Actions",
                """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach(dataset => {
                        dataset.data = generateData();
                      });
                      chart.update();
                    }
                  }
                ];
                """),
        ]);

    protected static string CallbacksCode { get; } =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const radialGradientCache = new WeakMap();
        const radialGradientColors = ['#ff6384', '#ff9f40', '#ffcd56', '#4bc0c0', '#36a2eb'];

        function createRadialGradient3(context, c1, c2, c3) {
          const { chart } = context;
          const { chartArea } = chart;
          if (!chartArea) {
            return undefined;
          }

          const width = chartArea.right - chartArea.left;
          const height = chartArea.bottom - chartArea.top;
          let cached = radialGradientCache.get(chart);
          if (!cached || cached.width !== width || cached.height !== height) {
            cached = { width, height, gradients: new Map() };
            radialGradientCache.set(chart, cached);
          }

          const cacheKey = `${c1}|${c2}|${c3}`;
          let gradient = cached.gradients.get(cacheKey);
          if (!gradient) {
            const centerX = (chartArea.left + chartArea.right) / 2;
            const centerY = (chartArea.top + chartArea.bottom) / 2;
            const radius = Math.min(width / 2, height / 2);
            gradient = chart.ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, radius);
            gradient.addColorStop(0, c1);
            gradient.addColorStop(0.5, c2);
            gradient.addColorStop(1, c3);
            cached.gradients.set(cacheKey, gradient);
          }

          return gradient;
        }

        const callbacks = Object.assign(Object.create(null), {
          radialGradientArcBackgroundColor(context) {
            const helpers = Chart.helpers;
            let color = radialGradientColors[context.dataIndex];
            if (!color || !helpers?.color) {
              return undefined;
            }

            if (context.active && helpers.getHoverColor) {
              color = helpers.getHoverColor(color);
            }

            const mid = helpers.color(color).desaturate(0.2).darken(0.2).rgbString();
            const start = helpers.color(color).lighten(0.2).rotate(270).rgbString();
            const end = helpers.color(color).lighten(0.1).rgbString();
            return createRadialGradient3(context, start, mid, end);
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;

    private static ChartJsConfig CreateRadialGradientConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.polarArea,
            Data = new ChartJsData
            {
                Labels = [.. MonthLabels],
                Datasets =
                [
                    new PolarAreaDataset
                    {
                        Data = GenerateData(),
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Legend = new Legend { Display = false },
                    Tooltip = new Tooltip { Enabled = false },
                },
                Elements = new ChartJsElementsOptions
                {
                    Arc = new ChartJsArcElementOptions
                    {
                        BackgroundColor = ChartJsFunction.FromName("radialGradientArcBackgroundColor"),
                    },
                },
            },
        };
    }

    private void Randomize()
    {
        var datasets = config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            data[dataset] = new SetDataObject(GenerateData());
        }

        config.SetData(data);
    }

    private static List<object> GenerateData()
    {
        List<object> data = new(DataCount);

        for (var i = 0; i < DataCount; i++)
        {
            data.Add(Random.Shared.Next(0, RandomMax + 1));
        }

        return data;
    }
}
