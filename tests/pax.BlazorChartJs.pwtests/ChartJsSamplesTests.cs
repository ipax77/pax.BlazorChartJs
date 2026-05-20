using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChartJsSamplesTests : PageTest
{
    [TestCase("border-radius", "Bar Chart Border Radius")]
    [TestCase("floating", "Floating Bars")]
    [TestCase("horizontal", "Horizontal Bar Chart")]
    [TestCase("stacked", "Stacked Bar Chart")]
    [TestCase("stacked-groups", "Stacked Bar Chart with Groups")]
    [TestCase("vertical", "Vertical Bar Chart")]
    public async Task BarSamplePageRenders(string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/bar/{sampleId}");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = title, Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}']")).ToHaveCountAsync(1);
        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas")).ToHaveCountAsync(1);
    }

    [Test]
    public async Task BarSamplesRenderAndRandomize()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/bar/vertical");

        await Expect(Page).ToHaveTitleAsync(new Regex("Chart.js Bar Charts"),
            new PageAssertionsToHaveTitleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Vertical Bar Chart", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("[data-chartjs-sample]")).ToHaveCountAsync(1);
        await Expect(Page.Locator("[data-chartjs-sample] canvas")).ToHaveCountAsync(1);
        await Expect(Page.Locator("code.language-csharp")).ToBeVisibleAsync();
        await Expect(Page.Locator("code.language-javascript")).ToBeVisibleAsync();
        await Expect(Page.Locator("code.language-csharp .token.keyword").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("code.language-javascript .token.keyword").Nth(0)).ToBeVisibleAsync();

        var verticalCanvas = Page.Locator("[data-chartjs-sample='vertical'] canvas");
        var canvasId = await verticalCanvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Task.Delay(Startup.ChartJsLoadDelay);

        var before = await GetFirstDatasetDataJson(canvasId);
        await Page.Locator("[data-sample-action='vertical-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        var after = await GetFirstDatasetDataJson(canvasId);

        Assert.That(after, Is.Not.EqualTo(before));
    }

    [Test]
    public async Task HorizontalBarSampleDatasetActionsMatchChartJsSample()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/bar/horizontal");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Horizontal Bar Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvas = Page.Locator("[data-chartjs-sample='horizontal'] canvas");
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDatasets = await GetDatasetCount(canvasId);
        var initialDataCount = await GetFirstDatasetDataCount(canvasId);

        await Page.Locator("[data-sample-action='horizontal-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets + 1));

        await Page.Locator("[data-sample-action='horizontal-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));

        await Page.Locator("[data-sample-action='horizontal-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets));

        await Page.Locator("[data-sample-action='horizontal-remove-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount));
    }

    private async Task<string> GetFirstDatasetDataJson(string? canvasId)
    {
        return await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return JSON.stringify(chart.data.datasets[0].data);
            }", canvasId);
    }

    private async Task<int> GetDatasetCount(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.datasets.length;
            }", canvasId);
    }

    private async Task<int> GetFirstDatasetDataCount(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.datasets[0].data.length;
            }", canvasId);
    }
}
