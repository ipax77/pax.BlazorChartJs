using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UpdateDatasetTests : PageTest
{
    [Test]
    public async Task UpdateChartTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // create a locator
        var updateDataset = Page.GetByText("UpdateDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(updateDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await updateDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var borderWidthUpdate = await GetBorderWidth(canvasId);

        Assert.That(borderWidthUpdate, Is.EqualTo(3));

        // create a locator
        var resetDataset = Page.GetByText("ResetDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(resetDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await resetDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var borderWidthReset = await GetBorderWidth(canvasId);

        Assert.That(borderWidthReset, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateChartNullTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // create a locator
        var updateDataset = Page.GetByText("UpdateDataset2", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(updateDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await updateDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var barThicknessUpdate = await GetBarThickness(canvasId);

        Assert.That(barThicknessUpdate, Is.EqualTo(22));

        // create a locator
        var resetDataset = Page.GetByText("ResetDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(resetDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await resetDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var barThicknessReset = await GetBorderWidth(canvasId);

        Assert.That(barThicknessReset, Is.EqualTo(1));
    }

    private async Task<double?> GetBorderWidth(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<double?>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                const dataset = chart.data.datasets[" + dataset + @"];
                if (dataset.borderWidth === undefined) {
                    return null;
                } else {
                    return dataset.borderWidth;
                }
            }");
    }

    private async Task<object?> GetBarThickness(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<object?>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                const dataset = chart.data.datasets[" + dataset + @"];
                if (dataset.barThickness === undefined) {
                    return null;
                } else {
                    return dataset.barThickness;
                }
            }");
    }
}
