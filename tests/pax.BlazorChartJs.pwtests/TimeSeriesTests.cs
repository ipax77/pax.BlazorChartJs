using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TimeSeriesTests : ChartPageTest
{
    [Test]
    public async Task TimeSeriesStartDateTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/timeschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("TimesChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // Provoke ChartComponent.Dispose while initializing
        // create a locator
        var showChart = Page.GetByText("Show Chart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(showChart).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await showChart.ClickAsync();

        string startdateString = "2022-09-10";
        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));
        await WaitForChartXAxisTimeScaleAsync(canvasId);
        await Page.GetByLabel("StartDate").FillAsync(startdateString);
        await WaitForChartXAxisMinAsync(canvasId, startdateString);
        var minValue = await GetChartXAxisMin(canvasId);

        Assert.That(minValue, Is.EqualTo(startdateString));

        var error = Page.GetByText("An unhandled error has occurred.", new Microsoft.Playwright.PageGetByTextOptions() { Exact = false });
        await Expect(error).ToHaveCSSAsync("display", "none");
    }

    private async Task<string> GetChartXAxisMin(string? canvasId)
    {
        return await Page.EvaluateAsync<string>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.options.scales.x.min;
            }");
    }

    private Task WaitForChartXAxisMinAsync(string canvasId, string expected)
    {
        return Page.WaitForFunctionAsync(
            "args => Chart.getChart(args.canvasId)?.options?.scales?.x?.min === args.expected",
            new { canvasId, expected },
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }

    private Task WaitForChartXAxisTimeScaleAsync(string canvasId)
    {
        return Page.WaitForFunctionAsync(
            "canvasId => Chart.getChart(canvasId)?.options?.scales?.x?.type === 'time'",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }
}
