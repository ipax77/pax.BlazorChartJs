using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RemoveDatasetsTests : PageTest
{
    [Test]
    public async Task RemoveDatasetsTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // Current data count
        int countPrev = await GetDatasetCount(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeAllDatasets = Page.GetByText("Remove All Datasets", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Click the button.
        await removeAllDatasets.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        int countAfter = await GetDatasetCount(canvasId);

        Assert.That(countAfter, Is.EqualTo(0));
    }

    private async Task<int> GetDatasetCount(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.data.datasets.length;
            }");
    }
}