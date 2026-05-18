using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DisposeTests : PageTest
{
    [Test]
    public async Task DisposeTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/timeschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("TimesChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // Provoke ChartComponent.Dispose while initializing
        // create a locator
        var showAndDispose = Page.GetByText("ShowAndDispose Chart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(showAndDispose).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await showAndDispose.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        // await Page.ScreenshotAsync(new()
        // {
        //     Path = "/data/screenshot.png",
        //     FullPage = true,
        // });

        var error = Page.GetByText("An unhandled error has occurred.", new Microsoft.Playwright.PageGetByTextOptions() { Exact = false });
        await Expect(error).ToHaveCSSAsync("display", "none");
    }

    [Test]
    public async Task DatasetInteropAfterDisposeIsIgnored()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        var chartInteropVersion = await Page.EvaluateAsync<string>("() => window.chartJsInteropVersion ?? ''");
        if (chartInteropVersion != "0.8.8")
        {
            Assert.Ignore($"The configured sample site is serving pax.BlazorChartJs {chartInteropVersion}; this regression requires 0.8.8.");
        }

        await Page.GotoAsync(Startup.GetSampleBaseUrl());

        await Page.EvaluateAsync(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.8.8');
                const dataset = { id: 'disposed-test', label: 'Disposed Test', data: [1, 2, 3] };
                chartInterop.addDatasets(chartId, [dataset]);
                chartInterop.updateDatasets(chartId, [dataset]);
                chartInterop.removeDatasets(chartId, [dataset.id]);
                chartInterop.setDatasets(chartId, [dataset]);
            }",
            canvasId);

        var error = Page.GetByText("An unhandled error has occurred.", new Microsoft.Playwright.PageGetByTextOptions() { Exact = false });
        await Expect(error).ToHaveCSSAsync("display", "none");
    }
}
