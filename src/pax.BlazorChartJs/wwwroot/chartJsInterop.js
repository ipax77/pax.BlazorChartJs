export const chartJsInteropVersion = "0.9.0-preview";
const chartJsFunctionMarker = "__chartJsFunction";
const chartJsFunctionNamePattern = /^[A-Za-z_$][A-Za-z0-9_$]*$/;
const reservedChartJsFunctionNames = new Set([
    "__proto__",
    "constructor",
    "prototype",
    "toString",
    "valueOf",
    "hasOwnProperty"
]);
class ChartJsInterop {
    constructor() {
        this.dotnetRefs = new Map();
        this.charts = new Map();
        this.chartInitPromises = new Map();
        this.chartJsCallbackRegistryPromises = new Map();
        this.appliedDefaultsKey = null;
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
        const datasetIdSet = new Set(datasetIds);
        for (const index of this.reverseKeys(chart.data.datasets)) {
            const dataset = chart.data.datasets[index];
            if (datasetIdSet.has(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }
    updateDatasetsSmooth(chart, datasets) {
        if (!chart || !chart.data) {
            return;
        }
        const existingDatasetsById = this.createDatasetMap(chart.data.datasets);
        datasets.forEach((newDataset) => {
            const existingDataset = existingDatasetsById.get(newDataset['id']);
            if (existingDataset != undefined) {
                this.assignDatasetSmooth(existingDataset, newDataset);
            }
        });
        chart.update();
    }
    updateDatasets(chart, datasets) {
        if (!chart || !chart.data) {
            return;
        }
        const datasetIndexesById = new Map();
        for (let i = 0; i < chart.data.datasets.length; i++) {
            datasetIndexesById.set(chart.data.datasets[i]['id'], i);
        }
        datasets.forEach((dataset) => {
            const datasetIndex = datasetIndexesById.get(dataset['id']);
            if (datasetIndex != undefined) {
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
    applyDatasetChangesSmooth(chart, desiredDatasetIds, datasetsToAdd, datasetsToUpdateSmooth, datasetIdsToRemove, labels, options, beforeUpdate) {
        if (!chart || !chart.data) {
            return;
        }
        if (labels != undefined) {
            chart.data.labels = labels;
        }
        if (options != undefined) {
            chart.options = options;
        }
        const removeDatasetIdSet = new Set(datasetIdsToRemove ?? []);
        const candidateDatasetsById = this.createDatasetMap(chart.data.datasets);
        for (let i = 0; i < datasetsToAdd.length; i++) {
            const dataset = datasetsToAdd[i];
            const datasetId = dataset['id'];
            if (!removeDatasetIdSet.has(datasetId)) {
                candidateDatasetsById.set(datasetId, dataset);
            }
        }
        for (let i = 0; i < datasetsToUpdateSmooth.length; i++) {
            const newDataset = datasetsToUpdateSmooth[i];
            const datasetId = newDataset['id'];
            const existingDataset = candidateDatasetsById.get(datasetId);
            if (!removeDatasetIdSet.has(datasetId) && existingDataset != undefined) {
                this.assignDatasetSmooth(existingDataset, newDataset);
            }
        }
        const finalDatasets = [];
        for (let i = 0; i < desiredDatasetIds.length; i++) {
            const datasetId = desiredDatasetIds[i];
            const dataset = candidateDatasetsById.get(datasetId);
            if (!removeDatasetIdSet.has(datasetId) && dataset != undefined) {
                finalDatasets.push(dataset);
            }
        }
        chart.data.datasets = finalDatasets;
        beforeUpdate?.();
        chart.update();
    }
    assignDatasetSmooth(existingDataset, newDataset) {
        Object.assign(existingDataset, newDataset);
        for (const prop in existingDataset) {
            if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
                delete existingDataset[prop];
            }
        }
    }
    createDatasetMap(datasets) {
        const datasetsById = new Map();
        for (let i = 0; i < datasets.length; i++) {
            datasetsById.set(datasets[i]['id'], datasets[i]);
        }
        return datasetsById;
    }
    async resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions) {
        if (hasChartJsFunctions !== true) {
            return;
        }
        const callbacks = await this.getChartJsCallbackRegistryIfConfigured(setupOptions);
        this.reviveChartJsFunctions(config, callbacks, "$", null, null, null);
    }
    async applyDefaults(setupOptions, defaults, hasChartJsFunctions, defaultsKey) {
        if (defaults == undefined) {
            return;
        }
        const resolvedDefaultsKey = typeof defaultsKey === "string" && defaultsKey.length > 0
            ? defaultsKey
            : JSON.stringify(defaults);
        if (this.appliedDefaultsKey === resolvedDefaultsKey) {
            return;
        }
        await this.resolveChartJsFunctions(setupOptions, defaults, hasChartJsFunctions);
        Chart.defaults.set(defaults);
        this.appliedDefaultsKey = resolvedDefaultsKey;
    }
    reviveChartJsFunctions(value, callbacks, path, key, parentKey, grandparentKey) {
        if (value == null || typeof value !== "object") {
            return value;
        }
        if (this.shouldSkipChartJsFunctionValue(key, parentKey, grandparentKey)) {
            return value;
        }
        if (Array.isArray(value)) {
            for (let i = 0; i < value.length; i++) {
                value[i] = this.reviveChartJsFunctions(value[i], callbacks, `${path}[${i}]`, String(i), key, parentKey);
            }
            return value;
        }
        if (this.isChartJsFunctionMarker(value, path)) {
            if (callbacks == undefined) {
                throw new Error(`Chart.js callback marker at ${path} requires ChartJsSetupOptions.ChartJsCallbacksModuleLocation.`);
            }
            return this.resolveChartJsFunction(value, callbacks, path);
        }
        const keys = Object.keys(value);
        for (let i = 0; i < keys.length; i++) {
            const childKey = keys[i];
            value[childKey] = this.reviveChartJsFunctions(value[childKey], callbacks, `${path}.${childKey}`, childKey, key, parentKey);
        }
        return value;
    }
    isChartJsFunctionMarker(value, path) {
        if (value == null
            || typeof value !== "object"
            || Array.isArray(value)
            || !this.objectHasOwn(value, chartJsFunctionMarker)) {
            return false;
        }
        const keys = Object.keys(value);
        if (keys.length !== 1) {
            throw new Error(`Invalid Chart.js callback marker at ${path}: '${chartJsFunctionMarker}' marker must not contain other properties.`);
        }
        return true;
    }
    shouldSkipChartJsFunctionValue(key, parentKey, grandparentKey) {
        return (grandparentKey === "datasets" && key === "data")
            || (parentKey === "data" && key === "labels");
    }
    async getChartJsCallbackRegistryIfConfigured(setupOptions) {
        const moduleLocation = setupOptions?.chartJsCallbacksModuleLocation;
        if (typeof moduleLocation !== "string" || moduleLocation.length === 0) {
            return undefined;
        }
        return await this.getChartJsCallbackRegistry(moduleLocation);
    }
    async getChartJsCallbackRegistry(moduleLocation) {
        let registryPromise = this.chartJsCallbackRegistryPromises.get(moduleLocation);
        if (!registryPromise) {
            registryPromise = import(moduleLocation)
                .then((module) => {
                const callbacks = module?.chartJsCallbacks;
                return this.createChartJsCallbackRegistry(moduleLocation, callbacks);
            })
                .catch((error) => {
                this.chartJsCallbackRegistryPromises.delete(moduleLocation);
                throw error;
            });
            this.chartJsCallbackRegistryPromises.set(moduleLocation, registryPromise);
        }
        return await registryPromise;
    }
    createChartJsCallbackRegistry(moduleLocation, callbacks) {
        if (callbacks == undefined || typeof callbacks !== "object") {
            throw new Error(`Chart.js callback module '${moduleLocation}' must export a chartJsCallbacks object.`);
        }
        const registry = Object.create(null);
        const callbackNames = Object.keys(callbacks);
        for (let i = 0; i < callbackNames.length; i++) {
            const callbackName = callbackNames[i];
            this.validateChartJsFunctionName(callbackName);
            const callback = callbacks[callbackName];
            if (typeof callback !== "function") {
                throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
            }
            registry[callbackName] = callback;
        }
        return Object.freeze(registry);
    }
    resolveChartJsFunction(reference, callbacks, path) {
        const callbackName = reference[chartJsFunctionMarker];
        if (typeof callbackName !== "string" || callbackName.length === 0) {
            throw new Error(`Invalid Chart.js callback marker at ${path}.`);
        }
        this.validateChartJsFunctionName(callbackName);
        if (!this.objectHasOwn(callbacks, callbackName)) {
            throw new Error(`Unknown Chart.js callback '${callbackName}' at ${path}.`);
        }
        const callback = callbacks[callbackName];
        if (typeof callback !== "function") {
            throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
        }
        return callback;
    }
    validateChartJsFunctionName(name) {
        if (typeof name !== "string" || !chartJsFunctionNamePattern.test(name)) {
            throw new Error(`Invalid Chart.js callback name: ${this.describeChartJsFunctionName(name)}. Names must match ${chartJsFunctionNamePattern}.`);
        }
        if (reservedChartJsFunctionNames.has(name)) {
            throw new Error(`Reserved Chart.js callback name: ${name}`);
        }
    }
    describeChartJsFunctionName(name) {
        if (typeof name === "string") {
            return name;
        }
        if (name === null) {
            return "null";
        }
        return typeof name;
    }
    objectHasOwn(value, key) {
        const hasOwn = Object.hasOwn;
        return typeof hasOwn === "function"
            ? hasOwn(value, key)
            : Object.prototype.hasOwnProperty.call(value, key);
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
export async function initChart(setupOptions, chartId, dotnetConfig, hasChartJsFunctions, dotnetRef, defaults, hasDefaultChartJsFunctions, defaultsKey) {
    const runningInit = ChartJsInteropModule.chartInitPromises.get(chartId) ?? Promise.resolve({ success: true });
    const initPromise = runningInit
        .catch(() => ({ success: true }))
        .then(() => initChartCore(setupOptions, chartId, dotnetConfig, hasChartJsFunctions, dotnetRef, defaults, hasDefaultChartJsFunctions, defaultsKey));
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
async function initChartCore(setupOptions, chartId, dotnetConfig, hasChartJsFunctions, dotnetRef, defaults, hasDefaultChartJsFunctions, defaultsKey) {
    try {
        await ensureChartJsLoaded(setupOptions);
        await ChartJsInteropModule.applyDefaults(setupOptions, defaults, hasDefaultChartJsFunctions === true, defaultsKey);
        const element = document.getElementById(chartId);
        if (!element) {
            return { success: false };
        }
        destroyExistingChart(chartId, element);
        const config = ChartJsInteropModule.buildChartConfig(dotnetConfig);
        await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions);
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
    const nativeCallback = typeof chart.options[optionName] === "function"
        ? chart.options[optionName]
        : undefined;
    chart.options[optionName] = (e, elements, chartInstance) => {
        nativeCallback?.call(chart, e, elements, chartInstance ?? chart);
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
        const nativeOnResize = typeof chart.options.onResize === "function"
            ? chart.options.onResize
            : undefined;
        chart.options.onResize = (_chart, size) => {
            nativeOnResize?.call(chart, _chart, size);
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
export async function updateChartOptions(chartId, setupOptionsOrOptions, options, hasChartJsFunctions) {
    const hasSetupOptions = arguments.length >= 3 && (arguments.length >= 4 || typeof options !== "boolean");
    const setupOptions = hasSetupOptions ? setupOptionsOrOptions : undefined;
    const resolvedOptions = hasSetupOptions ? options : setupOptionsOrOptions;
    const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctions : options;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, resolvedHasChartJsFunctions === true);
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
export async function addDatasets(chartId, setupOptionsOrDatasets, datasets, hasChartJsFunctions) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.addDatasets(chart, resolvedDatasets);
}
export async function addChartDataset(chartId, setupOptionsOrDataset, datasetOrHasChartJsFunctions, hasChartJsFunctionsOrAfterDatasetId, afterDatasetId) {
    const hasSetupOptions = datasetOrHasChartJsFunctions != undefined
        && typeof datasetOrHasChartJsFunctions === "object"
        && !Array.isArray(datasetOrHasChartJsFunctions);
    const setupOptions = hasSetupOptions ? setupOptionsOrDataset : undefined;
    const dataset = hasSetupOptions ? datasetOrHasChartJsFunctions : setupOptionsOrDataset;
    const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctionsOrAfterDatasetId : datasetOrHasChartJsFunctions;
    const resolvedAfterDatasetId = hasSetupOptions
        ? afterDatasetId
        : typeof hasChartJsFunctionsOrAfterDatasetId === "string" || hasChartJsFunctionsOrAfterDatasetId === null
            ? hasChartJsFunctionsOrAfterDatasetId
            : undefined;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: [dataset] } }, resolvedHasChartJsFunctions === true);
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
export async function updateDatasetsSmooth(chartId, setupOptionsOrDatasets, datasets, hasChartJsFunctions) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasetsSmooth(chart, resolvedDatasets);
}
export async function updateDatasets(chartId, setupOptionsOrDatasets, datasets, hasChartJsFunctions) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);
    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasets(chart, resolvedDatasets);
}
export async function applyDatasetChangesSmooth(chartId, setupOptions, desiredDatasetIds, datasetsToAdd, datasetsToUpdateSmooth, datasetIdsToRemove, labels, options, hasChartJsFunctions) {
    const resolvedDatasetsToAdd = datasetsToAdd ?? [];
    const resolvedDatasetsToUpdateSmooth = datasetsToUpdateSmooth ?? [];
    const resolvedDatasetIdsToRemove = datasetIdsToRemove ?? [];
    if (hasChartJsFunctions === true) {
        const config = {};
        if (resolvedDatasetsToAdd.length > 0 || resolvedDatasetsToUpdateSmooth.length > 0) {
            config.data = { datasets: resolvedDatasetsToAdd.concat(resolvedDatasetsToUpdateSmooth) };
        }
        if (options != undefined) {
            config.options = options;
        }
        await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, config, true);
    }
    const chart = getLiveChart(chartId);
    if (!chart || !desiredDatasetIds) {
        return;
    }
    ChartJsInteropModule.applyDatasetChangesSmooth(chart, desiredDatasetIds, resolvedDatasetsToAdd, resolvedDatasetsToUpdateSmooth, resolvedDatasetIdsToRemove, labels, options, () => {
        if (options != undefined) {
            registerEvents(options, chartId, chart);
        }
    });
}
export async function setDatasets(chartId, setupOptionsOrDatasets, datasets, hasChartJsFunctions) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets;
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);
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
