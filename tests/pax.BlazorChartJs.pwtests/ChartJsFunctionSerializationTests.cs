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
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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

}
