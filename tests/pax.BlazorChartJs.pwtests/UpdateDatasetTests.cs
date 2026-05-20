using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UpdateDatasetTests : PageTest
{
    [Test]
    public async Task UpdateChartTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // create a locator
        var updateDataset = Page.GetByText("UpdateDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(updateDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await updateDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var borderWidthUpdate = await GetBorderWidth(canvasId);

        Assert.That(borderWidthUpdate, Is.EqualTo(3));

        // create a locator
        var resetDataset = Page.GetByText("ResetDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(resetDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await resetDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var borderWidthReset = await GetBorderWidth(canvasId);

        Assert.That(borderWidthReset, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateChartNullTest()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        // GetCanvasId
        var canvas = Page.Locator("canvas");

        var canvasId = await canvas.GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out Guid canvasGuid), Is.True);

        // wait for ChartJs to load
        await Task.Delay(Startup.ChartJsLoadDelay);

        // create a locator
        var updateDataset = Page.GetByText("UpdateDataset2", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(updateDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await updateDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var barThicknessUpdate = await GetBarThickness(canvasId);

        Assert.That(barThicknessUpdate, Is.EqualTo(22));

        // create a locator
        var resetDataset = Page.GetByText("ResetDataset", new PageGetByTextOptions() { Exact = true });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(resetDataset).ToHaveAttributeAsync("type", "button");

        // Click the button.
        await resetDataset.ClickAsync();

        // wait for Chartjs
        await Task.Delay(Startup.ChartJsComputeDelay);

        var barThicknessReset = await GetBorderWidth(canvasId);

        Assert.That(barThicknessReset, Is.EqualTo(1));
    }

    [Test]
    public async Task ApplyDatasetChangesSmoothBatchesDatasetsLabelsAndOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.EvaluateAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                chart.__applyChangesUpdateCount = 0;
                const originalUpdate = chart.update.bind(chart);
                chart.update = (...args) => {
                    chart.__applyChangesUpdateCount++;
                    return originalUpdate(...args);
                };
            }",
            canvasId);

        var applyDatasetChangesSmooth = Page.GetByText("ApplyDatasetChangesSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(applyDatasetChangesSmooth).ToHaveAttributeAsync("type", "button");
        await applyDatasetChangesSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.length === 2
                    && chart.data.datasets[0].id === 'upsert-added'
                    && chart.data.datasets[1].id === 'upsert-primary'
                    && chart.data.datasets[1].borderWidth === 5
                    && chart.options.responsive === false
                    && chart.__applyChangesUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.labels.join(','),
                    chart.data.datasets[1].label,
                    chart.data.datasets[1].borderWidth,
                    chart.data.datasets[1].barThickness,
                    chart.options.responsive,
                    chart.options.maintainAspectRatio,
                    chart.__applyChangesUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("upsert-added,upsert-primary|Apr,May,Jun,Jul|Dataset 1 Upserted|5|18|False|False|1").IgnoreCase);
    }

    [Test]
    public async Task SetDatasetsSmoothBatchesDerivedAddsUpdatesRemovesLabelsAndOptions()
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/udpatechart");

        await Expect(Page).ToHaveTitleAsync(new Regex("UpdateDatset Chart"),
            new PageAssertionsToHaveTitleOptions() { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var canvasId = await Page.Locator("canvas").GetAttributeAsync("id");
        Assert.That(Guid.TryParse(canvasId, out _), Is.True);

        await Page.WaitForFunctionAsync(
            @"(chartId) => typeof Chart !== 'undefined' && Chart.getChart(chartId) != undefined",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Task.Delay(Startup.ChartJsLoadDelay);

        await Page.EvaluateAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                chart.__setSmoothUpdateCount = 0;
                const originalUpdate = chart.update.bind(chart);
                chart.update = (...args) => {
                    chart.__setSmoothUpdateCount++;
                    return originalUpdate(...args);
                };
            }",
            canvasId);

        var setDatasetsSmooth = Page.GetByText("SetDatasetsSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(setDatasetsSmooth).ToHaveAttributeAsync("type", "button");
        await setDatasetsSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.length === 2
                    && chart.data.datasets[0].id === 'upsert-added'
                    && chart.data.datasets[1].id === 'upsert-primary'
                    && chart.data.datasets[1].borderWidth === 6
                    && chart.options.responsive === false
                    && chart.__setSmoothUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.labels.join(','),
                    chart.data.datasets[1].label,
                    chart.data.datasets[1].borderWidth,
                    chart.data.datasets[1].barThickness,
                    chart.options.responsive,
                    chart.options.maintainAspectRatio,
                    chart.__setSmoothUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("upsert-added,upsert-primary|Apr,May,Jun,Jul|Dataset 1 Set Smooth|6|19|False|False|1").IgnoreCase);
    }

    private async Task<double?> GetBorderWidth(string? canvasId, int dataset = 0)
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

    private async Task<object?> GetBarThickness(string? canvasId, int dataset = 0)
    {
        return await Page.EvaluateAsync<object?>(@"() => {
                const chart = Chart.getChart(""" + canvasId + @""");
                const dataset = chart.data.datasets[" + dataset + @"];
                if (dataset.barThickness === undefined) {
                    return null;
                } else {
                    return dataset.barThickness;
                }
            }");
    }
}
