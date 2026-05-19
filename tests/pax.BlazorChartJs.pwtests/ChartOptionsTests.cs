using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChartOptionsTests : PageTest
{
    [Test]
    public async Task StepSizeTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/stackedchart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("StackedChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.Locator("input").FillAsync("20");
        await Page.Keyboard.PressAsync("Enter");

        await Task.Delay(Startup.ChartJsComputeDelay);

        var stepSize = await GetChartStepSize(canvasId);

        Assert.That(stepSize, Is.EqualTo(20));
    }

    [Test]
    public async Task TickCallbackResolvesRegisteredCallback()
    {
        var canvasId = await OpenCallbackChart();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.config?.options?.scales?.y?.ticks?.callback === 'function';
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var formattedValue = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart.config.options.scales.y.ticks.callback(1, 0, []);
            }",
            canvasId);

        Assert.That(formattedValue, Is.EqualTo("$1"));
    }

    [Test]
    public async Task IndexableOptionBackgroundColorResolvesRegisteredPatternCallback()
    {
        var canvasId = await OpenCallbackChart();

        await WaitForBackgroundColorCallback(canvasId);

        var result = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const backgroundColor = chart.data.datasets[0].backgroundColor;
                const firstPattern = backgroundColor({ chart });
                const secondPattern = backgroundColor({ chart });

                return `${typeof backgroundColor}|${firstPattern instanceof CanvasPattern}|${Object.is(firstPattern, secondPattern)}`;
            }",
            canvasId);

        Assert.That(result, Is.EqualTo("function|true|true"));
    }

    [Test]
    public async Task DatasetColorCallbackSwitchesWithDatasetUpdateApis()
    {
        var canvasId = await OpenCallbackChart();

        await WaitForBackgroundColorCallback(canvasId);

        await AssertBackgroundColorCallback(
            canvasId,
            "UpdateDataset Color Callback",
            "#dc2626");

        await AssertBackgroundColorCallback(
            canvasId,
            "UpdateDatasets Color Callback",
            "#16a34a");

        await AssertBackgroundColorCallback(
            canvasId,
            "UpdateDatasetSmooth Color Callback",
            "#7c3aed");
    }

    [Test]
    public async Task LegendLabelFilterResolvesRegisteredCallbackFromTypedConfig()
    {
        var canvasId = await OpenCallbackChart();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.config?.options?.plugins?.legend?.labels?.filter === 'function';
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var result = await Page.EvaluateAsync<bool>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart.config.options.plugins.legend.labels.filter({}, chart.data);
            }",
            canvasId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task TickCallbackResolvesRegisteredCallbackForCustomScaleId()
    {
        var canvasId = await OpenCallbackChart();

        var formattedValue = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                await chartInterop.updateChartOptions(
                    chartId,
                    { chartJsCallbacksModuleLocation: callbacksUrl },
                    {
                        scales: {
                            myScale: {
                                type: 'linear',
                                position: 'right',
                                ticks: {
                                    callback: { __chartJsFunction: 'formatCurrency' }
                                }
                            }
                        }
                    },
                    true);

                const chart = Chart.getChart(chartId);
                return chart.config.options.scales.myScale.ticks.callback(1, 0, []);
            }",
            canvasId);

        Assert.That(formattedValue, Is.EqualTo("$1"));
    }

    [Test]
    public async Task TooltipCallbackResolvesRegisteredCallbackFromGenericPath()
    {
        var canvasId = await OpenCallbackChart();

        var formattedValue = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                await chartInterop.updateChartOptions(
                    chartId,
                    { chartJsCallbacksModuleLocation: callbacksUrl },
                    {
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: { __chartJsFunction: 'formatTooltipLabel' }
                                }
                            }
                        }
                    },
                    true);

                const chart = Chart.getChart(chartId);
                return chart.config.options.plugins.tooltip.callbacks.label({ raw: 7 });
            }",
            canvasId);

        Assert.That(formattedValue, Is.EqualTo("tooltip:7"));
    }

    [Test]
    public async Task CallbackMarkerIsIgnoredWhenInternalMarkerFlagIsFalse()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var markerName = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                await chartInterop.updateChartOptions(
                    chartId,
                    { chartJsCallbacksModuleLocation: callbacksUrl },
                    {
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: { __chartJsFunction: 'formatTooltipLabel' }
                                }
                            }
                        }
                    },
                    false);

                const chart = Chart.getChart(chartId);
                return chart.config.options.plugins.tooltip.callbacks.label.__chartJsFunction;
            }",
            canvasId);

        Assert.That(markerName, Is.EqualTo("formatTooltipLabel"));
    }

    [Test]
    public async Task CallbackMarkerWithExtraPropertiesFailsClosedWithPath()
    {
        var canvasId = await OpenCallbackChart();

        var errorMessage = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                try {
                    await chartInterop.updateChartOptions(
                        chartId,
                        { chartJsCallbacksModuleLocation: callbacksUrl },
                        {
                            plugins: {
                                tooltip: {
                                    callbacks: {
                                        label: {
                                            __chartJsFunction: 'formatTooltipLabel',
                                            extra: true
                                        }
                                    }
                                }
                        }
                        },
                        true);
                    return '';
                } catch (error) {
                    return error?.message ?? String(error);
                }
            }",
            canvasId);

        Assert.That(errorMessage, Does.Contain("Invalid Chart.js callback marker"));
        Assert.That(errorMessage, Does.Contain("$.options.plugins.tooltip.callbacks.label"));
        Assert.That(errorMessage, Does.Contain("must not contain other properties"));
    }

    [Test]
    public async Task CallbackMarkerWithoutConfiguredModuleFailsClosedWithPath()
    {
        var canvasId = await OpenCallbackChart();

        var errorMessage = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.0-preview');

                try {
                    await chartInterop.updateChartOptions(
                        chartId,
                        {},
                        {
                            plugins: {
                                tooltip: {
                                    callbacks: {
                                        label: { __chartJsFunction: 'formatTooltipLabel' }
                                    }
                                }
                        }
                        },
                        true);
                    return '';
                } catch (error) {
                    return error?.message ?? String(error);
                }
            }",
            canvasId);

        Assert.That(errorMessage, Does.Contain("Chart.js callback marker"));
        Assert.That(errorMessage, Does.Contain("$.options.plugins.tooltip.callbacks.label"));
        Assert.That(errorMessage, Does.Contain("ChartJsSetupOptions.ChartJsCallbacksModuleLocation"));
    }

    [Test]
    public async Task CallbackMarkersUnderSkippedDataPathsAreIgnored()
    {
        await OpenCallbackChart();

        var result = await Page.EvaluateAsync<string>(
            @"async () => {
                const payload = {
                    data: {
                        labels: [{ __chartJsFunction: 'missingLabelCallback' }],
                        datasets: [{
                            data: [{ __chartJsFunction: 'missingDataCallback' }]
                        }]
                    }
                };

                await window.ChartJsInterop.resolveChartJsFunctions({}, payload, true);

                return `${payload.data.labels[0].__chartJsFunction}|${payload.data.datasets[0].data[0].__chartJsFunction}`;
            }");

        Assert.That(result, Is.EqualTo("missingLabelCallback|missingDataCallback"));
    }

    [Test]
    public async Task ScriptableLayoutPaddingResolvesRegisteredCallback()
    {
        var canvasId = await OpenScriptablePaddingChart();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.config?.options?.layout?.padding === 'function'
                    && chart?.config?.options?.plugins?.latestValueLabel?.display === true
                    && Boolean(Chart.registry?.plugins?.get?.('latestValueLabel'));
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var result = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const padding = chart.config.options.layout.padding;
                const small = padding({ chart: { width: 400 } });
                const large = padding({ chart: { width: 640 } });
                const enabled = chart.config.options.plugins.latestValueLabel.display === true;
                const pluginRegistered = Boolean(Chart.registry?.plugins?.get?.('latestValueLabel'));

                return `${typeof padding}|${small.right}|${large.right}|${enabled}|${pluginRegistered}`;
            }",
            canvasId);

        Assert.That(result, Is.EqualTo("function|70|30|true|true"));

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/barchart");
        await Expect(Page).ToHaveTitleAsync(new Regex("BarChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var barCanvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(barCanvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            barCanvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var latestValueLabelOptions = await Page.EvaluateAsync<bool>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.config?.options?.plugins?.latestValueLabel === undefined;
            }",
            barCanvasId);

        Assert.That(latestValueLabelOptions, Is.True);
    }

    private async Task<int> GetChartStepSize(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.options.scales.x.ticks.stepSize;
            }");
    }

    private async Task WaitForBackgroundColorCallback(string canvasId)
    {
        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.data?.datasets?.[0]?.backgroundColor === 'function';
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }

    private async Task AssertBackgroundColorCallback(string canvasId, string buttonText, string expectedColor)
    {
        var button = Page.GetByText(buttonText, new Microsoft.Playwright.PageGetByTextOptions { Exact = true });
        await Expect(button).ToHaveAttributeAsync("type", "button");
        await button.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"([chartId, expectedColor]) => {
                const chart = Chart.getChart(chartId);
                const backgroundColor = chart?.data?.datasets?.[0]?.backgroundColor;
                return typeof backgroundColor === 'function'
                    && backgroundColor({ chart, dataIndex: 0 }) === expectedColor;
            }",
            new[] { canvasId, expectedColor },
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var color = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart.data.datasets[0].backgroundColor({ chart, dataIndex: 0 });
            }",
            canvasId);

        Assert.That(color, Is.EqualTo(expectedColor));
    }

    private async Task<string> OpenCallbackChart()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/callbackchart");
        await Expect(Page).ToHaveTitleAsync(new Regex("Tick callback Chart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined && window.ChartJsInterop != undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        return canvasId!;
    }

    private async Task<string> OpenScriptablePaddingChart()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/scriptablepaddingchart");
        await Expect(Page).ToHaveTitleAsync(new Regex("Scriptable Padding Chart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined && window.ChartJsInterop != undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        return canvasId!;
    }
}
