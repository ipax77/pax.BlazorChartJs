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

    [TestCase("interpolation", "Interpolation Modes")]
    [TestCase("line", "Line Chart")]
    [TestCase("multi-axis", "Multi Axis Line Chart")]
    [TestCase("point-styling", "Point Styling")]
    [TestCase("segments", "Line Segment Styling")]
    [TestCase("stepped", "Stepped Line Charts")]
    [TestCase("styling", "Line Styling")]
    public async Task LineSamplePageRenders(string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/line/{sampleId}");

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
        await Expect(Page.Locator("[data-code-tab='config']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("var config");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const config");
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
    public async Task BarSampleCodeTabsShowCSharpAndJavaScriptTogether()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/bar/vertical");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Vertical Bar Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='setup']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("string[] labels");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const labels");

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='actions']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("void Randomize");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const actions");
        await Expect(Page.Locator("code.language-csharp .token.keyword").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("code.language-javascript .token.keyword").Nth(0)).ToBeVisibleAsync();

        await Page.Locator("[data-code-tab='config']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='config']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("var config");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const config");
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

    [Test]
    public async Task LineSampleDatasetActionsMatchChartJsSample()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/line/line");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page).ToHaveTitleAsync(new Regex("Chart.js Line Charts"));

        var canvas = Page.Locator("[data-chartjs-sample='line'] canvas");
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDatasets = await GetDatasetCount(canvasId);
        var initialDataCount = await GetFirstDatasetDataCount(canvasId);
        var before = await GetFirstDatasetDataJson(canvasId);

        await Page.Locator("[data-sample-action='line-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataJson(canvasId), Is.Not.EqualTo(before));

        await Page.Locator("[data-sample-action='line-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets + 1));

        await Page.Locator("[data-sample-action='line-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));
        Assert.That(await GetLastLabel(canvasId), Is.EqualTo("August"));

        await Page.Locator("[data-sample-action='line-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets));

        await Page.Locator("[data-sample-action='line-remove-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount));
    }

    [Test]
    public async Task LinePointStylingSampleIncludesAllPointStyleActions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/line/point-styling");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Point Styling", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-chartjs-sample='point-styling'] [data-sample-action]")).ToHaveCountAsync(11);

        var canvasId = await Page.Locator("[data-chartjs-sample='point-styling'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='point-styling-point-style-false']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await GetFirstDatasetPropertyJson(canvasId, "pointStyle"), Is.EqualTo("false"));
    }

    [Test]
    public async Task LineCallbackSamplesExposeCallbackImplementationPanel()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/line/segments");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Segment Styling", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-code-tab='callbacks']")).ToHaveCountAsync(0);
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const config");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const skipped");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "chartJsCallbacks.js", Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("ChartJsCallbacksModuleLocation");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("Object.assign(Object.create(null)");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("lineSegmentBorderColor");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("lineSegmentBorderDash");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("Object.freeze(callbacks)");
    }

    [Test]
    public async Task LineSteppedSampleIncludesAllStepActions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/line/stepped");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Stepped Line Charts", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-chartjs-sample='stepped'] [data-sample-action]")).ToHaveCountAsync(5);

        var canvasId = await Page.Locator("[data-chartjs-sample='stepped'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='stepped-step-middle']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await GetFirstDatasetPropertyJson(canvasId, "stepped"), Is.EqualTo("\"middle\""));
    }

    [Test]
    public async Task LineSegmentSampleRevivesSegmentCallbacks()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/line/segments");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Segment Styling", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='segments'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var callbackTypes = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return `${typeof chart.data.datasets[0].segment.borderColor}:${typeof chart.data.datasets[0].segment.borderDash}`;
            }", canvasId);

        Assert.That(callbackTypes, Is.EqualTo("function:function"));
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

    private async Task<string?> GetLastLabel(string? canvasId)
    {
        return await Page.EvaluateAsync<string?>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.labels.at(-1);
            }", canvasId);
    }

    private async Task<string> GetFirstDatasetPropertyJson(string? canvasId, string propertyName)
    {
        return await Page.EvaluateAsync<string>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return JSON.stringify(chart.data.datasets[0][args.propertyName]);
            }", new { canvasId, propertyName });
    }
}
