using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BarChartTests : ChartPageTest
{
    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var addData = Page.GetByText("Add Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addData.ClickAsync();

        await WaitForDatasetDataCountAsync(canvasId, 0, countPrev + 1);
        int countAfter = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeData = Page.GetByText("Remove Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeData.ClickAsync();

        await WaitForDatasetDataCountAsync(canvasId, 0, countPrev - 1);
        int countAfter = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }

    [Test]
    public async Task AddDatasetTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var addDataset = Page.GetByText("Add Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addDataset.ClickAsync();

        await WaitForDatasetCountAsync(canvasId, countPrev + 1);
        int countAfter = await GetDatasetCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDatasetTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeDataset = Page.GetByText("Remove Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeDataset.ClickAsync();

        await WaitForDatasetCountAsync(canvasId, countPrev - 1);
        int countAfter = await GetDatasetCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }
}
