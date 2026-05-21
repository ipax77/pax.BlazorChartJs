using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Buffers.Binary;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Advanced;

public sealed partial class ChartJsAdvancedDataDecimationSample : ChartJsAdvancedDataDecimationSampleBase
{
}

public abstract class ChartJsAdvancedDataDecimationSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const int PointCount = 100_000;
    private const long PointIntervalMilliseconds = 30000;
    private const string TimeAdapterModule = "./_content/pax.BlazorChartJs.samplelib/timeChart.js";
    private const string LargeDatasetId = "large-dataset";
    private const string Red = "rgb(255, 99, 132)";
    private static readonly DateTimeOffset Start = new(2021, 4, 1, 0, 0, 0, TimeSpan.Zero);

    private ChartJsConfig config = new();
    private IJSObjectReference? timeAdapterModule;
    private IReadOnlyList<ChartJsDocsAction> actions = [];
    private bool loadingTimeAdapter;
    private bool timeAdapterReady;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IOptions<ChartJsSetupOptions> ChartJsSetupOptions { get; set; } = default!;

    protected ChartJsConfig Config => config;

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    protected bool RenderChart => timeAdapterReady;

    protected static ChartJsDocsCodeSet CSharpCode { get; } = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Animation = false,
                Parsing = false,
                Interaction = new Interactions { Mode = "nearest", Axis = "x", Intersect = false },
                Plugins = new Plugins
                {
                    Decimation = new DecimationConfig
                    {
                        Enabled = false,
                        Algorithm = "min-max",
                    },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new TimeCartesianAxis
                    {
                        Type = "time",
                        Ticks = new TimeCartesianAxisTicks
                        {
                            Source = "auto",
                            MaxRotation = 0,
                            AutoSkip = true,
                        },
                    },
                },
            },
        };
        """,
        """
        const int PointCount = 100000;
        const long PointIntervalMilliseconds = 30000;
        var start = new DateTimeOffset(2021, 4, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var seed = 10u;
        var pointData = GC.AllocateUninitializedArray<byte>(PointCount * 2 * sizeof(double));
        var bytes = pointData.AsSpan();

        for (var i = 0; i < PointCount; i++)
        {
            var max = NextUnit(ref seed) < 0.001 ? 100 : 20;
            var x = start + (i * PointIntervalMilliseconds);
            var y = NextUnit(ref seed) * max;
            var offset = i * 2 * sizeof(double);

            BinaryPrimitives.WriteDoubleLittleEndian(bytes.Slice(offset, sizeof(double)), x);
            BinaryPrimitives.WriteDoubleLittleEndian(bytes.Slice(offset + sizeof(double), sizeof(double)), y);
        }

        static double NextUnit(ref uint seed)
        {
            seed = unchecked((1664525u * seed) + 1013904223u);
            return seed / (double)uint.MaxValue;
        }

        var data = new ChartJsData
        {
            Datasets =
            [
                new LineDataset
                {
                    Id = "large-dataset",
                    Label = "Large Dataset",
                    BorderColor = "rgb(255, 99, 132)",
                    BorderWidth = 1,
                    PointRadius = 0,
                    Data = [],
                },
            ],
        };

        config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = "large-dataset",
            Bytes = pointData,
            Count = PointCount,
            Format = ChartJsBinaryDataFormat.Float64XY,
        });
        """,
        """
        void DisableDecimation()
        {
            config.Options!.Plugins!.Decimation!.Enabled = false;
            config.UpdateChartOptions();
        }

        void EnableMinMaxDecimation()
        {
            var decimation = config.Options!.Plugins!.Decimation!;
            decimation.Enabled = true;
            decimation.Algorithm = "min-max";
            decimation.Samples = null;
            config.UpdateChartOptions();
        }

        void EnableLttbDecimation(double samples)
        {
            var decimation = config.Options!.Plugins!.Decimation!;
            decimation.Enabled = true;
            decimation.Algorithm = "lttb";
            decimation.Samples = samples;
            config.UpdateChartOptions();
        }
        """);

    protected static ChartJsDocsCodeSet JavaScriptCode { get; } = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            animation: false,
            parsing: false,
            interaction: {
              mode: 'nearest',
              axis: 'x',
              intersect: false
            },
            plugins: {
              decimation
            },
            scales: {
              x: {
                type: 'time',
                ticks: {
                  source: 'auto',
                  maxRotation: 0,
                  autoSkip: true
                }
              }
            }
          }
        };
        """,
        """
        const decimation = {
          enabled: false,
          algorithm: 'min-max'
        };

        const NUM_POINTS = 100000;
        Utils.srand(10);

        const start = Utils.parseISODate('2021-04-01T00:00:00Z').toMillis();
        const pointData = [];
        for (let i = 0; i < NUM_POINTS; ++i) {
          const max = Math.random() < 0.001 ? 100 : 20;
          pointData.push({x: start + (i * 30000), y: Utils.rand(0, max)});
        }

        const data = {
          datasets: [{
            borderColor: Utils.CHART_COLORS.red,
            borderWidth: 1,
            data: [],
            label: 'Large Dataset',
            radius: 0
          }]
        };

        // Blazor passes byte[] to JavaScript as Uint8Array.
        setDatasetBinaryData(chart.id, 'large-dataset', pointData, NUM_POINTS, 'Float64XY');
        """,
        """
        const actions = [
          {
            name: 'No decimation (default)',
            handler(chart) {
              chart.options.plugins.decimation.enabled = false;
              chart.update();
            }
          },
          {
            name: 'min-max decimation',
            handler(chart) {
              chart.options.plugins.decimation.algorithm = 'min-max';
              chart.options.plugins.decimation.enabled = true;
              chart.update();
            }
          },
          {
            name: 'LTTB decimation (50 samples)',
            handler(chart) {
              chart.options.plugins.decimation.algorithm = 'lttb';
              chart.options.plugins.decimation.enabled = true;
              chart.options.plugins.decimation.samples = 50;
              chart.update();
            }
          },
          {
            name: 'LTTB decimation (500 samples)',
            handler(chart) {
              chart.options.plugins.decimation.algorithm = 'lttb';
              chart.options.plugins.decimation.enabled = true;
              chart.options.plugins.decimation.samples = 500;
              chart.update();
            }
          }
        ];
        """);

    protected override void OnInitialized()
    {
        actions =
        [
            CreateAction("no-decimation", "No decimation (default)", DisableDecimation),
            CreateAction("min-max", "min-max decimation", EnableMinMaxDecimation),
            CreateAction("lttb-50", "LTTB decimation (50 samples)", () => EnableLttbDecimation(50)),
            CreateAction("lttb-500", "LTTB decimation (500 samples)", () => EnableLttbDecimation(500)),
        ];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !timeAdapterReady && !loadingTimeAdapter)
        {
            loadingTimeAdapter = true;
            timeAdapterModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", TimeAdapterModule).ConfigureAwait(false);
            await timeAdapterModule.InvokeVoidAsync("registerPlugin", ChartJsSetupOptions.Value.ChartJsLocation).ConfigureAwait(false);
            config = CreateDataDecimationConfig();
            timeAdapterReady = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (timeAdapterModule is not null)
        {
            await timeAdapterModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    protected async Task ChartEventTriggered(ChartJsEvent chartEvent)
    {
        if (chartEvent is not ChartJsInitEvent initEvent
            || initEvent.ChartJsConfigGuid != Config.ChartJsConfigGuid)
        {
            return;
        }

        var pointData = CreatePointDataBytes();
        Config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = LargeDatasetId,
            Bytes = pointData,
            Count = PointCount,
            Format = ChartJsBinaryDataFormat.Float64XY
        });

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private void DisableDecimation()
    {
        var decimation = GetDecimationConfig();
        decimation.Enabled = false;
        Config.UpdateChartOptions();
    }

    private void EnableMinMaxDecimation()
    {
        var decimation = GetDecimationConfig();
        decimation.Enabled = true;
        decimation.Algorithm = "min-max";
        decimation.Samples = null;
        Config.UpdateChartOptions();
    }

    private void EnableLttbDecimation(double samples)
    {
        var decimation = GetDecimationConfig();
        decimation.Enabled = true;
        decimation.Algorithm = "lttb";
        decimation.Samples = samples;
        Config.UpdateChartOptions();
    }

    private DecimationConfig GetDecimationConfig()
    {
        Config.Options ??= new ChartJsOptions();
        Config.Options.Plugins ??= new Plugins();
        Config.Options.Plugins.Decimation ??= new DecimationConfig();
        return Config.Options.Plugins.Decimation;
    }

    private static ChartJsConfig CreateDataDecimationConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Datasets =
                [
                    new LineDataset
                    {
                        Id = LargeDatasetId,
                        Label = "Large Dataset",
                        BorderColor = Red,
                        BorderWidth = 1,
                        PointRadius = 0,
                        Data = [],
                    },
                ],
            },
            Options = new ChartJsOptions
            {
                Animation = false,
                Parsing = false,
                Interaction = new Interactions
                {
                    Mode = "nearest",
                    Axis = "x",
                    Intersect = false,
                },
                Plugins = new Plugins
                {
                    Decimation = new DecimationConfig
                    {
                        Enabled = false,
                        Algorithm = "min-max",
                    },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new TimeCartesianAxis
                    {
                        Type = "time",
                        Ticks = new TimeCartesianAxisTicks
                        {
                            Source = "auto",
                            MaxRotation = 0,
                            AutoSkip = true,
                        },
                    },
                },
            },
        };
    }

    private static byte[] CreatePointDataBytes()
    {
        var start = Start.ToUnixTimeMilliseconds();
        var seed = 10u;
        var pointData = GC.AllocateUninitializedArray<byte>(PointCount * 2 * sizeof(double));
        var bytes = pointData.AsSpan();

        for (var i = 0; i < PointCount; i++)
        {
            var max = NextUnit(ref seed) < 0.001 ? 100 : 20;
            var x = start + (i * PointIntervalMilliseconds);
            var y = NextUnit(ref seed) * max;
            var offset = i * 2 * sizeof(double);

            BinaryPrimitives.WriteDoubleLittleEndian(bytes.Slice(offset, sizeof(double)), x);
            BinaryPrimitives.WriteDoubleLittleEndian(bytes.Slice(offset + sizeof(double), sizeof(double)), y);
        }

        return pointData;
    }

    private static double NextUnit(ref uint seed)
    {
        seed = unchecked((1664525u * seed) + 1013904223u);
        return seed / (double)uint.MaxValue;
    }
}
