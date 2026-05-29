using System.Text.Json;
using System.Text.Json.Nodes;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class RealWorldChartSerializationTests
{
    [TestMethod]
    public void MixedChartDocsSampleMatchesChartJsConfigShape()
    {
        var actual = NormalizeChartJsJson(SerializeToNode(CreateMixedDocsSampleConfig()));
        var expected = ParseNode(
            """
            {
              "type": "scatter",
              "data": {
                "labels": [
                  "January",
                  "February",
                  "March",
                  "April"
                ],
                "datasets": [{
                  "type": "bar",
                  "label": "Bar Dataset",
                  "data": [10, 20, 30, 40],
                  "borderColor": "rgb(255, 99, 132)",
                  "backgroundColor": "rgba(255, 99, 132, 0.2)"
                }, {
                  "type": "line",
                  "label": "Line Dataset",
                  "data": [50, 50, 50, 50],
                  "fill": false,
                  "borderColor": "rgb(54, 162, 235)"
                }]
              },
              "options": {
                "scales": {
                  "y": {
                    "beginAtZero": true
                  }
                }
              }
            }
            """);

        JsonAssert.AreEquivalent(expected, actual);
    }

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
    public void RadarChartDocsSampleMatchesChartJsDataShape()
    {
        var actual = NormalizeChartJsJson(SerializeToNode(CreateRadarDocsSampleConfig()));
        var expected = ParseNode(
            """
            {
              "type": "radar",
              "data": {
                "labels": [
                  "Eating",
                  "Drinking",
                  "Sleeping",
                  "Designing",
                  "Coding",
                  "Cycling",
                  "Running"
                ],
                "datasets": [{
                  "label": "My First Dataset",
                  "data": [65, 59, 90, 81, 56, 55, 40],
                  "fill": true,
                  "backgroundColor": "rgba(255, 99, 132, 0.2)",
                  "borderColor": "rgb(255, 99, 132)",
                  "pointBackgroundColor": "rgb(255, 99, 132)",
                  "pointBorderColor": "#fff",
                  "pointHoverBackgroundColor": "#fff",
                  "pointHoverBorderColor": "rgb(255, 99, 132)"
                }, {
                  "label": "My Second Dataset",
                  "data": [28, 48, 40, 19, 96, 27, 100],
                  "fill": true,
                  "backgroundColor": "rgba(54, 162, 235, 0.2)",
                  "borderColor": "rgb(54, 162, 235)",
                  "pointBackgroundColor": "rgb(54, 162, 235)",
                  "pointBorderColor": "#fff",
                  "pointHoverBackgroundColor": "#fff",
                  "pointHoverBorderColor": "rgb(54, 162, 235)"
                }]
              }
            }
            """);

        JsonAssert.AreEquivalent(expected, actual);
    }

    [TestMethod]
    public void AdvDataDecimationChartDocsSampleMatchesChartJsDataShape()
    {
        var actual = NormalizeChartJsJson(SerializeToNode(CreateAdvDataDecimationConfig()));
        var expected = ParseNode(
            """
            {
                "type": "line",
                "data": {
                    "datasets": [{
                        "label": "Large Dataset",
                        "borderColor": "red",
                        "borderWidth": 1,
                        "pointRadius": 0,
                        "data": []
                    }]
                },
                "options": {
                    "animation": false,
                    "parsing": false,
                    "interaction": {
                        "mode": "nearest",
                        "axis": "x",
                        "intersect": false
                    },
                    "plugins": {
                        "decimation": {
                            "enabled": false,
                            "algorithm": "min-max"
                        }
                    },
                    "scales": {
                        "x": {
                            "type": "time",
                            "ticks": {
                                "source": "auto",
                                "maxRotation": 0,
                                "autoSkip": true
                            }
                        }
                    }
                }
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

    private static ChartJsConfig CreateMixedDocsSampleConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.scatter,
            Data = new ChartJsData
            {
                Labels = ["January", "February", "March", "April"],
                Datasets =
                [
                    new BarDataset
                    {
                        Type = ChartType.bar,
                        Label = "Bar Dataset",
                        Data = [10, 20, 30, 40],
                        BorderColor = "rgb(255, 99, 132)",
                        BackgroundColor = "rgba(255, 99, 132, 0.2)"
                    },
                    new LineDataset
                    {
                        Type = ChartType.line,
                        Label = "Line Dataset",
                        Data = [50, 50, 50, 50],
                        Fill = false,
                        BorderColor = "rgb(54, 162, 235)"
                    }
                ]
            },
            Options = new ChartJsOptions
            {
                Scales = new ChartJsOptionsScales
                {
                    Y = new LinearAxis
                    {
                        BeginAtZero = true
                    }
                }
            }
        };
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

    private static ChartJsConfig CreateRadarDocsSampleConfig()
    {
        return new ChartJsConfig
        {
            Type = ChartType.radar,
            Data = new ChartJsData
            {
                Labels = ["Eating", "Drinking", "Sleeping", "Designing", "Coding", "Cycling", "Running"],
                Datasets =
                [
                    new RadarDataset
                    {
                        Label = "My First Dataset",
                        Data = [65, 59, 90, 81, 56, 55, 40],
                        Fill = true,
                        BackgroundColor = "rgba(255, 99, 132, 0.2)",
                        BorderColor = "rgb(255, 99, 132)",
                        PointBackgroundColor = "rgb(255, 99, 132)",
                        PointBorderColor = "#fff",
                        PointHoverBackgroundColor = "#fff",
                        PointHoverBorderColor = "rgb(255, 99, 132)"
                    },
                    new RadarDataset
                    {
                        Label = "My Second Dataset",
                        Data = [28, 48, 40, 19, 96, 27, 100],
                        Fill = true,
                        BackgroundColor = "rgba(54, 162, 235, 0.2)",
                        BorderColor = "rgb(54, 162, 235)",
                        PointBackgroundColor = "rgb(54, 162, 235)",
                        PointBorderColor = "#fff",
                        PointHoverBackgroundColor = "#fff",
                        PointHoverBorderColor = "rgb(54, 162, 235)"
                    }
                ]
            }
        };
    }

    private static ChartJsConfig CreateAdvDataDecimationConfig()
    {
        return new()
        {
            Type = ChartType.line,
            Data = new ChartJsData
            {
                Datasets =
                [
                    new LineDataset
                    {
                        Label = "Large Dataset",
                        BorderColor = "red",
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
