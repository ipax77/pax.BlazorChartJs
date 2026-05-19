using System.Text.Json;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class ChartJsFunctionSerializationTests
{
    [TestMethod]
    [DataRow("formatPercent")]
    [DataRow("_formatPercent")]
    [DataRow("$formatPercent")]
    [DataRow("formatPercent1")]
    public void FromNameAcceptsFlatJavaScriptIdentifiers(string name)
    {
        var chartJsFunction = ChartJsFunction.FromName(name);

        Assert.AreEqual(name, chartJsFunction.Name);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void FromNameRejectsEmptyNames(string? name)
    {
        Throws<ArgumentException>(() => ChartJsFunction.FromName(name!));
    }

    [TestMethod]
    [DataRow("1formatPercent")]
    [DataRow("format-percent")]
    [DataRow("format.percent")]
    [DataRow("format/percent")]
    [DataRow("format percent")]
    [DataRow("formatPercent()")]
    public void FromNameRejectsNonFlatJavaScriptIdentifiers(string name)
    {
        Throws<ArgumentException>(() => ChartJsFunction.FromName(name));
    }

    [TestMethod]
    [DataRow("__proto__")]
    [DataRow("constructor")]
    [DataRow("prototype")]
    [DataRow("toString")]
    [DataRow("valueOf")]
    [DataRow("hasOwnProperty")]
    public void FromNameRejectsReservedNames(string name)
    {
        Throws<ArgumentException>(() => ChartJsFunction.FromName(name));
    }

    [TestMethod]
    public void ChartJsFunctionSerializesAsMarker()
    {
        var json = JsonSerializer.Serialize(
            ChartJsFunction.FromName("formatPercent"),
            JsonSerializationTestOptions.Default);

        using var document = JsonDocument.Parse(json);

        Assert.AreEqual("formatPercent", GetMarkerName(document.RootElement));
        Assert.AreEqual(1, document.RootElement.EnumerateObject().Count());
    }

    [TestMethod]
    public void DataLabelsFormatterSerializesAsChartJsFunctionMarker()
    {
        DataLabelsConfig dataLabelsConfig = new()
        {
            Formatter = ChartJsFunction.FromName("formatPercent")
        };

        using var document = SerializeToDocument(dataLabelsConfig);
        var formatter = document.RootElement.GetProperty("formatter");

        Assert.AreEqual("formatPercent", GetMarkerName(formatter));
    }

    [TestMethod]
    public void LinearAxisTickCallbackSerializesAsChartJsFunctionMarker()
    {
        LinearAxisTick ticks = new()
        {
            Callback = ChartJsFunction.FromName("formatCurrency")
        };

        using var document = SerializeToDocument(ticks);
        var callback = document.RootElement.GetProperty("callback");

        Assert.AreEqual("formatCurrency", GetMarkerName(callback));
    }

    [TestMethod]
    public void AxisTickColorOptionsSerializeIndexableAndFunctionValues()
    {
        ChartJsAxisTick ticks = new()
        {
            Color = ChartJsFunction.FromName("tickColor"),
            TextStrokeColor = ["red", "blue"]
        };

        using var document = SerializeToDocument(ticks);

        Assert.AreEqual("tickColor", GetMarkerName(document.RootElement.GetProperty("color")));
        Assert.AreEqual(2, document.RootElement.GetProperty("textStrokeColor").GetArrayLength());
    }

    [TestMethod]
    public void TooltipAndTitleColorsSerializeIndexableColorOptions()
    {
        ChartJsOptions options = new()
        {
            Plugins = new Plugins()
            {
                Title = new Title()
                {
                    Color = "navy"
                },
                Tooltip = new Tooltip()
                {
                    TitleColor = ChartJsFunction.FromName("tooltipTitleColor")
                }
            }
        };

        using var document = SerializeToDocument(options);
        var plugins = document.RootElement.GetProperty("plugins");

        Assert.AreEqual("navy", plugins.GetProperty("title").GetProperty("color").GetString());
        Assert.AreEqual("tooltipTitleColor", GetMarkerName(plugins.GetProperty("tooltip").GetProperty("titleColor")));
    }

    [TestMethod]
    public void DataLabelsTextStrokeColorSerializesAsIndexableColorOption()
    {
        DataLabelsConfig dataLabelsConfig = new()
        {
            TextStrokeColor = ChartJsFunction.FromName("labelStrokeColor")
        };

        using var document = SerializeToDocument(dataLabelsConfig);

        Assert.AreEqual("labelStrokeColor", GetMarkerName(document.RootElement.GetProperty("textStrokeColor")));
    }

    [TestMethod]
    public void PieHoverBorderColorSerializesAsIndexableColorOption()
    {
        PieDataset dataset = new()
        {
            HoverBorderColor = ["#111111", "#222222"]
        };

        using var document = SerializeToDocument(dataset);

        Assert.AreEqual(2, document.RootElement.GetProperty("hoverBorderColor").GetArrayLength());
    }

    [TestMethod]
    public void TooltipCallbacksSerializeAsChartJsFunctionMarkers()
    {
        Tooltip tooltip = new()
        {
            External = ChartJsFunction.FromName("externalTooltip"),
            ItemSort = ChartJsFunction.FromName("sortTooltipItems"),
            Filter = ChartJsFunction.FromName("filterTooltipItems"),
            Callbacks = new TooltipCallbacks()
            {
                Label = ChartJsFunction.FromName("formatTooltipLabel"),
                LabelTextColor = ChartJsFunction.FromName("tooltipTextColor")
            }
        };

        using var document = SerializeToDocument(tooltip);
        var root = document.RootElement;
        var callbacks = root.GetProperty("callbacks");

        Assert.AreEqual("externalTooltip", GetMarkerName(root.GetProperty("external")));
        Assert.AreEqual("sortTooltipItems", GetMarkerName(root.GetProperty("itemSort")));
        Assert.AreEqual("filterTooltipItems", GetMarkerName(root.GetProperty("filter")));
        Assert.AreEqual("formatTooltipLabel", GetMarkerName(callbacks.GetProperty("label")));
        Assert.AreEqual("tooltipTextColor", GetMarkerName(callbacks.GetProperty("labelTextColor")));
    }

    [TestMethod]
    public void LegendCallbacksSerializeAsChartJsFunctionMarkers()
    {
        Legend legend = new()
        {
            OnClick = ChartJsFunction.FromName("legendClick"),
            Labels = new Labels()
            {
                Filter = ChartJsFunction.FromName("showLegendItem"),
                Sort = ChartJsFunction.FromName("sortLegendItems")
            }
        };

        using var document = SerializeToDocument(legend);
        var labels = document.RootElement.GetProperty("labels");

        Assert.AreEqual("legendClick", GetMarkerName(document.RootElement.GetProperty("onClick")));
        Assert.AreEqual("showLegendItem", GetMarkerName(labels.GetProperty("filter")));
        Assert.AreEqual("sortLegendItems", GetMarkerName(labels.GetProperty("sort")));
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
