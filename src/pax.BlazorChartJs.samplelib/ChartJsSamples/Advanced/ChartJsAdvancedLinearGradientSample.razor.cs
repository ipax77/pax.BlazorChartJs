using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedLinearGradientSample : ChartJsAdvancedLinearGradientSampleBase
{
}

public abstract class ChartJsAdvancedLinearGradientSampleBase : ChartJsDocsBaseComponent
{
    private const int DataCount = 7;
    private const int RandomMin = -100;
    private const int RandomMax = 100;

    private static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];

    private readonly ChartJsConfig config = CreateLinearGradientConfig();
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected ChartJsConfig Config => config;

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override void OnInitialized()
    {
        actions =
        [
            CreateAction("randomize", "Randomize", Randomize),
            CreateAction("add-data", "Add Data", AddData),
            CreateAction("remove-data", "Remove Data", RemoveData),
        ];
    }

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Legend = new Legend { Position = "top" },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(7),
                    BorderColor = ChartJsFunction.FromName("linearGradientBorderColor"),
                },
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));

        void AddData() => config.AddData(
            GetMonthLabel(config.Data.Labels.Count),
            null,
            CreateAddedData(config.Data.Datasets));

        void RemoveData()
        {
            if (config.Data.Labels.Count > 0)
            {
                config.RemoveData();
            }
        }
        """);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' }
            }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            {
              label: 'Dataset 1',
              data: Utils.numbers(NUMBER_CFG),
              borderColor(context) {
                const {ctx, chartArea} = context.chart;
                return chartArea ? getGradient(ctx, chartArea) : undefined;
              }
            }
          ]
        };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}); }); chart.update(); } },
          { name: 'Add Data', handler(chart) { chart.data.labels = Utils.months({count: chart.data.labels.length + 1}); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(-100, 100))); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);

    protected static string CallbacksCode { get; } =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const linearGradientCache = new WeakMap();

        function getLinearGradient(chart, chartArea) {
          const width = chartArea.right - chartArea.left;
          const height = chartArea.bottom - chartArea.top;
          const cached = linearGradientCache.get(chart);

          if (cached && cached.width === width && cached.height === height) {
            return cached.gradient;
          }

          const gradient = chart.ctx.createLinearGradient(0, chartArea.bottom, 0, chartArea.top);
          gradient.addColorStop(0, '#36a2eb');
          gradient.addColorStop(0.5, '#ffcd56');
          gradient.addColorStop(1, '#ff6384');
          linearGradientCache.set(chart, { width, height, gradient });
          return gradient;
        }

        const callbacks = Object.assign(Object.create(null), {
          linearGradientBorderColor(context) {
            const { chart } = context;
            if (!chart.chartArea) {
              return undefined;
            }

            return getLinearGradient(chart, chart.chartArea);
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;

    private static ChartJsConfig CreateLinearGradientConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    new LineDataset
                    {
                        Label = "Dataset 1",
                        Data = RandomNumbers(DataCount),
                        BorderColor = ChartJsFunction.FromName("linearGradientBorderColor"),
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Legend = new Legend { Position = "top" },
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
            data[dataset] = new SetDataObject(RandomNumbers(config.Data.Labels.Count));
        }

        config.SetData(data);
    }

    private void AddData()
    {
        var datasets = config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        Dictionary<ChartJsDataset, AddDataObject> data = new(datasets.Count);
        foreach (var dataset in datasets)
        {
            data[dataset] = new AddDataObject(NextValue());
        }

        config.AddData(GetMonthLabel(config.Data.Labels.Count), null, data);
    }

    private void RemoveData()
    {
        if (config.Data.Labels.Count > 0)
        {
            config.RemoveData();
        }
    }

    private static List<string> CreateMonthLabels(int count)
    {
        List<string> labels = new(count);

        for (var i = 0; i < count; i++)
        {
            labels.Add(GetMonthLabel(i));
        }

        return labels;
    }

    private static List<object> RandomNumbers(int count)
    {
        List<object> data = new(count);

        for (var i = 0; i < count; i++)
        {
            data.Add(NextValue());
        }

        return data;
    }

    private static string GetMonthLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }

    private static int NextValue()
    {
        return Random.Shared.Next(RandomMin, RandomMax + 1);
    }
}
