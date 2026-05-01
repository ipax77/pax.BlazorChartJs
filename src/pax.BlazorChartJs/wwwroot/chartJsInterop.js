export const chartJsInteropVersion = "0.8.7";
class LoadInfo {
    constructor() {
        this.chartJsLoaded = false;
        this.chartJsDatalabels = false;
        this.chartJsLabels = false;
    }
}
class ChartJsInterop {
    constructor() {
        this.dotnetRefs = new Map();
        this.loadInfo = new LoadInfo();
    }
    async initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
        this.dotnetRefs.set(chartId, dotnetRef);
        const config = {
            'type': dotnetConfig['type'],
            data: dotnetConfig['data'],
            options: dotnetConfig['options'] ?? {},
            plugins: []
        };
        return config;
    }
    async loadPlugins(setupOptions, dotnetConfig) {
        const plugins = [];
        if (dotnetConfig['options'] != undefined
            && dotnetConfig['options'].plugins != undefined) {
            if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
                const arbitraryLines = this.arbitaryLinesPlugin();
                plugins.push(arbitraryLines);
            }
            if (dotnetConfig['options'].plugins.datalabels != undefined) {
                if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                    const ChartDataLabelsModule = await import(setupOptions['chartJsPluginDatalabelsLocation']);
                    plugins.push(ChartDataLabelsModule);
                }
            }
        }
        return plugins;
    }
    async triggerEvent(chartId, event, source, data) {
        if (this.dotnetRefs.has(chartId)) {
            await this.dotnetRefs.get(chartId).invokeMethodAsync("EventTriggered", event, source, data);
        }
    }
    addData(chart, label, pos, datas) {
        if (chart == undefined) {
            return;
        }
        this.addLabel(chart, label, pos);
        chart.data.datasets.forEach((dataset) => {
            if (datas[dataset['id']] != undefined) {
                const addData = datas[dataset['id']];
                this.addDatasetData(dataset, addData['data'], addData['atPosition']);
                if (addData['backgroundColor'] != undefined) {
                    this.addBackgroundColor(dataset, addData['backgroundColor'], addData['atPosition']);
                }
                if (addData['borderColor'] != undefined) {
                    this.addBorderColor(dataset, addData['borderColor'], addData['atPosition']);
                }
            }
        });
        chart.update();
    }
    addLabel(chart, label, pos) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            }
            else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }
    addDatasetData(dataset, data, pos) {
        if (pos == undefined) {
            dataset.data.push(data);
        }
        else {
            dataset.data.splice(pos, 0, data);
        }
    }
    addBackgroundColor(dataset, backgroundColor, pos) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            }
            else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }
    addBorderColor(dataset, borderColor, pos) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            }
            else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }
    *reverseKeys(arr) {
        let key = arr.length - 1;
        while (key >= 0) {
            yield key;
            key -= 1;
        }
    }
    removeData(chart) {
        if (!(chart.data.labels.length == 0)) {
            chart.data.labels.pop();
        }
        chart.data.datasets.forEach((dataset) => {
            if (!(dataset.data.length == 0)) {
                dataset.data.pop();
            }
            if (Array.isArray(dataset.backgroundColor)
                && !(dataset.backgroundColor.length == 0)) {
                dataset.backgroundColor.pop();
            }
            if (Array.isArray(dataset.borderColor)
                && !(dataset.borderColor.length == 0)) {
                dataset.borderColor.pop();
            }
        });
        chart.update();
    }
    setData(chart, labels, datas) {
        if (labels != undefined) {
            chart.data.labels = labels;
        }
        chart.data.datasets.forEach((dataset) => {
            if (datas[dataset['id']] != undefined) {
                const addData = datas[dataset['id']];
                dataset.data = addData['data'];
                if (addData['backgroundColor'] != undefined) {
                    dataset.backgroundColor = addData['backgroundColor'];
                }
                if (addData['borderColor'] != undefined) {
                    dataset.borderColor = addData['borderColor'];
                }
            }
        });
        chart.update();
    }
    addDatasets(chart, datasets) {
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }
    removeDatasets(chart, datasetIds) {
        for (const index of this.reverseKeys(chart.data.datasets)) {
            const dataset = chart.data.datasets[index];
            if (datasetIds.includes(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }
    updateDatasetsSmooth(chart, datasets) {
        datasets.forEach((newDataset) => {
            const datasetIndex = chart.data.datasets.findIndex((dataset) => dataset['id'] === newDataset['id']);
            if (datasetIndex >= 0) {
                const existingDataset = chart.data.datasets[datasetIndex];
                Object.assign(existingDataset, newDataset);
                for (const prop in existingDataset) {
                    if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
                        delete existingDataset[prop];
                    }
                }
            }
        });
        chart.update();
    }
    updateDatasets(chart, datasets) {
        datasets.forEach((dataset) => {
            const datasetIndex = chart.data.datasets.findIndex((existingDataset) => existingDataset['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }
    setDatasets(chart, datasets) {
        chart.data.datasets = datasets;
        chart.update();
    }
    disposeChart(chartId) {
        this.dotnetRefs.delete(chartId);
    }
    arbitaryLinesPlugin() {
        return {
            id: 'arbitraryLines',
            afterDraw(chart, args, options) {
                const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;
                ctx.save();
                for (let i = 0; i < options.length; i++) {
                    const option = options[i];
                    ctx.fillStyle = option.arbitraryLineColor;
                    const xWidth = option.xWidth;
                    const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                    const y0 = top;
                    const x1 = xWidth;
                    const y1 = height;
                    ctx.fillRect(x0, y0, x1, y1);
                }
                for (let i = 0; i < options.length; i++) {
                    const option = options[i];
                    ctx.fillStyle = option.arbitraryLineColor;
                    const xWidth = option.xWidth;
                    const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                    const y0 = top;
                    const x1 = xWidth;
                    const y1 = height;
                    ctx.fillStyle = 'white';
                    ctx.font = '14px arial';
                    ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
                }
                ctx.restore();
            }
        };
    }
}
const ChartJsInteropModule = new ChartJsInterop();
window[ChartJsInterop.name] = ChartJsInteropModule;
window.chartJsInteropVersion = chartJsInteropVersion;
let chartJsLoadPromise = null;
async function ensureChartJsLoaded(setupOptions) {
    if (!setupOptions?.chartJsLocation) {
        return;
    }
    if (!chartJsLoadPromise) {
        chartJsLoadPromise = import(setupOptions.chartJsLocation).then(() => { });
    }
    await chartJsLoadPromise;
}
export async function initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
    try {
        await ensureChartJsLoaded(setupOptions);
        const oldChart = Chart.getChart(chartId);
        if (oldChart != undefined) {
            oldChart.destroy();
        }
        const config = await ChartJsInteropModule.initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
        config.plugins = await loadPlugins(setupOptions, dotnetConfig);
        const element = document.getElementById(chartId);
        if (!element) {
            return false;
        }
        const ctx = element.getContext('2d');
        const chart = new Chart(ctx, config);
        if (dotnetConfig['options'] != undefined) {
            registerEvents(dotnetConfig.options, chartId, chart);
            chart.options.onResize?.(chart, { height: chart.height, width: chart.width });
        }
    }
    finally {
    }
    return true;
}
async function loadPlugins(setupOptions, dotnetConfig) {
    const plugins = [];
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
function registerChartPointEvent(chart, chartId, eventName, optionName) {
    chart.options[optionName] = (e) => {
        triggerEvent(chartId, eventName, "label", getChartPointEventArgs(e, chart));
    };
}
function registerEvents(dotnetConfigOptions, chartId, chart) {
    if (dotnetConfigOptions.onClickEvent == true) {
        registerChartPointEvent(chart, chartId, "click", "onClick");
    }
    if (dotnetConfigOptions.onHoverEvent == true) {
        registerChartPointEvent(chart, chartId, "hover", "onHover");
    }
    if (dotnetConfigOptions.onResizeEvent == true) {
        chart.options.onResize = (_chart, size) => {
            triggerEvent(chartId, "resize", "chart", {
                Height: size.height,
                Width: size.width,
                WindowHeight: window.innerHeight,
                WindowWidth: window.innerWidth
            });
        };
    }
    if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {
        chart.options.plugins.legend.onClick = (_event, legendItem, _legend) => {
            triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
        };
    }
    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {
        chart.options.plugins.legend.onHover = (_event, legendItem, _legend) => {
            triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
        };
    }
    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {
        chart.options.plugins.legend.onLeave = (_event, legendItem, _legend) => {
            triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
        };
    }
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {
        chart.options.animation.onProgress = (context) => {
            triggerEvent(chartId, "progress", "animation", {
                CurrentStep: context.currentStep,
                NumSteps: context.numSteps
            });
        };
    }
    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {
        chart.options.animation.onComplete = (context) => {
            triggerEvent(chartId, "complete", "animation", {
                Initial: context.initial
            });
        };
    }
}
function getChartPointEventArgs(e, chart) {
    const points = chart.getElementsAtEventForMode(e, "nearest", { intersect: true }, true);
    let label = "";
    let value = 0;
    let dataX = 0;
    let dataY = 0;
    let datasetLabel = null;
    let datasetIndex = null;
    const canvasPosition = Chart.helpers.getRelativePosition(e, chart);
    try {
        dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
    }
    catch { }
    try {
        dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
    }
    catch { }
    if (points.length) {
        const firstPoint = points[0];
        const currentDatasetIndex = firstPoint.datasetIndex;
        const currentDataset = chart.data.datasets[currentDatasetIndex];
        label = chart.data.labels?.[firstPoint.index] ?? "";
        datasetIndex = currentDatasetIndex;
        value = currentDataset.data[firstPoint.index];
        datasetLabel = currentDataset.label ?? null;
    }
    return {
        Label: label,
        Value: value,
        DataX: dataX,
        DataY: dataY,
        DatasetLabel: datasetLabel,
        DatasetIndex: datasetIndex
    };
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
export function setLabels(chartId, labels) {
    const chart = Chart.getChart(chartId);
    if (!chart || !chart.data) {
        return;
    }
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
    }
    else {
        chart.resize(width, height);
    }
    chart.options.onResize?.(chart, { height: chart.height, width: chart.width });
}
export function getChartImage(chartId, type, quality, width, height) {
    const chart = Chart.getChart(chartId);
    let currentWidth = 0;
    let currentHeight = 0;
    if (!(width == undefined || height == undefined)) {
        const ctx = document.getElementById(chartId);
        if (ctx.parentNode) {
            currentHeight = ctx.width;
            currentHeight = ctx.height;
            ctx.width = width;
            ctx.height = height;
            chart.options.animation = false;
            chart.resize(width, height);
        }
    }
    let chartImg;
    if (!(type == undefined || quality == undefined)) {
        chartImg = chart.toBase64Image(type, quality);
    }
    else {
        chartImg = chart.toBase64Image();
    }
    if (currentWidth > 0 && currentHeight > 0) {
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
    const datasetIndex = datasetMetas.findIndex((obj) => obj._dataset.id === datasetId);
    if (dataIndex == undefined) {
        chart.hide(datasetIndex);
    }
    else {
        chart.hide(datasetIndex, dataIndex);
    }
}
export function showDataset(chartId, datasetIndex, dataIndex) {
    const chart = Chart.getChart(chartId);
    if (dataIndex == undefined) {
        chart.show(datasetIndex);
    }
    else {
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
    const dataset = chart.data.datasets[datasetIndex];
    const elements = [];
    for (let i = 0; i < dataset.data.length; i++) {
        elements.push({ datasetIndex: datasetIndex, index: i });
    }
    chart.setActiveElements(elements);
    chart.update();
}
export function disposeChart(chartId) {
    ChartJsInteropModule.disposeChart(chartId);
}
