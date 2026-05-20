using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Scales;

public sealed partial class ChartJsScalesSample : ChartJsScalesSampleBase
{
}

public abstract class ChartJsScalesSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const int DataCount = 7;
    private const int PositiveMin = 0;
    private const int PositiveMax = 100;
    private const int LogMin = 1;
    private const int LogMax = 1000000;

    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Orange = "rgb(255, 159, 64)";
    private const string OrangeTransparent = "rgba(255, 159, 64, 0.5)";
    private const string Yellow = "rgb(255, 205, 86)";
    private const string Green = "rgb(75, 192, 192)";
    private const string GreenTransparent = "rgba(75, 192, 192, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Purple = "rgb(153, 102, 255)";
    private const string PurpleTransparent = "rgba(153, 102, 255, 0.5)";
    private const string Grey = "rgb(201, 203, 207)";
    private const string TimeAdapterModule = "./_content/pax.BlazorChartJs.samplelib/timeChart.js";

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

    private static readonly string[] ColorPalette = [Red, Orange, Yellow, Green, Blue, Purple, Grey];
    private static readonly string[] TransparentColorPalette = [RedTransparent, OrangeTransparent, "rgba(255, 205, 86, 0.5)", GreenTransparent, BlueTransparent, PurpleTransparent, "rgba(201, 203, 207, 0.5)"];
    private static readonly DateOnly TimeStart = new(2021, 11, 6);
    private static readonly Lazy<Dictionary<string, ScalesSampleDefinition>> Definitions = new(CreateDefinitions);

    private ChartJsConfig? config;
    private IReadOnlyList<ChartJsDocsAction> actions = [];
    private IJSObjectReference? timeAdapterModule;
    private bool loadingTimeAdapter;
    private bool timeAdapterReady;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public string SampleId { get; set; } = "linear-min-max";

    protected ScalesSampleDefinition ResolvedSample { get; private set; } = Definitions.Value["linear-min-max"];

    protected ChartJsConfig Config => config ?? throw new InvalidOperationException("Sample config has not been initialized.");

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    public static bool IsKnownSample(string sampleId)
    {
        return Definitions.Value.ContainsKey(sampleId);
    }

    protected override void OnInitialized()
    {
        ResolvedSample = Definitions.Value.TryGetValue(SampleId, out var definition)
            ? definition
            : Definitions.Value["linear-min-max"];
        config = ResolvedSample.RequiresTimeAdapter
            ? CreateTimeAdapterBootstrapConfig()
            : ResolvedSample.CreateConfig();
        actions = CreateActions(ResolvedSample.ActionSet);
    }

    protected async Task HandleChartEvent(ChartJsEvent chartJsEvent)
    {
        if (!ResolvedSample.RequiresTimeAdapter || timeAdapterReady || loadingTimeAdapter || chartJsEvent is not ChartJsInitEvent)
        {
            return;
        }

        loadingTimeAdapter = true;
        timeAdapterModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", TimeAdapterModule).ConfigureAwait(false);
        await timeAdapterModule.InvokeVoidAsync("registerPlugin").ConfigureAwait(false);
        CopyConfig(ResolvedSample.CreateConfig(), Config);
        timeAdapterReady = true;
        Config.ReinitializeChart();
    }

    public async ValueTask DisposeAsync()
    {
        if (timeAdapterModule is not null)
        {
            await timeAdapterModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    private ChartJsDocsAction[] CreateActions(ScalesActionSet actionSet)
    {
        return actionSet switch
        {
            ScalesActionSet.FullDataset => CreateFullDatasetActions(),
            ScalesActionSet.Randomize => [CreateAction("randomize", "Randomize", Randomize)],
            _ => [],
        };
    }

    private ChartJsDocsAction[] CreateFullDatasetActions()
    {
        return
        [
            CreateAction("randomize", "Randomize", Randomize),
            CreateAction("add-dataset", "Add Dataset", AddDataset),
            CreateAction("add-data", "Add Data", AddData),
            CreateAction("remove-dataset", "Remove Dataset", RemoveDataset),
            CreateAction("remove-data", "Remove Data", RemoveData),
        ];
    }

    private void Randomize()
    {
        var datasets = Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);

        for (var i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            data[dataset] = new SetDataObject(CreateRandomDataForDataset(dataset, i));
        }

        Config.SetData(data);
    }

    private void AddDataset()
    {
        var datasetIndex = Config.Data.Datasets.Count;
        var color = ColorPalette[datasetIndex % ColorPalette.Length];
        var transparentColor = TransparentColorPalette[datasetIndex % TransparentColorPalette.Length];

        Config.AddDataset(new LineDataset
        {
            Label = $"Dataset {datasetIndex + 1}",
            Data = RandomNumbers(Config.Data.Labels.Count, PositiveMin, PositiveMax),
            BorderColor = color,
            BackgroundColor = transparentColor,
        });
    }

    private void AddData()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        Dictionary<ChartJsDataset, AddDataObject> data = new(datasets.Count);
        for (var i = 0; i < datasets.Count; i++)
        {
            data[datasets[i]] = new AddDataObject(Random.Shared.Next(PositiveMin, PositiveMax + 1));
        }

        Config.AddData(GetMonthLabel(Config.Data.Labels.Count), null, data);
    }

    private void RemoveDataset()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count > 0)
        {
            Config.RemoveDataset(datasets[^1]);
        }
    }

    private void RemoveData()
    {
        if (Config.Data.Labels.Count > 0)
        {
            Config.RemoveData();
        }
    }

    private List<object> CreateRandomDataForDataset(ChartJsDataset dataset, int datasetIndex)
    {
        return ResolvedSample.Id switch
        {
            "log" => RandomLogScaleData(dataset.Data.Count),
            "time-line" when datasetIndex == 2 => TimePointData([60, 45, 75, 70]),
            "time-line" => RandomNumbers(Config.Data.Labels.Count, PositiveMin, PositiveMax),
            "time-max-span" => TimePointData(datasetIndex == 0 ? [60, 55, 80, 81] : [40, 45, 55, 75], datasetIndex == 0 ? [0, 2, 4, 6] : [0, 2, 5, 6]),
            "time-combo" => RandomNumbers(Config.Data.Labels.Count, PositiveMin, PositiveMax),
            _ => RandomNumbers(dataset.Data.Count, PositiveMin, PositiveMax),
        };
    }

    private static ChartJsConfig CreateLinearMinMaxConfig()
    {
        return CreateLineChart(
            "Min and Max Settings",
            [
                CreateLineDataset("Dataset 1", Red, RedTransparent, [10, 30, 50, 20, 25, 44, -10]),
                CreateLineDataset("Dataset 2", Blue, BlueTransparent, [100, 33, 22, 19, 11, 49, 30]),
            ],
            new ChartJsOptionsScales
            {
                Y = new LinearAxis { Min = 10, Max = 50 },
            });
    }

    private static ChartJsConfig CreateTimeAdapterBootstrapConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = ["Loading"],
                Datasets =
                [
                    CreateLineDataset("Loading", Grey, "rgba(201, 203, 207, 0.5)", [0]),
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Loading time adapter" },
                },
            },
        };
    }

    private static void CopyConfig(ChartJsConfig source, ChartJsConfig target)
    {
        target.Type = source.Type;
        target.Data = source.Data;
        target.Options = source.Options;
    }

    private static ChartJsConfig CreateLinearSuggestedConfig()
    {
        return CreateLineChart(
            "Suggested Min and Max Settings",
            [
                CreateLineDataset("Dataset 1", Red, RedTransparent, [10, 30, 39, 20, 25, 34, -10]),
                CreateLineDataset("Dataset 2", Blue, BlueTransparent, [18, 33, 22, 19, 11, 39, 30]),
            ],
            new ChartJsOptionsScales
            {
                Y = new LinearAxis { SuggestedMin = 30, SuggestedMax = 50 },
            });
    }

    private static ChartJsConfig CreateLinearStepSizeConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    CreateLineDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    CreateLineDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Interaction = new Interactions { Mode = "index", Intersect = false },
                Hover = new Interactions { Mode = "index", Intersect = false },
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Chart.js Line Chart" },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis { Title = new Title { Display = true, Text = "Month" } },
                    Y = new LinearAxis
                    {
                        Title = new Title { Display = true, Text = "Value" },
                        Min = 0,
                        Max = 100,
                        Ticks = new LinearAxisTick { StepSize = 50 },
                    },
                },
            },
        };
    }

    private static ChartJsConfig CreateLogConfig()
    {
        return CreateLineChart(
            "Chart.js Line Chart - Logarithmic",
            [
                CreateLineDataset("Dataset 1", Red, RedTransparent, LogScaleData()),
            ],
            new ChartJsOptionsScales
            {
                X = new CartesianAxis { Display = true },
                Y = new LinearAxis { Display = true, Type = "logarithmic" },
            });
    }

    private static ChartJsConfig CreateStackedConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    CreateLineDataset("Dataset 1", Red, RedTransparent, [10, 30, 50, 20, 25, 44, -10], yAxisId: "y"),
                    new LineDataset
                    {
                        Label = "Dataset 2",
                        Data = ["ON", "ON", "OFF", "ON", "OFF", "OFF", "ON"],
                        BorderColor = Blue,
                        BackgroundColor = BlueTransparent,
                        Stepped = true,
                        YAxisID = "y2",
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Stacked scales" },
                },
                Scales = new ChartJsOptionsScales
                {
                    Y = new CartesianAxis
                    {
                        Type = "linear",
                        Position = "left",
                        Stack = "demo",
                        StackWeight = 2,
                        Border = new ChartJsAxisBorder { Color = Red },
                    },
                    Y2 = new CartesianAxis
                    {
                        Type = "category",
                        Labels = ["ON", "OFF"],
                        Offset = true,
                        Position = "left",
                        Stack = "demo",
                        StackWeight = 1,
                        Border = new ChartJsAxisBorder { Color = Blue },
                    },
                },
            },
        };
    }

    private static ChartJsConfig CreateTimeLineConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateDateLabels(DataCount),
                Datasets =
                [
                    CreateLineDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    CreateLineDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    CreateLineDataset("Dataset 3", Green, GreenTransparent, TimePointData([60, 45, 75, 70])),
                ],
            },
            Options = CreateTimeLineOptions(),
        };
    }

    private static ChartJsConfig CreateTimeMaxSpanConfig()
    {
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Datasets =
                [
                    CreateLineDataset("Dataset 1", Red, RedTransparent, TimePointData([60, 55, 80, 81], [0, 2, 4, 6])),
                    CreateLineDataset("Dataset 2", Blue, BlueTransparent, TimePointData([40, 45, 55, 75], [0, 2, 5, 6])),
                ],
            },
            Options = CreateTimeMaxSpanOptions(),
        };

        if (config.Options.Scales?.X is TimeCartesianAxis timeAxis)
        {
            timeAxis.Ticks!.Font = ChartJsFunction.FromName("timeMaxSpanMajorTickFont");
        }

        return config;
    }

    private static ChartJsConfig CreateTimeComboConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateDateLabels(DataCount),
                Datasets =
                [
                    CreateBarDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    CreateBarDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    CreateLineDataset("Dataset 3", Green, GreenTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax), type: ChartType.line),
                ],
            },
            Options = CreateTimeComboOptions(),
        };
    }

    private static ChartJsOptions CreateTimeLineOptions()
    {
        return new ChartJsOptions
        {
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Time Scale" },
            },
            Scales = new ChartJsOptionsScales
            {
                X = new TimeCartesianAxis
                {
                    Type = "time",
                    Time = new TimeCartesianAxisTime
                    {
                        TooltipFormat = "PP p",
                    },
                    Title = new Title { Display = true, Text = "Date" },
                },
                Y = new LinearAxis
                {
                    Title = new Title { Display = true, Text = "value" },
                },
            },
        };
    }

    private static ChartJsOptions CreateTimeMaxSpanOptions()
    {
        return new ChartJsOptions
        {
            SpanGaps = 172800000,
            Responsive = true,
            Interaction = new Interactions { Mode = "nearest" },
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Time - spanGaps: 172800000 (2 days in ms)" },
            },
            Scales = new ChartJsOptionsScales
            {
                X = new TimeCartesianAxis
                {
                    Type = "time",
                    Display = true,
                    Title = new Title { Display = true, Text = "Date" },
                    Ticks = new TimeCartesianAxisTicks
                    {
                        AutoSkip = false,
                        MaxRotation = 0,
                        Major = new { enabled = true },
                    },
                },
                Y = new LinearAxis
                {
                    Display = true,
                    Title = new Title { Display = true, Text = "value" },
                },
            },
        };
    }

    private static ChartJsOptions CreateTimeComboOptions()
    {
        return new ChartJsOptions
        {
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Combo Time Scale" },
            },
            Scales = new ChartJsOptionsScales
            {
                X = new TimeCartesianAxis
                {
                    Type = "time",
                    Display = true,
                    Offset = true,
                    Ticks = new TimeCartesianAxisTicks { Source = "data" },
                    Time = new TimeCartesianAxisTime { Unit = "day" },
                },
            },
        };
    }

    private static ChartJsConfig CreateLineChart(string title, IList<ChartJsDataset> datasets, ChartJsOptionsScales scales)
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets = datasets,
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = title },
                },
                Scales = scales,
            },
        };
    }

    private static LineDataset CreateLineDataset(
        string label,
        string borderColor,
        string backgroundColor,
        IList<object> data,
        string? yAxisId = null,
        ChartType? type = null)
    {
        return new LineDataset
        {
            Type = type,
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            YAxisID = yAxisId,
        };
    }

    private static BarDataset CreateBarDataset(string label, string borderColor, string backgroundColor, IList<object> data)
    {
        return new BarDataset
        {
            Type = ChartType.bar,
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
        };
    }

    private static List<object> RandomNumbers(int count, int min, int max)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(min, max + 1));
        }

        return data;
    }

    private static List<object> LogScaleData()
    {
        return [1, 10, 100, 1000, 10000, 100000, 1000000];
    }

    private static List<object> RandomLogScaleData(int count)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(LogMin, LogMax + 1));
        }

        return data;
    }

    private static List<object> TimePointData(IReadOnlyList<int> values)
    {
        return TimePointData(values, [0, 5, 7, 15]);
    }

    private static List<object> TimePointData(IReadOnlyList<int> values, IReadOnlyList<int> dayOffsets)
    {
        List<object> data = new(Math.Min(values.Count, dayOffsets.Count));
        var pointCount = Math.Min(values.Count, dayOffsets.Count);
        for (var i = 0; i < pointCount; i++)
        {
            data.Add(new { x = GetDateLabel(dayOffsets[i]), y = values[i] });
        }

        return data;
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

    private static List<string> CreateDateLabels(int count)
    {
        List<string> labels = new(count);
        for (var i = 0; i < count; i++)
        {
            labels.Add(GetDateLabel(i));
        }

        return labels;
    }

    private static string GetMonthLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }

    private static string GetDateLabel(int dayOffset)
    {
        return TimeStart.AddDays(dayOffset).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static Dictionary<string, ScalesSampleDefinition> CreateDefinitions()
    {
        return new Dictionary<string, ScalesSampleDefinition>(StringComparer.Ordinal)
        {
            ["linear-min-max"] = new("linear-min-max", "Linear Scale - Min-Max", "https://www.chartjs.org/docs/latest/samples/scales/linear-min-max.html", ScalesActionSet.None, false, CreateLinearMinMaxConfig, LinearMinMaxCSharp, LinearMinMaxJavaScript),
            ["linear-min-max-suggested"] = new("linear-min-max-suggested", "Linear Scale - Suggested Min-Max", "https://www.chartjs.org/docs/latest/samples/scales/linear-min-max-suggested.html", ScalesActionSet.None, false, CreateLinearSuggestedConfig, LinearSuggestedCSharp, LinearSuggestedJavaScript),
            ["linear-step-size"] = new("linear-step-size", "Linear Scale - Step Size", "https://www.chartjs.org/docs/latest/samples/scales/linear-step-size.html", ScalesActionSet.FullDataset, false, CreateLinearStepSizeConfig, LinearStepSizeCSharp, LinearStepSizeJavaScript),
            ["log"] = new("log", "Log Scale", "https://www.chartjs.org/docs/latest/samples/scales/log.html", ScalesActionSet.Randomize, false, CreateLogConfig, LogCSharp, LogJavaScript),
            ["stacked"] = new("stacked", "Stacked Linear / Category", "https://www.chartjs.org/docs/latest/samples/scales/stacked.html", ScalesActionSet.None, false, CreateStackedConfig, StackedCSharp, StackedJavaScript),
            ["time-line"] = new("time-line", "Time Scale", "https://www.chartjs.org/docs/latest/samples/scales/time-line.html", ScalesActionSet.Randomize, true, CreateTimeLineConfig, TimeLineCSharp, TimeLineJavaScript),
            ["time-max-span"] = new("time-max-span", "Time Scale - Max Span", "https://www.chartjs.org/docs/latest/samples/scales/time-max-span.html", ScalesActionSet.Randomize, true, CreateTimeMaxSpanConfig, TimeMaxSpanCSharp, TimeMaxSpanJavaScript, TimeCallbacksCode),
            ["time-combo"] = new("time-combo", "Time Scale - Combo Chart", "https://www.chartjs.org/docs/latest/samples/scales/time-combo.html", ScalesActionSet.Randomize, true, CreateTimeComboConfig, TimeComboCSharp, TimeComboJavaScript),
        };
    }

    private static readonly ChartJsDocsCodeSet LinearMinMaxCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Min and Max Settings" } },
                Scales = new ChartJsOptionsScales { Y = new LinearAxis { Min = 10, Max = 50 } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = [10, 30, 50, 20, 25, 44, -10], BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = [100, 33, 22, 19, 11, 49, 30], BorderColor = Blue, BackgroundColor = BlueTransparent },
            ],
        };
        """,
        "No actions are defined for this official sample.");

    private static readonly ChartJsDocsCodeSet LinearMinMaxJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Min and Max Settings' } },
            scales: { y: { min: 10, max: 50 } }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: [10, 30, 50, 20, 25, 44, -10], borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Dataset 2', data: [100, 33, 22, 19, 11, 49, 30], borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
          ]
        };
        """,
        "const actions = [];");

    private static readonly ChartJsDocsCodeSet LinearSuggestedCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Suggested Min and Max Settings" } },
                Scales = new ChartJsOptionsScales { Y = new LinearAxis { SuggestedMin = 30, SuggestedMax = 50 } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = [10, 30, 39, 20, 25, 34, -10], BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = [18, 33, 22, 19, 11, 39, 30], BorderColor = Blue, BackgroundColor = BlueTransparent },
            ],
        };
        """,
        "No actions are defined for this official sample.");

    private static readonly ChartJsDocsCodeSet LinearSuggestedJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Suggested Min and Max Settings' } },
            scales: { y: { suggestedMin: 30, suggestedMax: 50 } }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: [10, 30, 39, 20, 25, 34, -10], borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Dataset 2', data: [18, 33, 22, 19, 11, 39, 30], borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
          ]
        };
        """,
        "const actions = [];");

    private static readonly ChartJsDocsCodeSet LinearStepSizeCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Interaction = new Interactions { Mode = "index", Intersect = false },
                Hover = new Interactions { Mode = "index", Intersect = false },
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Line Chart" } },
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis { Title = new Title { Display = true, Text = "Month" } },
                    Y = new LinearAxis { Title = new Title { Display = true, Text = "Value" }, Min = 0, Max = 100, Ticks = new LinearAxisTick { StepSize = 50 } },
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
                new LineDataset { Label = "Dataset 1", Data = RandomNumbers(7, 0, 100), BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = RandomNumbers(7, 0, 100), BorderColor = Blue, BackgroundColor = BlueTransparent },
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void AddDataset() => config.AddDataset(CreateLineDataset(config.Data.Labels.Count));
        void AddData() => config.AddData(nextMonth, null, valuesByDataset);
        void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);
        void RemoveData() => config.RemoveData();
        """);

    private static readonly ChartJsDocsCodeSet LinearStepSizeJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            interaction: { mode: 'index', intersect: false },
            hover: { mode: 'index', intersect: false },
            plugins: { title: { display: true, text: 'Chart.js Line Chart' } },
            scales: { x: { title: { display: true, text: 'Month' } }, y: { title: { display: true, text: 'Value' }, min: 0, max: 100, ticks: { stepSize: 50 } } }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = { labels, datasets: [
          { label: 'Dataset 1', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
          { label: 'Dataset 2', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
        ] };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: 0, max: 100}); }); chart.update(); } },
          { name: 'Add Dataset', handler(chart) { const color = Utils.namedColor(chart.data.datasets.length); chart.data.datasets.push({ label: 'Dataset ' + (chart.data.datasets.length + 1), data: Utils.numbers({count: chart.data.labels.length, min: 0, max: 100}), borderColor: color, backgroundColor: Utils.transparentize(color, 0.5) }); chart.update(); } },
          { name: 'Add Data', handler(chart) { chart.data.labels = Utils.months({count: chart.data.labels.length + 1}); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(0, 100))); chart.update(); } },
          { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);

    private static readonly ChartJsDocsCodeSet LogCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Line Chart - Logarithmic" } },
                Scales = new ChartJsOptionsScales { X = new CartesianAxis { Display = true }, Y = new LinearAxis { Type = "logarithmic", Display = true } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets = [new LineDataset { Label = "Dataset 1", Data = [1, 10, 100, 1000, 10000, 100000, 1000000], BorderColor = Red, BackgroundColor = RedTransparent }],
        };
        """,
        "void Randomize() => config.SetData(CreateRandomLogScaleData(config.Data.Datasets));");

    private static readonly ChartJsDocsCodeSet LogJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Chart.js Line Chart - Logarithmic' } },
            scales: { x: { display: true }, y: { display: true, type: 'logarithmic' } }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = { labels, datasets: [{ label: 'Dataset 1', data: [1, 10, 100, 1000, 10000, 100000, 1000000], borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) }] };
        """,
        "const actions = [{ name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: dataset.data.length, min: 1, max: 1000000}); }); chart.update(); } }];");

    private static readonly ChartJsDocsCodeSet StackedCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Stacked scales" } },
                Scales = new ChartJsOptionsScales
                {
                    Y = new CartesianAxis { Type = "linear", Position = "left", Stack = "demo", StackWeight = 2, Border = new ChartJsAxisBorder { Color = Red } },
                    Y2 = new CartesianAxis { Type = "category", Labels = ["ON", "OFF"], Offset = true, Position = "left", Stack = "demo", StackWeight = 1, Border = new ChartJsAxisBorder { Color = Blue } },
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
                new LineDataset { Label = "Dataset 1", Data = [10, 30, 50, 20, 25, 44, -10], BorderColor = Red, BackgroundColor = RedTransparent, YAxisID = "y" },
                new LineDataset { Label = "Dataset 2", Data = ["ON", "ON", "OFF", "ON", "OFF", "OFF", "ON"], BorderColor = Blue, BackgroundColor = BlueTransparent, Stepped = true, YAxisID = "y2" },
            ],
        };
        """,
        "No actions are defined for this official sample.");

    private static readonly ChartJsDocsCodeSet StackedJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Stacked scales' } },
            scales: {
              y: { type: 'linear', position: 'left', stack: 'demo', stackWeight: 2, border: { color: Utils.CHART_COLORS.red } },
              y2: { type: 'category', labels: ['ON', 'OFF'], offset: true, position: 'left', stack: 'demo', stackWeight: 1, border: { color: Utils.CHART_COLORS.blue } }
            }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = { labels, datasets: [
          { label: 'Dataset 1', data: [10, 30, 50, 20, 25, 44, -10], borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5), yAxisID: 'y' },
          { label: 'Dataset 2', data: ['ON', 'ON', 'OFF', 'ON', 'OFF', 'OFF', 'ON'], borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5), stepped: true, yAxisID: 'y2' }
        ] };
        """,
        "const actions = [];");

    private static readonly ChartJsDocsCodeSet TimeLineCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Time Scale" } },
                Scales = new ChartJsOptionsScales
                {
                    X = new TimeCartesianAxis
                    {
                        Type = "time",
                        Time = new TimeCartesianAxisTime { TooltipFormat = "PP p" },
                        Title = new Title { Display = true, Text = "Date" },
                    },
                    Y = new LinearAxis { Title = new Title { Display = true, Text = "value" } },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateDateLabels(7),
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = RandomNumbers(7, 0, 100), BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = RandomNumbers(7, 0, 100), BorderColor = Blue, BackgroundColor = BlueTransparent },
                new LineDataset { Label = "Dataset 3", Data = TimePointData([60, 45, 75, 70]), BorderColor = Green, BackgroundColor = GreenTransparent },
            ],
        };
        """,
        "void Randomize() => config.SetData(CreateRandomTimeData(config.Data.Datasets));");

    private static readonly ChartJsDocsCodeSet TimeLineJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            plugins: { title: { display: true, text: 'Chart.js Time Scale' } },
            scales: {
              x: { type: 'time', time: { tooltipFormat: 'DD T' }, title: { display: true, text: 'Date' } },
              y: { title: { display: true, text: 'value' } }
            }
          }
        };
        """,
        """
        const labels = Utils.newDateString(7);
        const data = { labels, datasets: [
          { label: 'Dataset 1', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
          { label: 'Dataset 2', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) },
          { label: 'Dataset 3', data: [{x: '2021-11-06', y: 60}, {x: '2021-11-11', y: 45}, {x: '2021-11-13', y: 75}, {x: '2021-11-21', y: 70}], borderColor: Utils.CHART_COLORS.green, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.green, 0.5) }
        ] };
        """,
        "const actions = [{ name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: dataset.data.length, min: 0, max: 100}); }); chart.update(); } }];");

    private static readonly ChartJsDocsCodeSet TimeMaxSpanCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                SpanGaps = 172800000,
                Responsive = true,
                Interaction = new Interactions { Mode = "nearest" },
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Time - spanGaps: 172800000 (2 days in ms)" } },
                Scales = new ChartJsOptionsScales
                {
                    X = new TimeCartesianAxis
                    {
                        Type = "time",
                        Display = true,
                        Title = new Title { Display = true, Text = "Date" },
                        Ticks = new TimeCartesianAxisTicks
                        {
                            AutoSkip = false,
                            MaxRotation = 0,
                            Major = new { enabled = true },
                            Font = ChartJsFunction.FromName("timeMaxSpanMajorTickFont"),
                        },
                    },
                    Y = new LinearAxis
                    {
                        Display = true,
                        Title = new Title { Display = true, Text = "value" },
                    },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = TimePointData([60, 55, 80, 81], [0, 2, 4, 6]), BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = TimePointData([40, 45, 55, 75], [0, 2, 5, 6]), BorderColor = Blue, BackgroundColor = BlueTransparent },
            ],
        };
        """,
        "void Randomize() => config.SetData(CreateRandomTimeData(config.Data.Datasets));");

    private static readonly ChartJsDocsCodeSet TimeMaxSpanJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            spanGaps: 172800000,
            responsive: true,
            interaction: { mode: 'nearest' },
            plugins: { title: { display: true, text: 'Chart.js Time - spanGaps: 172800000 (2 days in ms)' } },
            scales: {
              x: {
                type: 'time',
                display: true,
                title: { display: true, text: 'Date' },
                ticks: {
                  autoSkip: false,
                  maxRotation: 0,
                  major: { enabled: true },
                  font: context => context.tick && context.tick.major ? { weight: 'bold' } : undefined
                }
              },
              y: { display: true, title: { display: true, text: 'value' } }
            }
          }
        };
        """,
        """
        const data = { datasets: [
          { label: 'Dataset 1', data: [{x: '2021-11-06', y: 60}, {x: '2021-11-08', y: 55}, {x: '2021-11-10', y: 80}, {x: '2021-11-12', y: 81}], borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
          { label: 'Dataset 2', data: [{x: '2021-11-06', y: 40}, {x: '2021-11-08', y: 45}, {x: '2021-11-11', y: 55}, {x: '2021-11-12', y: 75}], borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
        ] };
        """,
        "const actions = [{ name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data.forEach(point => { point.y = Utils.rand(0, 100); }); }); chart.update(); } }];");

    private static readonly ChartJsDocsCodeSet TimeComboCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Combo Time Scale" } },
                Scales = new ChartJsOptionsScales
                {
                    X = new TimeCartesianAxis
                    {
                        Type = "time",
                        Display = true,
                        Offset = true,
                        Ticks = new TimeCartesianAxisTicks { Source = "data" },
                        Time = new TimeCartesianAxisTime { Unit = "day" },
                    },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateDateLabels(7),
            Datasets =
            [
                new BarDataset { Type = ChartType.bar, Label = "Dataset 1", Data = RandomNumbers(7, 0, 100), BorderColor = Red, BackgroundColor = RedTransparent },
                new BarDataset { Type = ChartType.bar, Label = "Dataset 2", Data = RandomNumbers(7, 0, 100), BorderColor = Blue, BackgroundColor = BlueTransparent },
                new LineDataset { Type = ChartType.line, Label = "Dataset 3", Data = RandomNumbers(7, 0, 100), BorderColor = Green, BackgroundColor = GreenTransparent },
            ],
        };
        """,
        "void Randomize() => config.SetData(CreateRandomTimeData(config.Data.Datasets));");

    private static readonly ChartJsDocsCodeSet TimeComboJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            plugins: { title: { display: true, text: 'Chart.js Combo Time Scale' } },
            scales: {
              x: {
                type: 'time',
                display: true,
                offset: true,
                ticks: { source: 'data' },
                time: { unit: 'day' }
              }
            }
          }
        };
        """,
        """
        const labels = Utils.newDateString(7);
        const data = { labels, datasets: [
          { type: 'bar', label: 'Dataset 1', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
          { type: 'bar', label: 'Dataset 2', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) },
          { type: 'line', label: 'Dataset 3', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.green, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.green, 0.5) }
        ] };
        """,
        "const actions = [{ name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: 0, max: 100}); }); chart.update(); } }];");

    private const string TimeCallbacksCode =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          timeMaxSpanMajorTickFont(context) {
            return context.tick && context.tick.major
              ? { weight: 'bold' }
              : undefined;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}

#pragma warning disable CA1054, CA1056
public sealed record ScalesSampleDefinition(
    string Id,
    string Title,
    string DocsUrl,
    ScalesActionSet ActionSet,
    bool RequiresTimeAdapter,
    Func<ChartJsConfig> CreateConfig,
    ChartJsDocsCodeSet CSharpCode,
    ChartJsDocsCodeSet JavaScriptCode,
    string? CallbacksCode = null);
#pragma warning restore CA1054, CA1056

public enum ScalesActionSet
{
    None,
    Randomize,
    FullDataset,
}
