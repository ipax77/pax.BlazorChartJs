using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HtmlLegendTests : PageTest
{
    [Test]
    public async Task HiddenDatasetCanBeUpdatedFromHtmlLegendTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/htmllegendchart");

        await Expect(Page).ToHaveTitleAsync(new Regex("Html Legend Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        var firstLegendItem = Page.GetByText("Team 1", new PageGetByTextOptions() { Exact = true });
        await Expect(firstLegendItem).ToBeVisibleAsync();

        await Task.Delay(Startup.ChartJsLoadDelay);

        Assert.That(await IsDatasetVisible(canvasId), Is.True);
        Assert.That(await GetDatasetBorderWidth(canvasId), Is.EqualTo(5));

        await firstLegendItem.ClickAsync();
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await IsDatasetVisible(canvasId), Is.False);

        await Page.Mouse.MoveAsync(0, 0);
        await Task.Delay(Startup.ChartJsComputeDelay);

        Assert.That(await GetDatasetBorderWidth(canvasId), Is.EqualTo(6));
    }

    private async Task<double?> GetDatasetBorderWidth(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<double?>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                const dataset = chart.data.datasets[" + dataset + @"];
                if (dataset.borderWidth === undefined) {
                    return null;
                } else {
                    return dataset.borderWidth;
                }
            }");
    }

    private async Task<bool> IsDatasetVisible(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<bool>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.isDatasetVisible(" + dataset + @");
            }");
    }
}
