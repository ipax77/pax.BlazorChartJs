using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedDerivedAxisTypeSample : ChartJsAdvancedDerivedAxisTypeSampleBase
{
}

public abstract class ChartJsAdvancedDerivedAxisTypeSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const int DataCount = 12;
    private const string DerivedAxisModule = "./_content/pax.BlazorChartJs.samplelib/derivedAxisType.js";
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];

    private readonly ChartJsConfig config = CreateDerivedAxisConfig();
    private IJSObjectReference? derivedAxisModule;
    private bool registeringScale;
    private bool scaleRegistered;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IOptions<ChartJsSetupOptions> ChartJsSetupOptions { get; set; } = default!;

    protected ChartJsConfig Config => config;

    protected static IReadOnlyList<ChartJsDocsAction> NoActions { get; } = [];

    protected bool RenderChart => scaleRegistered;

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        [
            new ChartJsDocsCodeTab(
                ConfigTab,
                "Config",
                """
                var config = new ChartJsConfig
                {
                    Type = ChartType.line,
                    Data = new ChartJsData
                    {
                        Labels = MonthLabels,
                        Datasets =
                        [
                            new LineDataset
                            {
                                Label = "My First dataset",
                                Data = RandomNumbers(12, 0, 1000),
                                BorderColor = "rgb(255, 99, 132)",
                                BackgroundColor = "rgba(255, 99, 132, 0.5)",
                                Fill = false,
                            },
                        ],
                    },
                    Options = new ChartJsOptions
                    {
                        Responsive = true,
                        Scales = new ChartJsOptionsScales
                        {
                            X = new CartesianAxis { Display = true },
                            Y = new Log2Axis { Display = true },
                        },
                    },
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                public sealed record Log2Axis : LinearAxis
                {
                    public Log2Axis()
                    {
                        Type = "log2";
                    }
                }

                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/pax.BlazorChartJs.samplelib/derivedAxisType.js");

                await module.InvokeVoidAsync(
                    "registerScale",
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
                  type: 'line',
                  data,
                  options: {
                    responsive: true,
                    scales: {
                      x: { display: true },
                      y: { display: true, type: 'log2' }
                    }
                  }
                };
                """),
            new ChartJsDocsCodeTab(
                SetupTab,
                "Setup",
                """
                let scaleRegistrationPromise;

                export function registerScale(chartJsLocation) {
                  scaleRegistrationPromise ??= registerScaleCore(chartJsLocation);
                  return scaleRegistrationPromise;
                }

                async function registerScaleCore(chartJsLocation) {
                  const chartJs = await import(chartJsLocation);
                  const Chart = chartJs.Chart ?? chartJs.default ?? globalThis.Chart;
                  const LinearScale = chartJs.LinearScale ?? Chart.registry.getScale('linear');
                  const Scale = chartJs.Scale ?? Object.getPrototypeOf(LinearScale.prototype).constructor;

                  class Log2Axis extends Scale {
                    constructor(cfg) {
                      super(cfg);
                      this._startValue = undefined;
                      this._valueRange = 0;
                    }

                    parse(raw, index) {
                      const value = LinearScale.prototype.parse.call(this, raw, index);
                      return Number.isFinite(value) && value > 0 ? value : null;
                    }

                    determineDataLimits() {
                      const {min, max} = this.getMinMax(true);
                      this.min = Number.isFinite(min) ? Math.max(0, min) : null;
                      this.max = Number.isFinite(max) ? Math.max(0, max) : null;
                    }

                    buildTicks() {
                      const ticks = [];
                      let power = Math.floor(Math.log2(this.min || 1));
                      const maxPower = Math.ceil(Math.log2(this.max || 2));

                      while (power <= maxPower) {
                        ticks.push({value: Math.pow(2, power)});
                        power += 1;
                      }

                      this.min = ticks[0].value;
                      this.max = ticks[ticks.length - 1].value;
                      return ticks;
                    }

                    configure() {
                      const start = this.min;
                      super.configure();
                      this._startValue = Math.log2(start);
                      this._valueRange = Math.log2(this.max) - Math.log2(start);
                    }

                    getPixelForValue(value) {
                      if (value === undefined || value === 0) {
                        value = this.min;
                      }

                      return this.getPixelForDecimal(value === this.min
                        ? 0
                        : (Math.log2(value) - this._startValue) / this._valueRange);
                    }

                    getValueForPixel(pixel) {
                      const decimal = this.getDecimalForPixel(pixel);
                      return Math.pow(2, this._startValue + decimal * this._valueRange);
                    }
                  }

                  Log2Axis.id = 'log2';
                  Log2Axis.defaults = {};
                  Chart.register(Log2Axis);
                }
                """),
        ]);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !scaleRegistered && !registeringScale)
        {
            registeringScale = true;
            derivedAxisModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", DerivedAxisModule).ConfigureAwait(false);
            await derivedAxisModule.InvokeVoidAsync("registerScale", ChartJsSetupOptions.Value.ChartJsLocation).ConfigureAwait(false);
            scaleRegistered = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (derivedAxisModule is not null)
        {
            await derivedAxisModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    private static ChartJsConfig CreateDerivedAxisConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Labels = MonthLabels,
                Datasets =
                [
                    new LineDataset
                    {
                        Label = "My First dataset",
                        Data = RandomNumbers(),
                        BorderColor = Red,
                        BackgroundColor = RedTransparent,
                        Fill = false,
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis { Display = true },
                    Y = new Log2Axis { Display = true },
                },
            },
        };
    }

    private static List<object> RandomNumbers()
    {
        List<object> data = new(DataCount);

        for (var i = 0; i < DataCount; i++)
        {
            data.Add(Random.Shared.Next(0, 1001));
        }

        return data;
    }
}

public sealed record Log2Axis : LinearAxis
{
    public Log2Axis()
    {
        Type = "log2";
    }
}
