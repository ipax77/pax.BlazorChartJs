using Microsoft.AspNetCore.Components;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.AreaCharts;

public abstract class ChartJsAreaChartSampleBase : ChartJsDocsBaseComponent
{
    private const int DataCount = 8;
    private const int StackedDataCount = 7;
    private const int RandomMin = -100;
    private const int RandomMax = 100;
    private const int PositiveMin = 20;
    private const int PositiveMax = 80;
    private const int RadarMin = 8;
    private const int RadarMax = 16;

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

    private static readonly string[] ColorPalette = [Red, Orange, Yellow, Green, Blue, Purple, Grey];
    private static readonly Lazy<Dictionary<string, AreaSampleDefinition>> Definitions = new(CreateDefinitions);

    private ChartJsConfig? config;
    private IReadOnlyList<ChartJsDocsAction> actions = [];
    private bool smooth;
    private bool propagate;

    [Parameter]
    public string SampleId { get; set; } = "line-boundaries";

    protected AreaSampleDefinition ResolvedSample { get; private set; } = Definitions.Value["line-boundaries"];

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
            : Definitions.Value["line-boundaries"];
        config = ResolvedSample.CreateConfig();
        actions = CreateActions(ResolvedSample.ActionSet);
    }

    private ChartJsDocsAction[] CreateActions(AreaActionSet actionSet)
    {
        return actionSet switch
        {
            AreaActionSet.Boundaries =>
            [
                CreateAction("fill-false", "Fill: false (default)", () => SetFill(false)),
                CreateAction("fill-origin", "Fill: origin", () => SetFill("origin")),
                CreateAction("fill-start", "Fill: start", () => SetFill("start")),
                CreateAction("fill-end", "Fill: end", () => SetFill("end")),
                CreateAction("randomize", "Randomize", Randomize),
                CreateAction("smooth", "Smooth", ToggleSmooth),
            ],
            AreaActionSet.Filler =>
            [
                CreateAction("randomize", "Randomize", Randomize),
                CreateAction("propagate", "Propagate", TogglePropagate),
                CreateAction("smooth", "Smooth", ToggleSmooth),
            ],
            AreaActionSet.DrawTime =>
            [
                CreateAction("drawtime-before-dataset-draw", "drawTime: beforeDatasetDraw (default)", () => SetDrawTime("beforeDatasetDraw")),
                CreateAction("drawtime-before-datasets-draw", "drawTime: beforeDatasetsDraw", () => SetDrawTime("beforeDatasetsDraw")),
                CreateAction("drawtime-before-draw", "drawTime: beforeDraw", () => SetDrawTime("beforeDraw")),
                CreateAction("randomize", "Randomize", Randomize),
                CreateAction("smooth", "Smooth", ToggleSmooth),
            ],
            AreaActionSet.StackedLine =>
            [
                CreateAction("stacked-true", "Stacked: true", () => SetStacked(true)),
                CreateAction("stacked-false", "Stacked: false (default)", () => SetStacked(false)),
                CreateAction("stacked-single", "Stacked Single", () => SetStacked("single")),
                CreateAction("randomize", "Randomize", Randomize),
                CreateAction("add-dataset", "Add Dataset", AddStackedLineDataset),
                CreateAction("add-data", "Add Data", AddData),
                CreateAction("remove-dataset", "Remove Dataset", RemoveDataset),
                CreateAction("remove-data", "Remove Data", RemoveData),
            ],
            _ => [],
        };
    }

    private void SetFill(object fill)
    {
        UpdateDatasets(dataset =>
        {
            if (dataset is LineDataset lineDataset)
            {
                lineDataset.Fill = fill;
                return true;
            }

            return false;
        });
    }

    private void Randomize()
    {
        var datasets = Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);
        var labelCount = Config.Data.Labels.Count;

        for (var i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            data[dataset] = new SetDataObject(CreateDataForSample(dataset, labelCount));
        }

        Config.SetData(data);
    }

    private void ToggleSmooth()
    {
        smooth = !smooth;
        UpdateDatasets(dataset =>
        {
            switch (dataset)
            {
                case LineDataset lineDataset:
                    lineDataset.Tension = smooth ? 0.4 : 0;
                    return true;
                case RadarDataset radarDataset:
                    radarDataset.Tension = smooth ? 0.4 : 0;
                    return true;
                default:
                    return false;
            }
        });
    }

    private void TogglePropagate()
    {
        propagate = !propagate;
        EnsureFiller().Propagate = propagate;
        Config.UpdateChartOptions();
    }

    private void SetDrawTime(string drawTime)
    {
        EnsureFiller().DrawTime = drawTime;
        Config.UpdateChartOptions();
    }

    private void SetStacked(object stacked)
    {
        var scales = Config.Options?.Scales ?? throw new InvalidOperationException("Sample options must define scales.");
        var yAxis = scales.Y ??= new ChartJsAxis();
        yAxis.Stacked = stacked;
        Config.UpdateChartOptions();
    }

    private void AddStackedLineDataset()
    {
        var datasetIndex = Config.Data.Datasets.Count;
        var color = ColorPalette[datasetIndex % ColorPalette.Length];
        Config.AddDataset(new LineDataset
        {
            Label = $"Dataset {datasetIndex + 1}",
            BackgroundColor = color,
            BorderColor = color,
            Fill = true,
            Data = RandomNumbers(Config.Data.Labels.Count, RandomMin, RandomMax),
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
            data[datasets[i]] = new AddDataObject(CreatePointForSample(datasets[i]));
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

    private FillerOptions EnsureFiller()
    {
        var options = Config.Options ??= new ChartJsOptions();
        var plugins = options.Plugins ??= new Plugins();
        return plugins.Filler ??= new FillerOptions();
    }

    private void UpdateDatasets(Func<ChartJsDataset, bool> update)
    {
        var datasets = Config.Data.Datasets;
        List<ChartJsDataset> updatedDatasets = new(datasets.Count);

        for (var i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            if (update(dataset))
            {
                updatedDatasets.Add(dataset);
            }
        }

        if (updatedDatasets.Count > 0)
        {
            Config.UpdateDatasets(updatedDatasets);
        }
    }

    private List<object> CreateDataForSample(ChartJsDataset dataset, int count)
    {
        return ResolvedSample.SampleId switch
        {
            "line-datasets" => RandomNumbers(count, PositiveMin, PositiveMax),
            "radar-stacked" => RandomNumbers(count, RadarMin, RadarMax),
            _ => RandomNumbers(count, RandomMin, RandomMax),
        };
    }

    private int CreatePointForSample(ChartJsDataset dataset)
    {
        return ResolvedSample.SampleId switch
        {
            "line-datasets" => Random.Shared.Next(PositiveMin, PositiveMax + 1),
            "radar-stacked" => Random.Shared.Next(RadarMin, RadarMax + 1),
            _ => Random.Shared.Next(RandomMin, RandomMax + 1),
        };
    }

    private static ChartJsConfig CreateLineBoundariesConfig()
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
                        Label = "Dataset",
                        Data = RandomNumbers(DataCount, RandomMin, RandomMax),
                        BorderColor = Red,
                        BackgroundColor = RedTransparent,
                        Fill = false,
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false },
                    Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaFillTitle") },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
    }

    private static ChartJsConfig CreateLineDatasetsConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    CreateLineDataset("D0", Red, RedTransparent, null, hidden: true),
                    CreateLineDataset("D1", Orange, OrangeTransparent, "-1"),
                    CreateLineDataset("D2", Yellow, YellowTransparent, 1, hidden: true),
                    CreateLineDataset("D3", Green, GreenTransparent, "-1"),
                    CreateLineDataset("D4", Blue, BlueTransparent, "-1"),
                    CreateLineDataset("D5", Grey, GreyTransparent, "+2"),
                    CreateLineDataset("D6", Purple, PurpleTransparent, false),
                    CreateLineDataset("D7", Red, RedTransparent, 8),
                    CreateLineDataset("D8", Orange, OrangeTransparent, "end", hidden: true),
                    CreateLineDataset(
                        "D9",
                        Yellow,
                        YellowTransparent,
                        new ChartJsFillOptions { Above = "blue", Below = "red", Target = new ChartJsFillTarget { Value = 350 } }),
                ],
            },
            Options = new ChartJsOptions
            {
                Scales = new ChartJsOptionsScales
                {
                    Y = new ChartJsAxis { Stacked = true },
                },
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
    }

    private static ChartJsConfig CreateLineDrawTimeConfig()
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
                        Data = RandomNumbers(DataCount, RandomMin, RandomMax),
                        BorderColor = Red,
                        BackgroundColor = Red,
                        PointBackgroundColor = "#fff",
                        PointRadius = 10,
                        Fill = true,
                    },
                    new LineDataset
                    {
                        Label = "Dataset 2",
                        Data = RandomNumbers(DataCount, RandomMin, RandomMax),
                        BorderColor = Blue,
                        BackgroundColor = BlueTransparent,
                        PointBackgroundColor = "#fff",
                        PointRadius = 10,
                        Fill = true,
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false, DrawTime = "beforeDatasetDraw" },
                    Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaDrawTimeTitle") },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
    }

    private static ChartJsConfig CreateLineStackedConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(StackedDataCount),
                Datasets =
                [
                    CreateStackedLineDataset("My First dataset", Red),
                    CreateStackedLineDataset("My Second dataset", Blue),
                    CreateStackedLineDataset("My Third dataset", Green),
                    CreateStackedLineDataset("My Fourth dataset", Yellow),
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaStackedTitle") },
                    Tooltip = new Tooltip { Mode = "index" },
                },
                Interaction = new Interactions { Mode = "nearest", Axis = "x", Intersect = false },
                Scales = new ChartJsOptionsScales
                {
                    X = new ChartJsAxis { Title = new Title { Display = true, Text = "Month" } },
                    Y = new ChartJsAxis { Stacked = true, Title = new Title { Display = true, Text = "Value" } },
                },
            },
        };
    }

    private static ChartJsConfig CreateRadarStackedConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.radar,
            Data = new ChartJsData
            {
                Labels = CreateMonthLabels(DataCount),
                Datasets =
                [
                    CreateRadarDataset("D0", Red, RedTransparent, null),
                    CreateRadarDataset("D1", Orange, OrangeTransparent, "-1", hidden: true),
                    CreateRadarDataset("D2", Yellow, YellowTransparent, 1),
                    CreateRadarDataset("D3", Green, GreenTransparent, false),
                    CreateRadarDataset("D4", Blue, BlueTransparent, "-1"),
                    CreateRadarDataset("D5", Purple, PurpleTransparent, "-1"),
                    CreateRadarDataset("D6", Grey, GreyTransparent, new { value = 85 }),
                ],
            },
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
    }

    private static LineDataset CreateLineDataset(string label, string borderColor, string backgroundColor, object? fill, bool hidden = false)
    {
        LineDataset dataset = new()
        {
            Label = label,
            Data = RandomNumbers(DataCount, PositiveMin, PositiveMax),
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            Hidden = hidden ? true : null,
        };

        if (fill != null)
        {
            dataset.Fill = fill;
        }

        return dataset;
    }

    private static LineDataset CreateStackedLineDataset(string label, string color)
    {
        return new LineDataset
        {
            Label = label,
            Data = RandomNumbers(StackedDataCount, RandomMin, RandomMax),
            BorderColor = color,
            BackgroundColor = color,
            Fill = true,
        };
    }

    private static RadarDataset CreateRadarDataset(string label, string borderColor, string backgroundColor, object? fill, bool hidden = false)
    {
        RadarDataset dataset = new()
        {
            Label = label,
            Data = RandomNumbers(DataCount, RadarMin, RadarMax),
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            Hidden = hidden ? true : null,
        };

        if (fill != null)
        {
            dataset.Fill = fill;
        }

        return dataset;
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

    private static List<string> CreateMonthLabels(int count)
    {
        List<string> labels = new(count);
        for (var i = 0; i < count; i++)
        {
            labels.Add(GetMonthLabel(i));
        }

        return labels;
    }

    private static string GetMonthLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }

    private static Dictionary<string, AreaSampleDefinition> CreateDefinitions()
    {
        return new Dictionary<string, AreaSampleDefinition>(StringComparer.Ordinal)
        {
            ["line-boundaries"] = new(
                "line-boundaries",
                "Line Chart Boundaries",
                "https://www.chartjs.org/docs/latest/samples/area/line-boundaries.html",
                AreaActionSet.Boundaries,
                CreateLineBoundariesConfig,
                LineBoundariesCSharp,
                LineBoundariesJavaScript,
                AreaCallbacksCode),
            ["line-datasets"] = new(
                "line-datasets",
                "Line Chart Datasets",
                "https://www.chartjs.org/docs/latest/samples/area/line-datasets.html",
                AreaActionSet.Filler,
                CreateLineDatasetsConfig,
                LineDatasetsCSharp,
                LineDatasetsJavaScript,
                null),
            ["line-drawtime"] = new(
                "line-drawtime",
                "Line Chart drawTime",
                "https://www.chartjs.org/docs/latest/samples/area/line-drawtime.html",
                AreaActionSet.DrawTime,
                CreateLineDrawTimeConfig,
                LineDrawTimeCSharp,
                LineDrawTimeJavaScript,
                AreaCallbacksCode),
            ["line-stacked"] = new(
                "line-stacked",
                "Line Chart Stacked",
                "https://www.chartjs.org/docs/latest/samples/area/line-stacked.html",
                AreaActionSet.StackedLine,
                CreateLineStackedConfig,
                LineStackedCSharp,
                LineStackedJavaScript,
                AreaCallbacksCode),
            ["radar-stacked"] = new(
                "radar-stacked",
                "Radar Chart Stacked",
                "https://www.chartjs.org/docs/latest/samples/area/radar.html",
                AreaActionSet.Filler,
                CreateRadarStackedConfig,
                RadarStackedCSharp,
                RadarStackedJavaScript,
                null),
        };
    }

    private static readonly ChartJsDocsCodeSet LineBoundariesCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false },
                    Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaFillTitle") },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(8),
            Datasets =
            [
                new LineDataset { Label = "Dataset", Data = RandomNumbers(8), BorderColor = Red, BackgroundColor = RedTransparent, Fill = false },
            ],
        };
        """,
        """
        void SetFill(object fill) => UpdateLineDatasets(dataset => dataset.Fill = fill);
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void Smooth() => UpdateLineDatasets(dataset => dataset.Tension = smooth ? 0.4 : 0);
        """);

    private static readonly ChartJsDocsCodeSet LineBoundariesJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            plugins: {
              filler: { propagate: false },
              title: { display: true, text: (ctx) => 'Fill: ' + ctx.chart.data.datasets[0].fill }
            },
            interaction: { intersect: false }
          }
        };
        """,
        """
        const inputs = { min: -100, max: 100, count: 8, decimals: 2, continuity: 1 };
        const data = { labels: Utils.months({count: inputs.count}), datasets: [{ label: 'Dataset', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red), fill: false }] };
        """,
        """
        const actions = [
          { name: 'Fill: false (default)', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.fill = false; }); chart.update(); } },
          { name: 'Fill: origin', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.fill = 'origin'; }); chart.update(); } },
          { name: 'Fill: start', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.fill = 'start'; }); chart.update(); } },
          { name: 'Fill: end', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.fill = 'end'; }); chart.update(); } },
          { name: 'Randomize', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers(inputs); }); chart.update(); } },
          { name: 'Smooth', handler: chart => { smooth = !smooth; chart.options.elements.line.tension = smooth ? 0.4 : 0; chart.update(); } }
        ];
        """);

    private static readonly ChartJsDocsCodeSet LineDatasetsCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Scales = new ChartJsOptionsScales { Y = new ChartJsAxis { Stacked = true } },
                Plugins = new Plugins { Filler = new FillerOptions { Propagate = false } },
                Interaction = new Interactions { Intersect = false },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(8),
            Datasets =
            [
                CreateLineDataset("D0", Red, RedTransparent, null, hidden: true),
                CreateLineDataset("D1", Orange, OrangeTransparent, "-1"),
                CreateLineDataset("D9", Yellow, YellowTransparent, new ChartJsFillOptions { Above = "blue", Below = "red", Target = new ChartJsFillTarget { Value = 350 } }),
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void Propagate() { config.Options!.Plugins!.Filler!.Propagate = !config.Options.Plugins.Filler.Propagate; config.UpdateChartOptions(); }
        void Smooth() => UpdateLineDatasets(dataset => dataset.Tension = smooth ? 0.4 : 0);
        """);

    private static readonly ChartJsDocsCodeSet LineDatasetsJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            scales: { y: { stacked: true } },
            plugins: { filler: { propagate: false }, 'samples-filler-analyser': { target: 'chart-analyser' } },
            interaction: { intersect: false }
          }
        };
        """,
        """
        const inputs = { min: 20, max: 80, count: 8, decimals: 2, continuity: 1 };
        const data = {
          labels: Utils.months({count: inputs.count}),
          datasets: [
            { label: 'D0', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red), hidden: true },
            { label: 'D1', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.orange, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.orange), fill: '-1' },
            { label: 'D9', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.yellow, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.yellow), fill: { above: 'blue', below: 'red', target: { value: 350 } } }
          ]
        };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers(inputs); }); chart.update(); } },
          { name: 'Propagate', handler(chart) { propagate = !propagate; chart.options.plugins.filler.propagate = propagate; chart.update(); } },
          { name: 'Smooth', handler(chart) { smooth = !smooth; chart.options.elements.line.tension = smooth ? 0.4 : 0; chart.update(); } }
        ];
        """);

    private static readonly ChartJsDocsCodeSet LineDrawTimeCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Plugins = new Plugins
                {
                    Filler = new FillerOptions { Propagate = false, DrawTime = "beforeDatasetDraw" },
                    Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaDrawTimeTitle") },
                },
                Interaction = new Interactions { Intersect = false },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(8),
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = RandomNumbers(8), BorderColor = Red, BackgroundColor = Red, Fill = true, PointBackgroundColor = "#fff", PointRadius = 10 },
                new LineDataset { Label = "Dataset 2", Data = RandomNumbers(8), BorderColor = Blue, BackgroundColor = BlueTransparent, Fill = true, PointBackgroundColor = "#fff", PointRadius = 10 },
            ],
        };
        """,
        """
        void SetDrawTime(string drawTime) { config.Options!.Plugins!.Filler!.DrawTime = drawTime; config.UpdateChartOptions(); }
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void Smooth() => UpdateLineDatasets(dataset => dataset.Tension = smooth ? 0.4 : 0);
        """);

    private static readonly ChartJsDocsCodeSet LineDrawTimeJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            plugins: {
              filler: { propagate: false },
              title: { display: true, text: (ctx) => 'drawTime: ' + ctx.chart.options.plugins.filler.drawTime }
            },
            pointBackgroundColor: '#fff',
            radius: 10,
            interaction: { intersect: false }
          }
        };
        """,
        """
        const inputs = { min: -100, max: 100, count: 8, decimals: 2, continuity: 1 };
        const data = { labels: Utils.months({count: inputs.count}), datasets: [
          { label: 'Dataset 1', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.CHART_COLORS.red, fill: true },
          { label: 'Dataset 2', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue), fill: true }
        ] };
        """,
        """
        const actions = [
          { name: 'drawTime: beforeDatasetDraw (default)', handler: chart => { chart.options.plugins.filler.drawTime = 'beforeDatasetDraw'; chart.update(); } },
          { name: 'drawTime: beforeDatasetsDraw', handler: chart => { chart.options.plugins.filler.drawTime = 'beforeDatasetsDraw'; chart.update(); } },
          { name: 'drawTime: beforeDraw', handler: chart => { chart.options.plugins.filler.drawTime = 'beforeDraw'; chart.update(); } },
          { name: 'Randomize', handler: chart => { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers(inputs); }); chart.update(); } },
          { name: 'Smooth', handler: chart => { smooth = !smooth; chart.options.elements.line.tension = smooth ? 0.4 : 0; chart.update(); } }
        ];
        """);

    private static readonly ChartJsDocsCodeSet LineStackedCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = ChartJsFunction.FromName("areaStackedTitle") }, Tooltip = new Tooltip { Mode = "index" } },
                Interaction = new Interactions { Mode = "nearest", Axis = "x", Intersect = false },
                Scales = new ChartJsOptionsScales { X = new ChartJsAxis { Title = new Title { Display = true, Text = "Month" } }, Y = new ChartJsAxis { Stacked = true, Title = new Title { Display = true, Text = "Value" } } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(7),
            Datasets =
            [
                CreateStackedLineDataset("My First dataset", Red),
                CreateStackedLineDataset("My Second dataset", Blue),
                CreateStackedLineDataset("My Third dataset", Green),
                CreateStackedLineDataset("My Fourth dataset", Yellow),
            ],
        };
        """,
        """
        void SetStacked(object stacked) { config.Options!.Scales!.Y!.Stacked = stacked; config.UpdateChartOptions(); }
        void AddDataset() => config.AddDataset(CreateStackedLineDataset($"Dataset {config.Data.Datasets.Count + 1}", nextColor));
        void AddData() => config.AddData(nextMonth, null, valuesByDataset);
        void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);
        void RemoveData() => config.RemoveData();
        """);

    private static readonly ChartJsDocsCodeSet LineStackedJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: (ctx) => 'Chart.js Line Chart - stacked=' + ctx.chart.options.scales.y.stacked }, tooltip: { mode: 'index' } },
            interaction: { mode: 'nearest', axis: 'x', intersect: false },
            scales: { x: { title: { display: true, text: 'Month' } }, y: { stacked: true, title: { display: true, text: 'Value' } } }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = { labels, datasets: [
          { label: 'My First dataset', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.CHART_COLORS.red, fill: true },
          { label: 'My Second dataset', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.CHART_COLORS.blue, fill: true },
          { label: 'My Third dataset', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.green, backgroundColor: Utils.CHART_COLORS.green, fill: true },
          { label: 'My Fourth dataset', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.yellow, backgroundColor: Utils.CHART_COLORS.yellow, fill: true }
        ] };
        """,
        """
        const actions = [
          { name: 'Stacked: true', handler: chart => { chart.options.scales.y.stacked = true; chart.update(); } },
          { name: 'Stacked: false (default)', handler: chart => { chart.options.scales.y.stacked = false; chart.update(); } },
          { name: 'Stacked Single', handler: chart => { chart.options.scales.y.stacked = 'single'; chart.update(); } },
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}); }); chart.update(); } },
          { name: 'Add Dataset', handler(chart) { const dsColor = Utils.namedColor(chart.data.datasets.length); chart.data.datasets.push({ label: 'Dataset ' + (chart.data.datasets.length + 1), backgroundColor: dsColor, borderColor: dsColor, fill: true, data: Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}) }); chart.update(); } },
          { name: 'Add Data', handler(chart) { chart.data.labels = Utils.months({count: chart.data.labels.length + 1}); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(-100, 100))); chart.update(); } },
          { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);

    private static readonly ChartJsDocsCodeSet RadarStackedCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.radar,
            Data = data,
            Options = new ChartJsOptions
            {
                Plugins = new Plugins { Filler = new FillerOptions { Propagate = false } },
                Interaction = new Interactions { Intersect = false },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = CreateMonthLabels(8),
            Datasets =
            [
                CreateRadarDataset("D0", Red, RedTransparent, null),
                CreateRadarDataset("D1", Orange, OrangeTransparent, "-1", hidden: true),
                CreateRadarDataset("D6", Grey, GreyTransparent, new { value = 85 }),
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void Propagate() { config.Options!.Plugins!.Filler!.Propagate = !config.Options.Plugins.Filler.Propagate; config.UpdateChartOptions(); }
        void Smooth() => UpdateRadarDatasets(dataset => dataset.Tension = smooth ? 0.4 : 0);
        """);

    private static readonly ChartJsDocsCodeSet RadarStackedJavaScript = new(
        """
        const config = {
          type: 'radar',
          data,
          options: {
            plugins: { filler: { propagate: false }, 'samples-filler-analyser': { target: 'chart-analyser' } },
            interaction: { intersect: false }
          }
        };
        """,
        """
        const inputs = { min: 8, max: 16, count: 8, decimals: 2, continuity: 1 };
        const data = { labels: Utils.months({count: inputs.count}), datasets: [
          { label: 'D0', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red) },
          { label: 'D1', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.orange, hidden: true, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.orange), fill: '-1' },
          { label: 'D6', data: Utils.numbers(inputs), borderColor: Utils.CHART_COLORS.grey, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.grey), fill: { value: 85 } }
        ] };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { inputs.from = []; chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers(inputs); }); chart.update(); } },
          { name: 'Propagate', handler(chart) { propagate = !propagate; chart.options.plugins.filler.propagate = propagate; chart.update(); } },
          { name: 'Smooth', handler(chart) { smooth = !smooth; chart.options.elements.line.tension = smooth ? 0.4 : 0; chart.update(); } }
        ];
        """);

    private const string AreaCallbacksCode =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          areaFillTitle(context) {
            return `Fill: ${context.chart.data.datasets[0].fill}`;
          },
          areaDrawTimeTitle(context) {
            return `drawTime: ${context.chart.options.plugins.filler.drawTime}`;
          },
          areaStackedTitle(context) {
            return `Chart.js Line Chart - stacked=${context.chart.options.scales.y.stacked}`;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}

public sealed partial class ChartJsAreaChartSample : ChartJsAreaChartSampleBase
{
}

public sealed record AreaSampleDefinition(
    string SampleId,
    string Title,
    string DocsHref,
    AreaActionSet ActionSet,
    Func<ChartJsConfig> CreateConfig,
    ChartJsDocsCodeSet CSharpCode,
    ChartJsDocsCodeSet JavaScriptCode,
    string? CallbacksCode);

public enum AreaActionSet
{
    Boundaries,
    Filler,
    DrawTime,
    StackedLine,
}
