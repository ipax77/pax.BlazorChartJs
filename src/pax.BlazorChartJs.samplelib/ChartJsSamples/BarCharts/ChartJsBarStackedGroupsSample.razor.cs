using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public partial class ChartJsBarStackedGroupsSample
{
    private readonly ChartJsConfig config = CreateConfig(
        "Chart.js Bar Chart - Stacked",
        null,
        [
            new BarDataset { Label = "Dataset 1", Data = RandomNumbers(), BackgroundColor = Red, Stack = "Stack 0" },
            new BarDataset { Label = "Dataset 2", Data = RandomNumbers(), BackgroundColor = Blue, Stack = "Stack 0" },
            new BarDataset { Label = "Dataset 3", Data = RandomNumbers(), BackgroundColor = Green, Stack = "Stack 1" },
        ],
        stacked: true,
        interactionIntersect: false);

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "stacked-groups";

    protected override string Title => "Stacked Bar Chart with Groups";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/bar/stacked-groups.html";

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
            Options = new ChartJsOptions
            {
                Responsive = true,
                Interaction = new Interactions { Intersect = false },
                Scales = new ChartJsOptionsScales
                {
                    X = new ChartJsAxis { Stacked = true },
                    Y = new ChartJsAxis { Stacked = true },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = [.. MonthLabels],
            Datasets =
            [
                new BarDataset { Label = "Dataset 1", Data = RandomNumbers(), Stack = "Stack 0" },
                new BarDataset { Label = "Dataset 2", Data = RandomNumbers(), Stack = "Stack 0" },
                new BarDataset { Label = "Dataset 3", Data = RandomNumbers(), Stack = "Stack 1" },
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
            interaction: { intersect: false },
            scales: {
              x: { stacked: true },
              y: { stacked: true }
            }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: Utils.numbers({count: 7, min: -100, max: 100}), stack: 'Stack 0' },
            { label: 'Dataset 2', data: Utils.numbers({count: 7, min: -100, max: 100}), stack: 'Stack 0' },
            { label: 'Dataset 3', data: Utils.numbers({count: 7, min: -100, max: 100}), stack: 'Stack 1' }
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
