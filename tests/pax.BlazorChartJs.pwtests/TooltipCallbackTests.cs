using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TooltipCallbackTests : PageTest
{
    [Test]
    public async Task TooltipCallbackDesignsSwitchWithButtons()
    {
        var canvasId = await OpenTooltipCallbackChart();

        await WaitForContentCallbacks(canvasId);

        var contentResult = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const callbacks = chart.options.plugins.tooltip.callbacks;

                return [
                    callbacks.title([{ label: 'Apr' }]),
                    callbacks.label({ dataset: { label: 'Revenue' }, formattedValue: '32', raw: 32 }),
                    callbacks.footer([{ raw: 32 }, { raw: 24 }])
                ].join('|');
            }",
            canvasId);

        Assert.That(contentResult, Is.EqualTo("Sales Apr|Revenue: 32 units|Total: 56 units"));

        var colorButton = Page.GetByText("Color", new PageGetByTextOptions() { Exact = true });
        await Expect(colorButton).ToHaveAttributeAsync("type", "button");
        await colorButton.ClickAsync();

        await WaitForTooltipCallback(canvasId, "labelColor");
        await WaitForTooltipCallback(canvasId, "labelTextColor");

        var colorResult = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const callbacks = chart.options.plugins.tooltip.callbacks;
                const labelColor = callbacks.labelColor({ datasetIndex: 0 });
                const labelTextColor = callbacks.labelTextColor({ datasetIndex: 0 });

                return `${labelColor.backgroundColor}|${labelColor.borderColor}|${labelColor.borderWidth}|${labelTextColor}`;
            }",
            canvasId);

        Assert.That(colorResult, Is.EqualTo("#60a5fa|#1d4ed8|2|#1e3a8a"));

        var pointButton = Page.GetByText("Point", new PageGetByTextOptions() { Exact = true });
        await Expect(pointButton).ToHaveAttributeAsync("type", "button");
        await pointButton.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const tooltip = chart?.options?.plugins?.tooltip;
                return tooltip?.usePointStyle === true
                    && typeof tooltip?.callbacks?.labelPointStyle === 'function';
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var pointResult = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const pointStyle = chart.options.plugins.tooltip.callbacks.labelPointStyle({ datasetIndex: 1 });

                return `${pointStyle.pointStyle}|${pointStyle.rotation}`;
            }",
            canvasId);

        Assert.That(pointResult, Is.EqualTo("rectRounded|45"));

        var customDataButton = Page.GetByText("Custom Data", new PageGetByTextOptions() { Exact = true });
        await Expect(customDataButton).ToHaveAttributeAsync("type", "button");
        await customDataButton.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const callbacks = chart?.options?.plugins?.tooltip?.callbacks;
                const dataset = chart?.data?.datasets?.[0];

                return chart?.options?.plugins?.tooltip?.usePointStyle !== true
                    && typeof callbacks?.title === 'function'
                    && typeof callbacks?.label === 'function'
                    && callbacks.title([{ label: 'Apr', dataset }]) === 'Synergy Revenue Apr';
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var customDataResult = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const callbacks = chart.options.plugins.tooltip.callbacks;
                const dataset = chart.data.datasets[0];

                return [
                    callbacks.title([{ label: 'Apr', dataset }]),
                    callbacks.label({ dataset, dataIndex: 3, formattedValue: '32', raw: 32 })
                ].join('|');
            }",
            canvasId);

        Assert.That(customDataResult, Is.EqualTo("Synergy Revenue Apr|Raynor + Zagara: AvgGain 3.2, Winrate 56.8%, Games 61, Normalized 0.64"));

        var externalButton = Page.GetByText("External", new PageGetByTextOptions() { Exact = true });
        await Expect(externalButton).ToHaveAttributeAsync("type", "button");
        await externalButton.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const tooltip = chart?.options?.plugins?.tooltip;
                return tooltip?.enabled === false
                    && typeof tooltip?.external === 'function'
                    && typeof tooltip?.callbacks?.label === 'function';
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var externalResult = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const tooltip = chart.options.plugins.tooltip;

                tooltip.external({
                    chart,
                    tooltip: {
                        opacity: 1,
                        title: ['External Apr'],
                        body: [{ lines: ['Revenue: 32 units'] }],
                        footer: ['Total: 56 units'],
                        caretX: 120,
                        caretY: 80
                    }
                });

                const tooltipElement = chart.canvas.parentNode.querySelector('.chartjs-external-tooltip');
                const visible = tooltipElement?.style.opacity;
                const text = tooltipElement?.textContent;

                tooltip.external({ chart, tooltip: { opacity: 0 } });

                return `${visible}|${text}|${tooltipElement?.style.opacity}`;
            }",
            canvasId);

        Assert.That(externalResult, Is.EqualTo("1|External Apr - Revenue: 32 units - Total: 56 units|0"));
    }

    private async Task<string> OpenTooltipCallbackChart()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/tooltipcallbackchart");
        await Expect(Page).ToHaveTitleAsync(new Regex("Tooltip callback Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        return canvasId!;
    }

    private async Task WaitForContentCallbacks(string canvasId)
    {
        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const callbacks = chart?.options?.plugins?.tooltip?.callbacks;
                return typeof callbacks?.title === 'function'
                    && typeof callbacks?.label === 'function'
                    && typeof callbacks?.footer === 'function';
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }

    private async Task WaitForTooltipCallback(string canvasId, string callbackName)
    {
        await Page.WaitForFunctionAsync(
            @"([chartId, callbackName]) => {
                const chart = Chart.getChart(chartId);
                return typeof chart?.options?.plugins?.tooltip?.callbacks?.[callbackName] === 'function';
            }",
            new[] { canvasId, callbackName },
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });
    }
}
