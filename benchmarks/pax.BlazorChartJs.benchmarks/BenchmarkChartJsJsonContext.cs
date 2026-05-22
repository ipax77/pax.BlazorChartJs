using System.Text.Json.Serialization;
using pax.BlazorChartJs;

namespace pax.BlazorChartJs.Benchmarks;

// Keep metadata-heavy source generation in the non-packable benchmark assembly.
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization,
    Converters = new[]
    {
        typeof(JsonStringEnumConverter<ChartType>),
        typeof(JsonStringEnumConverter<ChartJsEventType>),
        typeof(JsonStringEnumConverter<ChartJsEventSource>),
        typeof(IndexableOptionStringConverter),
        typeof(IndexableOptionDoubleConverter),
        typeof(IndexableOptionIntConverter),
        typeof(IndexableOptionBoolConverter),
        typeof(IndexableOptionObjectConverter),
        typeof(IndexableOptionFontConverter),
        typeof(PaddingJsonConverter),
        typeof(StringOrDoubleValueConverter),
        typeof(ChartJsDatasetJsonConverter),
        typeof(ChartJsAxisJsonConverter),
        typeof(ChartJsAxisTickJsonConverter)
    }
)]
[JsonSerializable(typeof(ChartJsConfig))]
internal sealed partial class BenchmarkChartJsJsonContext : JsonSerializerContext
{
}
