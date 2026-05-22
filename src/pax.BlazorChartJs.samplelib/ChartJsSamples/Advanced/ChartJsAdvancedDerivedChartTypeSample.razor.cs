using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedDerivedChartTypeSample : ChartJsAdvancedDerivedChartTypeSampleBase
{
}

public abstract class ChartJsAdvancedDerivedChartTypeSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const int DataCount = 7;
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string DerivedChartTypeModule = "./_content/pax.BlazorChartJs.samplelib/derivedChartType.js";

    private readonly ChartJsConfig config = CreateDerivedChartTypeConfig();
    private IJSObjectReference? derivedChartTypeModule;
    private bool controllerRegistered;
    private bool registeringController;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IOptions<ChartJsSetupOptions> ChartJsSetupOptions { get; set; } = default!;

    protected ChartJsConfig Config => config;

    protected static IReadOnlyList<ChartJsDocsAction> NoActions { get; } = [];

    protected bool RenderChart => controllerRegistered;

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new DerivedBubbleChartJsConfig
                {
                    Data = new ChartJsData
                    {
                        Datasets =
                        [
                            new DerivedBubbleDataset
                            {
                                Label = "My First dataset",
                                BackgroundColor = "rgba(54, 162, 235, 0.5)",
                                BorderColor = "rgb(54, 162, 235)",
                                BorderWidth = 1,
                                BoxStrokeStyle = "red",
                                Data = RandomBubbles(7),
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
                                Text = "Derived Chart Type",
                            },
                        },
                    },
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                public sealed class DerivedBubbleChartJsConfig : ChartJsConfig
                {
                    public new string Type { get; set; } = "derivedBubble";
                }

                public sealed record DerivedBubbleDataset : BubbleDataset
                {
                    public string? BoxStrokeStyle { get; set; }
                }

                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/pax.BlazorChartJs.samplelib/derivedChartType.js");

                await module.InvokeVoidAsync(
                    "registerController",
                    ChartJsSetupOptions.Value.ChartJsLocation);
                """),
        ]);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                const config = {
                  type: 'derivedBubble',
                  data,
                  options: {
                    responsive: true,
                    plugins: {
                      title: {
                        display: true,
                        text: 'Derived Chart Type'
                      }
                    }
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                let controllerRegistrationPromise;

                export function registerController(chartJsLocation) {
                  controllerRegistrationPromise ??= registerControllerCore(chartJsLocation);
                  return controllerRegistrationPromise;
                }

                async function registerControllerCore(chartJsLocation) {
                  const chartJs = await import(chartJsLocation);
                  const Chart = chartJs.Chart ?? chartJs.default ?? globalThis.Chart;
                  const BubbleController = chartJs.BubbleController ?? Chart.registry.getController('bubble');

                  class DerivedBubbleController extends BubbleController {
                    draw() {
                      super.draw();

                      const firstPoint = this.getMeta().data[0];
                      if (!firstPoint) {
                        return;
                      }

                      const {x, y} = firstPoint.getProps(['x', 'y']);
                      const {radius} = firstPoint.options;
                      const ctx = this.chart.ctx;
                      ctx.save();
                      ctx.strokeStyle = this.options.boxStrokeStyle;
                      ctx.lineWidth = 1;
                      ctx.strokeRect(x - radius, y - radius, 2 * radius, 2 * radius);
                      ctx.restore();
                    }
                  }

                  DerivedBubbleController.id = 'derivedBubble';
                  DerivedBubbleController.defaults = {boxStrokeStyle: 'red'};
                  Chart.register(DerivedBubbleController);
                }
                """),
        ]);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !controllerRegistered && !registeringController)
        {
            registeringController = true;
            derivedChartTypeModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", DerivedChartTypeModule).ConfigureAwait(false);
            await derivedChartTypeModule.InvokeVoidAsync("registerController", ChartJsSetupOptions.Value.ChartJsLocation).ConfigureAwait(false);
            controllerRegistered = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (derivedChartTypeModule is not null)
        {
            await derivedChartTypeModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    private static DerivedBubbleChartJsConfig CreateDerivedChartTypeConfig()
    {
        return new DerivedBubbleChartJsConfig
        {
            Data = new ChartJsData
            {
                Datasets =
                [
                    new DerivedBubbleDataset
                    {
                        Label = "My First dataset",
                        BackgroundColor = BlueTransparent,
                        BorderColor = Blue,
                        BorderWidth = 1,
                        BoxStrokeStyle = "red",
                        Data = RandomBubbles(),
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
                        Text = "Derived Chart Type",
                    },
                },
            },
        };
    }

    private static List<object> RandomBubbles()
    {
        List<object> data = new(DataCount);

        for (var i = 0; i < DataCount; i++)
        {
            data.Add(new BubbleDataPoint
            {
                X = Random.Shared.Next(-100, 101),
                Y = Random.Shared.Next(-100, 101),
                R = Random.Shared.Next(1, 21),
            });
        }

        return data;
    }
}

public sealed class DerivedBubbleChartJsConfig : ChartJsConfig
{
    public new string Type { get; set; } = "derivedBubble";
}

public sealed record DerivedBubbleDataset : BubbleDataset
{
    public string? BoxStrokeStyle { get; set; }
}
