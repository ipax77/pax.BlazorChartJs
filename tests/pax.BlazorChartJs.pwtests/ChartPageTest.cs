using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public abstract class ChartPageTest : PageTest
{
    protected async Task<string> WaitForChartAsync(ILocator canvas)
    {
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await WaitForChartAsync(canvasId);
        return canvasId!;
    }

    protected Task WaitForChartAsync(string? canvasId)
    {
        return Page.WaitForFunctionAsync(
            "canvasId => window.Chart !== undefined && Chart.getChart(canvasId) !== undefined",
            canvasId,
            WaitOptions);
    }

    protected async Task<int> GetDatasetCountAsync(string canvasId)
    {
        await WaitForChartAsync(canvasId);
        return await Page.EvaluateAsync<int>(
            "canvasId => Chart.getChart(canvasId).data.datasets.length",
            canvasId);
    }

    protected async Task<int> GetDatasetDataCountAsync(string canvasId, int dataset = 0)
    {
        await WaitForChartAsync(canvasId);
        return await Page.EvaluateAsync<int>(
            "args => Chart.getChart(args.canvasId).data.datasets[args.dataset].data.length",
            new { canvasId, dataset });
    }

    protected Task WaitForDatasetCountAsync(string canvasId, int expected)
    {
        return Page.WaitForFunctionAsync(
            "args => Chart.getChart(args.canvasId)?.data?.datasets?.length === args.expected",
            new { canvasId, expected },
            WaitOptions);
    }

    protected Task WaitForDatasetDataCountAsync(string canvasId, int dataset, int expected)
    {
        return Page.WaitForFunctionAsync(
            "args => Chart.getChart(args.canvasId)?.data?.datasets?.[args.dataset]?.data?.length === args.expected",
            new { canvasId, dataset, expected },
            WaitOptions);
    }

    private static PageWaitForFunctionOptions WaitOptions =>
        new() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds };
}
