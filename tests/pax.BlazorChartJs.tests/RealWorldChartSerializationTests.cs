using System.Text.Json;
using System.Text.Json.Nodes;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class RealWorldChartSerializationTests
{
    [TestMethod]
    public void BubbleChartDocsSampleMatchesChartJsConfigShape()
    {
        var actual = NormalizeChartJsJson(SerializeToNode(CreateBubbleDocsSampleConfig()));
        var expected = ParseNode(
            """
            {
              "type": "bubble",
              "data": {
                "datasets": [{
                  "label": "First Dataset",
                  "data": [{
                    "x": 20,
                    "y": 30,
                    "r": 15
                  }, {
                    "x": 40,
                    "y": 10,
                    "r": 10
                  }],
                  "backgroundColor": "rgb(255, 99, 132)"
                }]
              },
              "options": {}
            }
            """);

        JsonAssert.AreEquivalent(expected, actual);
    }

    [TestMethod]
    public void BubbleDataPointSerializesUsingChartJsPointPropertyNames()
    {
        BubbleDataPoint point = new()
        {
            X = 20,
            Y = 30,
            R = 15
        };

        var actual = SerializeToNode(point);
        var expected = ParseNode("""{ "x": 20, "y": 30, "r": 15 }""");

        JsonAssert.AreEquivalent(expected, actual);
    }

    private static ChartJsConfig CreateBubbleDocsSampleConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.bubble,
            Data = new ChartJsData
            {
                Datasets =
                [
                    new BubbleDataset
                    {
                        Label = "First Dataset",
                        Data =
                        [
                            new BubbleDataPoint
                            {
                                X = 20,
                                Y = 30,
                                R = 15
                            },
                            new BubbleDataPoint
                            {
                                X = 40,
                                Y = 10,
                                R = 10
                            }
                        ],
                        BackgroundColor = "rgb(255, 99, 132)"
                    }
                ]
            },
            Options = new ChartJsOptions()
        };
    }

    private static JsonNode NormalizeChartJsJson(JsonNode node)
    {
        if (node["data"] is JsonObject data &&
            data["labels"] is JsonArray labels &&
            labels.Count == 0)
        {
            data.Remove("labels");
        }

        if (node["data"]?["datasets"] is JsonArray datasets)
        {
            foreach (var dataset in datasets.OfType<JsonObject>())
            {
                dataset.Remove("id");
            }
        }

        return node;
    }

    private static JsonNode SerializeToNode<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonNode.Parse(json) ?? throw new InvalidOperationException("Serialized JSON was empty.");
    }

    private static JsonNode ParseNode(string json)
    {
        return JsonNode.Parse(json) ?? throw new InvalidOperationException("Expected JSON was empty.");
    }
}
