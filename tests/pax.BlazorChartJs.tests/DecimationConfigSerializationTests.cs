using System.Text.Json;
using System.Text.Json.Nodes;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class DecimationConfigSerializationTests
{
    [TestMethod]
    [DataRow("lttb")]
    [DataRow("min-max")]
    public void DecimationConfigSerializesUnderPlugins(string algorithm)
    {
        ChartJsOptions options = new()
        {
            Parsing = false,
            Plugins = new Plugins
            {
                Decimation = new DecimationConfig
                {
                    Enabled = true,
                    Algorithm = algorithm,
                    Samples = 500,
                    Threshold = 2000
                }
            }
        };

        var actual = SerializeToNode(options);
        var expected = ParseNode(
            $$"""
            {
              "plugins": {
                "decimation": {
                  "enabled": true,
                  "algorithm": "{{algorithm}}",
                  "samples": 500,
                  "threshold": 2000
                }
              },
              "parsing": false
            }
            """);

        JsonAssert.AreEquivalent(expected, actual);
    }

    [TestMethod]
    public void DatasetParsingSerializesWhenSet()
    {
        using var document = SerializeToDocument(new LineDataset
        {
            Parsing = false
        });

        Assert.IsFalse(document.RootElement.GetProperty("parsing").GetBoolean());
    }

    private static JsonNode SerializeToNode<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonNode.Parse(json) ?? throw new InvalidOperationException("Serialized JSON was empty.");
    }

    private static JsonDocument SerializeToDocument<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonDocument.Parse(json);
    }

    private static JsonNode ParseNode(string json)
    {
        return JsonNode.Parse(json) ?? throw new InvalidOperationException("Expected JSON was empty.");
    }
}
