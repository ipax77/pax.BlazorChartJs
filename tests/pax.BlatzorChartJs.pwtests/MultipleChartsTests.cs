using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MultipleChartsTests : PageStartupTest
{
    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync("https://localhost:7193/multiplecharts");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("MultipleCharts"));

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;
        
        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(1000);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions() 
            {
                Position = new Microsoft.Playwright.Position() { X = 10, Y = 10 } 
            }
        );

        var clickResult = Page.Locator("p");
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"chart1$"));

    }
}