using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Serialization,
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
        typeof(PaddingJsonConverter),
        typeof(ChartJsDatasetJsonConverter),
        typeof(ChartJsAxisJsonConverter),
        typeof(ChartJsAxisTickJsonConverter)
    }
)]

// This context defines all *built-in* ChartJs types known at compile-time.
[JsonSerializable(typeof(ChartJsConfig))]
[JsonSerializable(typeof(ChartJsDataset))]
[JsonSerializable(typeof(ChartJsDatasetTooltip))]
[JsonSerializable(typeof(ChartJsAxis))]
[JsonSerializable(typeof(ChartJsAxisTick))]
[JsonSerializable(typeof(IndexableOption<string>))]
[JsonSerializable(typeof(IndexableOption<double>))]
[JsonSerializable(typeof(IndexableOption<int>))]
[JsonSerializable(typeof(IndexableOption<bool>))]
[JsonSerializable(typeof(IndexableOption<object>))]
[JsonSerializable(typeof(StringOrDoubleValue))]
[JsonSerializable(typeof(ChartJsOptions))]
[JsonSerializable(typeof(Plugins))]
[JsonSerializable(typeof(ChartJsData))]
[JsonSerializable(typeof(AddDataObject))]
[JsonSerializable(typeof(SetDataObject))]
[JsonSerializable(typeof(CartesianAxisTick))]
[JsonSerializable(typeof(ChartJsAxisBorder))]
[JsonSerializable(typeof(ChartJsFunction))]

// Datasets
[JsonSerializable(typeof(BarDataset))]
[JsonSerializable(typeof(BubbleDataset))]
[JsonSerializable(typeof(LineDataset))]
[JsonSerializable(typeof(PieDataset))]
[JsonSerializable(typeof(DoughnutDataset))]
[JsonSerializable(typeof(PolarAreaDataset))]
[JsonSerializable(typeof(RadarDataset))]
[JsonSerializable(typeof(ScatterDataset))]

// Axis
[JsonSerializable(typeof(ChartJsGrid))]
[JsonSerializable(typeof(ChartJsAxisBorder))]
[JsonSerializable(typeof(AngleLines))]
[JsonSerializable(typeof(CartesianAxis))]
[JsonSerializable(typeof(TimeCartesianAxis))]
[JsonSerializable(typeof(TimeCartesianAxisTicks))]
[JsonSerializable(typeof(TimeCartesianAxisTime))]
[JsonSerializable(typeof(TimeSeriesAxis))]
[JsonSerializable(typeof(LinearAxis))]
[JsonSerializable(typeof(LinearAxisTick))]
[JsonSerializable(typeof(LinearRadialAxis))]
[JsonSerializable(typeof(PointLabels))]

// Options
[JsonSerializable(typeof(Animation))]
[JsonSerializable(typeof(Animations))]
[JsonSerializable(typeof(ChartJsLayout))]
[JsonSerializable(typeof(ChartJsOptionsScales))]
[JsonSerializable(typeof(Font))]
[JsonSerializable(typeof(Interactions))]
[JsonSerializable(typeof(Labels))]
[JsonSerializable(typeof(Legend))]
[JsonSerializable(typeof(Padding))]
[JsonSerializable(typeof(Title))]
[JsonSerializable(typeof(Tooltip))]
[JsonSerializable(typeof(TooltipCallbacks))]

// Plugins
[JsonSerializable(typeof(ArbitraryLineConfig))]
[JsonSerializable(typeof(BarAvatarConfig))]
[JsonSerializable(typeof(DataLabelsConfig))]
[JsonSerializable(typeof(DecimationConfig))]

// Enums
[JsonSerializable(typeof(ChartType))]
[JsonSerializable(typeof(ChartJsEventType))]
[JsonSerializable(typeof(ChartJsEventSource))]

// Defaults
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]

// preperation for AOT friendly version
internal sealed partial class ChartJsJsonContext : JsonSerializerContext
{
    public static JsonSerializerOptions CreateDefaultOptions()
    {
        return new()
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
                        new PaddingJsonConverter(),
                        new ChartJsDatasetJsonConverter(),
                        new ChartJsAxisJsonConverter(),
                        new ChartJsAxisTickJsonConverter(),
                    }
        };
    }
}
