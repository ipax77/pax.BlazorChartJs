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

    [TestCase("line-boundaries", "Line Chart Boundaries")]
    [TestCase("line-datasets", "Line Chart Datasets")]
    [TestCase("line-drawtime", "Line Chart drawTime")]
    [TestCase("line-stacked", "Line Chart Stacked")]
    [TestCase("radar-stacked", "Radar Chart Stacked")]
    public async Task AreaSamplePageRenders(string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/area/{sampleId}");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = title, Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}']")).ToHaveCountAsync(1);
        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas")).ToHaveCountAsync(1);
    }

    [Test]
    public async Task AreaNavLinksSwitchSamplesAndFollowOtherCharts()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-boundaries");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Boundaries", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var sampleSectionOrder = await Page.EvaluateAsync<string[]>(@"() =>
            Array.from(document.querySelectorAll('.nav-subsection-heading'))
                .map(element => element.textContent.trim())");

        Assert.That(Array.IndexOf(sampleSectionOrder, "Area Charts"), Is.GreaterThan(Array.IndexOf(sampleSectionOrder, "Other Charts")));

        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Line Datasets", Exact = true }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Datasets", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("[data-chartjs-sample='line-datasets'] canvas")).ToHaveCountAsync(1);

        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radar Stacked", Exact = true }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Radar Chart Stacked", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("[data-chartjs-sample='radar-stacked'] canvas")).ToHaveCountAsync(1);
    }

    [TestCase("bubble", "Bubble")]
    [TestCase("combo-bar-line", "Combo bar/line")]
    [TestCase("doughnut", "Doughnut")]
    [TestCase("multi-series-pie", "Multi Series Pie")]
    [TestCase("pie", "Pie")]
    [TestCase("polar-area", "Polar area")]
    [TestCase("polar-area-center-labels", "Polar area centered point labels")]
    [TestCase("radar", "Radar")]
    [TestCase("radar-skip-points", "Radar skip points")]
    [TestCase("scatter", "Scatter")]
    [TestCase("scatter-multi-axis", "Scatter - Multi axis")]
    [TestCase("stacked-bar-line", "Stacked bar/line")]
    public async Task OtherSamplePageRenders(string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/other/{sampleId}");

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
        var mainJavaScriptCode = Page.Locator("[aria-label='JavaScript code'] code.language-javascript");
        await Expect(mainJavaScriptCode).ToContainTextAsync("const config");

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='setup']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(mainJavaScriptCode).ToContainTextAsync("const skipped");
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

    [Test]
    public async Task AreaBoundarySampleUpdatesFillAndData()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-boundaries");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Boundaries", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='line-boundaries'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var before = await GetFirstDatasetDataJson(canvasId);

        await Page.Locator("[data-sample-action='line-boundaries-fill-origin']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetPropertyJson(canvasId, "fill"), Is.EqualTo("\"origin\""));

        await Page.Locator("[data-sample-action='line-boundaries-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataJson(canvasId), Is.Not.EqualTo(before));

        await Page.Locator("[data-sample-action='line-boundaries-smooth']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetPropertyJson(canvasId, "tension"), Is.EqualTo("0.4"));
    }

    [Test]
    public async Task AreaFillerOptionActionsUpdateChartOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-datasets");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Datasets", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var lineDatasetsCanvasId = await Page.Locator("[data-chartjs-sample='line-datasets'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='line-datasets-propagate']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFillerPropertyJson(lineDatasetsCanvasId, "propagate"), Is.EqualTo("true"));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-drawtime");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart drawTime", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var drawTimeCanvasId = await Page.Locator("[data-chartjs-sample='line-drawtime'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='line-drawtime-drawtime-before-draw']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFillerPropertyJson(drawTimeCanvasId, "drawTime"), Is.EqualTo("\"beforeDraw\""));
    }

    [Test]
    public async Task AreaStackedSampleDatasetActionsMatchChartJsSample()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-stacked");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Stacked", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='line-stacked'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDatasets = await GetDatasetCount(canvasId);
        var initialDataCount = await GetFirstDatasetDataCount(canvasId);

        await Page.Locator("[data-sample-action='line-stacked-stacked-single']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetYAxisStackedJson(canvasId), Is.EqualTo("\"single\""));

        await Page.Locator("[data-sample-action='line-stacked-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets + 1));

        await Page.Locator("[data-sample-action='line-stacked-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));
        Assert.That(await GetLastLabel(canvasId), Is.EqualTo("August"));

        await Page.Locator("[data-sample-action='line-stacked-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets));

        await Page.Locator("[data-sample-action='line-stacked-remove-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount));
    }

    [Test]
    public async Task AreaSampleCodeTabsShowCSharpAndJavaScriptTogether()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-drawtime");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart drawTime", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-code-tab='config']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("Filler = new FillerOptions");
        var mainJavaScriptCode = Page.Locator("[aria-label='JavaScript code'] code.language-javascript");
        await Expect(mainJavaScriptCode).ToContainTextAsync("filler: { propagate: false }");

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='setup']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("PointBackgroundColor");
        await Expect(mainJavaScriptCode).ToContainTextAsync("Dataset 1");

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='actions']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("void SetDrawTime");
        await Expect(mainJavaScriptCode).ToContainTextAsync("drawTime: beforeDraw");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("areaDrawTimeTitle");
    }

    [Test]
    public async Task OtherBubbleSampleDatasetActionsMatchChartJsSample()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/bubble");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bubble", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='bubble'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDatasets = await GetDatasetCount(canvasId);
        var initialDataCount = await GetFirstDatasetDataCount(canvasId);
        var before = await GetFirstDatasetDataJson(canvasId);

        await Page.Locator("[data-sample-action='bubble-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataJson(canvasId), Is.Not.EqualTo(before));

        await Page.Locator("[data-sample-action='bubble-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets + 1));

        await Page.Locator("[data-sample-action='bubble-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));

        await Page.Locator("[data-sample-action='bubble-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets));

        await Page.Locator("[data-sample-action='bubble-remove-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount));
    }

    [Test]
    public async Task OtherRadarSampleAddDataWorksWithInitialArrayLabels()
    {
        List<string> pageErrors = [];
        Page.PageError += (_, error) => pageErrors.Add(error);

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/radar");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Radar", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='radar'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDataCount = await GetFirstDatasetDataCount(canvasId);

        await Page.Locator("[data-sample-action='radar-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));
        Assert.That(pageErrors, Is.Empty);
    }

    [Test]
    public async Task OtherScatterMultiAxisPlacesSecondYAxisOnRight()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/scatter-multi-axis");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Scatter - Multi axis", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='scatter-multi-axis'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var scalePositions = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return `${chart.options.scales.y.position}:${chart.options.scales.y2.position}`;
            }", canvasId);

        Assert.That(scalePositions, Is.EqualTo("left:right"));
    }

    [Test]
    public async Task OtherSampleCodeTabsShowCSharpAndJavaScriptTogether()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/combo-bar-line");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Combo bar/line", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-code-tab='config']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("var config");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("Type = ChartType.bar");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const config");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("type: 'bar'");

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='setup']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("new LineDataset");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("type: 'line'");

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='actions']")).ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.Locator("code.language-csharp")).ToContainTextAsync("void Randomize");
        await Expect(Page.Locator("code.language-javascript")).ToContainTextAsync("const actions");
    }

    [Test]
    public async Task OtherDoughnutSampleIncludesHideAndShowActions()
    {
        List<string> pageErrors = [];
        Page.PageError += (_, error) => pageErrors.Add(error);

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/doughnut");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Doughnut", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-chartjs-sample='doughnut'] [data-sample-action]")).ToHaveCountAsync(9);

        var canvasId = await Page.Locator("[data-chartjs-sample='doughnut'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='doughnut-hide-dataset-0']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await IsDatasetVisible(canvasId, 0), Is.False);

        await Page.Locator("[data-sample-action='doughnut-show-dataset-0']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await IsDatasetVisible(canvasId, 0), Is.True);

        await Page.Locator("[data-sample-action='doughnut-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        await Page.Locator("[data-sample-action='doughnut-hide-dataset-0']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        await Page.Locator("[data-sample-action='doughnut-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        await Page.Locator("[data-sample-action='doughnut-hide-dataset-0']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-show-dataset-0']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-remove-data']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-remove-data']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-remove-data']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-remove-data']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-hide-data-0-1']").ClickAsync();
        await Page.Locator("[data-sample-action='doughnut-show-data-0-1']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(pageErrors, Is.Empty);
    }

    [Test]
    public async Task OtherPolarCenteredLabelsSerializesPointLabelOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/polar-area-center-labels");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Polar area centered point labels", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='polar-area-center-labels'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var pointLabels = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return JSON.stringify({
                    display: chart.options.scales.r.pointLabels.display,
                    centerPointLabels: chart.options.scales.r.pointLabels.centerPointLabels,
                    fontSize: chart.options.scales.r.pointLabels.font.size
                });
            }", canvasId);

        Assert.That(pointLabels, Is.EqualTo("{\"display\":true,\"centerPointLabels\":true,\"fontSize\":18}"));
    }

    [Test]
    public async Task OtherMultiSeriesPieRevivesLegendAndTooltipCallbacks()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/other/multi-series-pie");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Multi Series Pie", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='multi-series-pie'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var callbackTypes = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return [
                    typeof chart.options.plugins.legend.labels.generateLabels,
                    typeof chart.options.plugins.legend.onClick,
                    typeof chart.options.plugins.tooltip.callbacks.title
                ].join(':');
            }", canvasId);

        Assert.That(callbackTypes, Is.EqualTo("function:function:function"));

        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("multiSeriesPieGenerateLabels");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("multiSeriesPieLegendClick");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("multiSeriesPieTooltipTitle");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("export const chartJsCallbacks");
    }

    private async Task<string> GetFirstDatasetDataJson(string? canvasId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return JSON.stringify(chart.data.datasets[0].data);
            }", canvasId);
    }

    private async Task<int> GetDatasetCount(string? canvasId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<int>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.datasets.length;
            }", canvasId);
    }

    private async Task<int> GetFirstDatasetDataCount(string? canvasId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<int>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.datasets[0].data.length;
            }", canvasId);
    }

    private async Task<string?> GetLastLabel(string? canvasId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string?>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return chart.data.labels.at(-1);
            }", canvasId);
    }

    private async Task<string> GetFirstDatasetPropertyJson(string? canvasId, string propertyName)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return JSON.stringify(chart.data.datasets[0][args.propertyName]);
            }", new { canvasId, propertyName });
    }

    private async Task<string> GetFillerPropertyJson(string? canvasId, string propertyName)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return JSON.stringify(chart.options.plugins.filler[args.propertyName]);
            }", new { canvasId, propertyName });
    }

    private async Task<string> GetYAxisStackedJson(string? canvasId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return JSON.stringify(chart.options.scales.y.stacked);
            }", canvasId);
    }

    private async Task<bool> IsDatasetVisible(string? canvasId, int datasetIndex)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<bool>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return chart.isDatasetVisible(args.datasetIndex);
            }", new { canvasId, datasetIndex });
    }

    private async Task WaitForChart(string? canvasId)
    {
        await Page.WaitForFunctionAsync(
            "canvasId => window.Chart !== undefined && Chart.getChart(canvasId) !== undefined",
            canvasId);
    }
}
