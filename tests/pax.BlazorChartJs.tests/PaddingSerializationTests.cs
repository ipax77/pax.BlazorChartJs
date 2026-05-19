using System.Text.Json;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class PaddingSerializationTests
{
    [TestMethod]
    public void ConstructorPaddingSerializesAsDirectionalObjectForCompatibility()
    {
        using var document = SerializeToDocument(new Padding(20));
        var root = document.RootElement;

        Assert.AreEqual(JsonValueKind.Object, root.ValueKind);
        Assert.AreEqual(20, root.GetProperty("left").GetDouble());
        Assert.AreEqual(20, root.GetProperty("top").GetDouble());
        Assert.AreEqual(20, root.GetProperty("right").GetDouble());
        Assert.AreEqual(20, root.GetProperty("bottom").GetDouble());
    }

    [TestMethod]
    public void NumericPaddingSerializesAsNumber()
    {
        using var document = SerializeToDocument(Padding.FromNumber(20));

        Assert.AreEqual(JsonValueKind.Number, document.RootElement.ValueKind);
        Assert.AreEqual(20, document.RootElement.GetDouble());
    }

    [TestMethod]
    public void ImplicitNumericPaddingSerializesAsNumberInLayout()
    {
        ChartJsOptions options = new()
        {
            Layout = new ChartJsLayout()
            {
                Padding = 20
            }
        };

        using var document = SerializeToDocument(options);
        var padding = document.RootElement.GetProperty("layout").GetProperty("padding");

        Assert.AreEqual(JsonValueKind.Number, padding.ValueKind);
        Assert.AreEqual(20, padding.GetDouble());
    }

    [TestMethod]
    public void DirectionalPaddingSerializesCamelCaseProperties()
    {
        using var document = SerializeToDocument(new Padding()
        {
            Top = 20,
            Left = 8,
            Bottom = 4,
            Right = 30
        });

        var root = document.RootElement;
        Assert.AreEqual(20, root.GetProperty("top").GetDouble());
        Assert.AreEqual(8, root.GetProperty("left").GetDouble());
        Assert.AreEqual(4, root.GetProperty("bottom").GetDouble());
        Assert.AreEqual(30, root.GetProperty("right").GetDouble());
    }

    [TestMethod]
    public void ShorthandPaddingSerializesXYProperties()
    {
        using var document = SerializeToDocument(new Padding()
        {
            X = 10,
            Y = 4
        });

        var root = document.RootElement;
        Assert.AreEqual(10, root.GetProperty("x").GetDouble());
        Assert.AreEqual(4, root.GetProperty("y").GetDouble());
        Assert.IsFalse(root.TryGetProperty("left", out _));
    }

    [TestMethod]
    public void ScriptablePaddingSerializesAsChartJsFunctionMarker()
    {
        using var document = SerializeToDocument(Padding.FromFunction(ChartJsFunction.FromName("responsivePadding")));

        Assert.AreEqual("responsivePadding", document.RootElement.GetProperty("__chartJsFunction").GetString());
        Assert.AreEqual(1, document.RootElement.EnumerateObject().Count());
    }

    private static JsonDocument SerializeToDocument<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonDocument.Parse(json);
    }
}
