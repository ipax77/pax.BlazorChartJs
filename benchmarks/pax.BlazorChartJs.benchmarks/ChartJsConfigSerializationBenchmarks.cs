using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using pax.BlazorChartJs;

namespace pax.BlazorChartJs.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ChartJsConfigSerializationBenchmarks
{
    private const int PointCount = 100_000;
    private const long PointIntervalMilliseconds = 30_000;
    private const string Red = "rgb(255, 99, 132)";
    private static readonly DateTimeOffset Start = new(2021, 4, 1, 0, 0, 0, TimeSpan.Zero);

    private ChartJsConfig config = default!;
    private JsonSerializerOptions nonIndentedDatasetOptions = default!;

    [GlobalSetup]
    public void Setup()
    {
        config = CreateDataDecimationConfig();
        nonIndentedDatasetOptions = CreateNonIndentedDatasetOptions();
    }

    [Benchmark(Baseline = true)]
    public int CurrentInteropPath()
    {
        return ChartJsInterop.SerializeConfigForBenchmark(config);
    }

    [Benchmark]
    public int SourceGeneratedContextPath()
    {
        var json = JsonSerializer.Serialize(config, BenchmarkChartJsJsonContext.Default.ChartJsConfig);
        return GetPayloadLengthWithMarkerScan(json);
    }

    [Benchmark]
    public int NonIndentedDatasetConverterPath()
    {
        var json = JsonSerializer.Serialize<object>(config, nonIndentedDatasetOptions);
        return GetPayloadLengthWithMarkerScan(json);
    }

    private static int GetPayloadLengthWithMarkerScan(string json)
    {
        return json.Length + (ChartJsInterop.ContainsChartJsFunctionMarkerForBenchmark(json) ? 1 : 0);
    }

    private static JsonSerializerOptions CreateNonIndentedDatasetOptions()
    {
        var options = ChartJsInterop.CreateBenchmarkJsonSerializerOptions();

        for (var i = 0; i < options.Converters.Count; i++)
        {
            if (options.Converters[i] is ChartJsDatasetJsonConverter)
            {
                options.Converters.RemoveAt(i);
                break;
            }
        }

        options.Converters.Add(new NonIndentedDatasetJsonConverter());
        return options;
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
                        Label = "Large Dataset",
                        BorderColor = Red,
                        BorderWidth = 1,
                        PointRadius = 0,
                        Data = CreatePointData(),
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

    private static object[] CreatePointData()
    {
        var start = Start.ToUnixTimeMilliseconds();
        var seed = 10u;
        object[] pointData = new object[PointCount];

        for (var i = 0; i < PointCount; i++)
        {
            var max = NextUnit(ref seed) < 0.001 ? 100 : 20;
            pointData[i] = new DataPoint
            {
                X = start + (i * PointIntervalMilliseconds),
                Y = NextUnit(ref seed) * max,
            };
        }

        return pointData;
    }

    private static double NextUnit(ref uint seed)
    {
        seed = unchecked((1664525u * seed) + 1013904223u);
        return seed / (double)uint.MaxValue;
    }

    private sealed class NonIndentedDatasetJsonConverter : JsonConverter<ChartJsDataset?>
    {
        private static readonly JsonSerializerOptions WriteOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter<ChartType>(),
                new JsonStringEnumConverter<ChartJsEventType>(),
                new JsonStringEnumConverter<ChartJsEventSource>(),
                new IndexableOptionStringConverter(),
                new IndexableOptionDoubleConverter(),
                new IndexableOptionIntConverter(),
                new IndexableOptionBoolConverter(),
                new IndexableOptionObjectConverter(),
                new IndexableOptionFontConverter(),
                new PaddingJsonConverter(),
                new StringOrDoubleValueConverter(),
            },
        };

        public override ChartJsDataset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ChartJsDataset? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                return;
            }

            writer.WriteRawValue(JsonSerializer.Serialize<object>(value, WriteOptions), true);
        }
    }
}
