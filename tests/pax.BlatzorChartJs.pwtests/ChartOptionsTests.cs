using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

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

    private async Task<int> GetChartStepSize(string? canvasId)
    {
        return await Page.EvaluateAsync<int>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                return chart.options.scales.x.ticks.stepSize;
            }");
    }
}