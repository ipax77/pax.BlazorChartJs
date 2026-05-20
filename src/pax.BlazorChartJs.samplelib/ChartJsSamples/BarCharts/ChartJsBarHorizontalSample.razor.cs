using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public partial class ChartJsBarHorizontalSample
{
    private readonly ChartJsConfig config = CreateConfig(
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
        indexAxis: "y");

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "horizontal";

    protected override string Title => "Horizontal Bar Chart";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/bar/horizontal.html";

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
            Type = ChartType.bar,
            Data = data,
            Options = new ChartJsOptions
            {
                IndexAxis = "y",
                Responsive = true,
                Plugins = new Plugins
                {
                    Legend = new Legend { Position = "right" },
                    Title = new Title { Display = true, Text = "Chart.js Horizontal Bar Chart" },
                },
            },
        };
        """,
        """
        string[] labels = ["January", "February", "March", "April", "May", "June", "July"];
        var data = new ChartJsData
        {
            Labels = [.. labels],
            Datasets =
            [
                new BarDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(),
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                    BorderWidth = 2,
                },
                new BarDataset
                {
                    Label = "Dataset 2",
                    Data = RandomNumbers(),
                    BorderColor = "rgb(54, 162, 235)",
                    BackgroundColor = "rgba(54, 162, 235, 0.5)",
                    BorderWidth = 2,
                },
            ],
        };
        """,
        """
        void Randomize() => config.SetData(CreateRandomData(config.Data.Datasets));

        void AddDataset() => config.AddDataset(new BarDataset
        {
            Label = $"Dataset {config.Data.Datasets.Count + 1}",
            Data = RandomNumbers(config.Data.Labels.Count),
            BorderWidth = 1,
        });

        void AddData()
        {
            var values = config.Data.Datasets.ToDictionary(
                dataset => dataset,
                dataset => new AddDataObject(Random.Shared.Next(-100, 100)));

            config.AddData("August", null, values);
        }

        void RemoveDataset() => config.RemoveDataset(config.Data.Datasets[^1]);

        void RemoveData() => config.RemoveData();
        """);

    private static readonly ChartJsDocsCodeSet JavaScript = new(
        """
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
        """,
        """
        const DATA_COUNT = 7;
        const NUMBER_CFG = {count: DATA_COUNT, min: -100, max: 100};
        const labels = Utils.months({count: DATA_COUNT});
        const data = {
          labels,
          datasets: [
            {
              label: 'Dataset 1',
              data: Utils.numbers(NUMBER_CFG),
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
              borderWidth: 2
            },
            {
              label: 'Dataset 2',
              data: Utils.numbers(NUMBER_CFG),
              borderColor: Utils.CHART_COLORS.blue,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5),
              borderWidth: 2
            }
          ]
        };
        """,
        """
        const actions = [
          {
            name: 'Randomize',
            handler(chart) {
              chart.data.datasets.forEach(dataset => {
                dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100});
              });
              chart.update();
            }
          },
          {
            name: 'Add Dataset',
            handler(chart) {
              const data = chart.data;
              const dsColor = Utils.namedColor(data.datasets.length);
              data.datasets.push({
                label: 'Dataset ' + (data.datasets.length + 1),
                backgroundColor: Utils.transparentize(dsColor, 0.5),
                borderColor: dsColor,
                borderWidth: 1,
                data: Utils.numbers({count: data.labels.length, min: -100, max: 100})
              });
              chart.update();
            }
          },
          {
            name: 'Add Data',
            handler(chart) {
              chart.data.labels = Utils.months({count: chart.data.labels.length + 1});
              chart.data.datasets.forEach(dataset => dataset.data.push(Utils.rand(-100, 100)));
              chart.update();
            }
          },
          { name: 'Remove Dataset', handler(chart) { chart.data.datasets.pop(); chart.update(); } },
          { name: 'Remove Data', handler(chart) { chart.data.labels.splice(-1, 1); chart.data.datasets.forEach(dataset => dataset.data.pop()); chart.update(); } }
        ];
        """);
}
