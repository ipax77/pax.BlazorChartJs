using System.Text.Json;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class IndexableOptionSerializationTests
{
    [TestMethod]
    public void FunctionOptionStoresFunctionState()
    {
        var function = ChartJsFunction.FromName("colorByIndex");
        IndexableOption<string> option = function;

        Assert.AreEqual(IndexableOptionKind.Function, option.Kind);
        Assert.IsTrue(option.IsFunction);
        Assert.IsFalse(option.IsIndexed);
        Assert.AreSame(function, option.FunctionValue);
        Assert.AreEqual(0, option.Count);
        Assert.IsEmpty(option.ToArray());
    }

    [TestMethod]
    public void SingleValueOptionEnumeratesSingleValue()
    {
        IndexableOption<string> option = "navy";

        CollectionAssert.AreEqual(new[] { "navy" }, option.ToArray());
    }

    [TestMethod]
    public void FunctionOptionRejectsIndexedMutation()
    {
        IndexableOption<string> option = ChartJsFunction.FromName("colorByIndex");

        Throws<InvalidOperationException>(() => option.Add("red"));
        Throws<InvalidOperationException>(() => option.Insert(0, "red"));
        Throws<InvalidOperationException>(() => option.Remove("red"));
        Throws<InvalidOperationException>(() => option.RemoveAt(0));
    }

    [TestMethod]
    public void SingleValueOptionRejectsIndexedMutation()
    {
        IndexableOption<string> option = "navy";

        Throws<InvalidOperationException>(() => option.Add("red"));
        Throws<InvalidOperationException>(() => option.Insert(0, "red"));
        Throws<InvalidOperationException>(() => option.Remove("red"));
        Throws<InvalidOperationException>(() => option.RemoveAt(0));
    }

    [TestMethod]
    public void IndexedOptionCopiesInputValues()
    {
        List<string> colors = ["red", "blue"];
        IndexableOption<string> option = colors;

        colors.Add("green");

        CollectionAssert.AreEqual(new[] { "red", "blue" }, option.ToArray());
        Assert.AreEqual(2, option.Count);
    }

    [TestMethod]
    [DataRow("string")]
    [DataRow("double")]
    [DataRow("int")]
    [DataRow("bool")]
    [DataRow("object")]
    public void FunctionOptionSerializesAsChartJsFunctionMarker(string valueType)
    {
        using var document = SerializeIndexableOption(CreateFunctionOption(valueType));

        Assert.AreEqual("resolveOption", GetMarkerName(document.RootElement));
    }

    [TestMethod]
    public void StringOptionSerializesSingleAndIndexedValues()
    {
        using var single = SerializeIndexableOption(new IndexableOption<string>("navy"));
        using var indexed = SerializeIndexableOption(new IndexableOption<string>(["red", "blue"]));

        Assert.AreEqual("navy", single.RootElement.GetString());
        Assert.AreEqual("red", indexed.RootElement[0].GetString());
        Assert.AreEqual("blue", indexed.RootElement[1].GetString());
    }

    [TestMethod]
    public void DoubleOptionSerializesSingleAndIndexedValues()
    {
        using var single = SerializeIndexableOption(new IndexableOption<double>(1.5));
        using var indexed = SerializeIndexableOption(new IndexableOption<double>([2.5, 3.5]));

        Assert.AreEqual(1.5, single.RootElement.GetDouble());
        Assert.AreEqual(2.5, indexed.RootElement[0].GetDouble());
        Assert.AreEqual(3.5, indexed.RootElement[1].GetDouble());
    }

    [TestMethod]
    public void IntOptionSerializesSingleAndIndexedValues()
    {
        using var single = SerializeIndexableOption(new IndexableOption<int>(1));
        using var indexed = SerializeIndexableOption(new IndexableOption<int>([2, 3]));

        Assert.AreEqual(1, single.RootElement.GetInt32());
        Assert.AreEqual(2, indexed.RootElement[0].GetInt32());
        Assert.AreEqual(3, indexed.RootElement[1].GetInt32());
    }

    [TestMethod]
    public void BoolOptionSerializesSingleAndIndexedValues()
    {
        using var single = SerializeIndexableOption(new IndexableOption<bool>(true));
        using var indexed = SerializeIndexableOption(new IndexableOption<bool>([false, true]));

        Assert.IsTrue(single.RootElement.GetBoolean());
        Assert.IsFalse(indexed.RootElement[0].GetBoolean());
        Assert.IsTrue(indexed.RootElement[1].GetBoolean());
    }

    [TestMethod]
    public void ObjectOptionSerializesSingleAndIndexedValues()
    {
        using var single = SerializeIndexableOption(new IndexableOption<object>("triangle"));
        using var indexed = SerializeIndexableOption(new IndexableOption<object>([1, "circle"]));

        Assert.AreEqual("triangle", single.RootElement.GetString());
        Assert.AreEqual(1, indexed.RootElement[0].GetInt32());
        Assert.AreEqual("circle", indexed.RootElement[1].GetString());
    }

    private static object CreateFunctionOption(string valueType)
    {
        var function = ChartJsFunction.FromName("resolveOption");

        return valueType switch
        {
            "string" => new IndexableOption<string>(function),
            "double" => new IndexableOption<double>(function),
            "int" => new IndexableOption<int>(function),
            "bool" => new IndexableOption<bool>(function),
            "object" => new IndexableOption<object>(function),
            _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
        };
    }

    private static JsonDocument SerializeIndexableOption<T>(T option)
    {
        var json = JsonSerializer.Serialize(option, JsonSerializationTestOptions.Default);
        return JsonDocument.Parse(json);
    }

    private static string? GetMarkerName(JsonElement element)
    {
        return element.GetProperty("__chartJsFunction").GetString();
    }

    private static void Throws<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        Assert.Fail($"Expected exception of type {typeof(TException).Name}.");
    }
}
