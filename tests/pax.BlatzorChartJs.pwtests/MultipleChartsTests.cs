using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MultipleChartsTests : PageTest
{
    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/multiplecharts");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("MultipleCharts"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;
        
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions() 
            {
                Position = new Microsoft.Playwright.Position() { X = 10, Y = 10 } 
            }
        );

        var clickResult = Page.Locator("p");
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"chart1$"));

    }
}