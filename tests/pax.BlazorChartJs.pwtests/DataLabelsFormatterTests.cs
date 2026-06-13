using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DataLabelsFormatterTests : PageTest
{
    [Test]
    public async Task DataLabelsFormatterResolvesRegisteredCallback()
    {
        var canvasId = await OpenDataLabelsChart();

        var showLabels = Page.GetByText("ShowLabels", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await Expect(showLabels).ToHaveAttributeAsync("type", "button");
        await showLabels.ClickAsync();

        await WaitForPluginFormatter(canvasId);

        var formattedValue = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart.config.options.plugins.datalabels.formatter(0.234, { chart, dataIndex: 0, datasetIndex: 0 });
            }",
            canvasId);

        Assert.That(formattedValue, Is.EqualTo("23%"));
    }

    [Test]
    public async Task DatasetDataLabelsFormatterResolvesRegisteredCallback()
    {
        var canvasId = await OpenDataLabelsChart();

        var showLabels = Page.GetByText("ShowLabels per Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await Expect(showLabels).ToHaveAttributeAsync("type", "button");
        await showLabels.ClickAsync();

        await WaitForDatasetFormatter(canvasId);

        var formattedValue = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart.config.data.datasets[0].datalabels.formatter(0.456, { chart, dataIndex: 0, datasetIndex: 0 });
            }",
            canvasId);

        Assert.That(formattedValue, Is.EqualTo("46%"));
    }

    [Test]
    public async Task MissingDataLabelsFormatterCallbackFailsClosed()
    {
        var canvasId = await OpenDataLabelsChart();

        var errorMessage = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.1');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                try {
                    await chartInterop.updateChartOptions(
                        chartId,
                        { chartJsCallbacksModuleLocation: callbacksUrl },
                        { plugins: { datalabels: { formatter: { __chartJsFunction: 'missingFormatter' } } } },
                        true);
                    return '';
                } catch (error) {
                    return error?.message ?? String(error);
                }
            }",
            canvasId);

        Assert.That(errorMessage, Does.Contain("missingFormatter"));
    }

    [Test]
    public async Task InvalidDataLabelsFormatterCallbackNameFailsClosed()
    {
        var canvasId = await OpenDataLabelsChart();

        var errorMessage = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.1');
                const callbacksUrl = new URL('./_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js', document.baseURI).href;

                try {
                    await chartInterop.updateChartOptions(
                        chartId,
                        { chartJsCallbacksModuleLocation: callbacksUrl },
                        { plugins: { datalabels: { formatter: { __chartJsFunction: 'bad.name' } } } },
                        true);
                    return '';
                } catch (error) {
                    return error?.message ?? String(error);
                }
            }",
            canvasId);

        Assert.That(errorMessage, Does.Contain("Invalid Chart.js callback name"));
        Assert.That(errorMessage, Does.Contain("bad.name"));
    }

    private async Task<string> OpenDataLabelsChart()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/datalabelschart");
        await Expect(Page).ToHaveTitleAsync(new Regex("DataLabelsChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        return canvasId!;
    }

    private async Task WaitForPluginFormatter(string canvasId)
    {
        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.config?.options?.plugins?.datalabels?.formatter === 'function';
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }

    private async Task WaitForDatasetFormatter(string canvasId)
    {
        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.config?.data?.datasets?.[0]?.datalabels?.formatter === 'function';
            }",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }
}
