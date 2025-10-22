// v0.8.6-rc2
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

export async function initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
    await lock.promise
    lock.enable();

    try {
        if (!isLoaded) {
            if (setupOptions?.chartJsLocation) {
                await import(setupOptions.chartJsLocation);
            }
            isLoaded = true;
        }

        var oldChart = Chart.getChart(chartId);
        if (oldChart != undefined) {
            oldChart.destroy();
        }

        const config = await ChartJsInteropModule.initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
        config.plugins = await loadPlugins(setupOptions, dotnetConfig);
        const ctx = document.getElementById(chartId).getContext('2d');
        const chart = new Chart(ctx, config);

        if (dotnetConfig['options'] != undefined) {
            registerEvents(dotnetConfig.options, chartId, chart);
        }
    } finally {
        lock.disable();
    }
    return true;
}

async function loadPlugins(setupOptions, dotnetConfig) {
    let plugins = []

    if (dotnetConfig['options'] != undefined
        && dotnetConfig['options'].plugins != undefined) {

        if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
            const arbitraryLines = ChartJsInteropModule.arbitaryLinesPlugin();
            plugins.push(arbitraryLines);
        }

        if (dotnetConfig['options'].plugins.labels != undefined) {
            if (setupOptions?.['chartJsPluginLabelsLocation']) {
                await import(setupOptions['chartJsPluginLabelsLocation']);
            }
        }

        if (dotnetConfig['options'].plugins.datalabels != undefined) {
            if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                await import(setupOptions['chartJsPluginDatalabelsLocation']);
            }
            plugins.push(ChartDataLabels);
        }
    }
    return plugins;
}

