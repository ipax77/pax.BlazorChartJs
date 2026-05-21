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

    [Test]
    public async Task UpdateDatasetDataSmoothUpdatesOneDatasetDataById()
    {
        var canvasId = await OpenUpdateChartAndTrackUpdates("__singleDataUpdateCount");

        var updateDatasetDataSmooth = Page.GetByText("UpdateDatasetDataSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(updateDatasetDataSmooth).ToHaveAttributeAsync("type", "button");
        await updateDatasetDataSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.[0]?.data?.join(',') === '10,11,12'
                    && chart.data.datasets[1].data.join(',') === '3,2,1'
                    && chart.__singleDataUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.datasets[0].data.join(','),
                    chart.data.datasets[1].data.join(','),
                    chart.__singleDataUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("upsert-primary,upsert-remove|10,11,12|3,2,1|1"));
    }

    [Test]
    public async Task UpdateDatasetsDataSmoothUpdatesMultipleDatasetDataById()
    {
        var canvasId = await OpenUpdateChartAndTrackUpdates("__multiDataUpdateCount");

        var updateDatasetsDataSmooth = Page.GetByText("UpdateDatasetsDataSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(updateDatasetsDataSmooth).ToHaveAttributeAsync("type", "button");
        await updateDatasetsDataSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.[0]?.data?.join(',') === '10,11,12'
                    && chart.data.datasets[1].data.join(',') === '13,14,15'
                    && chart.__multiDataUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.datasets[0].data.join(','),
                    chart.data.datasets[1].data.join(','),
                    chart.__multiDataUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("upsert-primary,upsert-remove|10,11,12|13,14,15|1"));
    }

    [Test]
    public async Task SetDatasetBinaryDataUpdatesFloat64XYByDatasetId()
    {
        var canvasId = await OpenUpdateChartAndTrackUpdates("__binaryXYUpdateCount");

        var setBinaryFloat64XY = Page.GetByText("SetBinaryFloat64XY", new PageGetByTextOptions() { Exact = true });
        await Expect(setBinaryFloat64XY).ToHaveAttributeAsync("type", "button");
        await setBinaryFloat64XY.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const data = chart?.data?.datasets?.[0]?.data;
                return data?.length === 3
                    && data[0].x === 1
                    && data[0].y === 20
                    && data[2].x === 3
                    && data[2].y === 22
                    && chart.__binaryXYUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                const data = chart.data.datasets[0].data;
                return [
                    data.map(point => `${point.x}:${point.y}`).join(','),
                    chart.data.datasets[1].data.join(','),
                    chart.__binaryXYUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("1:20,2:21,3:22|3,2,1|1"));
    }

    [Test]
    public async Task SetDatasetBinaryDataUpdatesFloat32YAsScalarData()
    {
        var canvasId = await OpenUpdateChartAndTrackUpdates("__binaryYUpdateCount");

        var setBinaryFloat32Y = Page.GetByText("SetBinaryFloat32Y", new PageGetByTextOptions() { Exact = true });
        await Expect(setBinaryFloat32Y).ToHaveAttributeAsync("type", "button");
        await setBinaryFloat32Y.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.[0]?.data?.join(',') === '30,31,32'
                    && chart.data.datasets[1].data.join(',') === '3,2,1'
                    && chart.__binaryYUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets[0].data.join(','),
                    chart.data.datasets[1].data.join(','),
                    chart.__binaryYUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("30,31,32|3,2,1|1"));
    }

    [Test]
    public async Task SetDatasetsBinaryDataBatchesMultipleDatasetsWithOneChartUpdate()
    {
        var canvasId = await OpenUpdateChartAndTrackUpdates("__binaryBatchUpdateCount");

        var setBinaryBatch = Page.GetByText("SetBinaryBatch", new PageGetByTextOptions() { Exact = true });
        await Expect(setBinaryBatch).ToHaveAttributeAsync("type", "button");
        await setBinaryBatch.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.[0]?.data?.join(',') === '40,41,42'
                    && chart.data.datasets[1].data.join(',') === '50,51,52'
                    && chart.__binaryBatchUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets[0].data.join(','),
                    chart.data.datasets[1].data.join(','),
                    chart.__binaryBatchUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("40,41,42|50,51,52|1"));
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

    private async Task<string> OpenUpdateChartAndTrackUpdates(string updateCountPropertyName)
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
            @"([chartId, updateCountPropertyName]) => {
                const chart = Chart.getChart(chartId);
                chart[updateCountPropertyName] = 0;
                const originalUpdate = chart.update.bind(chart);
                chart.update = (...args) => {
                    chart[updateCountPropertyName]++;
                    return originalUpdate(...args);
                };
            }",
            new[] { canvasId!, updateCountPropertyName });

        return canvasId!;
    }
}
