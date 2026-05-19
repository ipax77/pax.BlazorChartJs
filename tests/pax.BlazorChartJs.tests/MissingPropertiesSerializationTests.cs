using System.Text.Json;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class MissingPropertiesSerializationTests
{
    [TestMethod]
    public void DatasetCommonMissingPropertiesSerializeFromBaseDataset()
    {
        BarDataset dataset = new()
        {
            Animation = false,
            Animations = new Animations()
            {
                X = false
            },
            Normalized = true,
            Transitions = new Dictionary<string, object>()
            {
                ["show"] = false
            }
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.IsFalse(root.GetProperty("animation").GetBoolean());
        Assert.IsFalse(root.GetProperty("animations").GetProperty("x").GetBoolean());
        Assert.IsTrue(root.GetProperty("normalized").GetBoolean());
        Assert.IsFalse(root.GetProperty("transitions").GetProperty("show").GetBoolean());
    }

    [TestMethod]
    public void BubbleAxisAndStackPropertiesSerializeWithChartJsPropertyNames()
    {
        BubbleDataset dataset = new()
        {
            IndexAxis = "y",
            Stack = "revenue",
            XAxisID = "x1",
            YAxisID = "y1"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual("y", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("revenue", root.GetProperty("stack").GetString());
        Assert.AreEqual("x1", root.GetProperty("xAxisID").GetString());
        Assert.AreEqual("y1", root.GetProperty("yAxisID").GetString());
    }

    [TestMethod]
    public void PieArcPropertiesSerializeScalarsAndScriptableMarkers()
    {
        PieDataset dataset = new()
        {
            BorderDash = ChartJsFunction.FromName("resolveArcDash"),
            BorderDashOffset = 2,
            Circular = true,
            HoverBorderDash = ChartJsFunction.FromName("resolveHoverArcDash"),
            HoverBorderDashOffset = 3,
            IndexAxis = "x",
            Label = "Doughnut Dataset",
            Order = 4,
            SelfJoin = false,
            Stack = "rings"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual("resolveArcDash", GetMarkerName(root.GetProperty("borderDash")));
        Assert.AreEqual(2, root.GetProperty("borderDashOffset").GetDouble());
        Assert.IsTrue(root.GetProperty("circular").GetBoolean());
        Assert.AreEqual("resolveHoverArcDash", GetMarkerName(root.GetProperty("hoverBorderDash")));
        Assert.AreEqual(3, root.GetProperty("hoverBorderDashOffset").GetDouble());
        Assert.AreEqual("x", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("Doughnut Dataset", root.GetProperty("label").GetString());
        Assert.AreEqual(4, root.GetProperty("order").GetInt32());
        Assert.IsFalse(root.GetProperty("selfJoin").GetBoolean());
        Assert.AreEqual("rings", root.GetProperty("stack").GetString());
    }

    [TestMethod]
    public void PolarAreaArcPropertiesSerializeScalarsAndScriptableMarkers()
    {
        PolarAreaDataset dataset = new()
        {
            Angle = 90,
            BorderDash = ChartJsFunction.FromName("resolvePolarDash"),
            BorderDashOffset = 1.5,
            BorderJoinStyle = "round",
            BorderRadius = ChartJsFunction.FromName("resolvePolarRadius"),
            Circumference = 180,
            HoverBorderDash = ChartJsFunction.FromName("resolvePolarHoverDash"),
            HoverBorderDashOffset = 2.5,
            HoverOffset = 8,
            IndexAxis = "r",
            Label = "Polar Dataset",
            Offset = 6,
            Order = 7,
            Rotation = 45,
            SelfJoin = true,
            Spacing = 3,
            Stack = "polar",
            Weight = 2
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual(90, root.GetProperty("angle").GetDouble());
        Assert.AreEqual("resolvePolarDash", GetMarkerName(root.GetProperty("borderDash")));
        Assert.AreEqual(1.5, root.GetProperty("borderDashOffset").GetDouble());
        Assert.AreEqual("round", root.GetProperty("borderJoinStyle").GetString());
        Assert.AreEqual("resolvePolarRadius", GetMarkerName(root.GetProperty("borderRadius")));
        Assert.AreEqual(180, root.GetProperty("circumference").GetDouble());
        Assert.AreEqual("resolvePolarHoverDash", GetMarkerName(root.GetProperty("hoverBorderDash")));
        Assert.AreEqual(2.5, root.GetProperty("hoverBorderDashOffset").GetDouble());
        Assert.AreEqual(8, root.GetProperty("hoverOffset").GetDouble());
        Assert.AreEqual("r", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("Polar Dataset", root.GetProperty("label").GetString());
        Assert.AreEqual(6, root.GetProperty("offset").GetDouble());
        Assert.AreEqual(7, root.GetProperty("order").GetInt32());
        Assert.AreEqual(45, root.GetProperty("rotation").GetDouble());
        Assert.IsTrue(root.GetProperty("selfJoin").GetBoolean());
        Assert.AreEqual(3, root.GetProperty("spacing").GetDouble());
        Assert.AreEqual("polar", root.GetProperty("stack").GetString());
        Assert.AreEqual(2, root.GetProperty("weight").GetDouble());
    }

    [TestMethod]
    public void RadarUnprefixedLineAndPointPropertiesSerialize()
    {
        RadarDataset dataset = new()
        {
            CapBezierPoints = true,
            CubicInterpolationMode = "monotone",
            DrawActiveElementsOnTop = false,
            HitRadius = 5,
            HoverRadius = ChartJsFunction.FromName("resolveHoverRadius"),
            IndexAxis = "x",
            Radius = 4,
            Rotation = 45,
            Segment = new { borderColor = "red" },
            ShowLine = true,
            Stack = "radar",
            Stepped = "middle",
            XAxisID = "xRadar",
            YAxisID = "yRadar"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.IsTrue(root.GetProperty("capBezierPoints").GetBoolean());
        Assert.AreEqual("monotone", root.GetProperty("cubicInterpolationMode").GetString());
        Assert.IsFalse(root.GetProperty("drawActiveElementsOnTop").GetBoolean());
        Assert.AreEqual(5, root.GetProperty("hitRadius").GetDouble());
        Assert.AreEqual("resolveHoverRadius", GetMarkerName(root.GetProperty("hoverRadius")));
        Assert.AreEqual("x", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual(4, root.GetProperty("radius").GetDouble());
        Assert.AreEqual(45, root.GetProperty("rotation").GetDouble());
        Assert.AreEqual("red", root.GetProperty("segment").GetProperty("borderColor").GetString());
        Assert.IsTrue(root.GetProperty("showLine").GetBoolean());
        Assert.AreEqual("radar", root.GetProperty("stack").GetString());
        Assert.AreEqual("middle", root.GetProperty("stepped").GetString());
        Assert.AreEqual("xRadar", root.GetProperty("xAxisID").GetString());
        Assert.AreEqual("yRadar", root.GetProperty("yAxisID").GetString());
    }

    [TestMethod]
    public void AxisTooltipAndLegendLabelMissingPropertiesSerialize()
    {
        LinearAxis axis = new()
        {
            Clip = true
        };
        Tooltip tooltip = new()
        {
            Animation = false,
            Animations = false,
            Axis = "xy",
            IncludeInvisible = true
        };
        Labels labels = new()
        {
            BorderRadius = 6,
            UseBorderRadius = true
        };

        using var axisDocument = SerializeToDocument(axis);
        using var tooltipDocument = SerializeToDocument(tooltip);
        using var labelsDocument = SerializeToDocument(labels);

        Assert.IsTrue(axisDocument.RootElement.GetProperty("clip").GetBoolean());
        Assert.IsFalse(tooltipDocument.RootElement.GetProperty("animation").GetBoolean());
        Assert.IsFalse(tooltipDocument.RootElement.GetProperty("animations").GetBoolean());
        Assert.AreEqual("xy", tooltipDocument.RootElement.GetProperty("axis").GetString());
        Assert.IsTrue(tooltipDocument.RootElement.GetProperty("includeInvisible").GetBoolean());
        Assert.AreEqual(6, labelsDocument.RootElement.GetProperty("borderRadius").GetDouble());
        Assert.IsTrue(labelsDocument.RootElement.GetProperty("useBorderRadius").GetBoolean());
    }

    private static JsonDocument SerializeToDocument<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonDocument.Parse(json);
    }

    private static string? GetMarkerName(JsonElement element)
    {
        return element.GetProperty("__chartJsFunction").GetString();
    }
}
