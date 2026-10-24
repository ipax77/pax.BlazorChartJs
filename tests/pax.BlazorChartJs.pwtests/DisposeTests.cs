using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DisposeTests : PageTest
{
    [Test]
    public async Task DisposeTest()
    {
        var rendererErrors = TrackRendererErrors();

        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/timeschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("TimesChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var showChart = Page.GetByText("Show Chart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });
        await Expect(showChart).ToHaveAttributeAsync("type", "button");
        await showChart.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"() => {
                const canvas = document.querySelector('canvas');
                if (!canvas || typeof Chart === 'undefined') {
                    return false;
                }

                const chart = Chart.getChart(canvas.id);
                return chart?.options?.scales?.x?.type === 'time';
            }",
            null,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // Provoke ChartComponent.Dispose after TimesChartComp initialized its JS module.
        var showAndDispose = Page.GetByText("ShowAndDispose Chart", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(showAndDispose).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await showAndDispose.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        // await Page.ScreenshotAsync(new()
        // {
        //     Path = "/data/screenshot.png",
        //     FullPage = true,
        // });

        var error = Page.GetByText("An unhandled error has occurred.", new Microsoft.Playwright.PageGetByTextOptions() { Exact = false });
        await Expect(error).ToHaveCSSAsync("display", "none");
        Assert.That(rendererErrors, Is.Empty, string.Join(Environment.NewLine, rendererErrors));
    }

    [Test]
    public async Task DatasetInteropAfterDisposeIsIgnored()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            "chartId => window.Chart !== undefined && Chart.getChart(chartId) !== undefined",
            canvasId,
            new Microsoft.Playwright.PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // Use the running app's interop version, including for external targets.
        // A version bump must not silently skip this regression.
        var chartInteropVersion = await Page.EvaluateAsync<string>("() => window.chartJsInteropVersion ?? ''");
        Assert.That(chartInteropVersion, Is.Not.Empty, "The initialized app must expose its interop version.");

        await Page.GotoAsync(Startup.GetSampleBaseUrl());

        await Page.EvaluateAsync(
            @"async ({ chartId, version }) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=' + encodeURIComponent(version));
                const dataset = { id: 'disposed-test', label: 'Disposed Test', data: [1, 2, 3] };
                await chartInterop.addDatasets(chartId, [dataset]);
                await chartInterop.updateDatasets(chartId, [dataset]);
                await chartInterop.removeDatasets(chartId, [dataset.id]);
                await chartInterop.setDatasets(chartId, [dataset]);
            }",
            new { chartId = canvasId, version = chartInteropVersion });

        var error = Page.GetByText("An unhandled error has occurred.", new Microsoft.Playwright.PageGetByTextOptions() { Exact = false });
        await Expect(error).ToHaveCSSAsync("display", "none");
    }

    private List<string> TrackRendererErrors()
    {
        List<string> rendererErrors = [];

        Page.Console += (_, message) =>
        {
            if (message.Type == "error" && IsRendererError(message.Text))
            {
                rendererErrors.Add(message.Text);
            }
        };

        Page.PageError += (_, error) =>
        {
            if (IsRendererError(error))
            {
                rendererErrors.Add(error);
            }
        };

        return rendererErrors;
    }

    private static bool IsRendererError(string message)
    {
        return message.Contains("Unhandled exception rendering component", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Cannot wait on monitors", StringComparison.OrdinalIgnoreCase)
            || message.Contains("PlatformNotSupportedException", StringComparison.OrdinalIgnoreCase);
    }
}
