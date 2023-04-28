using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TimeSeriesTests : PageTest
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

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsLoadDelay);

        string startdateString = "2022-09-10";
        await Page.GetByLabel("StartDate").FillAsync(startdateString);

        await Task.Delay(Startup.ChartJsComputeDelay);

        var canvas = Page.Locator("canvas");
        var canvasId = await canvas.GetAttributeAsync("id");
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
}