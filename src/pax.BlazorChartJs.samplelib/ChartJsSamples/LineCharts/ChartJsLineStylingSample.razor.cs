using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineStylingSample
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
                    Label = "Unfilled",
                    Fill = false,
                    BackgroundColor = Blue,
                    BorderColor = Blue,
                    Data = RandomNumbers(),
                },
                new LineDataset
                {
                    Label = "Dashed",
                    Fill = false,
                    BackgroundColor = Green,
                    BorderColor = Green,
                    BorderDash = new List<double> { 5, 5 },
                    Data = RandomNumbers(),
                },
                new LineDataset
                {
                    Label = "Filled",
                    BackgroundColor = Red,
                    BorderColor = Red,
                    Data = RandomNumbers(),
                    Fill = true,
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Title = new Title { Display = true, Text = "Chart.js Line Chart" },
            },
            Interaction = new Interactions { Mode = "index", Intersect = false },
            Scales = new ChartJsOptionsScales
            {
                X = new ChartJsAxis
                {
                    Display = true,
                    Title = new Title { Display = true, Text = "Month" },
                },
                Y = new ChartJsAxis
                {
                    Display = true,
                    Title = new Title { Display = true, Text = "Value" },
                },
            },
        },
    };

    protected override string SampleId => "styling";

    protected override string Title => "Line Styling";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/styling.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => NoActions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    private static readonly ChartJsDocsCodeSet CSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Line Chart" } },
                Interaction = new Interactions { Mode = "index", Intersect = false },
                Scales = new ChartJsOptionsScales
                {
                    X = new ChartJsAxis { Display = true, Title = new Title { Display = true, Text = "Month" } },
                    Y = new ChartJsAxis { Display = true, Title = new Title { Display = true, Text = "Value" } },
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
                new LineDataset { Label = "Unfilled", Fill = false, BackgroundColor = Blue, BorderColor = Blue, Data = RandomNumbers() },
                new LineDataset { Label = "Dashed", Fill = false, BackgroundColor = Green, BorderColor = Green, BorderDash = new List<double> { 5, 5 }, Data = RandomNumbers() },
                new LineDataset { Label = "Filled", BackgroundColor = Red, BorderColor = Red, Data = RandomNumbers(), Fill = true },
            ],
        };
        """,
        """
        // This official sample has no actions.
        """);

    private static readonly ChartJsDocsCodeSet JavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Chart.js Line Chart' } },
            interaction: { mode: 'index', intersect: false },
            scales: {
              x: { display: true, title: { display: true, text: 'Month' } },
              y: { display: true, title: { display: true, text: 'Value' } }
            }
          }
        };
        """,
        """
        const data = {
          labels: Utils.months({count: 7}),
          datasets: [
            { label: 'Unfilled', fill: false, backgroundColor: Utils.CHART_COLORS.blue, borderColor: Utils.CHART_COLORS.blue, data: Utils.numbers(NUMBER_CFG) },
            { label: 'Dashed', fill: false, backgroundColor: Utils.CHART_COLORS.green, borderColor: Utils.CHART_COLORS.green, borderDash: [5, 5], data: Utils.numbers(NUMBER_CFG) },
            { label: 'Filled', backgroundColor: Utils.CHART_COLORS.red, borderColor: Utils.CHART_COLORS.red, data: Utils.numbers(NUMBER_CFG), fill: true }
          ]
        };
        """,
        """
        // This official sample has no actions.
        """);
}
