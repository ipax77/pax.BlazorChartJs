using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineMultiAxisSample
{
    private readonly ChartJsConfig config = new()
    {
        Type = ChartType.line,
        Data = new ChartJsData
        {
            Labels = [.. InitialMonthLabels],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(),
                    BorderColor = Red,
                    BackgroundColor = RedTransparent,
                    YAxisID = "y",
                },
                new LineDataset
                {
                    Label = "Dataset 2",
                    Data = RandomNumbers(),
                    BorderColor = Blue,
                    BackgroundColor = BlueTransparent,
                    YAxisID = "y1",
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Responsive = true,
            Interaction = new Interactions { Mode = "index", Intersect = false },
            Stacked = false,
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Line Chart - Multi Axis" },
            },
            Scales = new ChartJsOptionsScales
            {
                Y = new CartesianAxis { Type = "linear", Display = true, Position = "left" },
                Y1 = new CartesianAxis
                {
                    Type = "linear",
                    Display = true,
                    Position = "right",
                    Grid = new ChartJsGrid { DrawOnChartArea = false },
                },
            },
        },
    };

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "multi-axis";

    protected override string Title => "Multi Axis Line Chart";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/multi-axis.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override void OnInitialized()
    {
        actions = CreateRandomizeNumbersActions();
    }

    private static readonly ChartJsDocsCodeSet CSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Interaction = new Interactions { Mode = "index", Intersect = false },
                Stacked = false,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Line Chart - Multi Axis" } },
                Scales = new ChartJsOptionsScales
                {
                    Y = new ChartJsAxis { Type = "linear", Display = true, Position = "left" },
                    Y1 = new ChartJsAxis { Type = "linear", Display = true, Position = "right", Grid = new ChartJsGrid { DrawOnChartArea = false } },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = [.. InitialMonthLabels],
            Datasets =
            [
                new LineDataset { Label = "Dataset 1", Data = RandomNumbers(), BorderColor = Red, BackgroundColor = RedTransparent, YAxisID = "y" },
                new LineDataset { Label = "Dataset 2", Data = RandomNumbers(), BorderColor = Blue, BackgroundColor = BlueTransparent, YAxisID = "y1" },
            ],
        };
        """,
        """
        void Randomize()
        {
            config.SetData(CreateRandomData(config.Data.Datasets));
        }
        """);

    private static readonly ChartJsDocsCodeSet JavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            interaction: { mode: 'index', intersect: false },
            stacked: false,
            plugins: { title: { display: true, text: 'Chart.js Line Chart - Multi Axis' } },
            scales: {
              y: { type: 'linear', display: true, position: 'left' },
              y1: { type: 'linear', display: true, position: 'right', grid: { drawOnChartArea: false } }
            }
          }
        };
        """,
        """
        const data = {
          labels: Utils.months({count: 7}),
          datasets: [
            { label: 'Dataset 1', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5), yAxisID: 'y' },
            { label: 'Dataset 2', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5), yAxisID: 'y1' }
          ]
        };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}); }); chart.update(); } }
        ];
        """);
}
