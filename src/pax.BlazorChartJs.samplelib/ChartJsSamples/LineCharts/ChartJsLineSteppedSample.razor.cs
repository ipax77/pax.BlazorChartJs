using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineSteppedSample
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
                    Fill = false,
                    Stepped = true,
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Responsive = true,
            Interaction = new Interactions { Intersect = false, Axis = "x" },
            Plugins = new Plugins
            {
                Title = new Title
                {
                    Display = true,
                    Text = ChartJsFunction.FromName("lineSteppedTitle"),
                },
            },
        },
    };

    private IReadOnlyList<ChartJsDocsAction> actions = [];

    protected override string SampleId => "stepped";

    protected override string Title => "Stepped Line Charts";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/stepped.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override string CallbacksCode => Callbacks;

    protected override void OnInitialized()
    {
        actions = CreateSteppedActions();
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
                Interaction = new Interactions { Intersect = false, Axis = "x" },
                Plugins = new Plugins { Title = new Title { Display = true, Text = ChartJsFunction.FromName("lineSteppedTitle") } },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = ["Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6"],
            Datasets =
            [
                new LineDataset { Label = "Dataset", Data = RandomNumbers(6), BorderColor = Red, Fill = false, Stepped = true },
            ],
        };
        """,
        """
        void SetStepped(object stepped)
        {
            foreach (var dataset in config.Data.Datasets.OfType<LineDataset>())
            {
                dataset.Stepped = stepped;
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
            interaction: { intersect: false, axis: 'x' },
            plugins: { title: { display: true, text: (ctx) => 'Step ' + ctx.chart.data.datasets[0].stepped + ' Interpolation' } }
          }
        };
        """,
        """
        const data = {
          labels: ['Day 1', 'Day 2', 'Day 3', 'Day 4', 'Day 5', 'Day 6'],
          datasets: [
            { label: 'Dataset', data: Utils.numbers({count: 6, min: -100, max: 100}), borderColor: Utils.CHART_COLORS.red, fill: false, stepped: true }
          ]
        };
        """,
        """
        const actions = [false, true, 'before', 'after', 'middle'].map(step => ({
          name: 'Step: ' + step + (step === false ? ' (default)' : ''),
          handler(chart) {
            chart.data.datasets.forEach(dataset => { dataset.stepped = step; });
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
          lineSteppedTitle(context) {
            return `Step ${context.chart.data.datasets[0].stepped} Interpolation`;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}
