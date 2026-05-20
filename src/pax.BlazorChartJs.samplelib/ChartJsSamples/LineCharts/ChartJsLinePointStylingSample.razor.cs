using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLinePointStylingSample
{
    private readonly ChartJsConfig config = new()
    {
        Type = ChartType.line,
        Data = new ChartJsData
        {
            Labels = [.. DayLabels],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset",
                    Data = RandomNumbers(DayLabels.Length),
                    BorderColor = Red,
                    BackgroundColor = RedTransparent,
                    PointStyle = "circle",
                    PointRadius = 10,
                    PointHoverRadius = 15,
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
                    Text = ChartJsFunction.FromName("linePointStyleTitle"),
                },
            },
        },
    };

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "point-styling";

    protected override string Title => "Point Styling";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/point-styling.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override string CallbacksCode => Callbacks;

    protected override void OnInitialized()
    {
        actions = CreatePointStyleActions();
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
                Plugins = new Plugins { Title = new Title { Display = true, Text = ChartJsFunction.FromName("linePointStyleTitle") } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = ["Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6"],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset",
                    Data = RandomNumbers(6),
                    BorderColor = Red,
                    BackgroundColor = RedTransparent,
                    PointStyle = "circle",
                    PointRadius = 10,
                    PointHoverRadius = 15,
                },
            ],
        };
        """,
        """
        void SetPointStyle(object pointStyle)
        {
            foreach (var dataset in config.Data.Datasets.OfType<LineDataset>())
            {
                dataset.PointStyle = pointStyle;
            }

            config.UpdateDatasets(config.Data.Datasets);
        }
        """);

    private static readonly ChartJsDocsCodeSet JavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: { title: { display: true, text: (ctx) => 'Point Style: ' + ctx.chart.data.datasets[0].pointStyle } }
          }
        };
        """,
        """
        const data = {
          labels: ['Day 1', 'Day 2', 'Day 3', 'Day 4', 'Day 5', 'Day 6'],
          datasets: [
            { label: 'Dataset', data: Utils.numbers({count: 6, min: -100, max: 100}), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5), pointStyle: 'circle', pointRadius: 10, pointHoverRadius: 15 }
          ]
        };
        """,
        """
        const actions = [
          'circle (default)', 'cross', 'crossRot', 'dash', 'line', 'rect', 'rectRounded', 'rectRot', 'star', 'triangle', false
        ].map(style => ({
          name: 'pointStyle: ' + style,
          handler(chart) {
            chart.data.datasets.forEach(dataset => { dataset.pointStyle = style === 'circle (default)' ? 'circle' : style; });
            chart.update();
          }
        }));
        """);

    private const string Callbacks =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          linePointStyleTitle(context) {
            return `Point Style: ${context.chart.data.datasets[0].pointStyle}`;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}
