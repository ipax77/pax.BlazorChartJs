using System.Text.Json;
using System.Text.Json.Serialization;
using pax.BlazorChartJs;

namespace PlaywrightTests;

[TestFixture]
public class ChartJsFunctionSerializationTests
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            CreateConverter("IndexableOptionStringConverter"),
            CreateConverter("IndexableOptionDoubleConverter"),
            CreateConverter("IndexableOptionIntConverter"),
            CreateConverter("IndexableOptionBoolConverter"),
            CreateConverter("IndexableOptionObjectConverter")
        }
    };

    [TestCase("formatPercent")]
    [TestCase("_formatPercent")]
    [TestCase("$formatPercent")]
    [TestCase("formatPercent1")]
    public void FromNameAcceptsFlatJavaScriptIdentifiers(string name)
    {
        var chartJsFunction = ChartJsFunction.FromName(name);

        Assert.That(chartJsFunction.Name, Is.EqualTo(name));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void FromNameRejectsEmptyNames(string? name)
    {
        Assert.Throws<ArgumentException>(() => ChartJsFunction.FromName(name!));
    }

    [TestCase("1formatPercent")]
    [TestCase("format-percent")]
    [TestCase("format.percent")]
    [TestCase("format percent")]
    [TestCase("formatPercent()")]
    public void FromNameRejectsNonFlatJavaScriptIdentifiers(string name)
    {
        Assert.Throws<ArgumentException>(() => ChartJsFunction.FromName(name));
    }

    [TestCase("__proto__")]
    [TestCase("constructor")]
    [TestCase("prototype")]
    [TestCase("toString")]
    [TestCase("valueOf")]
    [TestCase("hasOwnProperty")]
    public void FromNameRejectsReservedNames(string name)
    {
        Assert.Throws<ArgumentException>(() => ChartJsFunction.FromName(name));
    }

    [Test]
    public void DataLabelsFormatterSerializesAsChartJsFunctionMarker()
    {
        DataLabelsConfig dataLabelsConfig = new()
        {
            Formatter = ChartJsFunction.FromName("formatPercent")
        };

        var json = JsonSerializer.Serialize(dataLabelsConfig, jsonOptions);

        using var document = JsonDocument.Parse(json);
        var formatter = document.RootElement.GetProperty("formatter");

        Assert.That(formatter.GetProperty("__chartJsFunction").GetString(), Is.EqualTo("formatPercent"));
    }

    [Test]
    public void LinearAxisTickCallbackSerializesAsChartJsFunctionMarker()
    {
        LinearAxisTick ticks = new()
        {
            Callback = ChartJsFunction.FromName("formatCurrency")
        };

        var json = JsonSerializer.Serialize(ticks, jsonOptions);

        using var document = JsonDocument.Parse(json);
        var callback = document.RootElement.GetProperty("callback");

        Assert.That(callback.GetProperty("__chartJsFunction").GetString(), Is.EqualTo("formatCurrency"));
    }

    [Test]
    public void AxisTickColorOptionsSerializeIndexableAndFunctionValues()
    {
        ChartJsAxisTick ticks = new()
        {
            Color = ChartJsFunction.FromName("tickColor"),
            TextStrokeColor = ["red", "blue"]
        };

        var json = JsonSerializer.Serialize(ticks, jsonOptions);

        using var document = JsonDocument.Parse(json);

        Assert.That(document.RootElement.GetProperty("color").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("tickColor"));
        Assert.That(document.RootElement.GetProperty("textStrokeColor").GetArrayLength(), Is.EqualTo(2));
    }

    [Test]
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

        var json = JsonSerializer.Serialize(options, jsonOptions);

        using var document = JsonDocument.Parse(json);
        var plugins = document.RootElement.GetProperty("plugins");

        Assert.That(plugins.GetProperty("title").GetProperty("color").GetString(), Is.EqualTo("navy"));
        Assert.That(plugins.GetProperty("tooltip").GetProperty("titleColor").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("tooltipTitleColor"));
    }

    [Test]
    public void DataLabelsTextStrokeColorSerializesAsIndexableColorOption()
    {
        DataLabelsConfig dataLabelsConfig = new()
        {
            TextStrokeColor = ChartJsFunction.FromName("labelStrokeColor")
        };

        var json = JsonSerializer.Serialize(dataLabelsConfig, jsonOptions);

        using var document = JsonDocument.Parse(json);

        Assert.That(document.RootElement.GetProperty("textStrokeColor").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("labelStrokeColor"));
    }

    [Test]
    public void PieHoverBorderColorSerializesAsIndexableColorOption()
    {
        PieDataset dataset = new()
        {
            HoverBorderColor = ["#111111", "#222222"]
        };

        var json = JsonSerializer.Serialize(dataset, jsonOptions);

        using var document = JsonDocument.Parse(json);

        Assert.That(document.RootElement.GetProperty("hoverBorderColor").GetArrayLength(), Is.EqualTo(2));
    }

    [Test]
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

        var json = JsonSerializer.Serialize(tooltip, jsonOptions);

        using var document = JsonDocument.Parse(json);

        Assert.That(document.RootElement.GetProperty("external").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("externalTooltip"));
        Assert.That(document.RootElement.GetProperty("itemSort").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("sortTooltipItems"));
        Assert.That(document.RootElement.GetProperty("filter").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("filterTooltipItems"));
        Assert.That(document.RootElement.GetProperty("callbacks").GetProperty("label").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("formatTooltipLabel"));
        Assert.That(document.RootElement.GetProperty("callbacks").GetProperty("labelTextColor").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("tooltipTextColor"));
    }

    [Test]
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

        var json = JsonSerializer.Serialize(legend, jsonOptions);

        using var document = JsonDocument.Parse(json);

        Assert.That(document.RootElement.GetProperty("onClick").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("legendClick"));
        Assert.That(document.RootElement.GetProperty("labels").GetProperty("filter").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("showLegendItem"));
        Assert.That(document.RootElement.GetProperty("labels").GetProperty("sort").GetProperty("__chartJsFunction").GetString(), Is.EqualTo("sortLegendItems"));
    }

    private static JsonConverter CreateConverter(string converterName)
    {
        var converterType = typeof(ChartJsFunction).Assembly.GetType($"pax.BlazorChartJs.{converterName}")
            ?? throw new InvalidOperationException($"Missing converter type: {converterName}.");

        return (JsonConverter)(Activator.CreateInstance(converterType)
            ?? throw new InvalidOperationException($"Could not create converter: {converterName}."));
    }
}
