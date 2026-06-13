using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LineChartTests : ChartPageTest
{


    [Test]
    public async Task AddDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var addData = Page.GetByText("Add Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addData.ClickAsync();

        await WaitForDatasetDataCountAsync(canvasId, 0, countPrev + 1);
        int countAfter = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDataTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeData = Page.GetByText("Remove Data", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeData).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeData.ClickAsync();

        await WaitForDatasetDataCountAsync(canvasId, 0, countPrev - 1);
        int countAfter = await GetDatasetDataCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }

    [Test]
    public async Task AddDatasetTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var addDataset = Page.GetByText("Add Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(addDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await addDataset.ClickAsync();

        await WaitForDatasetCountAsync(canvasId, countPrev + 1);
        int countAfter = await GetDatasetCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev + 1));
    }

    [Test]
    public async Task RemoveDatasetTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"), new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        // Current data count
        int countPrev = await GetDatasetCountAsync(canvasId);

        Assert.That(countPrev, Is.Not.Zero);

        // create a locator
        var removeDataset = Page.GetByText("Remove Dataset", new Microsoft.Playwright.PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(removeDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await removeDataset.ClickAsync();

        await WaitForDatasetCountAsync(canvasId, countPrev - 1);
        int countAfter = await GetDatasetCountAsync(canvasId);

        Assert.That(countAfter, Is.EqualTo(countPrev - 1));
    }

    [Test]
    public async Task LegacyDatasetInteropExportsUpdateLiveChart()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/linechart");

        await Expect(Page).ToHaveTitleAsync(new Regex("LineChart"),
            new Microsoft.Playwright.PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await WaitForChartAsync(Page.Locator("canvas"));

        var snapshot = await Page.EvaluateAsync<string>(
            @"async (chartId) => {
                const chartInterop = await import('./_content/pax.BlazorChartJs/chartJsInterop.js?v=0.9.1');
                const chart = Chart.getChart(chartId);
                let updateCount = 0;
                const originalUpdate = chart.update.bind(chart);
                chart.update = (...args) => {
                    updateCount++;
                    return originalUpdate(...args);
                };

                const firstDataset = chart.data.datasets[0];
                chartInterop.setDatasetsData(chartId, [{ datasetId: firstDataset.id, data: [710, 711] }]);
                chartInterop.addChartDataToDatasets(chartId, 'Legacy Export', [712]);

                const datasetCountBeforeRemove = chart.data.datasets.length;
                chart.data.datasets.push({ id: 'legacy-remove', label: 'Legacy Remove', data: [1, 2, 3] });
                chartInterop.removeDataset(chartId, 'legacy-remove');

                return [
                    typeof chartInterop.addChartDataToDatasets,
                    typeof chartInterop.setDatasetsData,
                    typeof chartInterop.removeDataset,
                    chart.data.datasets[0].data.join(','),
                    chart.data.labels[chart.data.labels.length - 1],
                    chart.data.datasets.length === datasetCountBeforeRemove,
                    updateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("function|function|function|710,711,712|Legacy Export|true|3"));
    }
}
