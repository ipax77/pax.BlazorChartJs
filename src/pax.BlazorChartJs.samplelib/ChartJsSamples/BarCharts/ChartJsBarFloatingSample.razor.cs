using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public partial class ChartJsBarFloatingSample
{
    private readonly ChartJsConfig config = CreateConfig(
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
        ]);

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "floating";

    protected override string Title => "Floating Bars";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/bar/floating.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override void OnInitialized()
    {
        actions = CreateRandomizeFloatingBarsActions();
    }

    private static readonly ChartJsDocsCodeSet CSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = data,
            Options = CreateOptions("Chart.js Floating Bar Chart", "top"),
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
                    Label = "Dataset 1",
                    Data = RandomFloatingBars(),
                    BackgroundColor = "rgb(255, 99, 132)",
                },
                new BarDataset
                {
                    Label = "Dataset 2",
                    Data = RandomFloatingBars(),
                    BackgroundColor = "rgb(54, 162, 235)",
                },
            ],
        };
        """,
        """
        void Randomize()
        {
            config.SetData(CreateRandomFloatingData(config.Data.Datasets));
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
              title: { display: true, text: 'Chart.js Floating Bar Chart' }
            }
          }
        };
        """,
        """
        const labels = Utils.months({count: 7});
        const data = {
          labels,
          datasets: [
            { label: 'Dataset 1', data: floatingBars(7), backgroundColor: Utils.CHART_COLORS.red },
            { label: 'Dataset 2', data: floatingBars(7), backgroundColor: Utils.CHART_COLORS.blue }
          ]
        };
        """,
        """
        const actions = [
          {
            name: 'Randomize',
            handler(chart) {
              chart.data.datasets.forEach(dataset => {
                dataset.data = floatingBars(chart.data.labels.length);
              });
              chart.update();
            }
          }
        ];
        """);
}
