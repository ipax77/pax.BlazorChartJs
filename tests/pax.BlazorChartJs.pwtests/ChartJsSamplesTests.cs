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

    [TestCase("linear-min-max", "Linear Scale - Min-Max")]
    [TestCase("linear-min-max-suggested", "Linear Scale - Suggested Min-Max")]
    [TestCase("linear-step-size", "Linear Scale - Step Size")]
    [TestCase("log", "Log Scale")]
    [TestCase("stacked", "Stacked Linear / Category")]
    [TestCase("time-line", "Time Scale")]
    [TestCase("time-max-span", "Time Scale - Max Span")]
    [TestCase("time-combo", "Time Scale - Combo Chart")]
    public async Task ScalesSamplePageRenders(string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/scales/{sampleId}");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = title, Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}']")).ToHaveCountAsync(1);
        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas")).ToHaveCountAsync(1);
    }

    [TestCase("linear-min-max")]
    [TestCase("linear-min-max-suggested")]
    [TestCase("linear-step-size")]
    [TestCase("log")]
    [TestCase("stacked")]
    [TestCase("time-line")]
    [TestCase("time-max-span")]
    [TestCase("time-combo")]
    public async Task ScalesSamplesShowScaleOptionsInConfigCode(string sampleId)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/scales/{sampleId}");

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas"))
            .ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Scales = new ChartJsOptionsScales");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("scales:");
    }

    [Test]
    public async Task AreaNavLinksSwitchSamplesAndFollowOtherCharts()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/area/line-boundaries");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Boundaries", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Line Datasets", Exact = true }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart Datasets", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("[data-chartjs-sample='line-datasets'] canvas")).ToHaveCountAsync(1);

        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radar Stacked", Exact = true }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Radar Chart Stacked", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("[data-chartjs-sample='radar-stacked'] canvas")).ToHaveCountAsync(1);
    }

    [Test]
    public async Task ScalesLinearStepSizeDatasetActionsMatchChartJsSample()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/linear-step-size");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Linear Scale - Step Size", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='linear-step-size'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var initialDatasets = await GetDatasetCount(canvasId);
        var initialDataCount = await GetFirstDatasetDataCount(canvasId);
        var before = await GetFirstDatasetDataJson(canvasId);

        await Page.Locator("[data-sample-action='linear-step-size-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataJson(canvasId), Is.Not.EqualTo(before));

        await Page.Locator("[data-sample-action='linear-step-size-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets + 1));

        await Page.Locator("[data-sample-action='linear-step-size-add-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount + 1));
        Assert.That(await GetLastLabel(canvasId), Is.EqualTo("August"));

        await Page.Locator("[data-sample-action='linear-step-size-remove-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(canvasId), Is.EqualTo(initialDatasets));

        await Page.Locator("[data-sample-action='linear-step-size-remove-data']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(initialDataCount));
    }

    [Test]
    public async Task ScalesLogSampleRandomizeChangesData()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/log");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Log Scale", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='log'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var before = await GetFirstDatasetDataJson(canvasId);

        await Page.Locator("[data-sample-action='log-randomize']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await GetFirstDatasetDataJson(canvasId), Is.Not.EqualTo(before));
    }

    [Test]
    public async Task ScalesSamplesSerializeExpectedScaleOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/log");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Log Scale", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var logCanvasId = await Page.Locator("[data-chartjs-sample='log'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetScaleType(logCanvasId, "y"), Is.EqualTo("logarithmic"));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/stacked");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Stacked Linear / Category", Exact = true }))
            .ToBeVisibleAsync();

        var stackedCanvasId = await Page.Locator("[data-chartjs-sample='stacked'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        var stackedScale = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return JSON.stringify({
                    type: chart.options.scales.y2.type,
                    labels: chart.options.scales.y2.labels
                });
            }", stackedCanvasId);

        Assert.That(stackedScale, Is.EqualTo("{\"type\":\"category\",\"labels\":[\"ON\",\"OFF\"]}"));
    }

    [TestCase("time-line")]
    [TestCase("time-max-span")]
    [TestCase("time-combo")]
    public async Task ScalesTimeSamplesUseTimeScale(string sampleId)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/scales/{sampleId}");

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas"))
            .ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        Assert.That(await GetScaleType(canvasId, "x"), Is.EqualTo("time"));
    }

    [Test]
    public async Task ScalesTimeComboUsesOfficialXScaleSettings()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/time-combo");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Time Scale - Combo Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='time-combo'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var scaleInfo = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                const x = chart.config.options.scales.x;
                return JSON.stringify({
                    type: x.type,
                    display: x.display,
                    offset: x.offset,
                    tickSource: x.ticks.source,
                    timeUnit: x.time.unit
                });
            }", canvasId);

        Assert.That(scaleInfo, Is.EqualTo("{\"type\":\"time\",\"display\":true,\"offset\":true,\"tickSource\":\"data\",\"timeUnit\":\"day\"}"));
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("display: true");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("ticks: { source: 'data' }");
    }

    [Test]
    public async Task ScalesTimeMaxSpanRevivesMajorTickFontCallback()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scales/time-max-span");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Time Scale - Max Span", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='time-max-span'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        var callbackInfo = await Page.EvaluateAsync<string>(@"canvasId => {
                const chart = Chart.getChart(canvasId);
                return `${chart.config.options.spanGaps}:${typeof chart.config.options.scales.x.ticks.font}`;
            }", canvasId);

        Assert.That(callbackInfo, Is.EqualTo("172800000:function"));
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("timeMaxSpanMajorTickFont");
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

    [TestCase("scale-options", "center", "Center Positioning")]
    [TestCase("scale-options", "grid", "Grid Configuration")]
    [TestCase("scale-options", "ticks", "Tick Configuration")]
    [TestCase("scale-options", "titles", "Title Configuration")]
    [TestCase("legend", "events", "Events")]
    [TestCase("legend", "html", "HTML Legend")]
    [TestCase("legend", "point-style", "Point Style")]
    [TestCase("legend", "position", "Position")]
    [TestCase("legend", "title", "Alignment and Title Position")]
    [TestCase("title", "alignment", "Alignment")]
    [TestCase("tooltip", "content", "Custom Tooltip Content")]
    [TestCase("tooltip", "html", "External HTML Tooltip")]
    [TestCase("tooltip", "interactions", "Interaction Modes")]
    [TestCase("tooltip", "point-style", "Point Style")]
    [TestCase("tooltip", "position", "Position")]
    [TestCase("scriptable", "bar", "Bar Chart")]
    [TestCase("scriptable", "bubble", "Bubble Chart")]
    [TestCase("scriptable", "line", "Line Chart")]
    [TestCase("scriptable", "pie", "Pie Chart")]
    [TestCase("scriptable", "polar", "Polar Area Chart")]
    [TestCase("scriptable", "radar", "Radar Chart")]
    [TestCase("animations", "delay", "Delay")]
    [TestCase("animations", "drop", "Drop")]
    [TestCase("animations", "loop", "Loop")]
    [TestCase("animations", "progressive-line", "Progressive Line")]
    [TestCase("animations", "progressive-line-easing", "Progressive Line With Easing")]
    public async Task BacklogSamplePageRenders(string category, string sampleId, string title)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + $"/chartjs-samples/{category}/{sampleId}");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = title, Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}']")).ToHaveCountAsync(1);
        await Expect(Page.Locator($"[data-chartjs-sample='{sampleId}'] canvas")).ToHaveCountAsync(1);
    }

    [Test]
    public async Task BacklogSamplesExposeOfficialExtraCodeTabs()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/animations/progressive-line");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Progressive Line", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Page.Locator("[data-code-tab='animation']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='animation']")).ToHaveAttributeAsync("aria-selected", "true");
        await Page.Locator("[data-code-tab='data']").ClickAsync();
        await Expect(Page.Locator("[data-code-tab='data']")).ToHaveAttributeAsync("aria-selected", "true");
    }

    [Test]
    public async Task AdvancedDataDecimationUsesLargeTimeDatasetAndOptionActions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/advanced/data-decimation");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Data Decimation", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.GetByText("Loading time adapter")).ToHaveCountAsync(0);
        await Expect(Page.Locator("[data-chartjs-sample='data-decimation'] [data-sample-action]")).ToHaveCountAsync(4);
        await Expect(Page.Locator("[data-chartjs-sample='data-decimation'] canvas"))
            .ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='data-decimation'] canvas").GetAttributeAsync("id");
        await Page.WaitForFunctionAsync(
            "canvasId => Chart.getChart(canvasId)?.data?.datasets?.[0]?.data?.length === 100000",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        Assert.That(await GetScaleType(canvasId, "x"), Is.EqualTo("time"));
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.animation"), Is.EqualTo("false"));
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.parsing"), Is.EqualTo("false"));
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.interaction.mode}:${chart.options.interaction.axis}:${chart.options.interaction.intersect}`"), Is.EqualTo("\"nearest:x:false\""));
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.plugins.decimation.enabled}:${chart.options.plugins.decimation.algorithm}`"), Is.EqualTo("\"false:min-max\""));

        await Page.Locator("[data-sample-action='data-decimation-min-max']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.plugins.decimation.enabled}:${chart.options.plugins.decimation.algorithm}:${chart.options.plugins.decimation.samples}`"), Is.EqualTo("\"true:min-max:undefined\""));

        await Page.Locator("[data-sample-action='data-decimation-lttb-50']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.plugins.decimation.enabled}:${chart.options.plugins.decimation.algorithm}:${chart.options.plugins.decimation.samples}`"), Is.EqualTo("\"true:lttb:50\""));

        await Page.Locator("[data-sample-action='data-decimation-lttb-500']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.plugins.decimation.enabled}:${chart.options.plugins.decimation.algorithm}:${chart.options.plugins.decimation.samples}`"), Is.EqualTo("\"true:lttb:500\""));

        await Page.Locator("[data-sample-action='data-decimation-no-decimation']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.plugins.decimation.enabled"), Is.EqualTo("false"));
    }

    [Test]
    public async Task AdvancedDataDecimationCodeTabsShowGeneratorInsteadOfExpandedData()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/advanced/data-decimation");

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Data Decimation", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.GetByText("Loading time adapter")).ToHaveCountAsync(0);
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Animation = false");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("animation: false");

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("object[] pointData = new object[PointCount]");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("const NUM_POINTS = 100000");
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).Not.ToContainTextAsync("generated values omitted");

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("config.UpdateChartOptions()");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("LTTB decimation (500 samples)");
    }

    [Test]
    public async Task BacklogTitleAlignmentActionUpdatesOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/title/alignment");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Alignment", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='alignment'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='alignment-title-alignment-start']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.plugins.title.align"), Is.EqualTo("\"start\""));

        await Page.Locator("[data-sample-action='alignment-title-alignment-end']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.plugins.title.align"), Is.EqualTo("\"end\""));
    }

    [Test]
    public async Task BacklogScaleOptionsCenterActionUpdatesScalePositions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/center");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Center Positioning", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='center'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='center-position-center']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.scales.x.position"), Is.EqualTo("\"center\""));

        await Page.Locator("[data-sample-action='center-position-values']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.scales.x.position"), Is.EqualTo("{\"y\":30}"));
    }

    [Test]
    public async Task BacklogScaleOptionsCodeTabsShowFullSampleCode()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/center");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Center Positioning", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Type = ChartType.scatter");
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Scales = new ChartJsOptionsScales");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("type: 'scatter'");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code']")).ToHaveCountAsync(0);

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("SetScalePositions");
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("config.UpdateChartOptions()");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("chart.options.scales.x.position");

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/grid");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Grid Configuration", Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("ChartJsGrid");
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("scaleOptionsGridColor");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("const DISPLAY");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("scaleOptionsGridColor(context)");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("export const chartJsCallbacks");

        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("config.SetData(data)");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("name: 'Randomize'");

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/ticks");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Tick Configuration", Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("ChartJsFunction.FromName(\"scaleOptionsTickLabel\")");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("scaleOptionsTickLabel(value, index)");
        await Expect(Page.Locator("[aria-label='Chart.js callback module code'] code")).ToContainTextAsync("this.getLabelForValue(value)");
        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("June\\n2015");
        await Page.Locator("[data-code-tab='actions']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("SetTickAlign");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("chart.options.scales.x.ticks.align");

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/titles");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Title Configuration", Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[data-code-tab='actions']")).ToHaveCountAsync(0);
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Family = \"Comic Sans MS\"");
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("Padding = new Padding");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("padding: {top: 20");

        await Page.Locator("[data-code-tab='setup']").ClickAsync();
        await Expect(Page.Locator("[aria-label='C# code'] code.language-csharp")).ToContainTextAsync("const int DataCount = 7");
        await Expect(Page.Locator("[aria-label='JavaScript code'] code.language-javascript")).ToContainTextAsync("const NUMBER_CFG");
    }

    [Test]
    public async Task BacklogRemainingSectionsShowGeneratedConfigAndCallbackCode()
    {
        await AssertBacklogGeneratedCode(
            "/chartjs-samples/legend/events",
            "Events",
            "OnHover = ChartJsFunction.FromName(\"legendHandleHover\")",
            "legendHandleHover(_event, item, legend)",
            "scriptableBarBackgroundColor(context)");

        await AssertBacklogGeneratedCode(
            "/chartjs-samples/title/alignment",
            "Alignment",
            "Title = new Title",
            null,
            null);

        await AssertBacklogGeneratedCode(
            "/chartjs-samples/tooltip/content",
            "Custom Tooltip Content",
            "Footer = ChartJsFunction.FromName(\"tooltipContentFooter\")",
            "tooltipContentFooter(items)",
            "renderExternalTooltip(context)");

        await AssertBacklogGeneratedCode(
            "/chartjs-samples/scriptable/bar",
            "Bar Chart",
            "BackgroundColor = ChartJsFunction.FromName(\"scriptableBarBackgroundColor\")",
            "scriptableBarBackgroundColor(context)",
            "scriptableBubbleRadius(context)");

        await AssertBacklogGeneratedCode(
            "/chartjs-samples/animations/delay",
            "Delay",
            "Animation = new Animation",
            "animationDelay(context)",
            "animationProgressiveFromNaN()");
    }

    [Test]
    public async Task BacklogScriptableLineRevivesElementCallbacksAndPieCutoutToggles()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scriptable/line");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Line Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var lineCanvasId = await Page.Locator("[data-chartjs-sample='line'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetChartOptionJson(lineCanvasId, "typeof chart.config.options.elements.point.radius"), Is.EqualTo("\"function\""));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scriptable/pie");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Pie Chart", Exact = true })).ToBeVisibleAsync();
        var pieCanvasId = await Page.Locator("[data-chartjs-sample='pie'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("[data-sample-action='pie-toggle-doughnut']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(pieCanvasId, "chart.options.cutout"), Is.EqualTo("\"50%\""));
    }

    [Test]
    public async Task BacklogAnimationProgressiveUsesLargeDatasetsAndCallbacks()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/animations/progressive-line");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Progressive Line", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='progressive-line'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(1000));
        Assert.That(await GetChartOptionJson(canvasId, "typeof chart.config.options.animations.x.delay"), Is.EqualTo("\"function\""));
    }

    [Test]
    public async Task BacklogTicksActionsUpdateAlignment()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scale-options/ticks");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Tick Configuration", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("[data-chartjs-sample='ticks'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);

        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(12));
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.scales.x.ticks.color"), Is.EqualTo("\"red\""));

        await Page.Locator("[data-sample-action='ticks-align-start']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.scales.x.ticks.align"), Is.EqualTo("\"start\""));

        await Page.Locator("[data-sample-action='ticks-align-end']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.scales.x.ticks.align"), Is.EqualTo("\"end\""));
    }

    [Test]
    public async Task BacklogTooltipInteractionsIncludeToggleIntersect()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/tooltip/interactions");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Interaction Modes", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-sample-action='interactions-toggle-intersect']")).ToHaveCountAsync(1);

        var canvasId = await Page.Locator("[data-chartjs-sample='interactions'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.interaction.intersect"), Is.EqualTo("false"));

        await Page.Locator("[data-sample-action='interactions-toggle-intersect']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.interaction.intersect"), Is.EqualTo("true"));

        await Page.Locator("[data-sample-action='interactions-mode-nearest-y']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "`${chart.options.interaction.mode}:${chart.options.interaction.axis}`"), Is.EqualTo("\"nearest:y\""));
    }

    [Test]
    public async Task BacklogScriptableBarAndBubbleUseOfficialCallbacks()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scriptable/bar");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bar Chart", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var barCanvasId = await Page.Locator("[data-chartjs-sample='bar'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetFirstDatasetDataCount(barCanvasId), Is.EqualTo(16));
        Assert.That(await GetChartOptionJson(barCanvasId, "typeof chart.config.options.elements.bar.backgroundColor"), Is.EqualTo("\"function\""));
        Assert.That(await GetChartOptionJson(barCanvasId, "typeof chart.config.options.elements.bar.borderColor"), Is.EqualTo("\"function\""));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/scriptable/bubble");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bubble Chart", Exact = true })).ToBeVisibleAsync();

        var bubbleCanvasId = await Page.Locator("[data-chartjs-sample='bubble'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetDatasetCount(bubbleCanvasId), Is.EqualTo(2));
        Assert.That(await GetFirstDatasetDataCount(bubbleCanvasId), Is.EqualTo(16));
        Assert.That(await GetChartOptionJson(bubbleCanvasId, "chart.options.aspectRatio"), Is.EqualTo("1"));
        Assert.That(await GetChartOptionJson(bubbleCanvasId, "chart.options.elements.point.hoverBackgroundColor"), Is.EqualTo("\"transparent\""));
        Assert.That(await GetChartOptionJson(bubbleCanvasId, "typeof chart.config.options.elements.point.hoverBorderColor"), Is.EqualTo("\"function\""));
    }

    [Test]
    public async Task BacklogAnimationDelayAndLoopMatchOfficialChartTypes()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/animations/delay");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Delay", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var delayCanvasId = await Page.Locator("[data-chartjs-sample='delay'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetChartOptionJson(delayCanvasId, "chart.config.type"), Is.EqualTo("\"bar\""));
        Assert.That(await GetDatasetCount(delayCanvasId), Is.EqualTo(3));
        Assert.That(await GetChartOptionJson(delayCanvasId, "`${chart.options.scales.x.stacked}:${chart.options.scales.y.stacked}`"), Is.EqualTo("\"true:true\""));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/animations/loop");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Loop", Exact = true })).ToBeVisibleAsync();

        var loopCanvasId = await Page.Locator("[data-chartjs-sample='loop'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetChartOptionJson(loopCanvasId, "chart.config.type"), Is.EqualTo("\"line\""));
        Assert.That(await GetDatasetCount(loopCanvasId), Is.EqualTo(2));
        Assert.That(await GetChartOptionJson(loopCanvasId, "chart.data.datasets.map(dataset => dataset.label).join('|')"), Is.EqualTo("\"Dataset 1|Dataset 2\""));
        Assert.That(await GetChartOptionJson(loopCanvasId, "chart.options.elements.point.hoverBackgroundColor"), Is.EqualTo("\"yellow\""));

        await Page.Locator("[data-sample-action='loop-add-dataset']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetDatasetCount(loopCanvasId), Is.EqualTo(3));
    }

    [Test]
    public async Task BacklogProgressiveLineEasingExposesOfficialActionsAndRestarts()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/chartjs-samples/animations/progressive-line-easing");
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Progressive Line With Easing", Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Expect(Page.Locator("[data-chartjs-sample='progressive-line-easing'] [data-sample-action]")).ToHaveCountAsync(8);

        var canvasId = await Page.Locator("[data-chartjs-sample='progressive-line-easing'] canvas").GetAttributeAsync("id");
        await Task.Delay(Startup.ChartJsLoadDelay);
        Assert.That(await GetFirstDatasetDataCount(canvasId), Is.EqualTo(1000));
        Assert.That(await GetChartOptionJson(canvasId, "typeof chart.config.options.animations.x.delay"), Is.EqualTo("\"function\""));

        await Page.Locator("[data-sample-action='progressive-line-easing-easing-ease-in-quint']").ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);
        Assert.That(await GetChartOptionJson(canvasId, "chart.options.animation.easing"), Is.EqualTo("\"easeInQuint\""));
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

    private async Task<string?> GetScaleType(string? canvasId, string scaleId)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string?>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return chart.options.scales[args.scaleId]?.type;
            }", new { canvasId, scaleId });
    }

    private async Task<string> GetChartOptionJson(string? canvasId, string expression)
    {
        await WaitForChart(canvasId);
        return await Page.EvaluateAsync<string>(@"args => {
                const chart = Chart.getChart(args.canvasId);
                return JSON.stringify(Function('chart', `return ${args.expression}`)(chart));
            }", new { canvasId, expression });
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

    private async Task AssertBacklogGeneratedCode(string path, string heading, string csharpNeedle, string? callbackNeedle, string? unexpectedCallbackNeedle)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + path);
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = heading, Exact = true }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var csharpCode = Page.Locator("[aria-label='C# code'] code.language-csharp");
        await Expect(csharpCode).ToContainTextAsync("var config = new ChartJsConfig");
        await Expect(csharpCode).ToContainTextAsync(csharpNeedle);
        await Expect(csharpCode).Not.ToHaveTextAsync("var config = new ChartJsConfig { Data = data, Options = options };");

        var javaScriptCode = Page.Locator("[aria-label='JavaScript code'] code.language-javascript");
        await Expect(javaScriptCode).ToContainTextAsync("const config = {");
        await Expect(javaScriptCode).Not.ToHaveTextAsync("const config = { type, data, options };");

        if (callbackNeedle is not null)
        {
            var callbackCode = Page.Locator("[aria-label='Chart.js callback module code'] code");
            await Expect(callbackCode).ToContainTextAsync(callbackNeedle);
            await Expect(callbackCode).ToContainTextAsync("export const chartJsCallbacks");
            if (unexpectedCallbackNeedle is not null)
            {
                await Expect(callbackCode).Not.ToContainTextAsync(unexpectedCallbackNeedle);
            }
        }
    }
}
