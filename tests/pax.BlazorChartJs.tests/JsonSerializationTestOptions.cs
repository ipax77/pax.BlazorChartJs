using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs.tests;

internal static class JsonSerializationTestOptions
{
    public static readonly JsonSerializerOptions Default = CreateDefaultOptions();

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter<ChartType>(),
                new JsonStringEnumConverter<ChartJsEventType>(),
                new JsonStringEnumConverter<ChartJsEventSource>(),
                CreateConverter("IndexableOptionStringConverter"),
                CreateConverter("IndexableOptionDoubleConverter"),
                CreateConverter("IndexableOptionIntConverter"),
                CreateConverter("IndexableOptionBoolConverter"),
                CreateConverter("IndexableOptionObjectConverter"),
                CreateConverter("IndexableOptionFontConverter"),
                CreateConverter("PaddingJsonConverter"),
                CreateConverter("ChartJsDatasetJsonConverter"),
                CreateConverter("ChartJsAxisJsonConverter"),
                CreateConverter("ChartJsAxisTickJsonConverter")
            }
        };
    }

    private static JsonConverter CreateConverter(string converterName)
    {
        var converterType = typeof(ChartJsFunction).Assembly.GetType($"pax.BlazorChartJs.{converterName}")
            ?? throw new InvalidOperationException($"Missing converter type: {converterName}.");

        return (JsonConverter)(Activator.CreateInstance(converterType)
            ?? throw new InvalidOperationException($"Could not create converter: {converterName}."));
    }
}
