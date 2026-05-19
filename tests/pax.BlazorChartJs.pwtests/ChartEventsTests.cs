using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChartEventsTests : PageTest
{
    [Test]
    public async Task ResizeEventIncludesViewportDimensionsTest()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        await Expect(Page).ToHaveTitleAsync(new Regex("EventsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvas = Page.Locator("canvas").First;
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.SetViewportSizeAsync(960, 900);
        await Page.GetByText("ResizeChart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true }).ClickAsync();

        var resizeText = await WaitForLatestResizeEventTextAsync(new Regex(@"ChartJsResizeEvent.*WindowHeight = 900.*WindowWidth = 960"));
        var dimensions = await Page.EvaluateAsync<BrowserDimensions>(
            """
            () => {
                const canvas = document.querySelector('canvas');
                return {
                    CanvasWidth: canvas.clientWidth,
                    WindowHeight: window.innerHeight,
                    WindowWidth: window.innerWidth
                };
            }
            """);

        Assert.Multiple(() =>
        {
            Assert.That(resizeText, Does.Contain("ChartJsResizeEvent"));
            Assert.That(resizeText, Does.Match(@"Height = [1-9]\d*"));
            Assert.That(resizeText, Does.Match(@"Width = [1-9]\d*"));
            Assert.That(resizeText, Does.Contain("WindowHeight = 900"));
            Assert.That(resizeText, Does.Contain("WindowWidth = 960"));
            Assert.That(dimensions.WindowHeight, Is.EqualTo(900));
            Assert.That(dimensions.WindowWidth, Is.EqualTo(960));
            Assert.That(dimensions.CanvasWidth, Is.Positive);
            Assert.That(dimensions.CanvasWidth, Is.Not.EqualTo(dimensions.WindowWidth));
        });
    }

    [Test]
    public async Task ClickEventTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("EventsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );
        await Task.Delay(Startup.ChartJsLoadDelay);
        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );
        await Task.Delay(Startup.ChartJsComputeDelay);
        var clickResult = Page.GetByTestId("latest-chart-event");
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsLabelClickEvent"));
    }

    [Test]
    public async Task GlobalNativeClickCallbackIsPreservedWhenClickEventBridgeIsEnabled()
    {
        var canvasId = await OpenEventsChartAsync();
        var canvas = Page.Locator("canvas").First;

        await Page.EvaluateAsync("() => { window.chartJsDefaultClickCount = 0; window.chartJsDefaultClickArgs = null; }");
        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        });

        var clickText = await WaitForLatestEventTextAsync(new Regex(@"ChartJsLabelClickEvent"));
        var snapshot = await Page.EvaluateAsync<string>(
            """
            (chartId) => [
                window.chartJsDefaultClickCount ?? 0,
                window.chartJsDefaultClickArgs?.chartId ?? '',
                window.chartJsDefaultClickArgs?.eventType ?? ''
            ].join('|')
            """,
            canvasId);

        Assert.That(clickText, Does.Contain("ChartJsLabelClickEvent"));
        Assert.That(snapshot, Is.EqualTo($"1|{canvasId}|click"));
    }

    [Test]
    public async Task PerChartNativeClickCallbackIsPreservedWhenClickEventBridgeIsEnabled()
    {
        var canvasId = await OpenEventsChartAsync();
        await ConfigureNativeCallbackOptionAsync(canvasId, "onClick", "chartEventBridgeClick", "onClickEvent");

        var canvas = Page.Locator("canvas").First;
        await Page.EvaluateAsync("() => { window.chartJsNativeClickCount = 0; window.chartJsNativeClickArgs = null; }");
        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        });

        var clickText = await WaitForLatestEventTextAsync(new Regex(@"ChartJsLabelClickEvent"));
        var snapshot = await Page.EvaluateAsync<string>(
            """
            (chartId) => [
                window.chartJsNativeClickCount ?? 0,
                window.chartJsNativeClickArgs?.chartId ?? '',
                window.chartJsNativeClickArgs?.eventType ?? ''
            ].join('|')
            """,
            canvasId);

        Assert.That(clickText, Does.Contain("ChartJsLabelClickEvent"));
        Assert.That(snapshot, Is.EqualTo($"1|{canvasId}|click"));
    }

    [Test]
    public async Task NativeResizeCallbackIsPreservedWhenResizeEventBridgeIsEnabled()
    {
        var canvasId = await OpenEventsChartAsync();
        await ConfigureNativeCallbackOptionAsync(canvasId, "onResize", "chartEventBridgeResize", "onResizeEvent");

        await Page.EvaluateAsync("() => { window.chartJsNativeResizeCount = 0; window.chartJsNativeResizeArgs = null; }");
        await Page.GetByText("ResizeChart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true }).ClickAsync();

        var resizeText = await WaitForLatestResizeEventTextAsync(new Regex(@"ChartJsResizeEvent"));
        var snapshot = await Page.EvaluateAsync<string>(
            """
            (chartId) => [
                (window.chartJsNativeResizeCount ?? 0) > 0,
                window.chartJsNativeResizeArgs?.chartId ?? '',
                window.chartJsNativeResizeArgs?.width > 0,
                window.chartJsNativeResizeArgs?.height > 0
            ].join('|')
            """,
            canvasId);

        Assert.That(resizeText, Does.Contain("ChartJsResizeEvent"));
        Assert.That(snapshot, Is.EqualTo($"true|{canvasId}|true|true"));
    }

    [Test]
    public async Task HoverEventTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("EventsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.HoverAsync(new Microsoft.Playwright.LocatorHoverOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        });

        var hoverText = await WaitForLatestEventTextAsync(new Regex(@"ChartJsLabelHoverEvent"));

        Assert.That(hoverText, Does.Contain("ChartJsLabelHoverEvent"));
    }

    [Test]
    public async Task DisableEventsTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("EventsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load and finish animation
        await Task.Delay(Startup.ChartJsLoadDelay);



        var removeEvents = Page.GetByText("RemoveEvents", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await removeEvents.ClickAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );
        var clickResult = Page.GetByTestId("latest-chart-event");
        var textPrev = await clickResult.AllInnerTextsAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 110, Y = 110 }
        }
        );
        // wait for ChartJs
        await Task.Delay(Startup.ChartJsLoadDelay);

        var textAfter = await clickResult.AllInnerTextsAsync();

        Assert.That(textAfter, Is.EqualTo(textPrev));
    }

    [Test]
    public async Task ToggleEventsTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("EventsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load and finish animation
        await Task.Delay(Startup.ChartJsLoadDelay);
        await Task.Delay(Startup.ChartJsLoadDelay);

        var clickResult = Page.GetByTestId("latest-chart-event");
        var textPrev = await clickResult.AllInnerTextsAsync();

        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsAnimationCompleteEvent"));

        var removeEvents = Page.GetByText("RemoveEvents", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await removeEvents.ClickAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var textAfter = await clickResult.AllInnerTextsAsync();

        Assert.That(textAfter, Is.EqualTo(textPrev));

        var addEvents = Page.GetByText("AddEvents", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await addEvents.ClickAsync();

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsLabelClickEvent"));
    }

    [Test]
    public async Task InitEventIncludesInitialViewportDimensionsTest()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        await Expect(Page).ToHaveTitleAsync(
            new Regex("EventsChart"),
            new Microsoft.Playwright.PageAssertionsToHaveTitleOptions()
            {
                Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds
            });

        var canvas = Page.Locator("canvas").First;
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        var initText = await WaitForLatestInitEventTextAsync(
            new Regex(@"ChartJsInitEvent.*WindowHeight = 900.*WindowWidth = 1280"));

        var dimensions = await Page.EvaluateAsync<BrowserDimensions>(
            """
        () => {
            const canvas = document.querySelector('canvas');
            return {
                CanvasWidth: canvas.clientWidth,
                CanvasHeight: canvas.clientHeight,
                WindowHeight: window.innerHeight,
                WindowWidth: window.innerWidth
            };
        }
        """);

        Assert.Multiple(() =>
        {
            Assert.That(initText, Does.Contain("ChartJsInitEvent"));
            Assert.That(initText, Does.Match(@"Height = [1-9]\d*"));
            Assert.That(initText, Does.Match(@"Width = [1-9]\d*"));
            Assert.That(initText, Does.Contain("WindowHeight = 900"));
            Assert.That(initText, Does.Contain("WindowWidth = 1280"));

            Assert.That(dimensions.WindowHeight, Is.EqualTo(900));
            Assert.That(dimensions.WindowWidth, Is.EqualTo(1280));
            Assert.That(dimensions.CanvasWidth, Is.Positive);
            Assert.That(dimensions.CanvasHeight, Is.Positive);
        });
    }

    [Test]
    public async Task InitEventIncludesViewportDimensionsTest()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        await Expect(Page).ToHaveTitleAsync(
            new Regex("EventsChart"),
            new Microsoft.Playwright.PageAssertionsToHaveTitleOptions()
            {
                Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds
            });

        var initText = await WaitForLatestInitEventTextAsync(
            new Regex(@"ChartJsInitEvent.*WindowHeight = 900.*WindowWidth = 1280"));

        Assert.Multiple(() =>
        {
            Assert.That(initText, Does.Contain("ChartJsInitEvent"));
            Assert.That(initText, Does.Match(@"Height = [1-9]\d*"));
            Assert.That(initText, Does.Match(@"Width = [1-9]\d*"));
            Assert.That(initText, Does.Contain("WindowHeight = 900"));
            Assert.That(initText, Does.Contain("WindowWidth = 1280"));
        });
    }

    private async Task<string> OpenEventsChartAsync()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/eventschart");

        await Expect(Page).ToHaveTitleAsync(
            new Regex("EventsChart"),
            new Microsoft.Playwright.PageAssertionsToHaveTitleOptions()
            {
                Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds
            });

        var canvas = Page.Locator("canvas").First;
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            "(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        return canvasId!;
    }

    private async Task ConfigureNativeCallbackOptionAsync(
        string canvasId,
        string optionName,
        string callbackName,
        string eventFlagName)
    {
        await Page.EvaluateAsync(
            """
            async ([chartId, optionName, callbackName, eventFlagName]) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;
                const options = {
                    responsive: true,
                    maintainAspectRatio: true,
                    [optionName]: { __chartJsFunction: callbackName },
                    [eventFlagName]: true
                };

                await chartInterop.updateChartOptions(
                    chartId,
                    { chartJsCallbacksModuleLocation: callbacksUrl },
                    options,
                    true);
            }
            """,
            new[] { canvasId, optionName, callbackName, eventFlagName });
    }

    private async Task<string> WaitForLatestEventTextAsync(Regex expected)
    {
        var chartEvent = Page.GetByTestId("latest-chart-event");
        var deadline = DateTime.UtcNow.AddSeconds(10);
        var latestText = "";

        while (DateTime.UtcNow < deadline)
        {
            if (await chartEvent.CountAsync() > 0)
            {
                latestText = await chartEvent.InnerTextAsync();

                if (expected.IsMatch(latestText))
                {
                    return latestText;
                }
            }

            await Task.Delay(250);
        }

        Assert.Fail($"Expected latest event text to match '{expected}', but was '{latestText}'.");
        return latestText;
    }

    private async Task<string> WaitForLatestResizeEventTextAsync(Regex expected)
    {
        var resizeEvent = Page.GetByTestId("latest-resize-event");
        var deadline = DateTime.UtcNow.AddSeconds(10);
        var latestText = "";

        while (DateTime.UtcNow < deadline)
        {
            if (await resizeEvent.CountAsync() > 0)
            {
                latestText = await resizeEvent.InnerTextAsync();

                if (expected.IsMatch(latestText))
                {
                    return latestText;
                }
            }

            await Task.Delay(250);
        }

        Assert.Fail($"Expected latest resize event text to match '{expected}', but was '{latestText}'.");
        return latestText;
    }

    private async Task<string> WaitForLatestInitEventTextAsync(Regex expected)
    {
        var initEvent = Page.GetByTestId("latest-init-event");
        var deadline = DateTime.UtcNow.AddSeconds(10);
        var latestText = "";

        while (DateTime.UtcNow < deadline)
        {
            if (await initEvent.CountAsync() > 0)
            {
                latestText = await initEvent.InnerTextAsync();

                if (expected.IsMatch(latestText))
                {
                    return latestText;
                }
            }

            await Task.Delay(250);
        }

        Assert.Fail($"Expected latest init event text to match '{expected}', but was '{latestText}'.");
        return latestText;
    }

    private sealed class BrowserDimensions
    {
        public double CanvasHeight { get; set; }
        public double CanvasWidth { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
    }
}
