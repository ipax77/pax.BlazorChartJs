using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public partial class ChartJsBarBorderRadiusSample
{
    private readonly ChartJsConfig config = CreateConfig(
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
        ]);

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "border-radius";

    protected override string Title => "Bar Chart Border Radius";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/bar/border-radius.html";

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
            Type = ChartType.bar,
            Data = data,
            Options = CreateOptions("Chart.js Bar Chart", "top"),
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels],
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
          type: 'bar',
          data,
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'top' },
              title: { display: true, text: 'Chart.js Bar Chart' }
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
              label: 'Fully Rounded',
              data: Utils.numbers({count: 7, min: -100, max: 100}),
              borderWidth: 2,
              borderRadius: Number.MAX_VALUE,
              borderSkipped: false
            },
            {
              label: 'Small Radius',
              data: Utils.numbers({count: 7, min: -100, max: 100}),
              borderWidth: 2,
              borderRadius: 5,
              borderSkipped: false
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
          }
        ];
        """);
}
