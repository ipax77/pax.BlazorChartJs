
using System.Text.Json;
using System.Text.Json.Nodes;

namespace pax.BlazorChartJs;

public sealed partial class ChartJsInterop
{
    internal static int SerializeConfigForBenchmark(ChartJsConfig config)
    {
        var serializedConfig = SerializeConfigToJsonObject(config, CreateJsonSerializerOptions());
        return serializedConfig?.Count ?? 0;
    }

    internal static bool ContainsChartJsFunctionMarkerForBenchmark(string json)
    {
        return ContainsChartJsFunctionMarker(json);
    }

    internal static JsonSerializerOptions CreateBenchmarkJsonSerializerOptions()
    {
        return CreateJsonSerializerOptions();
    }

    private static JsonObject? SerializeConfigToJsonObject(ChartJsConfig config, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize<object>(config, options) ?? throw new ArgumentNullException(nameof(config));
        return JsonSerializer.Deserialize<JsonObject>(json);
    }
}