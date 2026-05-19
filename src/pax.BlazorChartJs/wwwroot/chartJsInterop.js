export const chartJsInteropVersion = "0.8.8";
const chartJsFunctionMarker = "__chartJsFunction";
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
        this.charts = new Map();
        this.loadInfo = new LoadInfo();
        this.chartInitPromises = new Map();
        this.chartJsCallbackRegistryPromises = new Map();
    }
    buildChartConfig(dotnetConfig) {
        return {
            type: dotnetConfig.type,
            data: dotnetConfig.data,
            options: dotnetConfig.options ?? {},
            plugins: []
        };
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
        if (!chart || !chart.data) {
            return;
        }
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
        if (!chart || !chart.data) {
            return;
        }
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
        if (!chart || !chart.data) {
            return;
        }
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }
    addDataset(chart, dataset, afterDatasetId) {
        if (!chart || !chart.data) {
            return;
        }
        if (afterDatasetId == undefined) {
            chart.data.datasets.push(dataset);
        }
        else {
            const datasetIndex = chart.data.datasets.findIndex((existingDataset) => existingDataset['id'] === afterDatasetId);
            if (datasetIndex >= 0) {
                chart.data.datasets.splice(datasetIndex + 1, 0, dataset);
            }
            else {
                chart.data.datasets.push(dataset);
            }
        }
        chart.update();
    }
    removeDatasets(chart, datasetIds) {
        if (!chart || !chart.data) {
            return;
        }
        for (const index of this.reverseKeys(chart.data.datasets)) {
            const dataset = chart.data.datasets[index];
            if (datasetIds.includes(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }
    updateDatasetsSmooth(chart, datasets) {
        if (!chart || !chart.data) {
            return;
        }
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
        if (!chart || !chart.data) {
            return;
        }
        datasets.forEach((dataset) => {
            const datasetIndex = chart.data.datasets.findIndex((existingDataset) => existingDataset['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }
    setDatasets(chart, datasets) {
        if (!chart || !chart.data) {
            return;
        }
        chart.data.datasets = datasets;
        chart.update();
    }
    async resolveChartJsFunctions(setupOptions, config) {
        if (!this.hasChartJsFunctionMarkers(config)) {
            return;
        }
        const callbacks = await this.getChartJsCallbackRegistry(setupOptions);
        this.resolveDataLabelsFormatter(config?.options?.plugins?.datalabels, callbacks);
        const datasets = config?.data?.datasets;
        if (Array.isArray(datasets)) {
            for (let i = 0; i < datasets.length; i++) {
                this.resolveDataLabelsFormatter(datasets[i]?.datalabels, callbacks);
            }
        }
    }
    hasChartJsFunctionMarkers(config) {
        if (this.isChartJsFunctionReference(config?.options?.plugins?.datalabels?.formatter)) {
            return true;
        }
        const datasets = config?.data?.datasets;
        if (!Array.isArray(datasets)) {
            return false;
        }
        for (let i = 0; i < datasets.length; i++) {
            if (this.isChartJsFunctionReference(datasets[i]?.datalabels?.formatter)) {
                return true;
            }
        }
        return false;
    }
    resolveDataLabelsFormatter(dataLabels, callbacks) {
        if (dataLabels == undefined || !this.isChartJsFunctionReference(dataLabels.formatter)) {
            return;
        }
        dataLabels.formatter = this.resolveChartJsFunction(dataLabels.formatter, callbacks);
    }
    isChartJsFunctionReference(value) {
        return value != undefined
            && typeof value === "object"
            && typeof value[chartJsFunctionMarker] === "string";
    }
    async getChartJsCallbackRegistry(setupOptions) {
        const moduleLocation = setupOptions?.chartJsCallbacksModuleLocation;
        if (typeof moduleLocation !== "string" || moduleLocation.length === 0) {
            throw new Error("ChartJsFunction callbacks require ChartJsSetupOptions.ChartJsCallbacksModuleLocation.");
        }
        let registryPromise = this.chartJsCallbackRegistryPromises.get(moduleLocation);
        if (!registryPromise) {
            registryPromise = import(moduleLocation)
                .then((module) => {
                const callbacks = module?.chartJsCallbacks;
                if (callbacks == undefined || typeof callbacks !== "object") {
                    throw new Error(`Chart.js callback module '${moduleLocation}' must export a chartJsCallbacks object.`);
                }
                return callbacks;
            })
                .catch((error) => {
                this.chartJsCallbackRegistryPromises.delete(moduleLocation);
                throw error;
            });
            this.chartJsCallbackRegistryPromises.set(moduleLocation, registryPromise);
        }
        return await registryPromise;
    }
    resolveChartJsFunction(reference, callbacks) {
        const callbackName = reference[chartJsFunctionMarker];
        if (!Object.prototype.hasOwnProperty.call(callbacks, callbackName)) {
            throw new Error(`Chart.js callback '${callbackName}' was not found in chartJsCallbacks.`);
        }
        const callback = callbacks[callbackName];
        if (typeof callback !== "function") {
            throw new Error(`Chart.js callback '${callbackName}' must be a function.`);
        }
        return callback;
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
    const runningInit = ChartJsInteropModule.chartInitPromises.get(chartId) ?? Promise.resolve({ success: true });
    const initPromise = runningInit
        .catch(() => ({ success: true }))
        .then(() => initChartCore(setupOptions, chartId, dotnetConfig, dotnetRef));
    ChartJsInteropModule.chartInitPromises.set(chartId, initPromise);
    try {
        return await initPromise;
    }
    finally {
        if (ChartJsInteropModule.chartInitPromises.get(chartId) === initPromise) {
            ChartJsInteropModule.chartInitPromises.delete(chartId);
        }
    }
}
async function initChartCore(setupOptions, chartId, dotnetConfig, dotnetRef) {
    try {
        await ensureChartJsLoaded(setupOptions);
        const element = document.getElementById(chartId);
        if (!element) {
            return { success: false };
        }
        destroyExistingChart(chartId, element);
        const config = ChartJsInteropModule.buildChartConfig(dotnetConfig);
        await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, config);
        config.plugins = await loadPlugins(setupOptions, dotnetConfig);
        const ctx = element.getContext('2d');
        if (!ctx) {
            return { success: false };
        }
        const chart = new Chart(ctx, config);
        ChartJsInteropModule.charts.set(chartId, chart);
        ChartJsInteropModule.dotnetRefs.set(chartId, dotnetRef);
        if (dotnetConfig['options'] != undefined) {
            registerEvents(dotnetConfig.options, chartId, chart);
        }
        return {
            success: true,
            height: chart.height,
            width: chart.width,
            windowHeight: window.innerHeight,
            windowWidth: window.innerWidth
        };
    }
    finally {
    }
}
function destroyExistingChart(chartId, element) {
    const mappedChart = ChartJsInteropModule.charts.get(chartId);
    let destroyedChart;
    let destroyedElementChart;
    if (mappedChart != undefined) {
        mappedChart.destroy();
        destroyedChart = mappedChart;
    }
    if (typeof Chart !== "undefined") {
        const elementChart = element ? Chart.getChart(element) : undefined;
        if (elementChart != undefined && elementChart !== destroyedChart) {
            elementChart.destroy();
            destroyedElementChart = elementChart;
        }
        const idChart = Chart.getChart(chartId);
        if (idChart != undefined
            && idChart !== destroyedChart
            && idChart !== destroyedElementChart) {
            idChart.destroy();
        }
    }
    ChartJsInteropModule.charts.delete(chartId);
    ChartJsInteropModule.dotnetRefs.delete(chartId);
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
function getLiveChart(chartId) {
    const mappedChart = ChartJsInteropModule.charts.get(chartId);
    if (mappedChart && mappedChart.data) {
        return mappedChart;
    }
    if (typeof Chart === "undefined") {
        return undefined;
    }
    const chart = Chart.getChart(chartId);
    return chart && chart.data ? chart : undefined;
}
export async function updateChartOptions(chartId, setupOptionsOrOptions, options) {
    const hasSetupOptions = arguments.length >= 3;
    const setupOptions = hasSetupOptions ? setupOptionsOrOptions : undefined;
    const resolvedOptions = hasSetupOptions ? options : setupOptionsOrOptions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { options: resolvedOptions });
    const chart = getLiveChart(chartId);
    if (chart != undefined) {
        chart.options = resolvedOptions;
        chart.update();
        registerEvents(resolvedOptions, chartId, chart);
    }
}
export function addData(chartId, label, pos, datas) {
    const chart = getLiveChart(chartId);
    ChartJsInteropModule.addData(chart, label, pos, datas);
}
export function removeData(chartId) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.removeData(chart);
}
export function setData(chartId, labels, datas) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.setData(chart, labels, datas);
}
export async function addDatasets(chartId, setupOptionsOrDatasets, datasets) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } });
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.addDatasets(chart, resolvedDatasets);
}
export async function addChartDataset(chartId, setupOptionsOrDataset, datasetOrAfterDatasetId, afterDatasetId) {
    const hasSetupOptions = datasetOrAfterDatasetId != undefined
        && typeof datasetOrAfterDatasetId === "object"
        && !Array.isArray(datasetOrAfterDatasetId);
    const setupOptions = hasSetupOptions ? setupOptionsOrDataset : undefined;
    const dataset = hasSetupOptions ? datasetOrAfterDatasetId : setupOptionsOrDataset;
    const resolvedAfterDatasetId = hasSetupOptions ? afterDatasetId : datasetOrAfterDatasetId;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: [dataset] } });
    const chart = getLiveChart(chartId);
    if (!chart || !dataset) {
        return;
    }
    ChartJsInteropModule.addDataset(chart, dataset, resolvedAfterDatasetId);
}
export function removeDatasets(chartId, datasets) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.removeDatasets(chart, datasets);
}
export async function updateDatasetsSmooth(chartId, setupOptionsOrDatasets, datasets) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } });
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasetsSmooth(chart, resolvedDatasets);
}
export async function updateDatasets(chartId, setupOptionsOrDatasets, datasets) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } });
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasets(chart, resolvedDatasets);
}
export async function setDatasets(chartId, setupOptionsOrDatasets, datasets) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } });
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.setDatasets(chart, resolvedDatasets);
}
export function setLabels(chartId, labels) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.data.labels = labels;
    chart.update();
}
export function resizeChart(chartId, width, height) {
    const chart = getLiveChart(chartId);
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
    const chart = getLiveChart(chartId);
    if (!chart) {
        return "";
    }
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
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.reset();
}
export function renderChart(chartId) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.render();
}
export function stopChart(chartId) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.stop();
}
export function setDatasetVisibility(chartId, datasetIndex, value) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.setDatasetVisibility(datasetIndex, value);
    chart.update();
}
export function toggleDataVisibility(chartId, index) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.toggleDataVisibility(index);
    chart.update();
}
export function getDataVisibility(chartId, index) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return false;
    }
    return chart.getDataVisibility(index);
}
export function hideDataset(chartId, datasetId, dataIndex) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
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
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    if (dataIndex == undefined) {
        chart.show(datasetIndex);
    }
    else {
        chart.show(datasetIndex, dataIndex);
    }
}
export function getLabels(chartId) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return [];
    }
    const items = chart.options.plugins.legend.labels.generateLabels(chart);
    return items;
}
export function isDatasetVisible(chartId, datasetIndex) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return false;
    }
    const isVisible = chart.isDatasetVisible(datasetIndex);
    return isVisible;
}
export function setDatasetPointsActive(chartId, datasetIndex) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
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
    destroyExistingChart(chartId);
    ChartJsInteropModule.disposeChart(chartId);
}
