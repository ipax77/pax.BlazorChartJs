import ChartJsInteropModule from './ChartJsInteropModule.js';

class AsyncLock {
    constructor() {
        this.disable = () => { }
        this.promise = Promise.resolve()
    }

    enable() {
        this.promise = new Promise(resolve => this.disable = resolve)
    }
}

const lock = new AsyncLock()
let isLoaded = false;

export async function initChart(setupOptions, chartId, dotnetConfig, dotnetRef)
{
    await lock.promise
    lock.enable();

    try {
        if (!isLoaded) {
            if (setupOptions?.chartJsLocation) {
                await import(setupOptions.chartJsLocation);
            }
            else {
                await import('./chart.min.js');
            }
            isLoaded = true;
        }

        var oldChart = Chart.getChart(chartId);
        if (oldChart != undefined) {
            oldChart.destroy();
        }

        const config = await ChartJsInteropModule.initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
        const ctx = document.getElementById(chartId).getContext('2d');
        const chart = new Chart(ctx, config);
    } finally {
        lock.disable();
    }
}

export function addData(chartId, label, pos, datas) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.addData(chart, label, pos, datas);
}

export function removeData(chartId) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.removeData(chart);
}

export function setData(chartId, labels, datas) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.setData(chart, labels, datas);
}

export function addDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.addDatasets(chart, datasets);
}

export function removeDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.removeDatasets(chart, datasets);
}

export function updateDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.updateDatasets(chart, datasets);
}

export function setDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.setDatasets(chart, datasets);
}
