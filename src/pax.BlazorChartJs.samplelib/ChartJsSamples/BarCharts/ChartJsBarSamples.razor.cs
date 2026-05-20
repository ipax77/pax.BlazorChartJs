using Microsoft.JSInterop;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public partial class ChartJsBarSamples
{
    private const int DataCount = 7;
    private const int RandomMin = -100;
    private const int RandomMax = 100;
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Green = "rgb(75, 192, 192)";
    private const string GreenTransparent = "rgba(75, 192, 192, 0.5)";

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

    private static readonly string[] ColorPalette =
    [
        Red,
        Blue,
        Green,
        "rgb(255, 159, 64)",
        "rgb(153, 102, 255)",
        "rgb(255, 205, 86)",
        "rgb(201, 203, 207)",
    ];

    private static readonly string[] TransparentColorPalette =
    [
        RedTransparent,
        BlueTransparent,
        GreenTransparent,
        "rgba(255, 159, 64, 0.5)",
        "rgba(153, 102, 255, 0.5)",
        "rgba(255, 205, 86, 0.5)",
        "rgba(201, 203, 207, 0.5)",
    ];

    [Microsoft.AspNetCore.Components.Parameter]
    public string SampleId { get; set; } = "vertical";

    private readonly List<BarSample> samples =
    [
        CreateBorderRadiusSample(),
        CreateFloatingBarsSample(),
        CreateHorizontalSample(),
        CreateStackedSample(),
        CreateStackedGroupsSample(),
        CreateVerticalSample(),
    ];

    private bool highlightPending = true;
    private string? previousSampleId;

    private BarSample? SelectedSample => samples.FirstOrDefault(sample => sample.Id == SampleId) ?? samples[^1];

    protected override void OnParametersSet()
    {
        if (!string.Equals(previousSampleId, SampleId, StringComparison.Ordinal))
        {
            previousSampleId = SampleId;
            highlightPending = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || highlightPending)
        {
            highlightPending = false;
            await JSRuntime.InvokeVoidAsync("highlightCode").ConfigureAwait(false);
        }
    }

    private static BarSample CreateBorderRadiusSample()
    {
        return new BarSample(
            "border-radius",
            "Bar Chart Border Radius",
            "https://www.chartjs.org/docs/latest/samples/bar/border-radius.html",
            CreateConfig(
                "Chart.js Bar Chart",
                "top",
                [
                    new BarDataset
                    {
                        Label = "Fully Rounded",
                        Data = RandomNumbers(),
                        BorderColor = Red,
                        BackgroundColor = RedTransparent,
                        BorderWidth = 2,
                        BorderRadius = double.MaxValue,
                        BorderSkipped = new IndexableOption<object>(false),
                    },
                    new BarDataset
                    {
                        Label = "Small Radius",
                        Data = RandomNumbers(),
                        BorderColor = Blue,
                        BackgroundColor = BlueTransparent,
                        BorderWidth = 2,
                        BorderRadius = 5,
                        BorderSkipped = new IndexableOption<object>(false),
                    },
                ]),
            BorderRadiusCSharp,
            BorderRadiusJavaScript,
            [RandomizeAction]);
    }

    private static BarSample CreateFloatingBarsSample()
    {
        return new BarSample(
            "floating",
            "Floating Bars",
            "https://www.chartjs.org/docs/latest/samples/bar/floating.html",
            CreateConfig(
                "Chart.js Floating Bar Chart",
                "top",
                [
                    new BarDataset
                    {
                        Label = "Dataset 1",
                        Data = RandomFloatingBars(),
                        BackgroundColor = Red,
                    },
                    new BarDataset
                    {
                        Label = "Dataset 2",
                        Data = RandomFloatingBars(),
                        BackgroundColor = Blue,
                    },
                ]),
            FloatingBarsCSharp,
            FloatingBarsJavaScript,
            [RandomizeFloatingAction]);
    }

    private static BarSample CreateHorizontalSample()
    {
        return new BarSample(
            "horizontal",
            "Horizontal Bar Chart",
            "https://www.chartjs.org/docs/latest/samples/bar/horizontal.html",
            CreateConfig(
                "Chart.js Horizontal Bar Chart",
                "right",
                [
                    new BarDataset
                    {
                        Label = "Dataset 1",
                        Data = RandomNumbers(),
                        BorderColor = Red,
                        BackgroundColor = RedTransparent,
                        BorderWidth = 2,
                    },
                    new BarDataset
                    {
                        Label = "Dataset 2",
                        Data = RandomNumbers(),
                        BorderColor = Blue,
                        BackgroundColor = BlueTransparent,
                        BorderWidth = 2,
                    },
                ],
                indexAxis: "y"),
            HorizontalCSharp,
            HorizontalJavaScript,
            CreateFullDatasetActions());
    }

    private static BarSample CreateStackedSample()
    {
        return new BarSample(
            "stacked",
            "Stacked Bar Chart",
            "https://www.chartjs.org/docs/latest/samples/bar/stacked.html",
            CreateConfig(
                "Chart.js Bar Chart - Stacked",
                null,
                [
                    new BarDataset { Label = "Dataset 1", Data = RandomNumbers(), BackgroundColor = Red },
                    new BarDataset { Label = "Dataset 2", Data = RandomNumbers(), BackgroundColor = Blue },
                    new BarDataset { Label = "Dataset 3", Data = RandomNumbers(), BackgroundColor = Green },
                ],
                stacked: true),
            StackedCSharp,
            StackedJavaScript,
            [RandomizeAction]);
    }

    private static BarSample CreateStackedGroupsSample()
    {
        return new BarSample(
            "stacked-groups",
            "Stacked Bar Chart with Groups",
            "https://www.chartjs.org/docs/latest/samples/bar/stacked-groups.html",
            CreateConfig(
                "Chart.js Bar Chart - Stacked",
                null,
                [
                    new BarDataset { Label = "Dataset 1", Data = RandomNumbers(), BackgroundColor = Red, Stack = "Stack 0" },
                    new BarDataset { Label = "Dataset 2", Data = RandomNumbers(), BackgroundColor = Blue, Stack = "Stack 0" },
                    new BarDataset { Label = "Dataset 3", Data = RandomNumbers(), BackgroundColor = Green, Stack = "Stack 1" },
                ],
                stacked: true,
                interactionIntersect: false),
            StackedGroupsCSharp,
            StackedGroupsJavaScript,
            [RandomizeAction]);
    }

    private static BarSample CreateVerticalSample()
    {
        return new BarSample(
            "vertical",
            "Vertical Bar Chart",
            "https://www.chartjs.org/docs/latest/samples/bar/vertical.html",
            CreateConfig(
                "Chart.js Bar Chart",
                "top",
                [
                    new BarDataset
                    {
                        Label = "Dataset 1",
                        Data = RandomNumbers(),
                        BorderColor = Red,
                        BackgroundColor = RedTransparent,
                    },
                    new BarDataset
                    {
                        Label = "Dataset 2",
                        Data = RandomNumbers(),
                        BorderColor = Blue,
                        BackgroundColor = BlueTransparent,
                    },
                ]),
            VerticalCSharp,
            VerticalJavaScript,
            CreateFullDatasetActions());
    }

    private static ChartJsConfig CreateConfig(
        string title,
        string? legendPosition,
        IList<ChartJsDataset> datasets,
        bool stacked = false,
        string? indexAxis = null,
        bool interactionIntersect = true)
    {
        return new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = [.. MonthLabels],
                Datasets = datasets,
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                IndexAxis = indexAxis,
                Interaction = interactionIntersect
                    ? null
                    : new Interactions { Intersect = false },
                Plugins = new Plugins
                {
                    Legend = legendPosition is null ? null : new Legend { Position = legendPosition },
                    Title = new Title
                    {
                        Display = true,
                        Text = title,
                    },
                },
                Scales = stacked
                    ? new ChartJsOptionsScales
                    {
                        X = new ChartJsAxis { Stacked = true },
                        Y = new ChartJsAxis { Stacked = true },
                    }
                    : null,
            },
        };
    }

    private static void RunAction(BarSample sample, BarSampleAction action)
    {
        action.Handler(sample);
    }

    private static void Randomize(BarSample sample)
    {
        sample.Config.SetData(sample.CreateRandomizedData(sample));
    }

    private static void AddDataset(BarSample sample)
    {
        var datasetIndex = sample.Config.Data.Datasets.Count;
        sample.Config.AddDataset(new BarDataset
        {
            Label = $"Dataset {datasetIndex + 1}",
            Data = RandomNumbers(sample.Config.Data.Labels.Count),
            BackgroundColor = TransparentColorPalette[datasetIndex % TransparentColorPalette.Length],
            BorderColor = ColorPalette[datasetIndex % ColorPalette.Length],
            BorderWidth = 1,
        });
    }

    private static void AddData(BarSample sample)
    {
        var datasets = sample.Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        var label = GetLabel(sample.Config.Data.Labels.Count);
        Dictionary<ChartJsDataset, AddDataObject> data = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            data[dataset] = new AddDataObject(Random.Shared.Next(RandomMin, RandomMax));
        }

        sample.Config.AddData(label, null, data);
    }

    private static void RemoveDataset(BarSample sample)
    {
        var datasets = sample.Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        sample.Config.RemoveDataset(datasets[^1]);
    }

    private static void RemoveData(BarSample sample)
    {
        var labels = sample.Config.Data.Labels;
        if (labels.Count == 0)
        {
            return;
        }

        sample.Config.RemoveData();
    }

    private static Dictionary<ChartJsDataset, SetDataObject> RandomizeNumbers(BarSample sample)
    {
        var datasets = sample.Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            data[dataset] = new SetDataObject(RandomNumbers(sample.Config.Data.Labels.Count));
        }

        return data;
    }

    private static Dictionary<ChartJsDataset, SetDataObject> RandomizeFloatingBars(BarSample sample)
    {
        var datasets = sample.Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            data[dataset] = new SetDataObject(RandomFloatingBars(sample.Config.Data.Labels.Count));
        }

        return data;
    }

    private static List<object> RandomNumbers()
    {
        return RandomNumbers(DataCount);
    }

    private static List<object> RandomNumbers(int count)
    {
        List<object> data = new(count);

        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(RandomMin, RandomMax));
        }

        return data;
    }

    private static List<object> RandomFloatingBars()
    {
        return RandomFloatingBars(DataCount);
    }

    private static List<object> RandomFloatingBars(int count)
    {
        List<object> data = new(count);

        for (var i = 0; i < count; i++)
        {
            data.Add(new[] { Random.Shared.Next(RandomMin, RandomMax), Random.Shared.Next(RandomMin, RandomMax) });
        }

        return data;
    }

    private static string GetLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }

    private static List<BarSampleAction> CreateFullDatasetActions()
    {
        return [RandomizeAction, AddDatasetAction, AddDataAction, RemoveDatasetAction, RemoveDataAction];
    }

    private static readonly BarSampleAction RandomizeAction = new("randomize", "Randomize", Randomize, RandomizeNumbers);
    private static readonly BarSampleAction RandomizeFloatingAction = new("randomize", "Randomize", Randomize, RandomizeFloatingBars);
    private static readonly BarSampleAction AddDatasetAction = new("add-dataset", "Add Dataset", AddDataset);
    private static readonly BarSampleAction AddDataAction = new("add-data", "Add Data", AddData);
    private static readonly BarSampleAction RemoveDatasetAction = new("remove-dataset", "Remove Dataset", RemoveDataset);
    private static readonly BarSampleAction RemoveDataAction = new("remove-data", "Remove Data", RemoveData);

    private sealed record BarSample(
        string Id,
        string Title,
        string DocsUrl,
        ChartJsConfig Config,
        string CSharpCode,
        string JavaScriptCode,
        IReadOnlyList<BarSampleAction> Actions)
    {
        public Func<BarSample, Dictionary<ChartJsDataset, SetDataObject>> CreateRandomizedData { get; } =
            Actions[0].CreateRandomizedData;
    }

    private sealed record BarSampleAction(
        string Id,
        string Name,
        Action<BarSample> Handler,
        Func<BarSample, Dictionary<ChartJsDataset, SetDataObject>>? RandomizeData = null)
    {
        public Func<BarSample, Dictionary<ChartJsDataset, SetDataObject>> CreateRandomizedData { get; } =
            RandomizeData ?? RandomizeNumbers;
    }

    private const string VerticalCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = ["January", "February", "March", "April", "May", "June", "July"],
                Datasets =
                [
                    new BarDataset
                    {
                        Label = "Dataset 1",
                        Data = RandomNumbers(),
                        BorderColor = "rgb(255, 99, 132)",
                        BackgroundColor = "rgba(255, 99, 132, 0.5)",
                    },
                    new BarDataset
                    {
                        Label = "Dataset 2",
                        Data = RandomNumbers(),
                        BorderColor = "rgb(54, 162, 235)",
                        BackgroundColor = "rgba(54, 162, 235, 0.5)",
                    },
                ],
            },
            Options = CreateOptions("Chart.js Bar Chart", "top"),
        };

        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }

        void AddDataset()
        {
            config.AddDataset(new BarDataset
            {
                Label = $"Dataset {config.Data.Datasets.Count + 1}",
                Data = RandomNumbers(config.Data.Labels.Count),
                BorderWidth = 1,
            });
        }

        void AddData()
        {
            var values = config.Data.Datasets.ToDictionary(
                dataset => dataset,
                dataset => new AddDataObject(Random.Shared.Next(-100, 100)));

            config.AddData("August", null, values);
        }

        void RemoveDataset()
        {
            config.RemoveDataset(config.Data.Datasets[^1]);
        }

        void RemoveData()
        {
            config.RemoveData();
        }
        """;

    private const string VerticalJavaScript = """
        const config = {
          type: 'bar',
          data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [
              {
                label: 'Dataset 1',
                data: numbers(7, -100, 100),
                borderColor: 'rgb(255, 99, 132)',
                backgroundColor: 'rgba(255, 99, 132, 0.5)'
              },
              {
                label: 'Dataset 2',
                data: numbers(7, -100, 100),
                borderColor: 'rgb(54, 162, 235)',
                backgroundColor: 'rgba(54, 162, 235, 0.5)'
              }
            ]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' },
              title: { display: true, text: 'Chart.js Bar Chart' }
            }
          }
        };

        function randomize(chart) {
          chart.data.datasets.forEach(dataset => dataset.data = numbers(7, -100, 100));
          chart.update();
        }

        function addDataset(chart) {
          const data = chart.data;
          const dsColor = namedColor(data.datasets.length);
          data.datasets.push({
            label: 'Dataset ' + (data.datasets.length + 1),
            backgroundColor: transparentize(dsColor, 0.5),
            borderColor: dsColor,
            borderWidth: 1,
            data: numbers(data.labels.length, -100, 100)
          });
          chart.update();
        }

        function addData(chart) {
          chart.data.labels = months(chart.data.labels.length + 1);
          chart.data.datasets.forEach(dataset => dataset.data.push(rand(-100, 100)));
          chart.update();
        }

        function removeDataset(chart) {
          chart.data.datasets.pop();
          chart.update();
        }

        function removeData(chart) {
          chart.data.labels.splice(-1, 1);
          chart.data.datasets.forEach(dataset => dataset.data.pop());
          chart.update();
        }
        """;

    private const string HorizontalCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = CreateTwoDatasetBarData(),
            Options = CreateOptions("Chart.js Horizontal Bar Chart", "right", indexAxis: "y"),
        };

        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }

        void AddDataset()
        {
            config.AddDataset(new BarDataset
            {
                Label = $"Dataset {config.Data.Datasets.Count + 1}",
                Data = RandomNumbers(config.Data.Labels.Count),
                BorderWidth = 1,
            });
        }

        void AddData()
        {
            var values = config.Data.Datasets.ToDictionary(
                dataset => dataset,
                dataset => new AddDataObject(Random.Shared.Next(-100, 100)));

            config.AddData("August", null, values);
        }

        void RemoveDataset()
        {
            config.RemoveDataset(config.Data.Datasets[^1]);
        }

        void RemoveData()
        {
            config.RemoveData();
        }
        """;

    private const string HorizontalJavaScript = """
        const config = {
          type: 'bar',
          data,
          options: {
            indexAxis: 'y',
            responsive: true,
            plugins: {
              legend: { position: 'right' },
              title: { display: true, text: 'Chart.js Horizontal Bar Chart' }
            }
          }
        };

        function randomize(chart) {
          chart.data.datasets.forEach(dataset => dataset.data = numbers(7, -100, 100));
          chart.update();
        }

        function addDataset(chart) {
          const data = chart.data;
          const dsColor = namedColor(data.datasets.length);
          data.datasets.push({
            label: 'Dataset ' + (data.datasets.length + 1),
            backgroundColor: transparentize(dsColor, 0.5),
            borderColor: dsColor,
            borderWidth: 1,
            data: numbers(data.labels.length, -100, 100)
          });
          chart.update();
        }

        function addData(chart) {
          chart.data.labels = months(chart.data.labels.length + 1);
          chart.data.datasets.forEach(dataset => dataset.data.push(rand(-100, 100)));
          chart.update();
        }

        function removeDataset(chart) {
          chart.data.datasets.pop();
          chart.update();
        }

        function removeData(chart) {
          chart.data.labels.splice(-1, 1);
          chart.data.datasets.forEach(dataset => dataset.data.pop());
          chart.update();
        }
        """;

    private const string BorderRadiusCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = MonthLabels,
                Datasets =
                [
                    new BarDataset
                    {
                        Label = "Fully Rounded",
                        Data = RandomNumbers(),
                        BorderWidth = 2,
                        BorderRadius = double.MaxValue,
                        BorderSkipped = new IndexableOption<object>(false),
                    },
                    new BarDataset
                    {
                        Label = "Small Radius",
                        Data = RandomNumbers(),
                        BorderWidth = 2,
                        BorderRadius = 5,
                        BorderSkipped = new IndexableOption<object>(false),
                    },
                ],
            },
            Options = CreateOptions("Chart.js Bar Chart", "top"),
        };

        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }
        """;

    private const string BorderRadiusJavaScript = """
        const config = {
          type: 'bar',
          data: {
            labels,
            datasets: [
              {
                label: 'Fully Rounded',
                data: numbers(7, -100, 100),
                borderWidth: 2,
                borderRadius: Number.MAX_VALUE,
                borderSkipped: false
              },
              {
                label: 'Small Radius',
                data: numbers(7, -100, 100),
                borderWidth: 2,
                borderRadius: 5,
                borderSkipped: false
              }
            ]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' },
              title: { display: true, text: 'Chart.js Bar Chart' }
            }
          }
        };
        """;

    private const string FloatingBarsCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = MonthLabels,
                Datasets =
                [
                    new BarDataset { Label = "Dataset 1", Data = RandomFloatingBars() },
                    new BarDataset { Label = "Dataset 2", Data = RandomFloatingBars() },
                ],
            },
            Options = CreateOptions("Chart.js Floating Bar Chart", "top"),
        };

        void Randomize()
        {
            config.SetData(CreateRandomFloatingData(config.Data.Datasets));
        }
        """;

    private const string FloatingBarsJavaScript = """
        const config = {
          type: 'bar',
          data: {
            labels,
            datasets: [
              { label: 'Dataset 1', data: floatingBars(7), backgroundColor: 'rgb(255, 99, 132)' },
              { label: 'Dataset 2', data: floatingBars(7), backgroundColor: 'rgb(54, 162, 235)' }
            ]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' },
              title: { display: true, text: 'Chart.js Floating Bar Chart' }
            }
          }
        };

        function randomize(chart) {
          chart.data.datasets.forEach(dataset => dataset.data = floatingBars(7));
          chart.update();
        }
        """;

    private const string StackedCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = CreateThreeDatasetBarData(),
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Chart.js Bar Chart - Stacked" },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new ChartJsAxis { Stacked = true },
                    Y = new ChartJsAxis { Stacked = true },
                },
            },
        };

        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }
        """;

    private const string StackedJavaScript = """
        const config = {
          type: 'bar',
          data,
          options: {
            responsive: true,
            plugins: {
              title: { display: true, text: 'Chart.js Bar Chart - Stacked' }
            },
            scales: {
              x: { stacked: true },
              y: { stacked: true }
            }
          }
        };
        """;

    private const string StackedGroupsCSharp = """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = MonthLabels,
                Datasets =
                [
                    new BarDataset { Label = "Dataset 1", Data = RandomNumbers(), Stack = "Stack 0" },
                    new BarDataset { Label = "Dataset 2", Data = RandomNumbers(), Stack = "Stack 0" },
                    new BarDataset { Label = "Dataset 3", Data = RandomNumbers(), Stack = "Stack 1" },
                ],
            },
            Options = CreateStackedOptions(intersect: false),
        };

        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }
        """;

    private const string StackedGroupsJavaScript = """
        const config = {
          type: 'bar',
          data: {
            labels,
            datasets: [
              { label: 'Dataset 1', data: numbers(7, -100, 100), stack: 'Stack 0' },
              { label: 'Dataset 2', data: numbers(7, -100, 100), stack: 'Stack 0' },
              { label: 'Dataset 3', data: numbers(7, -100, 100), stack: 'Stack 1' }
            ]
          },
          options: {
            responsive: true,
            interaction: { intersect: false },
            scales: {
              x: { stacked: true },
              y: { stacked: true }
            }
          }
        };
        """;
}
