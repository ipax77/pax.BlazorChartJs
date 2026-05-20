using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineSample
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
                },
                new LineDataset
                {
                    Label = "Dataset 2",
                    Data = RandomNumbers(),
                    BorderColor = Blue,
                    BackgroundColor = BlueTransparent,
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Legend = new Legend { Position = "top" },
                Title = new Title { Display = true, Text = "Chart.js Line Chart" },
            },
        },
    };

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "line";

    protected override string Title => "Line Chart";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/line.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override void OnInitialized()
    {
        actions = CreateFullDatasetActions();
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
                Plugins = new Plugins
                {
                    Legend = new Legend { Position = "top" },
                    Title = new Title { Display = true, Text = "Chart.js Line Chart" },
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
                new LineDataset { Label = "Dataset 1", Data = RandomNumbers(), BorderColor = Red, BackgroundColor = RedTransparent },
                new LineDataset { Label = "Dataset 2", Data = RandomNumbers(), BorderColor = Blue, BackgroundColor = BlueTransparent },
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));
        void AddDataset() => config.AddDataset(new LineDataset { Label = $"Dataset {config.Data.Datasets.Count + 1}", Data = RandomNumbers(config.Data.Labels.Count) });
        void AddData() => config.AddData("August", null, valuesByDataset);
        void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);
        void RemoveData() => config.RemoveData();
        """);

    private static readonly ChartJsDocsCodeSet JavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' },
              title: { display: true, text: 'Chart.js Line Chart' }
            }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5) },
            { label: 'Dataset 2', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5) }
          ]
        };
        """,
        """
        const actions = [
          { name: 'Randomize', handler(chart) { chart.data.datasets.forEach(dataset => { dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}); }); chart.update(); } },
          { name: 'Add Dataset', handler(chart) { const dsColor = Utils.namedColor(chart.data.datasets.length); chart.data.datasets.push({ label: 'Dataset ' + (chart.data.datasets.length + 1), backgroundColor: Utils.transparentize(dsColor, 0.5), borderColor: dsColor, data: Utils.numbers({count: chart.data.labels.length, min: -100, max: 100}) }); chart.update(); } },
          { name: 'Add Data', handler(chart) { chart.data.labels = Utils.months({count: chart.data.labels.length + 1}); chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(-100, 100))); chart.update(); } },
          { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);
}
