using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MultipleChartsTests : ChartPageTest
{
    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/multiplecharts");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("MultipleCharts"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        await WaitForChartAsync(canvas);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 10, Y = 10 }
        }
        );

        var clickResult = Page.Locator("p");
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"chart1$"));

    }
}
