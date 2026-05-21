using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RemoveDatasetsTests : ChartPageTest
{
    [Test]
    public async Task RemoveDatasetsTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeAllDatasets = Page.GetByText("Remove All Datasets", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Click the button.
        await removeAllDatasets.ClickAsync();

        await WaitForDatasetCountAsync(canvasId, 0);
        int countAfter = await GetDatasetCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(0));
    }
}
