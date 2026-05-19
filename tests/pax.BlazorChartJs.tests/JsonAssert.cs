using System.Text.Json;
using System.Text.Json.Nodes;

namespace pax.BlazorChartJs.tests;

internal static class JsonAssert
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = false
    };

    public static void AreEquivalent(JsonNode expected, JsonNode actual)
    {
        var expectedJson = Canonicalize(expected).ToJsonString(WriteOptions);
        var actualJson = Canonicalize(actual).ToJsonString(WriteOptions);

        Assert.AreEqual(expectedJson, actualJson);
    }

    private static JsonNode Canonicalize(JsonNode node)
    {
        return node switch
        {
            JsonObject jsonObject => CanonicalizeObject(jsonObject),
            JsonArray jsonArray => CanonicalizeArray(jsonArray),
            _ => JsonNode.Parse(node.ToJsonString(WriteOptions))
                ?? throw new InvalidOperationException("Could not clone JSON value.")
        };
    }

    private static JsonObject CanonicalizeObject(JsonObject jsonObject)
    {
        JsonObject sorted = [];

        foreach (var property in jsonObject.OrderBy(property => property.Key, StringComparer.Ordinal))
        {
            sorted[property.Key] = property.Value is null ? null : Canonicalize(property.Value);
        }

        return sorted;
    }

    private static JsonArray CanonicalizeArray(JsonArray jsonArray)
    {
        JsonArray canonicalArray = [];

        foreach (var item in jsonArray)
        {
            canonicalArray.Add(item is null ? null : Canonicalize(item));
        }

        return canonicalArray;
    }
}
