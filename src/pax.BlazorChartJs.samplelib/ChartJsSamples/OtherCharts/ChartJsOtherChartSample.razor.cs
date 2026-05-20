using Microsoft.AspNetCore.Components;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.OtherCharts;

public sealed partial class ChartJsOtherChartSample : ChartJsOtherChartSampleBase
{
}

public abstract class ChartJsOtherChartSampleBase : ChartJsDocsBaseComponent
{
    private const int DataCount = 7;
    private const int SliceCount = 5;
    private const int PositiveMin = 0;
    private const int PositiveMax = 100;
    private const int MixedMin = -100;
    private const int MixedMax = 100;
    private const int BubbleMinRadius = 5;
    private const int BubbleMaxRadius = 15;
    private const int ScatterRadius = 1;

    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Orange = "rgb(255, 159, 64)";
    private const string OrangeTransparent = "rgba(255, 159, 64, 0.5)";
    private const string Yellow = "rgb(255, 205, 86)";
    private const string YellowTransparent = "rgba(255, 205, 86, 0.5)";
    private const string Green = "rgb(75, 192, 192)";
    private const string GreenTransparent = "rgba(75, 192, 192, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Purple = "rgb(153, 102, 255)";
    private const string PurpleTransparent = "rgba(153, 102, 255, 0.5)";
    private const string Grey = "rgb(201, 203, 207)";
    private const string GreyTransparent = "rgba(201, 203, 207, 0.5)";

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

    private static readonly string[] SliceLabels = ["Red", "Orange", "Yellow", "Green", "Blue"];

    private static readonly string[] ColorPalette = [Red, Orange, Yellow, Green, Blue, Purple, Grey];

    private static readonly string[] TransparentColorPalette =
    [
        RedTransparent,
        OrangeTransparent,
        YellowTransparent,
        GreenTransparent,
        BlueTransparent,
        PurpleTransparent,
        GreyTransparent,
    ];

    private static readonly Dictionary<string, OtherChartSampleDefinition> Definitions = CreateDefinitions();

    private ChartComponent? chartComponent;
    private ChartJsConfig? config;
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    [Parameter]
    public string SampleId { get; set; } = "bubble";

    protected OtherChartSampleDefinition ResolvedSample { get; private set; } = Definitions["bubble"];

    protected ChartJsConfig Config => config ?? throw new InvalidOperationException("Sample config has not been initialized.");

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    public static bool IsKnownSample(string sampleId)
    {
        return Definitions.ContainsKey(sampleId);
    }

    protected override void OnInitialized()
    {
        ResolvedSample = Definitions.TryGetValue(SampleId, out var definition)
            ? definition
            : Definitions["bubble"];
        config = ResolvedSample.CreateConfig();
        actions = CreateActions(ResolvedSample.ActionSet);
    }

    protected Task CaptureChartComponent(ChartComponent component)
    {
        chartComponent = component;
        return Task.CompletedTask;
    }

