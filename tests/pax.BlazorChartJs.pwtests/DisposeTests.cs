using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DisposeTests : PageTest
{
    [Test]
    public async Task DisposeTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/timeschart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("TimesChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // Provoke ChartComponent.Dispose while initializing
                // create a locator
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
    }
}