function registerEvents(dotnetConfigOptions, chartId, chart) {
    // chart events
    if (dotnetConfigOptions.onClickEvent == true) {
        chart.options.onClick = (e) => {
            const points = chart.getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;
            let datasetLabel = null;
            let datasetIndex = null;

            const canvasPosition = Chart.helpers.getRelativePosition(e, chart);

            // Substitute the appropriate scale IDs
            // todo: not working on pie.. charts
            try {
                dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
            } catch { }
            try {
                dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
            } catch { }

            if (points.length) {
                const firstPoint = points[0];
                label = chart.data.labels[firstPoint.index];
                datasetIndex = firstPoint.datasetIndex;
                value = chart.data.datasets[datasetIndex].data[firstPoint.index];
                datasetLabel = chart.data.datasets[datasetIndex].label;
            }
            triggerEvent(chartId, "click", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY, DatasetLabel: datasetLabel, DatasetIndex: datasetIndex });
        }
    }

    if (dotnetConfigOptions.onHoverEvent == true) {
        chart.options.onHover = (e) => {
            const points = chart.getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;
            let datasetLabel = null;
            let datasetIndex = null;

            const canvasPosition = Chart.helpers.getRelativePosition(e, chart);

            // Substitute the appropriate scale IDs
            // todo: not working on pie.. charts
            try {
                dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
            } catch { }
            try {
                dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
            } catch { }

            if (points.length) {
                const firstPoint = points[0];
                label = chart.data.labels[firstPoint.index];
                datasetIndex = firstPoint.datasetIndex;
                value = chart.data.datasets[datasetIndex].data[firstPoint.index];
                datasetLabel = chart.data.datasets[datasetIndex].label;
            }
            triggerEvent(chartId, "hover", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY, DatasetLabel: datasetLabel, DatasetIndex: datasetIndex });
        }
    }

    if (dotnetConfigOptions.onResizeEvent == true) {
        chart.options.onResize = (chart, size) => {
            triggerEvent(chartId, "resize", "chart", { Height: size.height, Width: size.width });
        };
    }

    // legend events
    if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {

        chart.options.plugins.legend.onClick = (event, legendItem, legend) => {
            triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {

        chart.options.plugins.legend.onHover = (event, legendItem, legend) => {
            triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {

        chart.options.plugins.legend.onLeave = (event, legendItem, legend) => {
            triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
        };
    }

    // animation events
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {

        chart.options.animation.onProgress = (context) => {
            triggerEvent(chartId, "progress", "animation", { CurrentStep: context.currentStep, NumSteps: context.numSteps });
        };
    }

    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {

        chart.options.animation.onComplete = (context) => {
            triggerEvent(chartId, "complete", "animation", { Initial: context.initial });
        };
    }
}

async function triggerEvent(chartId, event, source, data) {
    await ChartJsInteropModule.triggerEvent(chartId, event, source, data);
}

export function updateChartOptions(chartId, options) {
    const chart = Chart.getChart(chartId);
    if (chart != undefined) {
        chart.options = options;
        chart.update();
        registerEvents(options, chartId, chart);
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

export function updateDatasetsSmooth(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.updateDatasetsSmooth(chart, datasets);
}

export function updateDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.updateDatasets(chart, datasets);
}

export function setDatasets(chartId, datasets) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.setDatasets(chart, datasets);
}

// - ts
export function setLabels(chartId, labels) {
    const chart = Chart.getChart(chartId);
    chart.data.labels = labels;
    chart.update();
}

export function resizeChart(chartId, width, height) {
    const chart = Chart.getChart(chartId);
    if (chart == undefined) {
        return;
    }
    if (width == undefined || height == undefined) {
        chart.resize();
    } else {
        chart.resize(width, height);
    }
}

export function getChartImage(chartId, type, quality, width, height) {

    const chart = Chart.getChart(chartId);
    let currentWidth = 0;
    let currentHeight = 0;
    if (!(width == undefined || height == undefined)) {
        var ctx = document.getElementById(chartId);
        // var ctx = document.getElementById(chartId).getContext('2d');
        if (ctx.parentNode) {
            currentHeight = ctx.width;
            currentHeight = ctx.height;

            //ctx.parentNode.style.resize = 'both';
            //ctx.parentNode.style.width = width + 'px !important';
            //ctx.parentNode.style.height = height + 'px !important';
            //chart.resize();

            ctx.width = width;
            ctx.height = height;
            chart.options.animation = false;
            chart.resize(width, height);
        }
    }

    let chartImg;
    if (!(type == undefined || quality == undefined)) {
        chartImg = chart.toBase64Image(type, quality);
    } else {
        chartImg = chart.toBase64Image();
    }

    if (currentWidth > 0 && currentHeight > 0) {
        //ctx.parentNode.style.width = currentWidth;
        //ctx.parentNode.style.height = currentHeight;
        chart.resize();
    }
    chart.options.animation = true;
    return chartImg;
}

export function resetChart(chartId) {
    const chart = Chart.getChart(chartId);
    chart.reset();
}

export function renderChart(chartId) {
    const chart = Chart.getChart(chartId);
    chart.render();
}

export function stopChart(chartId) {
    const chart = Chart.getChart(chartId);
    chart.stop();
}

export function setDatasetVisibility(chartId, datasetIndex, value) {
    const chart = Chart.getChart(chartId);
    chart.setDatasetVisibility(datasetIndex, value);
    chart.update();
}

export function toggleDataVisibility(chartId, index) {
    const chart = Chart.getChart(chartId);
    chart.toggleDataVisibility(index);
    chart.update();
}

export function getDataVisibility(chartId, index) {
    const chart = Chart.getChart(chartId);
    return chart.getDataVisibility(index);
}

export function hideDataset(chartId, datasetId, dataIndex) {
    const chart = Chart.getChart(chartId);
    const datasetMetas = chart.getSortedVisibleDatasetMetas();
    var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === datasetId);
    if (dataIndex == undefined) {
        chart.hide(datasetIndex);
    } else {
        chart.hide(datasetIndex, dataIndex);
    }
}

export function showDataset(chartId, datasetIndex, dataIndex) {
    const chart = Chart.getChart(chartId);
    if (dataIndex == undefined) {
        chart.show(datasetIndex);
    } else {
        chart.show(datasetIndex, dataIndex);
    }
}

export function getLabels(chartId) {
    const chart = Chart.getChart(chartId);
    const items = chart.options.plugins.legend.labels.generateLabels(chart);
    return items;
}

export function isDatasetVisible(chartId, datasetIndex) {
    const chart = Chart.getChart(chartId);
    const isVisible = chart.isDatasetVisible(datasetIndex);
    return isVisible;
}

export function setDatasetPointsActive(chartId, datasetIndex) {
    const chart = Chart.getChart(chartId);

    if (chart.getActiveElements().length > 0) {
        chart.setActiveElements([]);
        chart.update();
    }

    if (datasetIndex == -1 || chart.data.datasets.length <= datasetIndex) {
        return;
    }

    var dataset = chart.data.datasets[datasetIndex];

    var elements = [];
    for (var i = 0; i < dataset.data.length; i++) {
        elements.push({ datasetIndex: datasetIndex, index: i });
    }

    chart.setActiveElements(elements);
    chart.update();
}

export function disposeChart(chartId) {
    ChartJsInteropModule.disposeChart(chartId);
}