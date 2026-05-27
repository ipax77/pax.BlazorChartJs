using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedProgrammaticEventsSample : ChartJsAdvancedProgrammaticEventsSampleBase
{
}

public abstract class ChartJsAdvancedProgrammaticEventsSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const int DataCount = 7;
    private const int RandomMin = -100;
    private const int RandomMax = 100;
    private const string ProgrammaticEventsModule = "./_content/pax.BlazorChartJs.samplelib/programmaticEvents.js";
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";

    private static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
    ];

    private readonly ChartJsConfig config = CreateProgrammaticEventsConfig();
    private IJSObjectReference? programmaticEventsModule;
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected ChartJsConfig Config => config;

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected override void OnInitialized()
    {
        actions =
        [
            CreateAction("trigger-hover", "Trigger Hover", TriggerHover),
            CreateAction("trigger-tooltip", "Trigger Tooltip", TriggerTooltip),
        ];
    }

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                "hover",
                "Hover",
                """
                async Task TriggerHover()
                {
                    var module = await GetProgrammaticEventsModule();
                    await module.InvokeVoidAsync(
                        "triggerHover",
                        config.ChartJsConfigGuid.ToString());
                }
                """),
            new ChartJsDocsCodeTab(
                "tooltip",
                "Tooltip",
                """
                async Task TriggerTooltip()
                {
                    var module = await GetProgrammaticEventsModule();
                    await module.InvokeVoidAsync(
                        "triggerTooltip",
                        config.ChartJsConfigGuid.ToString());
                }
                """),
            new ChartJsDocsCodeTab(
                ActionsTab,
                "Actions",
                """
                actions =
                [
                    CreateAction("trigger-hover", "Trigger Hover", TriggerHover),
                    CreateAction("trigger-tooltip", "Trigger Tooltip", TriggerTooltip),
                ];
                """),
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new ChartJsConfig
                {
                    Type = ChartType.bar,
                    Data = data,
                    Options = new ChartJsOptions(),
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                var data = new ChartJsData
                {
                    Labels = MonthLabels,
                    Datasets =
                    [
                        new BarDataset
                        {
                            Label = "Dataset 1",
                            Data = RandomNumbers(),
                            BorderColor = "rgb(255, 99, 132)",
                            BackgroundColor = "rgba(255, 99, 132, 0.5)",
                            HoverBorderWidth = 5,
                            HoverBorderColor = "green",
                        },
                        new BarDataset
                        {
                            Label = "Dataset 2",
                            Data = RandomNumbers(),
                            BorderColor = "rgb(54, 162, 235)",
                            BackgroundColor = "rgba(54, 162, 235, 0.5)",
                            HoverBorderWidth = 5,
                            HoverBorderColor = "green",
                        },
                    ],
                };

                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/pax.BlazorChartJs.samplelib/programmaticEvents.js");
                """),
        ]);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                "hover",
                "Hover",
                """
                function triggerHover(chart) {
                  if (chart.getActiveElements().length > 0) {
                    chart.setActiveElements([]);
                  } else {
                    chart.setActiveElements([
                      { datasetIndex: 0, index: 0 },
                      { datasetIndex: 1, index: 0 }
                    ]);
                  }

                  chart.update();
                }
                """),
            new ChartJsDocsCodeTab(
                "tooltip",
                "Tooltip",
                """
                function triggerTooltip(chart) {
                  const tooltip = chart.tooltip;
                  if (tooltip.getActiveElements().length > 0) {
                    tooltip.setActiveElements([], {x: 0, y: 0});
                  } else {
                    const chartArea = chart.chartArea;
                    tooltip.setActiveElements([
                      { datasetIndex: 0, index: 2 },
                      { datasetIndex: 1, index: 2 }
                    ], {
                      x: (chartArea.left + chartArea.right) / 2,
                      y: (chartArea.top + chartArea.bottom) / 2
                    });
                  }

                  chart.update();
                }
                """),
            new ChartJsDocsCodeTab(
                ActionsTab,
                "Actions",
                """
                const actions = [
                  { name: 'Trigger Hover', handler: triggerHover },
                  { name: 'Trigger Tooltip', handler: triggerTooltip }
                ];
                """),
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                const config = {
                  type: 'bar',
                  data,
                  options: {}
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                const labels = Utils.months({count: 7});
                const data = {
                  labels,
                  datasets: [
                    { label: 'Dataset 1', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.red, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5), hoverBorderWidth: 5, hoverBorderColor: 'green' },
                    { label: 'Dataset 2', data: Utils.numbers(NUMBER_CFG), borderColor: Utils.CHART_COLORS.blue, backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5), hoverBorderWidth: 5, hoverBorderColor: 'green' }
                  ]
                };
                """),
        ]);

    public async ValueTask DisposeAsync()
    {
        if (programmaticEventsModule is not null)
        {
            await programmaticEventsModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    private static ChartJsConfig CreateProgrammaticEventsConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = [.. MonthLabels],
                Datasets =
                [
                    CreateDataset("Dataset 1", Red, RedTransparent),
                    CreateDataset("Dataset 2", Blue, BlueTransparent),
                ],
            },
            Options = new ChartJsOptions(),
        };
    }

    private async Task TriggerHover()
    {
        var module = await GetProgrammaticEventsModule().ConfigureAwait(false);
        await module.InvokeVoidAsync("triggerHover", config.ChartJsConfigGuid.ToString()).ConfigureAwait(false);
    }

    private async Task TriggerTooltip()
    {
        var module = await GetProgrammaticEventsModule().ConfigureAwait(false);
        await module.InvokeVoidAsync("triggerTooltip", config.ChartJsConfigGuid.ToString()).ConfigureAwait(false);
    }

    private async ValueTask<IJSObjectReference> GetProgrammaticEventsModule()
    {
        programmaticEventsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", ProgrammaticEventsModule).ConfigureAwait(false);
        return programmaticEventsModule;
    }

    private static BarDataset CreateDataset(string label, string borderColor, string backgroundColor)
    {
        return new BarDataset
        {
            Label = label,
            Data = RandomNumbers(),
            BorderColor = borderColor,
            BackgroundColor = backgroundColor,
            HoverBorderWidth = 5,
            HoverBorderColor = "green",
        };
    }

    private static List<object> RandomNumbers()
    {
        List<object> data = new(DataCount);

        for (var i = 0; i < DataCount; i++)
        {
            data.Add(Random.Shared.Next(RandomMin, RandomMax + 1));
        }

        return data;
    }
}
