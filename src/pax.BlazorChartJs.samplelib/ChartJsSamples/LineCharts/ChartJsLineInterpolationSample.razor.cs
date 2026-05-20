using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineInterpolationSample
{
    private readonly ChartJsConfig config = new()
    {
        Type = ChartType.line,
        Data = new ChartJsData
        {
            Labels = [.. InterpolationLabels],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Cubic interpolation (monotone)",
                    Data = InterpolationData(),
                    BorderColor = Red,
                    Fill = false,
                    CubicInterpolationMode = "monotone",
                    Tension = 0.4,
                },
                new LineDataset
                {
                    Label = "Cubic interpolation",
                    Data = InterpolationData(),
                    BorderColor = Blue,
                    Fill = false,
                    Tension = 0.4,
                },
                new LineDataset
                {
                    Label = "Linear interpolation (default)",
                    Data = InterpolationData(),
                    BorderColor = Green,
                    Fill = false,
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Responsive = true,
            Plugins = new Plugins
            {
                Title = new Title
                {
                    Display = true,
                    Text = "Chart.js Line Chart - Cubic interpolation mode",
                },
            },
            Interaction = new Interactions { Intersect = false },
            Scales = new ChartJsOptionsScales
            {
                X = new ChartJsAxis
                {
                    Display = true,
                    Title = new Title { Display = true },
                },
                Y = new ChartJsAxis
                {
                    Display = true,
                    Title = new Title { Display = true, Text = "Value" },
                    SuggestedMin = -10,
                    SuggestedMax = 200,
                },
            },
        },
    };

    protected override string SampleId => "interpolation";

    protected override string Title => "Interpolation Modes";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/interpolation.html";

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
                Plugins = new Plugins { Title = new Title { Display = true, Text = "Chart.js Line Chart - Cubic interpolation mode" } },
                Interaction = new Interactions { Intersect = false },
                Scales = new ChartJsOptionsScales
                {
                    X = new ChartJsAxis { Display = true, Title = new Title { Display = true } },
                    Y = new ChartJsAxis { Display = true, Title = new Title { Display = true, Text = "Value" }, SuggestedMin = -10, SuggestedMax = 200 },
                },
            },
        };
        """,
        """
        string[] labels = Enumerable.Range(0, 12).Select(value => value.ToString(CultureInfo.InvariantCulture)).ToArray();
        IList<object> datapoints = [0, 20, 20, 60, 60, 120, null!, 180, 120, 125, 105, 110, 170];

        var data = new ChartJsData
        {
            Labels = [.. labels],
            Datasets =
            [
                new LineDataset { Label = "Cubic interpolation (monotone)", Data = [.. datapoints], BorderColor = Red, Fill = false, CubicInterpolationMode = "monotone", Tension = 0.4 },
                new LineDataset { Label = "Cubic interpolation", Data = [.. datapoints], BorderColor = Blue, Fill = false, Tension = 0.4 },
                new LineDataset { Label = "Linear interpolation (default)", Data = [.. datapoints], BorderColor = Green, Fill = false },
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
            plugins: { title: { display: true, text: 'Chart.js Line Chart - Cubic interpolation mode' } },
            interaction: { intersect: false },
            scales: {
              x: { display: true, title: { display: true } },
              y: { display: true, title: { display: true, text: 'Value' }, suggestedMin: -10, suggestedMax: 200 }
            }
          }
        };
        """,
        """
        const labels = Array.from({length: 12}, (_, i) => i.toString());
        const datapoints = [0, 20, 20, 60, 60, 120, NaN, 180, 120, 125, 105, 110, 170];
        const data = {
          labels,
          datasets: [
            { label: 'Cubic interpolation (monotone)', data: datapoints, borderColor: Utils.CHART_COLORS.red, fill: false, cubicInterpolationMode: 'monotone', tension: 0.4 },
            { label: 'Cubic interpolation', data: datapoints, borderColor: Utils.CHART_COLORS.blue, fill: false, tension: 0.4 },
            { label: 'Linear interpolation (default)', data: datapoints, borderColor: Utils.CHART_COLORS.green, fill: false }
          ]
        };
        """,
        """
        // This official sample has no actions.
        """);
}
