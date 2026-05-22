using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedProgressBarSample : ChartJsAdvancedProgressBarSampleBase
{
}

public abstract class ChartJsAdvancedProgressBarSampleBase : ChartJsDocsBaseComponent
{
    private const int DataCount = 7;
    private const int RandomMin = -100;
    private const int RandomMax = 100;
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Yellow = "rgb(255, 205, 86)";
    private const string YellowTransparent = "rgba(255, 205, 86, 0.5)";
    private const string Green = "rgb(75, 192, 192)";
    private const string GreenTransparent = "rgba(75, 192, 192, 0.5)";
    private const string Purple = "rgb(153, 102, 255)";
    private const string PurpleTransparent = "rgba(153, 102, 255, 0.5)";
    private const string Orange = "rgb(255, 159, 64)";
    private const string OrangeTransparent = "rgba(255, 159, 64, 0.5)";

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

    private static readonly (string Border, string Background)[] DatasetColors =
    [
        (Red, RedTransparent),
        (Blue, BlueTransparent),
        (Yellow, YellowTransparent),
        (Green, GreenTransparent),
        (Purple, PurpleTransparent),
        (Orange, OrangeTransparent),
    ];

    private readonly ChartJsConfig config = CreateProgressBarConfig();
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected ChartJsConfig Config => config;

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override void OnInitialized()
    {
        actions =
        [
            CreateAction("randomize", "Randomize", Randomize),
            CreateAction("add-dataset", "Add Dataset", AddDataset),
            CreateAction("add-data", "Add Data", AddData),
            CreateAction("remove-dataset", "Remove Dataset", RemoveDataset),
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
                Animation = new Animation
                {
                    Duration = 2000,
                    OnProgress = ChartJsFunction.FromName("animationProgressBarProgress"),
                    OnComplete = ChartJsFunction.FromName("animationProgressBarComplete"),
                },
                Interaction = new Interactions
                {
                    Mode = "nearest",
                    Axis = "x",
                    Intersect = false,
                },
                Plugins = new Plugins
                {
                    Title = new Title
                    {
                        Display = true,
                        Text = "Chart.js Line Chart - Animation Progress Bar",
                    },
                },
            },
        };
        """,
        """
        <label for="initialProgress">Initial animation</label>
        <progress id="initialProgress" max="1" value="0"></progress>
        <label for="animationProgress">Other animations</label>
        <progress id="animationProgress" max="1" value="0"></progress>

        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets =
            [
                CreateDataset("Dataset 1", 0, 7),
                CreateDataset("Dataset 2", 1, 7),
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));

        void AddDataset() => config.AddDataset(CreateDataset(
            $"Dataset {config.Data.Datasets.Count + 1}",
            config.Data.Datasets.Count,
            config.Data.Labels.Count));

        void AddData() => config.AddData(
            GetMonthLabel(config.Data.Labels.Count),
            null,
            CreateAddedData(config.Data.Datasets));

        void RemoveDataset()
        {
            if (config.Data.Datasets.Count > 0)
            {
                config.RemoveDataset(config.Data.Datasets[^1]);
            }
        }

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
            animation: {
              duration: 2000,
              onProgress(context) {
                if (context.initial) {
                  initProgress.value = context.currentStep / context.numSteps;
                } else {
                  progress.value = context.currentStep / context.numSteps;
                }
              },
              onComplete(context) {
                console.log(context.initial ? 'Initial animation finished' : 'animation finished');
              }
            },
            interaction: { mode: 'nearest', axis: 'x', intersect: false },
            plugins: {
              title: { display: true, text: 'Chart.js Line Chart - Animation Progress Bar' }
            }
          }
        };
        """,
        """
        const initProgress = document.getElementById('initialProgress');
        const progress = document.getElementById('animationProgress');
        const labels = Utils.months({count: 7});
        const data = {
          labels,
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
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}); }); chart.update(); } },
          { name: 'Add Dataset', handler(chart) { chart.data.datasets.push({ label: 'Dataset ' + (chart.data.datasets.length + 1), data: Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}) }); chart.update(); } },
          { name: 'Add Data', handler(chart) { chart.data.labels = Utils.months({count: chart.data.labels.length + 1}); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(-100, 100))); chart.update(); } },
          { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);

    protected static string CallbacksCode { get; } =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const animationProgressBarCache = new WeakMap();

        function getAnimationProgressBars(chart) {
          let progressBars = animationProgressBarCache.get(chart);
          if (progressBars) {
            return progressBars;
          }

          const sample = chart.canvas.closest('[data-chartjs-sample="progress-bar"]');
          progressBars = {
            initial: sample?.querySelector('#initialProgress') ?? null,
            update: sample?.querySelector('#animationProgress') ?? null
          };
          animationProgressBarCache.set(chart, progressBars);
          return progressBars;
        }

        const callbacks = Object.assign(Object.create(null), {
          animationProgressBarProgress(context) {
            const progressBars = getAnimationProgressBars(context.chart);
            const progress = context.initial ? progressBars.initial : progressBars.update;
            if (progress instanceof HTMLProgressElement) {
              progress.value = context.currentStep / context.numSteps;
            }
          },
          animationProgressBarComplete(context) {
            console.log(context.initial ? 'Initial animation finished' : 'animation finished');
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;

    private static ChartJsConfig CreateProgressBarConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    CreateDataset("Dataset 1", 0, DataCount),
                    CreateDataset("Dataset 2", 1, DataCount),
                ],
            },
            Options = new ChartJsOptions
            {
                Animation = new Animation
                {
                    Duration = 2000,
                    OnProgress = ChartJsFunction.FromName("animationProgressBarProgress"),
                    OnComplete = ChartJsFunction.FromName("animationProgressBarComplete"),
                },
                Interaction = new Interactions
                {
                    Mode = "nearest",
                    Axis = "x",
                    Intersect = false,
                },
                Plugins = new Plugins
                {
                    Title = new Title
                    {
                        Display = true,
                        Text = "Chart.js Line Chart - Animation Progress Bar",
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
            data[dataset] = new SetDataObject(RandomNumbers(config.Data.Labels.Count));
        }

        config.SetData(data);
    }

    private void AddDataset()
    {
        config.AddDataset(CreateDataset(
            $"Dataset {config.Data.Datasets.Count + 1}",
            config.Data.Datasets.Count,
            config.Data.Labels.Count));
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

    private void RemoveDataset()
    {
        if (config.Data.Datasets.Count > 0)
        {
            config.RemoveDataset(config.Data.Datasets[^1]);
        }
    }

    private void RemoveData()
    {
        if (config.Data.Labels.Count > 0)
        {
            config.RemoveData();
        }
    }

    private static LineDataset CreateDataset(string label, int datasetIndex, int dataCount)
    {
        var colors = DatasetColors[datasetIndex % DatasetColors.Length];
        return new LineDataset
        {
            Label = label,
            Data = RandomNumbers(dataCount),
            BorderColor = colors.Border,
            BackgroundColor = colors.Background,
        };
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
