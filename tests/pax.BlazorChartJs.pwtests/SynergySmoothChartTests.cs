using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SynergySmoothChartTests : PageTest
{
    [Test]
    public async Task SetDatasetsSmoothUpdatesRosterLabelsOptionsInOneUpdate()
    {
        var canvasId = await OpenSynergySmoothChartAndTrackUpdates("__setDatasetsSmoothUpdateCount");

        var setDatasetsSmooth = Page.GetByText("SetDatasetsSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(setDatasetsSmooth).ToHaveAttributeAsync("type", "button");
        await setDatasetsSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.length === 3
                    && chart.data.datasets[0].id === 'synergy-tychus'
                    && chart.data.datasets[1].id === 'synergy-nova'
                    && chart.data.datasets[2].id === 'synergy-raynor'
                    && chart.data.labels.join(',') === 'Artanis,Dehaka,Kerrigan,Stetmann,Vorazun'
                    && chart.options.plugins.title.text === 'Expanded synergy roster'
                    && chart.__setDatasetsSmoothUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.labels.join(','),
                    chart.data.datasets[0].data.join(','),
                    chart.options.plugins.title.text,
                    chart.__setDatasetsSmoothUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(
            snapshot,
            Is.EqualTo("synergy-tychus,synergy-nova,synergy-raynor|Artanis,Dehaka,Kerrigan,Stetmann,Vorazun|0.82,0.73,0.65,0.88,0.7|Expanded synergy roster|1"));
    }

    [Test]
    public async Task UpdateDatasetsDataSmoothUpdatesExistingDatasetsInOneUpdate()
    {
        var canvasId = await OpenSynergySmoothChartAndTrackUpdates("__updateDatasetsDataSmoothUpdateCount");

        var setDatasetsSmooth = Page.GetByText("SetDatasetsSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(setDatasetsSmooth).ToHaveAttributeAsync("type", "button");
        await setDatasetsSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.length === 3
                    && chart.data.datasets[0].id === 'synergy-tychus'
                    && chart.__updateDatasetsDataSmoothUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        await Page.EvaluateAsync(
            @"(chartId) => {
                Chart.getChart(chartId).__updateDatasetsDataSmoothUpdateCount = 0;
            }",
            canvasId);

        var updateDatasetsDataSmooth = Page.GetByText("UpdateDatasetsDataSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(updateDatasetsDataSmooth).ToHaveAttributeAsync("type", "button");
        await updateDatasetsDataSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.datasets?.length === 3
                    && chart.data.datasets[0].id === 'synergy-tychus'
                    && chart.data.datasets[1].id === 'synergy-nova'
                    && chart.data.datasets[2].id === 'synergy-raynor'
                    && chart.data.datasets[0].data.join(',') === '0.78,0.84,0.7,0.92,0.75'
                    && chart.data.datasets[1].data.join(',') === '0.88,0.81,0.77,0.86,0.94'
                    && chart.data.datasets[2].data.join(',') === '0.66,0.74,0.89,0.8,0.79'
                    && chart.__updateDatasetsDataSmoothUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.data.datasets.map(dataset => dataset.data.join(',')).join(';'),
                    chart.__updateDatasetsDataSmoothUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(
            snapshot,
            Is.EqualTo("synergy-tychus,synergy-nova,synergy-raynor|0.78,0.84,0.7,0.92,0.75;0.88,0.81,0.77,0.86,0.94;0.66,0.74,0.89,0.8,0.79|1"));
    }

    [Test]
    public async Task UpdateDatasetsDataSmoothBeforeRosterChangeKeepsDataAlignedWithInitialLabels()
    {
        var canvasId = await OpenSynergySmoothChartAndTrackUpdates("__initialDataSmoothUpdateCount");

        var updateDatasetsDataSmooth = Page.GetByText("UpdateDatasetsDataSmooth", new PageGetByTextOptions() { Exact = true });
        await Expect(updateDatasetsDataSmooth).ToHaveAttributeAsync("type", "button");
        await updateDatasetsDataSmooth.ClickAsync();

        await Page.WaitForFunctionAsync(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return chart?.data?.labels?.length === 4
                    && chart.data.datasets.length === 3
                    && chart.data.datasets.every(dataset => dataset.data.length === chart.data.labels.length)
                    && chart.data.datasets[0].id === 'synergy-nova'
                    && chart.data.datasets[1].id === 'synergy-raynor'
                    && chart.data.datasets[2].id === 'synergy-stukov'
                    && chart.data.datasets[0].data.join(',') === '0.91,0.67,0.79,0.82'
                    && chart.data.datasets[2].data.join(',') === '0.7,0.86,0.63,0.75'
                    && chart.__initialDataSmoothUpdateCount === 1;
            }",
            canvasId,
            new PageWaitForFunctionOptions { Timeout = (float)Startup.WasmLoadDelay.TotalMilliseconds });

        var snapshot = await Page.EvaluateAsync<string>(
            @"(chartId) => {
                const chart = Chart.getChart(chartId);
                return [
                    chart.data.labels.length,
                    chart.data.datasets.map(dataset => dataset.data.length).join(','),
                    chart.data.datasets.map(dataset => dataset.id).join(','),
                    chart.__initialDataSmoothUpdateCount
                ].join('|');
            }",
            canvasId);

        Assert.That(snapshot, Is.EqualTo("4|4,4,4|synergy-nova,synergy-raynor,synergy-stukov|1"));
    }

    private async Task<string> OpenSynergySmoothChartAndTrackUpdates(string updateCountPropertyName)
    {
        await Page.GotoAsync(Startup.GetSampleBaseUrl() + "/synergysmoothchart");

        await Expect(Page).ToHaveTitleAsync(new Regex("Synergy Smooth Chart"),
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