    private ChartJsDocsAction[] CreateActions(OtherChartActionSet actionSet)
    {
        return actionSet switch
        {
            OtherChartActionSet.None => [],
            OtherChartActionSet.RandomizeOnly => [CreateAction("randomize", "Randomize", Randomize)],
            OtherChartActionSet.RadialData => CreateRadialDataActions(),
            OtherChartActionSet.Pie => CreatePieActions(includeVisibilityActions: false),
            OtherChartActionSet.Doughnut => CreatePieActions(includeVisibilityActions: true),
            OtherChartActionSet.FullDatasets => CreateFullDatasetActions(),
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

    private ChartJsDocsAction[] CreateRadialDataActions()
    {
        return
        [
            CreateAction("randomize", "Randomize", Randomize),
            CreateAction("add-data", "Add Data", AddData),
            CreateAction("remove-data", "Remove Data", RemoveData),
        ];
    }

    private ChartJsDocsAction[] CreatePieActions(bool includeVisibilityActions)
    {
        List<ChartJsDocsAction> pieActions = new(includeVisibilityActions ? 9 : 5)
        {
            CreateAction("randomize", "Randomize", Randomize),
            CreateAction("add-dataset", "Add Dataset", AddDataset),
            CreateAction("add-data", "Add Data", AddData),
        };

        if (includeVisibilityActions)
        {
            pieActions.Add(CreateAction("hide-dataset-0", "Hide(0)", HideDatasetZero));
            pieActions.Add(CreateAction("show-dataset-0", "Show(0)", ShowDatasetZero));
            pieActions.Add(CreateAction("hide-data-0-1", "Hide (0, 1)", HideDatasetZeroDataOne));
            pieActions.Add(CreateAction("show-data-0-1", "Show (0, 1)", ShowDatasetZeroDataOne));
        }

        pieActions.Add(CreateAction("remove-dataset", "Remove Dataset", RemoveDataset));
        pieActions.Add(CreateAction("remove-data", "Remove Data", RemoveData));

        return [.. pieActions];
    }

    private void Randomize()
    {
        Dictionary<ChartJsDataset, SetDataObject> data = new(Config.Data.Datasets.Count);

        for (var i = 0; i < Config.Data.Datasets.Count; i++)
        {
            var dataset = Config.Data.Datasets[i];
            data[dataset] = new SetDataObject(CreateRandomDataForDataset(dataset, i, Config.Data.Labels.Count));
        }

        Config.SetData(data);
    }

    private void AddDataset()
    {
        Config.AddDataset(CreateDatasetForCurrentSample(Config.Data.Datasets.Count));
    }

    private void AddData()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        Dictionary<ChartJsDataset, AddDataObject> data = new(datasets.Count);
        var label = ResolvedSample.DataMode switch
        {
            OtherChartDataMode.CartesianLabeled => GetMonthLabel(Config.Data.Labels.Count),
            OtherChartDataMode.Radial => $"data #{Config.Data.Labels.Count + 1}",
            _ => null,
        };

        for (var i = 0; i < datasets.Count; i++)
        {
            data[datasets[i]] = new AddDataObject(CreateRandomPointForDataset(datasets[i]), null, GetTransparentColor(Config.Data.Labels.Count), null);
        }

        Config.AddData(label, null, data);
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
        if (Config.Data.Datasets.Count == 0)
        {
            return;
        }

        Config.RemoveData();
    }

    private async Task HideDatasetZero()
    {
        if (chartComponent is not null && Config.Data.Datasets.Count > 0)
        {
            await chartComponent.HideDataset(Config.Data.Datasets[0], null).ConfigureAwait(false);
        }
    }

    private async Task ShowDatasetZero()
    {
        if (chartComponent is not null && Config.Data.Datasets.Count > 0)
        {
            await chartComponent.ShowDataset(Config.Data.Datasets[0], null).ConfigureAwait(false);
        }
    }

    private async Task HideDatasetZeroDataOne()
    {
        if (chartComponent is not null && Config.Data.Datasets.Count > 0)
        {
            await chartComponent.HideDataset(Config.Data.Datasets[0], 1).ConfigureAwait(false);
        }
    }

    private async Task ShowDatasetZeroDataOne()
    {
        if (chartComponent is not null && Config.Data.Datasets.Count > 0)
        {
            await chartComponent.ShowDataset(Config.Data.Datasets[0], 1).ConfigureAwait(false);
        }
    }

    private ChartJsDataset CreateDatasetForCurrentSample(int datasetIndex)
    {
        var label = $"Dataset {datasetIndex + 1}";
        var color = GetColor(datasetIndex);
        var transparentColor = GetTransparentColor(datasetIndex);
        var labelCount = Config.Data.Labels.Count;

        return ResolvedSample.Id switch
        {
            "bubble" => CreateBubbleDataset(label, color, transparentColor, CreateBubbleData(DataCount, PositiveMin, PositiveMax, BubbleMinRadius, BubbleMaxRadius)),
            "scatter" => CreateScatterDataset(label, color, transparentColor, CreateBubbleData(DataCount, PositiveMin, PositiveMax, ScatterRadius, ScatterRadius)),
            "scatter-multi-axis" => CreateScatterDataset(label, color, transparentColor, CreateBubbleData(DataCount, MixedMin, MixedMax, ScatterRadius, ScatterRadius)),
            "radar" => CreateRadarDataset(label, color, transparentColor, RandomNumbers(labelCount, PositiveMin, PositiveMax)),
            "combo-bar-line" => CreateBarDataset(label, color, transparentColor, RandomNumbers(labelCount, MixedMin, MixedMax), borderWidth: 1),
            "stacked-bar-line" => CreateLineDataset(label, color, transparentColor, RandomNumbers(labelCount, PositiveMin, PositiveMax), stack: "combined", borderWidth: 1),
            "pie" => CreatePieDataset(label, RandomNumbers(labelCount, PositiveMin, PositiveMax), CreateIndexedColors(labelCount, transparent: false)),
            "doughnut" => CreateDoughnutDataset(label, RandomNumbers(labelCount, PositiveMin, PositiveMax), CreateIndexedColors(labelCount, transparent: false)),
            _ => CreateBarDataset(label, color, transparentColor, RandomNumbers(labelCount, MixedMin, MixedMax), borderWidth: 1),
        };
    }

    private IList<object> CreateRandomDataForDataset(ChartJsDataset dataset, int datasetIndex, int labelCount)
    {
        return ResolvedSample.Id switch
        {
            "bubble" => CreateBubbleData(DataCount, PositiveMin, PositiveMax, BubbleMinRadius, BubbleMaxRadius),
            "scatter" => CreateBubbleData(DataCount, PositiveMin, PositiveMax, ScatterRadius, ScatterRadius),
            "scatter-multi-axis" => CreateBubbleData(DataCount, MixedMin, MixedMax, ScatterRadius, ScatterRadius),
            "radar-skip-points" => CreateRadarSkipData(datasetIndex, labelCount),
            "combo-bar-line" => RandomNumbers(labelCount, MixedMin, MixedMax),
            "stacked-bar-line" or "radar" or "pie" or "doughnut" or "polar-area" or "polar-area-center-labels" =>
                RandomNumbers(labelCount, PositiveMin, PositiveMax),
            _ => dataset.Data,
        };
    }

    private object CreateRandomPointForDataset(ChartJsDataset dataset)
    {
        return ResolvedSample.DataMode switch
        {
            OtherChartDataMode.CartesianUnlabeled when dataset is BubbleDataset =>
                CreateBubblePoint(PositiveMin, PositiveMax, BubbleMinRadius, BubbleMaxRadius),
            OtherChartDataMode.CartesianUnlabeled when ResolvedSample.Id == "scatter-multi-axis" =>
                CreateBubblePoint(MixedMin, MixedMax, ScatterRadius, ScatterRadius),
            OtherChartDataMode.CartesianUnlabeled =>
                CreateBubblePoint(PositiveMin, PositiveMax, ScatterRadius, ScatterRadius),
            OtherChartDataMode.Radial or OtherChartDataMode.CartesianLabeled =>
                Random.Shared.Next(ResolvedSample.Id == "combo-bar-line" ? MixedMin : PositiveMin, ResolvedSample.Id == "combo-bar-line" ? MixedMax : PositiveMax),
            _ => Random.Shared.Next(PositiveMin, PositiveMax),
        };
    }

    private static ChartJsConfig CreateChart(
        ChartType type,
        string title,
        IList<ChartJsDataset> datasets,
        IList<string>? labels = null,
        string? legendPosition = "top",
        ChartJsOptionsScales? scales = null)
    {
        return new ChartJsConfig
        {
            Type = type,
            Data = new ChartJsData
            {
                Labels = labels ?? [],
                Datasets = datasets,
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Scales = scales,
                Plugins = new Plugins
                {
                    Legend = legendPosition is null ? null : new Legend { Position = legendPosition },
                    Title = new Title { Display = true, Text = title },
                },
            },
        };
    }

    private static ChartJsConfig CreateMultiSeriesPie()
    {
        var config = CreateChart(
            ChartType.pie,
            String.Empty,
            [
                CreatePieDataset(null, [21, 79], ["#AAA", "#777"]),
                CreatePieDataset(null, [33, 67], ["hsl(0, 100%, 60%)", "hsl(0, 100%, 35%)"]),
                CreatePieDataset(null, [20, 80], ["hsl(100, 100%, 60%)", "hsl(100, 100%, 35%)"]),
                CreatePieDataset(null, [10, 90], ["hsl(180, 100%, 60%)", "hsl(180, 100%, 35%)"]),
            ],
            ["Overall Yay", "Overall Nay", "Group A Yay", "Group A Nay", "Group B Yay", "Group B Nay", "Group C Yay", "Group C Nay"],
            legendPosition: null);

        config.Options!.Plugins!.Legend = new Legend
        {
            Labels = new Labels { GenerateLabels = ChartJsFunction.FromName("multiSeriesPieGenerateLabels") },
            OnClick = ChartJsFunction.FromName("multiSeriesPieLegendClick"),
        };
        config.Options.Plugins.Tooltip = new Tooltip
        {
            Callbacks = new TooltipCallbacks { Title = ChartJsFunction.FromName("multiSeriesPieTooltipTitle") },
        };

        return config;
    }

    private static ChartJsConfig CreatePolarArea(bool centeredPointLabels)
    {
        var scales = centeredPointLabels
            ? new ChartJsOptionsScales
            {
                R = new LinearRadialAxis
                {
                    PointLabels = new PointLabels
                    {
                        Display = true,
                        CenterPointLabels = true,
                        Font = new Font { Size = 18 },
                    },
                },
            }
            : null;

        return CreateChart(
            ChartType.polarArea,
            centeredPointLabels ? "Chart.js Polar Area Chart With Centered Point Labels" : "Chart.js Polar Area Chart",
            [CreatePolarAreaDataset("Dataset 1", RandomNumbers(SliceCount, PositiveMin, PositiveMax), CreateIndexedColors(SliceCount, transparent: true))],
            [.. SliceLabels],
            scales: scales);
    }

    private static ChartJsConfig CreateScatterMultiAxis()
    {
        return CreateChart(
            ChartType.scatter,
            "Chart.js Scatter Multi Axis Chart",
            [
                CreateScatterDataset("Dataset 1", Red, RedTransparent, CreateBubbleData(DataCount, MixedMin, MixedMax, ScatterRadius, ScatterRadius), yAxisId: "y"),
                CreateScatterDataset("Dataset 2", Orange, OrangeTransparent, CreateBubbleData(DataCount, MixedMin, MixedMax, ScatterRadius, ScatterRadius), yAxisId: "y2"),
            ],
            scales: new ChartJsOptionsScales
            {
                Y = new CartesianAxis
                {
                    Type = "linear",
                    Axis = "y",
                    Position = "left",
                    Ticks = new ChartJsAxisTick { Color = Red },
                },
                Y2 = new CartesianAxis
                {
                    Type = "linear",
                    Axis = "y",
                    Position = "right",
                    Reverse = true,
                    Ticks = new ChartJsAxisTick { Color = Blue },
                    Grid = new ChartJsGrid { DrawOnChartArea = false },
                },
            });
    }

    private static ChartJsConfig CreateStackedBarLine()
    {
        return CreateChart(
            ChartType.line,
            "Chart.js Stacked Line/Bar Chart",
            [
                CreateBarDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax), stack: "combined", type: ChartType.bar),
                CreateLineDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax), stack: "combined"),
            ],
            MonthLabels[..DataCount],
            legendPosition: null,
            scales: new ChartJsOptionsScales { Y = new ChartJsAxis { Stacked = true } });
    }

    private static BarDataset CreateBarDataset(
        string? label,
        string borderColor,
        string backgroundColor,
        IList<object> data,
        string? stack = null,
        int? borderWidth = null,
        ChartType? type = null)
    {
        return new BarDataset
        {
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            BorderWidth = borderWidth.HasValue ? new IndexableOption<double>(borderWidth.Value) : null,
            Stack = stack,
            Type = type,
        };
    }

    private static LineDataset CreateLineDataset(
        string? label,
        string borderColor,
        string backgroundColor,
        IList<object> data,
        string? stack = null,
        int? borderWidth = null,
        ChartType? type = null)
    {
        return new LineDataset
        {
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            BorderWidth = borderWidth.HasValue ? new IndexableOption<double>(borderWidth.Value) : null,
            Stack = stack,
            Type = type,
        };
    }

    private static RadarDataset CreateRadarDataset(string label, string borderColor, string backgroundColor, IList<object> data)
    {
        return new RadarDataset
        {
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
        };
    }

    private static BubbleDataset CreateBubbleDataset(string label, string borderColor, string backgroundColor, IList<object> data)
    {
        return new BubbleDataset
        {
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
        };
    }

    private static ScatterDataset CreateScatterDataset(string label, string borderColor, string backgroundColor, IList<object> data, string? yAxisId = null)
    {
        return new ScatterDataset
        {
            Label = label,
            Data = data,
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            YAxisID = yAxisId,
        };
    }

    private static PieDataset CreatePieDataset(string? label, IList<object> data, IList<string> backgroundColor)
    {
        return new PieDataset
        {
            Label = label,
            Data = data,
            BackgroundColor = new IndexableOption<string>(backgroundColor),
        };
    }

    private static DoughnutDataset CreateDoughnutDataset(string label, IList<object> data, IList<string> backgroundColor)
    {
        return new DoughnutDataset
        {
            Label = label,
            Data = data,
            BackgroundColor = new IndexableOption<string>(backgroundColor),
        };
    }

    private static PolarAreaDataset CreatePolarAreaDataset(string label, IList<object> data, IList<string> backgroundColor)
    {
        return new PolarAreaDataset
        {
            Label = label,
            Data = data,
            BackgroundColor = new IndexableOption<string>(backgroundColor),
        };
    }

    private static List<object> RandomNumbers(int count, int min, int max)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(min, max));
        }

        return data;
    }

    private static List<object> CreateBubbleData(int count, int min, int max, int minRadius, int maxRadius)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(CreateBubblePoint(min, max, minRadius, maxRadius));
        }

        return data;
    }

    private static BubbleDataPoint CreateBubblePoint(int min, int max, int minRadius, int maxRadius)
    {
        return new BubbleDataPoint
        {
            X = Random.Shared.Next(min, max),
            Y = Random.Shared.Next(min, max),
            R = Random.Shared.Next(minRadius, maxRadius + 1),
        };
    }

    private static List<object> CreateRadarSkipData(int datasetIndex, int count)
    {
        var data = RandomNumbers(count, PositiveMin, PositiveMax);
        if (data.Count == 0)
        {
            return data;
        }

        var skipIndex = datasetIndex switch
        {
            0 => 0,
            1 => data.Count / 2,
            _ => data.Count - 1,
        };
        data[skipIndex] = null!;

        return data;
    }

    private static List<string> CreateIndexedColors(int count, bool transparent)
    {
        List<string> colors = new(count);
        for (var i = 0; i < count; i++)
        {
            colors.Add(transparent ? GetTransparentColor(i) : GetColor(i));
        }

        return colors;
    }

    private static string GetColor(int index)
    {
        return ColorPalette[index % ColorPalette.Length];
    }

    private static string GetTransparentColor(int index)
    {
        return TransparentColorPalette[index % TransparentColorPalette.Length];
    }

    private static string GetMonthLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }

    private static Dictionary<string, OtherChartSampleDefinition> CreateDefinitions()
    {
        return new Dictionary<string, OtherChartSampleDefinition>(StringComparer.Ordinal)
        {
            ["bubble"] = new(
                "bubble",
                "Bubble",
                "https://www.chartjs.org/docs/latest/samples/other-charts/bubble.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianUnlabeled,
                () => CreateChart(
                    ChartType.bubble,
                    "Chart.js Bubble Chart",
                    [
                        CreateBubbleDataset("Dataset 1", Red, RedTransparent, CreateBubbleData(DataCount, PositiveMin, PositiveMax, BubbleMinRadius, BubbleMaxRadius)),
                        CreateBubbleDataset("Dataset 2", Orange, OrangeTransparent, CreateBubbleData(DataCount, PositiveMin, PositiveMax, BubbleMinRadius, BubbleMaxRadius)),
                    ]),
                CreateCSharpCode("ChartType.bubble", "Chart.js Bubble Chart", CSharpBubbleSetup, OtherChartActionSet.FullDatasets),
                CreateJavaScriptCode("bubble", "Chart.js Bubble Chart", JavaScriptBubbleSetup, OtherChartActionSet.FullDatasets)),
            ["combo-bar-line"] = new(
                "combo-bar-line",
                "Combo bar/line",
                "https://www.chartjs.org/docs/latest/samples/other-charts/combo-bar-line.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianLabeled,
                () => CreateChart(
                    ChartType.bar,
                    "Chart.js Combined Line/Bar Chart",
                    [
                        CreateBarDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, MixedMin, MixedMax), type: null),
                        CreateLineDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, MixedMin, MixedMax), type: ChartType.line),
                    ],
                    MonthLabels[..DataCount]),
                CreateCSharpCode("ChartType.bar", "Chart.js Combined Line/Bar Chart", CSharpComboSetup, OtherChartActionSet.FullDatasets),
                CreateJavaScriptCode("bar", "Chart.js Combined Line/Bar Chart", JavaScriptComboSetup, OtherChartActionSet.FullDatasets)),
            ["doughnut"] = new(
                "doughnut",
                "Doughnut",
                "https://www.chartjs.org/docs/latest/samples/other-charts/doughnut.html",
                OtherChartActionSet.Doughnut,
                OtherChartDataMode.Radial,
                () => CreateChart(
                    ChartType.doughnut,
                    "Chart.js Doughnut Chart",
                    [CreateDoughnutDataset("Dataset 1", RandomNumbers(SliceCount, PositiveMin, PositiveMax), CreateIndexedColors(SliceCount, transparent: false))],
                    [.. SliceLabels]),
                CreateCSharpCode("ChartType.doughnut", "Chart.js Doughnut Chart", CSharpDoughnutSetup, OtherChartActionSet.Doughnut),
                CreateJavaScriptCode("doughnut", "Chart.js Doughnut Chart", JavaScriptDoughnutSetup, OtherChartActionSet.Doughnut)),
            ["multi-series-pie"] = new(
                "multi-series-pie",
                "Multi Series Pie",
                "https://www.chartjs.org/docs/latest/samples/other-charts/multi-series-pie.html",
                OtherChartActionSet.None,
                OtherChartDataMode.Radial,
                CreateMultiSeriesPie,
                CreateCSharpCode("ChartType.pie", "Multi Series Pie", CSharpMultiSeriesPieSetup, OtherChartActionSet.None, CSharpMultiSeriesPieConfigOptions),
                CreateJavaScriptCode("pie", "Multi Series Pie", JavaScriptMultiSeriesPieSetup, OtherChartActionSet.None, JavaScriptMultiSeriesPieConfigOptions),
                MultiSeriesPieCallbacks),
            ["pie"] = new(
                "pie",
                "Pie",
                "https://www.chartjs.org/docs/latest/samples/other-charts/pie.html",
                OtherChartActionSet.Pie,
                OtherChartDataMode.Radial,
                () => CreateChart(
                    ChartType.pie,
                    "Chart.js Pie Chart",
                    [CreatePieDataset("Dataset 1", RandomNumbers(SliceCount, PositiveMin, PositiveMax), CreateIndexedColors(SliceCount, transparent: false))],
                    [.. SliceLabels]),
                CreateCSharpCode("ChartType.pie", "Chart.js Pie Chart", CSharpPieSetup, OtherChartActionSet.Pie),
                CreateJavaScriptCode("pie", "Chart.js Pie Chart", JavaScriptPieSetup, OtherChartActionSet.Pie)),
            ["polar-area"] = new(
                "polar-area",
                "Polar area",
                "https://www.chartjs.org/docs/latest/samples/other-charts/polar-area.html",
                OtherChartActionSet.RadialData,
                OtherChartDataMode.Radial,
                () => CreatePolarArea(centeredPointLabels: false),
                CreateCSharpCode("ChartType.polarArea", "Chart.js Polar Area Chart", CSharpPolarAreaSetup, OtherChartActionSet.RadialData),
                CreateJavaScriptCode("polarArea", "Chart.js Polar Area Chart", JavaScriptPolarAreaSetup, OtherChartActionSet.RadialData)),
            ["polar-area-center-labels"] = new(
                "polar-area-center-labels",
                "Polar area centered point labels",
                "https://www.chartjs.org/docs/latest/samples/other-charts/polar-area-center-labels.html",
                OtherChartActionSet.RadialData,
                OtherChartDataMode.Radial,
                () => CreatePolarArea(centeredPointLabels: true),
                CreateCSharpCode("ChartType.polarArea", "Chart.js Polar Area Chart With Centered Point Labels", CSharpPolarAreaSetup, OtherChartActionSet.RadialData, CSharpPolarAreaCenteredConfigOptions),
                CreateJavaScriptCode("polarArea", "Chart.js Polar Area Chart With Centered Point Labels", JavaScriptPolarAreaSetup, OtherChartActionSet.RadialData, JavaScriptPolarAreaCenteredConfigOptions)),
            ["radar"] = new(
                "radar",
                "Radar",
                "https://www.chartjs.org/docs/latest/samples/other-charts/radar.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianLabeled,
                () => CreateChart(
                    ChartType.radar,
                    "Chart.js Radar Chart",
                    [
                        CreateRadarDataset("Dataset 1", Red, RedTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                        CreateRadarDataset("Dataset 2", Blue, BlueTransparent, RandomNumbers(DataCount, PositiveMin, PositiveMax)),
                    ],
                    MonthLabels[..DataCount],
                    legendPosition: null),
                CreateCSharpCode("ChartType.radar", "Chart.js Radar Chart", CSharpRadarSetup, OtherChartActionSet.FullDatasets),
                CreateJavaScriptCode("radar", "Chart.js Radar Chart", JavaScriptRadarSetup, OtherChartActionSet.FullDatasets)),
            ["radar-skip-points"] = new(
                "radar-skip-points",
                "Radar skip points",
                "https://www.chartjs.org/docs/latest/samples/other-charts/radar-skip-points.html",
                OtherChartActionSet.RandomizeOnly,
                OtherChartDataMode.CartesianLabeled,
                () => CreateChart(
                    ChartType.radar,
                    "Chart.js Radar Skip Points Chart",
                    [
                        CreateRadarDataset("Skip first dataset", Red, RedTransparent, CreateRadarSkipData(0, DataCount)),
                        CreateRadarDataset("Skip mid dataset", Blue, BlueTransparent, CreateRadarSkipData(1, DataCount)),
                        CreateRadarDataset("Skip last dataset", Green, GreenTransparent, CreateRadarSkipData(2, DataCount)),
                    ],
                    MonthLabels[..DataCount],
                    legendPosition: null),
                CreateCSharpCode("ChartType.radar", "Chart.js Radar Skip Points Chart", CSharpRadarSkipSetup, OtherChartActionSet.RandomizeOnly),
                CreateJavaScriptCode("radar", "Chart.js Radar Skip Points Chart", JavaScriptRadarSkipSetup, OtherChartActionSet.RandomizeOnly)),
            ["scatter"] = new(
                "scatter",
                "Scatter",
                "https://www.chartjs.org/docs/latest/samples/other-charts/scatter.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianUnlabeled,
                () => CreateChart(
                    ChartType.scatter,
                    "Chart.js Scatter Chart",
                    [
                        CreateScatterDataset("Dataset 1", Red, RedTransparent, CreateBubbleData(DataCount, PositiveMin, PositiveMax, ScatterRadius, ScatterRadius)),
                        CreateScatterDataset("Dataset 2", Orange, OrangeTransparent, CreateBubbleData(DataCount, PositiveMin, PositiveMax, ScatterRadius, ScatterRadius)),
                    ]),
                CreateCSharpCode("ChartType.scatter", "Chart.js Scatter Chart", CSharpScatterSetup, OtherChartActionSet.FullDatasets),
                CreateJavaScriptCode("scatter", "Chart.js Scatter Chart", JavaScriptScatterSetup, OtherChartActionSet.FullDatasets)),
            ["scatter-multi-axis"] = new(
                "scatter-multi-axis",
                "Scatter - Multi axis",
                "https://www.chartjs.org/docs/latest/samples/other-charts/scatter-multi-axis.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianUnlabeled,
                CreateScatterMultiAxis,
                CreateCSharpCode("ChartType.scatter", "Chart.js Scatter Multi Axis Chart", CSharpScatterMultiAxisSetup, OtherChartActionSet.FullDatasets, CSharpScatterMultiAxisConfigOptions),
                CreateJavaScriptCode("scatter", "Chart.js Scatter Multi Axis Chart", JavaScriptScatterMultiAxisSetup, OtherChartActionSet.FullDatasets, JavaScriptScatterMultiAxisConfigOptions)),
            ["stacked-bar-line"] = new(
                "stacked-bar-line",
                "Stacked bar/line",
                "https://www.chartjs.org/docs/latest/samples/other-charts/stacked-bar-line.html",
                OtherChartActionSet.FullDatasets,
                OtherChartDataMode.CartesianLabeled,
                CreateStackedBarLine,
                CreateCSharpCode("ChartType.line", "Chart.js Stacked Line/Bar Chart", CSharpStackedBarLineSetup, OtherChartActionSet.FullDatasets, CSharpStackedBarLineConfigOptions),
                CreateJavaScriptCode("line", "Chart.js Stacked Line/Bar Chart", JavaScriptStackedBarLineSetup, OtherChartActionSet.FullDatasets, JavaScriptStackedBarLineConfigOptions)),
        };
    }

    private static ChartJsDocsCodeSet CreateCSharpCode(
        string chartType,
        string title,
        string setup,
        OtherChartActionSet actionSet,
        string? optionsOverride = null)
    {
        var options = optionsOverride ?? $$"""
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Legend = new Legend { Position = "top" },
                    Title = new Title { Display = true, Text = "{{title}}" },
                },
            },
        """;

        return new ChartJsDocsCodeSet(
            $$"""
            var config = new ChartJsConfig
            {
                Type = {{chartType}},
                Data = data,
                {{options}}
            };
            """,
            setup,
            GetCSharpActions(actionSet));
    }

    private static ChartJsDocsCodeSet CreateJavaScriptCode(
        string chartType,
        string title,
        string setup,
        OtherChartActionSet actionSet,
        string? optionsOverride = null)
    {
        var options = optionsOverride ?? $$"""
            options: {
              responsive: true,
              plugins: {
                legend: { position: 'top' },
                title: { display: true, text: '{{title}}' }
              }
            }
        """;

        return new ChartJsDocsCodeSet(
            $$"""
            const config = {
              type: '{{chartType}}',
              data,
              {{options}}
            };
            """,
            setup,
            GetJavaScriptActions(actionSet));
    }

    private static string GetCSharpActions(OtherChartActionSet actionSet)
    {
        return actionSet switch
        {
            OtherChartActionSet.None => """
                // This official sample has no actions.
                """,
            OtherChartActionSet.RandomizeOnly => """
                void Randomize()
                {
                    var data = new Dictionary<ChartJsDataset, SetDataObject>(config.Data.Datasets.Count);

                    for (var i = 0; i < config.Data.Datasets.Count; i++)
                    {
                        data[config.Data.Datasets[i]] = new SetDataObject(CreateRadarSkipData(i, config.Data.Labels.Count));
                    }

                    config.SetData(data);
                }
                """,
            OtherChartActionSet.RadialData => """
                void Randomize()
                {
                    var data = new Dictionary<ChartJsDataset, SetDataObject>(config.Data.Datasets.Count);

                    foreach (var dataset in config.Data.Datasets)
                    {
                        data[dataset] = new SetDataObject(RandomNumbers(config.Data.Labels.Count, 0, 100));
                    }

                    config.SetData(data);
                }

                void AddData()
                {
                    var values = config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new AddDataObject(Random.Shared.Next(0, 100), null, NextColor(), null));

                    config.AddData($"data #{config.Data.Labels.Count + 1}", null, values);
                }

                void RemoveData() => config.RemoveData();
                """,
            OtherChartActionSet.Pie => """
                void Randomize()
                {
                    config.SetData(config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new SetDataObject(RandomNumbers(config.Data.Labels.Count, 0, 100))));
                }

                void AddDataset() => config.AddDataset(CreatePieDataset($"Dataset {config.Data.Datasets.Count + 1}", config.Data.Labels.Count));

                void AddData()
                {
                    var values = config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new AddDataObject(Random.Shared.Next(0, 100), null, NextColor(), null));

                    config.AddData($"data #{config.Data.Labels.Count + 1}", null, values);
                }

                void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);

                void RemoveData() => config.RemoveData();
                """,
            OtherChartActionSet.Doughnut => """
                ChartComponent? chartComponent;

                void Randomize()
                {
                    config.SetData(config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new SetDataObject(RandomNumbers(config.Data.Labels.Count, 0, 100))));
                }

                void AddDataset() => config.AddDataset(CreateDoughnutDataset($"Dataset {config.Data.Datasets.Count + 1}", config.Data.Labels.Count));

                void AddData()
                {
                    var values = config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new AddDataObject(Random.Shared.Next(0, 100), null, NextColor(), null));

                    config.AddData($"data #{config.Data.Labels.Count + 1}", null, values);
                }

                Task Hide0() => chartComponent?.HideDataset(config.Data.Datasets[0], null).AsTask() ?? Task.CompletedTask;

                Task Show0() => chartComponent?.ShowDataset(config.Data.Datasets[0], null).AsTask() ?? Task.CompletedTask;

                Task Hide01() => chartComponent?.HideDataset(config.Data.Datasets[0], 1).AsTask() ?? Task.CompletedTask;

                Task Show01() => chartComponent?.ShowDataset(config.Data.Datasets[0], 1).AsTask() ?? Task.CompletedTask;

                void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);

                void RemoveData() => config.RemoveData();
                """,
            _ => """
                void Randomize()
                {
                    config.SetData(config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new SetDataObject(CreateRandomData(dataset, config.Data.Labels.Count))));
                }

                void AddDataset() => config.AddDataset(CreateNextDataset(config.Data.Datasets.Count, config.Data.Labels.Count));

                void AddData()
                {
                    var values = config.Data.Datasets.ToDictionary(
                        dataset => dataset,
                        dataset => new AddDataObject(CreateRandomPoint(dataset)));

                    config.AddData(GetNextLabelOrNull(), null, values);
                }

                void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);

                void RemoveData() => config.RemoveData();
                """,
        };
    }

    private static string GetJavaScriptActions(OtherChartActionSet actionSet)
    {
        return actionSet switch
        {
            OtherChartActionSet.None => """
                // This official sample has no actions.
                """,
            OtherChartActionSet.RandomizeOnly => """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach((dataset, datasetIndex) => {
                        dataset.data = generateSkipPointData(datasetIndex, chart.data.labels.length);
                      });
                      chart.update();
                    }
                  }
                ];
                """,
            OtherChartActionSet.RadialData => """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach(dataset => {
                        dataset.data = Utils.numbers({count: chart.data.labels.length, min: 0, max: 100});
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Data',
                    handler(chart) {
                      chart.data.labels.push('data #' + (chart.data.labels.length + 1));
                      chart.data.datasets.forEach(dataset => {
                        dataset.data.push(Utils.rand(0, 100));
                        dataset.backgroundColor.push(Utils.transparentize(Utils.namedColor(chart.data.labels.length - 1), 0.5));
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Remove Data',
                    handler(chart) {
                      chart.data.labels.splice(-1, 1);
                      chart.data.datasets.forEach(dataset => dataset.data.pop());
                      chart.update();
                    }
                  }
                ];
                """,
            OtherChartActionSet.Pie => """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach(dataset => {
                        dataset.data = Utils.numbers({count: chart.data.labels.length, min: 0, max: 100});
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Dataset',
                    handler(chart) {
                      chart.data.datasets.push({
                        label: 'Dataset ' + (chart.data.datasets.length + 1),
                        backgroundColor: chart.data.labels.map((_, i) => Utils.namedColor(i)),
                        data: Utils.numbers({count: chart.data.labels.length, min: 0, max: 100})
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Data',
                    handler(chart) {
                      chart.data.labels.push('data #' + (chart.data.labels.length + 1));
                      chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(0, 100)));
                      chart.update();
                    }
                  },
                  { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
                  { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
                ];
                """,
            OtherChartActionSet.Doughnut => """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach(dataset => {
                        dataset.data = Utils.numbers({count: chart.data.labels.length, min: 0, max: 100});
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Dataset',
                    handler(chart) {
                      chart.data.datasets.push({
                        label: 'Dataset ' + (chart.data.datasets.length + 1),
                        backgroundColor: chart.data.labels.map((_, i) => Utils.namedColor(i)),
                        data: Utils.numbers({count: chart.data.labels.length, min: 0, max: 100})
                      });
                      chart.update();
                    }
                  },
                  { name: 'Add Data', handler(chart) { chart.data.labels.push('data #' + (chart.data.labels.length + 1)); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(0, 100))); chart.update(); } },
                  { name: 'Hide(0)', handler(chart) { chart.hide(0); } },
                  { name: 'Show(0)', handler(chart) { chart.show(0); } },
                  { name: 'Hide (0, 1)', handler(chart) { chart.hide(0, 1); } },
                  { name: 'Show (0, 1)', handler(chart) { chart.show(0, 1); } },
                  { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
                  { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
                ];
                """,
            _ => """
                const actions = [
                  {
                    name: 'Randomize',
                    handler(chart) {
                      chart.data.datasets.forEach(dataset => {
                        dataset.data = createRandomDataForDataset(dataset, chart.data.labels.length);
                      });
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Dataset',
                    handler(chart) {
                      chart.data.datasets.push(createNextDataset(chart.data.datasets.length, chart.data.labels.length));
                      chart.update();
                    }
                  },
                  {
                    name: 'Add Data',
                    handler(chart) {
                      const label = getNextLabelOrNull(chart);
                      if (label) {
                        chart.data.labels.push(label);
                      }
                      chart.data.datasets.forEach(dataset => dataset.data.push(createRandomPoint(dataset)));
                      chart.update();
                    }
                  },
                  { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
                  { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
                ];
                """,
        };
    }

    private const string CSharpBubbleSetup =
        """
        var data = new ChartJsData
        {
            Datasets =
            [
                new BubbleDataset
                {
                    Label = "Dataset 1",
                    Data = CreateBubbleData(7, 0, 100, 5, 15),
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                },
                new BubbleDataset
                {
                    Label = "Dataset 2",
                    Data = CreateBubbleData(7, 0, 100, 5, 15),
                    BorderColor = "rgb(255, 159, 64)",
                    BackgroundColor = "rgba(255, 159, 64, 0.5)",
                },
            ],
        };
        """;

    private const string JavaScriptBubbleSetup =
        """
        const data = {
          datasets: [
            {
              label: 'Dataset 1',
              data: bubbles({count: 7, rmin: 5, rmax: 15, min: 0, max: 100}),
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5)
            },
            {
              label: 'Dataset 2',
              data: bubbles({count: 7, rmin: 5, rmax: 15, min: 0, max: 100}),
              borderColor: Utils.CHART_COLORS.orange,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.orange, 0.5)
            }
          ]
        };
        """;

    private const string CSharpComboSetup =
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels[..7]],
            Datasets =
            [
                new BarDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(7, -100, 100),
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                },
                new LineDataset
                {
                    Type = ChartType.line,
                    Label = "Dataset 2",
                    Data = RandomNumbers(7, -100, 100),
                    BorderColor = "rgb(54, 162, 235)",
                    BackgroundColor = "rgba(54, 162, 235, 0.5)",
                },
            ],
        };
        """;

    private const string JavaScriptComboSetup =
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            {
              type: 'bar',
              label: 'Dataset 1',
              data: Utils.numbers({count: 7, min: -100, max: 100}),
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5)
            },
            {
              type: 'line',
              label: 'Dataset 2',
              data: Utils.numbers({count: 7, min: -100, max: 100}),
              borderColor: Utils.CHART_COLORS.blue,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5)
            }
          ]
        };
        """;

    private const string CSharpDoughnutSetup =
        """
        var data = new ChartJsData
        {
            Labels = ["Red", "Orange", "Yellow", "Green", "Blue"],
            Datasets =
            [
                new DoughnutDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(5, 0, 100),
                    BackgroundColor = new IndexableOption<string>(["rgb(255, 99, 132)", "rgb(255, 159, 64)", "rgb(255, 205, 86)", "rgb(75, 192, 192)", "rgb(54, 162, 235)"]),
                },
            ],
        };
        """;

    private const string JavaScriptDoughnutSetup =
        """
        const data = {
          labels: ['Red', 'Orange', 'Yellow', 'Green', 'Blue'],
          datasets: [{
            label: 'Dataset 1',
            data: Utils.numbers({count: 5, min: 0, max: 100}),
            backgroundColor: Object.values(Utils.CHART_COLORS)
          }]
        };
        """;

    private const string CSharpPieSetup =
        """
        var data = new ChartJsData
        {
            Labels = ["Red", "Orange", "Yellow", "Green", "Blue"],
            Datasets =
            [
                new PieDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(5, 0, 100),
                    BackgroundColor = new IndexableOption<string>(["rgb(255, 99, 132)", "rgb(255, 159, 64)", "rgb(255, 205, 86)", "rgb(75, 192, 192)", "rgb(54, 162, 235)"]),
                },
            ],
        };
        """;

    private const string JavaScriptPieSetup =
        """
        const data = {
          labels: ['Red', 'Orange', 'Yellow', 'Green', 'Blue'],
          datasets: [{
            label: 'Dataset 1',
            data: Utils.numbers({count: 5, min: 0, max: 100}),
            backgroundColor: Object.values(Utils.CHART_COLORS)
          }]
        };
        """;

    private const string CSharpPolarAreaSetup =
        """
        var data = new ChartJsData
        {
            Labels = ["Red", "Orange", "Yellow", "Green", "Blue"],
            Datasets =
            [
                new PolarAreaDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(5, 0, 100),
                    BackgroundColor = new IndexableOption<string>(["rgba(255, 99, 132, 0.5)", "rgba(255, 159, 64, 0.5)", "rgba(255, 205, 86, 0.5)", "rgba(75, 192, 192, 0.5)", "rgba(54, 162, 235, 0.5)"]),
                },
            ],
        };
        """;

    private const string JavaScriptPolarAreaSetup =
        """
        const data = {
          labels: ['Red', 'Orange', 'Yellow', 'Green', 'Blue'],
          datasets: [{
            label: 'Dataset 1',
            data: Utils.numbers({count: 5, min: 0, max: 100}),
            backgroundColor: Object.values(Utils.CHART_COLORS).map(color => Utils.transparentize(color, 0.5))
          }]
        };
        """;

    private const string CSharpRadarSetup =
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels[..7]],
            Datasets =
            [
                new RadarDataset { Label = "Dataset 1", Data = RandomNumbers(7, 0, 100), BorderColor = "rgb(255, 99, 132)", BackgroundColor = "rgba(255, 99, 132, 0.5)" },
                new RadarDataset { Label = "Dataset 2", Data = RandomNumbers(7, 0, 100), BorderColor = "rgb(54, 162, 235)", BackgroundColor = "rgba(54, 162, 235, 0.5)" },
            ],
        };
        """;

    private const string JavaScriptRadarSetup =
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Dataset 2', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
          ]
        };
        """;

    private const string CSharpRadarSkipSetup =
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels[..7]],
            Datasets =
            [
                new RadarDataset { Label = "Skip first dataset", Data = CreateRadarSkipData(0, 7), BorderColor = "rgb(255, 99, 132)", BackgroundColor = "rgba(255, 99, 132, 0.5)" },
                new RadarDataset { Label = "Skip mid dataset", Data = CreateRadarSkipData(1, 7), BorderColor = "rgb(54, 162, 235)", BackgroundColor = "rgba(54, 162, 235, 0.5)" },
                new RadarDataset { Label = "Skip last dataset", Data = CreateRadarSkipData(2, 7), BorderColor = "rgb(75, 192, 192)", BackgroundColor = "rgba(75, 192, 192, 0.5)" },
            ],
        };
        """;

    private const string JavaScriptRadarSkipSetup =
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Skip first dataset', data: generateSkipPointData(0, 7), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Skip mid dataset', data: generateSkipPointData(1, 7), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) },
            { label: 'Skip last dataset', data: generateSkipPointData(2, 7), borderColor: Utils.CHART_COLORS.green, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.green, 0.5) }
          ]
        };
        """;

    private const string CSharpScatterSetup =
        """
        var data = new ChartJsData
        {
            Datasets =
            [
                new ScatterDataset { Label = "Dataset 1", Data = CreateBubbleData(7, 0, 100, 1, 1), BorderColor = "rgb(255, 99, 132)", BackgroundColor = "rgba(255, 99, 132, 0.5)" },
                new ScatterDataset { Label = "Dataset 2", Data = CreateBubbleData(7, 0, 100, 1, 1), BorderColor = "rgb(255, 159, 64)", BackgroundColor = "rgba(255, 159, 64, 0.5)" },
            ],
        };
        """;

    private const string JavaScriptScatterSetup =
        """
        const data = {
          datasets: [
            { label: 'Dataset 1', data: Utils.bubbles({count: 7, rmin: 1, rmax: 1, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Dataset 2', data: Utils.bubbles({count: 7, rmin: 1, rmax: 1, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.orange, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.orange, 0.5) }
          ]
        };
        """;

    private const string CSharpScatterMultiAxisSetup =
        """
        var data = new ChartJsData
        {
            Datasets =
            [
                new ScatterDataset { Label = "Dataset 1", Data = CreateBubbleData(7, -100, 100, 1, 1), BorderColor = "rgb(255, 99, 132)", BackgroundColor = "rgba(255, 99, 132, 0.5)", YAxisID = "y" },
                new ScatterDataset { Label = "Dataset 2", Data = CreateBubbleData(7, -100, 100, 1, 1), BorderColor = "rgb(255, 159, 64)", BackgroundColor = "rgba(255, 159, 64, 0.5)", YAxisID = "y2" },
            ],
        };
        """;

    private const string JavaScriptScatterMultiAxisSetup =
        """
        const data = {
          datasets: [
            { label: 'Dataset 1', data: Utils.bubbles({count: 7, rmin: 1, rmax: 1, min: -100, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5), yAxisID: 'y' },
            { label: 'Dataset 2', data: Utils.bubbles({count: 7, rmin: 1, rmax: 1, min: -100, max: 100}), borderColor: Utils.CHART_COLORS.orange, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.orange, 0.5), yAxisID: 'y2' }
          ]
        };
        """;

    private const string CSharpStackedBarLineSetup =
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels[..7]],
            Datasets =
            [
                new BarDataset { Type = ChartType.bar, Label = "Dataset 1", Stack = "combined", Data = RandomNumbers(7, 0, 100), BorderColor = "rgb(255, 99, 132)", BackgroundColor = "rgba(255, 99, 132, 0.5)" },
                new LineDataset { Label = "Dataset 2", Stack = "combined", Data = RandomNumbers(7, 0, 100), BorderColor = "rgb(54, 162, 235)", BackgroundColor = "rgba(54, 162, 235, 0.5)" },
            ],
        };
        """;

    private const string JavaScriptStackedBarLineSetup =
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { type: 'bar', label: 'Dataset 1', stack: 'combined', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { type: 'line', label: 'Dataset 2', stack: 'combined', data: Utils.numbers({count: 7, min: 0, max: 100}), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
          ]
        };
        """;

    private const string CSharpMultiSeriesPieSetup =
        """
        var data = new ChartJsData
        {
            Labels = ["Overall Yay", "Overall Nay", "Group A Yay", "Group A Nay", "Group B Yay", "Group B Nay", "Group C Yay", "Group C Nay"],
            Datasets =
            [
                new PieDataset { Data = [21, 79], BackgroundColor = new IndexableOption<string>(["#AAA", "#777"]) },
                new PieDataset { Data = [33, 67], BackgroundColor = new IndexableOption<string>(["hsl(0, 100%, 60%)", "hsl(0, 100%, 35%)"]) },
                new PieDataset { Data = [20, 80], BackgroundColor = new IndexableOption<string>(["hsl(100, 100%, 60%)", "hsl(100, 100%, 35%)"]) },
                new PieDataset { Data = [10, 90], BackgroundColor = new IndexableOption<string>(["hsl(180, 100%, 60%)", "hsl(180, 100%, 35%)"]) },
            ],
        };
        """;

    private const string JavaScriptMultiSeriesPieSetup =
        """
        const data = {
          labels: ['Overall Yay', 'Overall Nay', 'Group A Yay', 'Group A Nay', 'Group B Yay', 'Group B Nay', 'Group C Yay', 'Group C Nay'],
          datasets: [
            { backgroundColor: ['#AAA', '#777'], data: [21, 79] },
            { backgroundColor: ['hsl(0, 100%, 60%)', 'hsl(0, 100%, 35%)'], data: [33, 67] },
            { backgroundColor: ['hsl(100, 100%, 60%)', 'hsl(100, 100%, 35%)'], data: [20, 80] },
            { backgroundColor: ['hsl(180, 100%, 60%)', 'hsl(180, 100%, 35%)'], data: [10, 90] }
          ]
        };
        """;

    private const string CSharpPolarAreaCenteredConfigOptions =
        """
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Legend = new Legend { Position = "top" },
                Title = new Title { Display = true, Text = "Chart.js Polar Area Chart With Centered Point Labels" },
            },
            Scales = new ChartJsOptionsScales
            {
                R = new LinearRadialAxis
                {
                    PointLabels = new PointLabels
                    {
                        Display = true,
                        CenterPointLabels = true,
                        Font = new Font { Size = 18 },
                    },
                },
            },
        },
        """;

    private const string JavaScriptPolarAreaCenteredConfigOptions =
        """
        options: {
          responsive: true,
          plugins: {
            legend: { position: 'top' },
            title: { display: true, text: 'Chart.js Polar Area Chart With Centered Point Labels' }
          },
          scales: {
            r: {
              pointLabels: {
                display: true,
                centerPointLabels: true,
                font: { size: 18 }
              }
            }
          }
        }
        """;

    private const string CSharpScatterMultiAxisConfigOptions =
        """
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Legend = new Legend { Position = "top" },
                Title = new Title { Display = true, Text = "Chart.js Scatter Multi Axis Chart" },
            },
            Scales = new ChartJsOptionsScales
            {
                Y = new CartesianAxis { Type = "linear", Axis = "y", Position = "left", Ticks = new ChartJsAxisTick { Color = "rgb(255, 99, 132)" } },
                Y2 = new CartesianAxis { Type = "linear", Axis = "y", Position = "right", Reverse = true, Ticks = new ChartJsAxisTick { Color = "rgb(54, 162, 235)" }, Grid = new ChartJsGrid { DrawOnChartArea = false } },
            },
        },
        """;

    private const string JavaScriptScatterMultiAxisConfigOptions =
        """
        options: {
          responsive: true,
          plugins: {
            legend: { position: 'top' },
            title: { display: true, text: 'Chart.js Scatter Multi Axis Chart' }
          },
          scales: {
            y: { type: 'linear', axis: 'y', ticks: { color: Utils.CHART_COLORS.red } },
            y2: { type: 'linear', axis: 'y', reverse: true, ticks: { color: Utils.CHART_COLORS.blue }, grid: { drawOnChartArea: false } }
          }
        }
        """;

    private const string CSharpStackedBarLineConfigOptions =
        """
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Stacked Line/Bar Chart" },
            },
            Scales = new ChartJsOptionsScales
            {
                Y = new ChartJsAxis { Stacked = true },
            },
        },
        """;

    private const string JavaScriptStackedBarLineConfigOptions =
        """
        options: {
          responsive: true,
          plugins: {
            title: { display: true, text: 'Chart.js Stacked Line/Bar Chart' }
          },
          scales: {
            y: { stacked: true }
          }
        }
        """;

    private const string CSharpMultiSeriesPieConfigOptions =
        """
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Legend = new Legend
                {
                    Labels = new Labels { GenerateLabels = ChartJsFunction.FromName("multiSeriesPieGenerateLabels") },
                    OnClick = ChartJsFunction.FromName("multiSeriesPieLegendClick"),
                },
                Tooltip = new Tooltip
                {
                    Callbacks = new TooltipCallbacks { Title = ChartJsFunction.FromName("multiSeriesPieTooltipTitle") },
                },
            },
        },
        """;

    private const string JavaScriptMultiSeriesPieConfigOptions =
        """
        options: {
          responsive: true,
          plugins: {
            legend: {
              labels: { generateLabels: getOrCreateLegendList },
              onClick: hideOrShowDataset
            },
            tooltip: {
              callbacks: { title: tooltipTitle }
            }
          }
        }
        """;

    private const string MultiSeriesPieCallbacks =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          multiSeriesPieGenerateLabels(chart) {
            const original = Chart.overrides.pie.plugins.legend.labels.generateLabels;
            const labelsOriginal = original.call(this, chart);
            const datasetColors = chart.data.datasets.flatMap(dataset => dataset.backgroundColor);

            labelsOriginal.forEach(label => {
              label.datasetIndex = (label.index - label.index % 2) / 2;
              label.hidden = !chart.isDatasetVisible(label.datasetIndex);
              label.fillStyle = datasetColors[label.index];
            });

            return labelsOriginal;
          },
          multiSeriesPieLegendClick(mouseEvent, legendItem, legend) {
            const meta = legend.chart.getDatasetMeta(legendItem.datasetIndex);
            meta.hidden = legend.chart.isDatasetVisible(legendItem.datasetIndex);
            legend.chart.update();
          },
          multiSeriesPieTooltipTitle(context) {
            const first = context?.[0];
            if (!first) {
              return '';
            }

            const labelIndex = first.datasetIndex * 2 + first.dataIndex;
            return `${first.chart.data.labels[labelIndex]}: ${first.formattedValue}`;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}

#pragma warning disable CA1054, CA1056
public sealed record OtherChartSampleDefinition(
    string Id,
    string Title,
    string DocsUrl,
    OtherChartActionSet ActionSet,
    OtherChartDataMode DataMode,
    Func<ChartJsConfig> CreateConfig,
    ChartJsDocsCodeSet CSharpCode,
    ChartJsDocsCodeSet JavaScriptCode,
    string? CallbacksCode = null);
#pragma warning restore CA1054, CA1056

public enum OtherChartActionSet
{
    None,
    RandomizeOnly,
    RadialData,
    Pie,
    Doughnut,
    FullDatasets,
}

public enum OtherChartDataMode
{
    CartesianUnlabeled,
    CartesianLabeled,
    Radial,
}
