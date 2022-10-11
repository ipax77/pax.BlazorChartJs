using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BarChartTests : PageTest
{
    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("barchart"));

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // Current data count
        int countPrev = await GetDatasetDataCount(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var addData = Page.GetByText("Add Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addData.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        int countAfter = await GetDatasetDataCount(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDataTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("barchart"));

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // Current data count
        int countPrev = await GetDatasetDataCount(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeData = Page.GetByText("Remove Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeData.ClickAsync();

        int countAfter = await GetDatasetDataCount(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }

    [Test]
    public async Task AddDatasetTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("barchart"));

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
        var addDataset = Page.GetByText("Add Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addDataset.ClickAsync();

        await Task.Delay(Startup.ChartJsComputeDelay);

        int countAfter = await GetDatasetCount(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDatasetTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "barchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("barchart"));

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
        var removeDataset = Page.GetByText("Remove Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeDataset.ClickAsync();

        int countAfter = await GetDatasetCount(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }

    private async Task<int> GetDatasetCount(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.data.datasets.length;
            }");
    }

    private async Task<int> GetDatasetDataCount(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<int>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.data.datasets[" + dataset + @"].data.length;
            }");
    }
}