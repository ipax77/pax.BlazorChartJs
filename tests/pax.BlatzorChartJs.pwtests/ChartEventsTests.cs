using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChartEventsTests : PageTest
{
    [Test]
    public async Task ClickEventTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "chartevents");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("ChartEvents"));

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var clickResult = Page.Locator("p");
        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsLabelClickEvent"));
    }

    [Test]
    public async Task DisableEventsTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "chartevents");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("ChartEvents"));

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load and finish animation
        await Task.Delay(Startup.ChartJsLoadDelay);

        var clickResult = Page.Locator("p");
        var textPrev = await clickResult.AllInnerTextsAsync();

        var removeEvents = Page.GetByText("RemoveEvents");
        await removeEvents.ClickAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
        {
            Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
        }
        );

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var textAfter = await clickResult.AllInnerTextsAsync();

        Assert.That(textAfter, Is.EqualTo(textPrev));
    }

    [Test]
    public async Task ToggleEventsTest()
    {
        await Page.GotoAsync(Startup.SampleBaseUrl + "chartevents");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("ChartEvents"));

        // GetCanvasId
        var canvas = Page.Locator("canvas").First;

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load and finish animation
        await Task.Delay(Startup.ChartJsLoadDelay);

        var clickResult = Page.Locator("p");
        var textPrev = await clickResult.AllInnerTextsAsync();

        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsAnimationCompleteEvent"));

        var removeEvents = Page.GetByText("RemoveEvents");
        await removeEvents.ClickAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
            {
                Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
            }
        );

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var textAfter = await clickResult.AllInnerTextsAsync();

        Assert.That(textAfter, Is.EqualTo(textPrev));

        var addEvents = Page.GetByText("AddEvents");
        await addEvents.ClickAsync();

        // wait for ChartJs
        await Task.Delay(Startup.ChartJsComputeDelay);

        await canvas.ClickAsync(new Microsoft.Playwright.LocatorClickOptions()
            {
                Position = new Microsoft.Playwright.Position() { X = 100, Y = 100 }
            }
        );

        await Expect(clickResult).ToHaveTextAsync(new Regex(@"ChartJsLabelClickEvent"));
    }
}