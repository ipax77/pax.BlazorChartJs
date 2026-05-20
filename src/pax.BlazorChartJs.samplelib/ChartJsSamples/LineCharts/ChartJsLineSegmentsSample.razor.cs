using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public partial class ChartJsLineSegmentsSample
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
                    Label = "My First Dataset",
                    Data = [65, 59, null!, 48, 56, 57, 40],
                    BorderColor = "rgb(75, 192, 192)",
                    Segment = new
                    {
                        BorderColor = ChartJsFunction.FromName("lineSegmentBorderColor"),
                        BorderDash = ChartJsFunction.FromName("lineSegmentBorderDash"),
                    },
                    SpanGaps = true,
                    Fill = false,
                    PointRadius = 0,
                },
            ],
        },
        Options = new ChartJsOptions
        {
            Interaction = new Interactions { Intersect = false },
        },
    };

    protected override string SampleId => "segments";

    protected override string Title => "Line Segment Styling";

    protected override string DocsHref => "https://www.chartjs.org/docs/latest/samples/line/segments.html";

    protected override ChartJsConfig Config => config;

    protected override IReadOnlyList<ChartJsDocsAction> Actions => NoActions;

    protected override ChartJsDocsCodeSet CSharpCode => CSharp;

    protected override ChartJsDocsCodeSet JavaScriptCode => JavaScript;

    protected override string CallbacksCode => Callbacks;

    private static readonly ChartJsDocsCodeSet CSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions { Interaction = new Interactions { Intersect = false } },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = [.. InitialMonthLabels],
            Datasets =
            [
                new LineDataset
                {
                    Label = "My First Dataset",
                    Data = [65, 59, null!, 48, 56, 57, 40],
                    BorderColor = "rgb(75, 192, 192)",
                    Segment = new
                    {
                        BorderColor = ChartJsFunction.FromName("lineSegmentBorderColor"),
                        BorderDash = ChartJsFunction.FromName("lineSegmentBorderDash"),
                    },
                    SpanGaps = true,
                    Fill = false,
                    PointRadius = 0,
                },
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
          data: {
            labels: Utils.months({count: 7}),
            datasets: [{
              label: 'My First Dataset',
              data: [65, 59, NaN, 48, 56, 57, 40],
              borderColor: 'rgb(75, 192, 192)',
              segment: {
                borderColor: ctx => skipped(ctx, 'rgb(0,0,0,0.2)') || down(ctx, 'rgb(192,75,75)'),
                borderDash: ctx => skipped(ctx, [6, 6])
              },
              spanGaps: true
            }]
          },
          options: genericOptions
        };
        """,
        """
        const skipped = (ctx, value) => ctx.p0.skip || ctx.p1.skip ? value : undefined;
        const down = (ctx, value) => ctx.p0.parsed.y > ctx.p1.parsed.y ? value : undefined;
        const genericOptions = { fill: false, interaction: { intersect: false }, radius: 0 };
        """,
        """
        // This official sample has no actions.
        """);

    private const string Callbacks =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          lineSegmentBorderColor(context) {
            if (context.p0.skip || context.p1.skip) {
              return 'rgb(0,0,0,0.2)';
            }

            return context.p0.parsed.y > context.p1.parsed.y
              ? 'rgb(192,75,75)'
              : undefined;
          },
          lineSegmentBorderDash(context) {
            return context.p0.skip || context.p1.skip
              ? [6, 6]
              : undefined;
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
}
