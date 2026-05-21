// v0.9.0-preview
export const chartJsInteropVersion = "0.9.0-preview";

declare const Chart: any;
declare const ChartDataLabels: any;

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
type ChartJsCallback = (...args: any[]) => any;
type ChartJsCallbackRegistry = Readonly<Record<string, ChartJsCallback>>;

class ChartJsInterop {
    public dotnetRefs = new Map<string, any>();
    public charts = new Map<string, any>();
    public chartInitPromises = new Map<string, Promise<ChartInitResult>>();
    public chartJsCallbackRegistryPromises = new Map<string, Promise<ChartJsCallbackRegistry>>();
    public appliedDefaultsKey: string | null = null;

    public buildChartConfig(dotnetConfig: any): any {
        return {
            type: dotnetConfig.type,
            data: dotnetConfig.data,
            options: dotnetConfig.options ?? {},
            plugins: []
        };
    }

    private async loadPlugins(setupOptions: any, dotnetConfig: any): Promise<Array<object>> {
        const plugins: object[] = [];

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

    public async triggerEvent(chartId: string, event: string, source: string, data: any) {
        if (this.dotnetRefs.has(chartId)) {
            await this.dotnetRefs.get(chartId).invokeMethodAsync("EventTriggered", event, source, data);
        }
    }

    public addData(chart: any, label: string, pos: number, datas: any) {

        if (chart == undefined) {
            return;
        }

        this.addLabel(chart, label, pos);

        chart.data.datasets.forEach((dataset: any) => {
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

    private addLabel(chart: any, label: string, pos: number) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            } else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }

    private addDatasetData(dataset: any, data: any, pos: number) {
        if (pos == undefined) {
            dataset.data.push(data);
        } else {
            dataset.data.splice(pos, 0, data);
        }
    }

    private addBackgroundColor(dataset: any, backgroundColor: string, pos: number) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            } else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }

    private addBorderColor(dataset: any, borderColor: string, pos: number) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            } else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }

    private *reverseKeys(arr: any[]) {
        let key = arr.length - 1;

        while (key >= 0) {
            yield key;
            key -= 1;
        }
    }

    public removeData(chart: any) {
        if (!chart || !chart.data) {
            return;
        }

        if (!(chart.data.labels.length == 0)) {
            chart.data.labels.pop();
        }

        chart.data.datasets.forEach((dataset: any) => {
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

    public setData(chart: any, labels: string[], datas: any) {
        if (!chart || !chart.data) {
            return;
        }

        if (labels != undefined) {
            chart.data.labels = labels;
        }

        chart.data.datasets.forEach((dataset: any) => {
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

    public setDatasetsBinaryData(chart: any, payloads: BinaryDatasetPayloadMetadata[], updateMode: string, binaryPayloads: Uint8Array[]) {
        if (!chart || !chart.data) {
            return;
        }

        if (payloads.length !== binaryPayloads.length) {
            throw new Error("Binary dataset metadata count does not match binary payload count.");
        }

        const datasetsById = this.createDatasetMap(chart.data.datasets);
        for (let i = 0; i < payloads.length; i++) {
            const payload = payloads[i];
            const dataset = datasetsById.get(payload.datasetId);
            if (dataset == undefined) {
                throw new Error(`Dataset '${payload.datasetId}' was not found.`);
            }

            dataset.data = decodeBinaryDatasetData(binaryPayloads[i], payload);
        }

        chart.update(updateMode ?? "none");
    }

    public addDatasets(chart: any, datasets: any[]) {
        if (!chart || !chart.data) {
            return;
        }

        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }

    public addDataset(chart: any, dataset: any, afterDatasetId: string | null | undefined) {
        if (!chart || !chart.data) {
            return;
        }

        if (afterDatasetId == undefined) {
            chart.data.datasets.push(dataset);
        } else {
            const datasetIndex = chart.data.datasets.findIndex((existingDataset: any) => existingDataset['id'] === afterDatasetId);
            if (datasetIndex >= 0) {
                chart.data.datasets.splice(datasetIndex + 1, 0, dataset);
            } else {
                chart.data.datasets.push(dataset);
            }
        }

        chart.update();
    }


    public removeDatasets(chart: any, datasetIds: string[]) {
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

    public updateDatasetsSmooth(chart: any, datasets: any[]) {
        if (!chart || !chart.data) {
            return;
        }

        const existingDatasetsById = this.createDatasetMap(chart.data.datasets);
        datasets.forEach((newDataset: any) => {
            const existingDataset = existingDatasetsById.get(newDataset['id']);
            if (existingDataset != undefined) {
                this.assignDatasetSmooth(existingDataset, newDataset);
            }
        });
        chart.update();
    }

    public updateDatasets(chart: any, datasets: any[]) {
        if (!chart || !chart.data) {
            return;
        }

        const datasetIndexesById = new Map<string, number>();
        for (let i = 0; i < chart.data.datasets.length; i++) {
            datasetIndexesById.set(chart.data.datasets[i]['id'], i);
        }

        datasets.forEach((dataset: any) => {
            const datasetIndex = datasetIndexesById.get(dataset['id']);
            if (datasetIndex != undefined) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }

    public setDatasets(chart: any, datasets: any[]) {
        if (!chart || !chart.data) {
            return;
        }

        chart.data.datasets = datasets;
        chart.update();
    }

    public applyDatasetChangesSmooth(
        chart: any,
        desiredDatasetIds: string[],
        datasetsToAdd: any[],
        datasetsToUpdateSmooth: any[],
        datasetIdsToRemove: string[],
        labels: string[] | null | undefined,
        options: any,
        beforeUpdate?: () => void) {
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

    private assignDatasetSmooth(existingDataset: any, newDataset: any) {
        Object.assign(existingDataset, newDataset);

        for (const prop in existingDataset) {
            if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
                delete existingDataset[prop];
            }
        }
    }

    private createDatasetMap(datasets: any[]): Map<string, any> {
        const datasetsById = new Map<string, any>();
        for (let i = 0; i < datasets.length; i++) {
            datasetsById.set(datasets[i]['id'], datasets[i]);
        }

        return datasetsById;
    }

    public async resolveChartJsFunctions(setupOptions: any, config: any, hasChartJsFunctions: boolean): Promise<void> {
        if (hasChartJsFunctions !== true) {
            return;
        }

        const callbacks = await this.getChartJsCallbackRegistryIfConfigured(setupOptions);
        this.reviveChartJsFunctions(config, callbacks, "$", null, null, null);
    }

    public async applyDefaults(
        setupOptions: any,
        defaults: any,
        hasChartJsFunctions: boolean,
        defaultsKey: string | null | undefined
    ): Promise<void> {
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

    private reviveChartJsFunctions(
        value: any,
        callbacks: ChartJsCallbackRegistry | undefined,
        path: string,
        key: string | null,
        parentKey: string | null,
        grandparentKey: string | null
    ): any {
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

    private isChartJsFunctionMarker(value: any, path: string): boolean {
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

    private shouldSkipChartJsFunctionValue(
        key: string | null,
        parentKey: string | null,
        grandparentKey: string | null
    ): boolean {
        return (grandparentKey === "datasets" && key === "data")
            || (parentKey === "data" && key === "labels");
    }

    private async getChartJsCallbackRegistryIfConfigured(setupOptions: any): Promise<ChartJsCallbackRegistry | undefined> {
        const moduleLocation = setupOptions?.chartJsCallbacksModuleLocation;
        if (typeof moduleLocation !== "string" || moduleLocation.length === 0) {
            return undefined;
        }

        return await this.getChartJsCallbackRegistry(moduleLocation);
    }

    private async getChartJsCallbackRegistry(moduleLocation: string): Promise<ChartJsCallbackRegistry> {
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

    private createChartJsCallbackRegistry(moduleLocation: string, callbacks: any): ChartJsCallbackRegistry {
        if (callbacks == undefined || typeof callbacks !== "object") {
            throw new Error(`Chart.js callback module '${moduleLocation}' must export a chartJsCallbacks object.`);
        }

        const registry = Object.create(null) as Record<string, ChartJsCallback>;
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

    private resolveChartJsFunction(reference: any, callbacks: ChartJsCallbackRegistry, path: string): ChartJsCallback {
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

    private validateChartJsFunctionName(name: any): asserts name is string {
        if (typeof name !== "string" || !chartJsFunctionNamePattern.test(name)) {
            throw new Error(`Invalid Chart.js callback name: ${this.describeChartJsFunctionName(name)}. Names must match ${chartJsFunctionNamePattern}.`);
        }

        if (reservedChartJsFunctionNames.has(name)) {
            throw new Error(`Reserved Chart.js callback name: ${name}`);
        }
    }

    private describeChartJsFunctionName(name: any): string {
        if (typeof name === "string") {
            return name;
        }

        if (name === null) {
            return "null";
        }

        return typeof name;
    }

    private objectHasOwn(value: any, key: string): boolean {
        const hasOwn = (Object as any).hasOwn;
        return typeof hasOwn === "function"
            ? hasOwn(value, key)
            : Object.prototype.hasOwnProperty.call(value, key);
    }

    public disposeChart(chartId: string) {
        this.dotnetRefs.delete(chartId);
    }

    public arbitaryLinesPlugin(): any {
        return {
            id: 'arbitraryLines',
            // beforeDraw(chart, args, options) {
            afterDraw(chart: any, args: any, options: any) {
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
(window as any)[ChartJsInterop.name] = ChartJsInteropModule;
(window as any).chartJsInteropVersion = chartJsInteropVersion;

let chartJsLoadPromise: Promise<void> | null = null;

async function ensureChartJsLoaded(setupOptions: any): Promise<void> {
    if (!setupOptions?.chartJsLocation) {
        return;
    }

    if (!chartJsLoadPromise) {
        chartJsLoadPromise = import(setupOptions.chartJsLocation).then(() => { });
    }

    await chartJsLoadPromise;
}

function parsePayload<T = any>(value: T | string | null | undefined): T | null | undefined {
    if (value == undefined || typeof value !== "string") {
        return value as T | null | undefined;
    }

    return JSON.parse(value) as T;
}

function parseArrayPayload<T = any>(value: T[] | string | null | undefined): T[] | null | undefined {
    return parsePayload<T[]>(value);
}

function isSetupOptions(value: any): boolean {
    return value != undefined
        && typeof value === "object"
        && !Array.isArray(value)
        && ("chartJsLocation" in value
            || "chartJsPluginLabelsLocation" in value
            || "chartJsPluginDatalabelsLocation" in value
            || "chartJsCallbacksModuleLocation" in value
            || "defaults" in value);
}

export async function initChart(
    setupOptions: any,
    chartId: string,
    dotnetConfig: any,
    hasChartJsFunctions: boolean,
    dotnetRef: any,
    defaults?: any,
    hasDefaultChartJsFunctions?: boolean,
    defaultsKey?: string
): Promise<ChartInitResult> {
    const runningInit = ChartJsInteropModule.chartInitPromises.get(chartId) ?? Promise.resolve({ success: true });
    const initPromise = runningInit
        .catch(() => ({ success: true }))
        .then(() => initChartCore(
            setupOptions,
            chartId,
            dotnetConfig,
            hasChartJsFunctions,
            dotnetRef,
            defaults,
            hasDefaultChartJsFunctions,
            defaultsKey));

    ChartJsInteropModule.chartInitPromises.set(chartId, initPromise);

    try {
        return await initPromise;
    } finally {
        if (ChartJsInteropModule.chartInitPromises.get(chartId) === initPromise) {
            ChartJsInteropModule.chartInitPromises.delete(chartId);
        }
    }
}

async function initChartCore(
    setupOptions: any,
    chartId: string,
    dotnetConfig: any,
    hasChartJsFunctions: boolean,
    dotnetRef: any,
    defaults?: any,
    hasDefaultChartJsFunctions?: boolean,
    defaultsKey?: string
): Promise<ChartInitResult> {
    try {
        const resolvedDotnetConfig = parsePayload(dotnetConfig);
        const resolvedDefaults = parsePayload(defaults);

        await ensureChartJsLoaded(setupOptions);
        await ChartJsInteropModule.applyDefaults(setupOptions, resolvedDefaults, hasDefaultChartJsFunctions === true, defaultsKey);

        const element = document.getElementById(chartId) as HTMLCanvasElement;
        if (!element) {
            return { success: false };
        }

        destroyExistingChart(chartId, element);

        const config = ChartJsInteropModule.buildChartConfig(resolvedDotnetConfig);
        await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions);
        config.plugins = await loadPlugins(setupOptions, resolvedDotnetConfig);
        
        const ctx = element.getContext('2d');
        if (!ctx) {
            return { success: false };
        }
        
        const chart = new Chart(ctx, config);
        ChartJsInteropModule.charts.set(chartId, chart);
        ChartJsInteropModule.dotnetRefs.set(chartId, dotnetRef);

        if (resolvedDotnetConfig['options'] != undefined) {
            registerEvents(resolvedDotnetConfig.options, chartId, chart);
        }

        const result = {
            success: true,
            height: chart.height,
            width: chart.width,
            windowHeight: window.innerHeight,
            windowWidth: window.innerWidth
        };

        return result;
    } finally {
    }
}

function destroyExistingChart(chartId: string, element?: HTMLCanvasElement | null) {
    const mappedChart = ChartJsInteropModule.charts.get(chartId);
    let destroyedChart: any | undefined;
    let destroyedElementChart: any | undefined;

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

async function loadPlugins(setupOptions: any, dotnetConfig: any): Promise<any[]> {
    const plugins: any[] = [];

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

function registerChartPointEvent(
    chart: any,
    chartId: string,
    eventName: "click" | "hover",
    optionName: "onClick" | "onHover"
) {
    const nativeCallback = typeof chart.options[optionName] === "function"
        ? chart.options[optionName]
        : undefined;

    chart.options[optionName] = (e: any, elements: any[], chartInstance: any) => {
        nativeCallback?.call(chart, e, elements, chartInstance ?? chart);
        triggerEvent(chartId, eventName, "label", getChartPointEventArgs(e, chart));
    };
}

function registerEvents(dotnetConfigOptions: any, chartId: string, chart: any) {
    // chart events
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

        chart.options.onResize = (_chart: any, size: any) => {
            nativeOnResize?.call(chart, _chart, size);
            triggerEvent(chartId, "resize", "chart", {
                Height: size.height,
                Width: size.width,
                WindowHeight: window.innerHeight,
                WindowWidth: window.innerWidth
            });
        };
    }

    // legend events
    if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {
        chart.options.plugins.legend.onClick = (_event: any, legendItem: any, _legend: any) => {
            triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {
        chart.options.plugins.legend.onHover = (_event: any, legendItem: any, _legend: any) => {
            triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {
        chart.options.plugins.legend.onLeave = (_event: any, legendItem: any, _legend: any) => {
            triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
        };
    }

    // animation events
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {
        chart.options.animation.onProgress = (context: any) => {
            triggerEvent(chartId, "progress", "animation", {
                CurrentStep: context.currentStep,
                NumSteps: context.numSteps
            });
        };
    }

    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {
        chart.options.animation.onComplete = (context: any) => {
            triggerEvent(chartId, "complete", "animation", {
                Initial: context.initial
            });
        };
    }
}

function getChartPointEventArgs(e: any, chart: any) {
    const points = chart.getElementsAtEventForMode(e, "nearest", { intersect: true }, true);

    let label = "";
    let value = 0;
    let dataX = 0;
    let dataY = 0;
    let datasetLabel: string | null = null;
    let datasetIndex: number | null = null;

    const canvasPosition = Chart.helpers.getRelativePosition(e, chart);

    // Substitute the appropriate scale IDs.
    // Not all chart types have x/y scales, e.g. pie/doughnut charts.
    try {
        dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
    } catch { }

    try {
        dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
    } catch { }

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

async function triggerEvent(chartId: string, event: string, source: string, data: any) {
    await ChartJsInteropModule.triggerEvent(chartId, event, source, data);
}

function getLiveChart(chartId: string): any | undefined {
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

type BinaryDataFormat = "Float64XY" | "Float32XY" | "Float64Y" | "Float32Y";

type BinaryDatasetPayloadMetadata = {
    datasetId: string;
    count: number;
    format: BinaryDataFormat;
    xOffset?: number | null;
    yOffset?: number | null;
    byteStride?: number | null;
};

type BinaryFloatArray = Float64Array | Float32Array;

const binaryTypedArraysReadLittleEndian = new Uint8Array(new Uint16Array([1]).buffer)[0] === 1;

function decodeBinaryDatasetData(bytes: Uint8Array, payload: BinaryDatasetPayloadMetadata): any[] {
    if (!(bytes instanceof Uint8Array)) {
        throw new Error("Binary dataset payload must be a Uint8Array.");
    }

    switch (payload.format) {
        case "Float64XY":
            return decodeBinaryXY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");

        case "Float32XY":
            return decodeBinaryXY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");

        case "Float64Y":
            return decodeBinaryY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");

        case "Float32Y":
            return decodeBinaryY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");

        default:
            throw new Error(`Unsupported binary dataset format '${payload.format}'.`);
    }
}

function decodeBinaryXY(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    valueSize: number,
    valueKind: "float64" | "float32"): Array<{ x: number, y: number }> {
    const layout = validateBinaryLayout(bytes, payload, valueSize * 2, valueSize, true);
    const data = new Array(payload.count);
    const typedValues = tryGetBinaryFloatArray(bytes, valueSize, valueKind, layout.byteStride, layout.xOffset, layout.yOffset);

    if (typedValues) {
        const valueStride = layout.byteStride / valueSize;
        const xOffset = layout.xOffset / valueSize;
        const yOffset = layout.yOffset / valueSize;

        for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
            data[i] = {
                x: typedValues[valueIndex + xOffset],
                y: typedValues[valueIndex + yOffset]
            };
        }

        return data;
    }

    const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);

    for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
        data[i] = {
            x: readBinaryFloat(view, recordOffset + layout.xOffset, valueKind),
            y: readBinaryFloat(view, recordOffset + layout.yOffset, valueKind)
        };
    }

    return data;
}

function decodeBinaryY(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    compactStride: number,
    valueKind: "float64" | "float32"): number[] {
    const layout = validateBinaryLayout(bytes, payload, compactStride, compactStride, false);
    const data = new Array(payload.count);
    const typedValues = tryGetBinaryFloatArray(bytes, compactStride, valueKind, layout.byteStride, layout.yOffset);

    if (typedValues) {
        const valueStride = layout.byteStride / compactStride;
        const yOffset = layout.yOffset / compactStride;

        for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
            data[i] = typedValues[valueIndex + yOffset];
        }

        return data;
    }

    const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);

    for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
        data[i] = readBinaryFloat(view, recordOffset + layout.yOffset, valueKind);
    }

    return data;
}

function validateBinaryLayout(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    compactStride: number,
    valueSize: number,
    usesX: boolean) {
    if (!Number.isInteger(payload.count) || payload.count < 0) {
        throw new Error("Binary dataset payload count must be a non-negative integer.");
    }

    const xOffset = payload.xOffset ?? 0;
    const rawYOffset = payload.yOffset ?? 0;
    const yOffset = usesX && xOffset === 0 && rawYOffset === 0
        ? valueSize
        : rawYOffset;
    const byteStride = payload.byteStride ?? compactStride;
    if (!Number.isInteger(xOffset) || xOffset < 0
        || !Number.isInteger(yOffset) || yOffset < 0
        || !Number.isInteger(byteStride) || byteStride < compactStride) {
        throw new Error("Binary dataset payload has invalid offsets or byte stride.");
    }

    const maxOffset = usesX ? Math.max(xOffset, yOffset) : yOffset;
    if (maxOffset + valueSize > byteStride) {
        throw new Error("Binary dataset payload offsets must fit inside the byte stride.");
    }

    const requiredBytes = payload.count === 0
        ? 0
        : ((payload.count - 1) * byteStride) + maxOffset + valueSize;
    if (bytes.byteLength < requiredBytes) {
        throw new Error(`Binary dataset payload for '${payload.datasetId}' is too small.`);
    }

    return {
        xOffset,
        yOffset,
        byteStride
    };
}

function tryGetBinaryFloatArray(
    bytes: Uint8Array,
    valueSize: number,
    valueKind: "float64" | "float32",
    byteStride: number,
    ...offsets: number[]): BinaryFloatArray | undefined {
    if (!binaryTypedArraysReadLittleEndian
        || bytes.byteOffset % valueSize !== 0
        || byteStride % valueSize !== 0
        || offsets.some(offset => offset % valueSize !== 0)) {
        return undefined;
    }

    const valueCount = Math.floor(bytes.byteLength / valueSize);
    return valueKind === "float64"
        ? new Float64Array(bytes.buffer, bytes.byteOffset, valueCount)
        : new Float32Array(bytes.buffer, bytes.byteOffset, valueCount);
}

function readBinaryFloat(view: DataView, byteOffset: number, valueKind: "float64" | "float32"): number {
    return valueKind === "float64"
        ? view.getFloat64(byteOffset, true)
        : view.getFloat32(byteOffset, true);
}

export async function updateChartOptions(chartId: string, setupOptionsOrOptions: any, options?: any, hasChartJsFunctions?: boolean) {
    const hasSetupOptions = arguments.length >= 3 && (arguments.length >= 4 || typeof options !== "boolean");
    const setupOptions = hasSetupOptions ? setupOptionsOrOptions : undefined;
    const resolvedOptions = parsePayload(hasSetupOptions ? options : setupOptionsOrOptions);
    const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctions : options;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (chart != undefined) {
        chart.options = resolvedOptions;
        chart.update();
        registerEvents(resolvedOptions, chartId, chart);
    }
}

export function addData(chartId: string, label: string, pos: number, datas: any) {
    const chart = getLiveChart(chartId);
    ChartJsInteropModule.addData(chart, label, pos, datas);
}

export function removeData(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.removeData(chart);
}

export function setData(chartId: string, labels: string[], datas: any) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.setData(chart, labels, datas);
}

export function setDatasetBinaryData(
    chartId: string,
    datasetId: string,
    bytes: Uint8Array,
    pointCount: number,
    format: BinaryDataFormat,
    xOffset = 0,
    yOffset = 0,
    byteStride?: number | null,
    updateMode = "none") {
    setDatasetsBinaryData(
        chartId,
        [{
            datasetId,
            count: pointCount,
            format,
            xOffset,
            yOffset,
            byteStride
        }],
        updateMode,
        bytes);
}

export function setDatasetsBinaryData(
    chartId: string,
    payloads: BinaryDatasetPayloadMetadata[],
    updateMode: string,
    ...binaryPayloads: Uint8Array[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    ChartJsInteropModule.setDatasetsBinaryData(chart, payloads, updateMode, binaryPayloads);
}

export async function addDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: any[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.addDatasets(chart, resolvedDatasets);
}

export async function addChartDataset(chartId: string, setupOptionsOrDataset: any, datasetOrHasChartJsFunctions?: any, hasChartJsFunctionsOrAfterDatasetId?: boolean | string | null, afterDatasetId?: string | null) {
    const hasSetupOptions = arguments.length >= 5 || isSetupOptions(setupOptionsOrDataset);
    const setupOptions = hasSetupOptions ? setupOptionsOrDataset : undefined;
    const dataset = parsePayload(hasSetupOptions ? datasetOrHasChartJsFunctions : setupOptionsOrDataset);
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


export function removeDatasets(chartId: string, datasets: string[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    ChartJsInteropModule.removeDatasets(chart, datasets);
}

export async function updateDatasetsSmooth(chartId: string, setupOptionsOrDatasets: any, datasets?: any[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasetsSmooth(chart, resolvedDatasets);
}

export async function updateDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: any[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.updateDatasets(chart, resolvedDatasets);
}

export async function applyDatasetChangesSmooth(
    chartId: string,
    setupOptions: any,
    desiredDatasetIds: string[],
    datasetsToAdd: any[],
    datasetsToUpdateSmooth: any[],
    datasetIdsToRemove: string[],
    labels?: string[] | null,
    options?: any,
    hasChartJsFunctions?: boolean) {
    const resolvedDatasetsToAdd = parseArrayPayload(datasetsToAdd) ?? [];
    const resolvedDatasetsToUpdateSmooth = parseArrayPayload(datasetsToUpdateSmooth) ?? [];
    const resolvedDatasetIdsToRemove = datasetIdsToRemove ?? [];
    const resolvedOptions = parsePayload(options);

    if (hasChartJsFunctions === true) {
        const config: any = {};
        if (resolvedDatasetsToAdd.length > 0 || resolvedDatasetsToUpdateSmooth.length > 0) {
            config.data = { datasets: resolvedDatasetsToAdd.concat(resolvedDatasetsToUpdateSmooth) };
        }

        if (resolvedOptions != undefined) {
            config.options = resolvedOptions;
        }

        await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, config, true);
    }

    const chart = getLiveChart(chartId);
    if (!chart || !desiredDatasetIds) {
        return;
    }

    ChartJsInteropModule.applyDatasetChangesSmooth(
        chart,
        desiredDatasetIds,
        resolvedDatasetsToAdd,
        resolvedDatasetsToUpdateSmooth,
        resolvedDatasetIdsToRemove,
        labels,
        resolvedOptions,
        () => {
            if (resolvedOptions != undefined) {
                registerEvents(resolvedOptions, chartId, chart);
            }
        });
}

export async function setDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: any[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await ChartJsInteropModule.resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }
    ChartJsInteropModule.setDatasets(chart, resolvedDatasets);
}

// - ts
export function setLabels(chartId: string, labels: string[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.data.labels = labels;
    chart.update();
}

export function resizeChart(chartId: string, width?: number, height?: number) {
    const chart = getLiveChart(chartId);
    if (chart == undefined) {
        return;
    }
    if (width == undefined || height == undefined) {
        chart.resize();
    } else {
        chart.resize(width, height);
    }
    chart.options.onResize?.(chart, { height: chart.height, width: chart.width });
}

export function getChartImage(chartId: string, type?: string, quality?: number, width?: number, height?: number) {

    const chart = getLiveChart(chartId);
    if (!chart) {
        return "";
    }
    let currentWidth = 0;
    let currentHeight = 0;
    if (!(width == undefined || height == undefined)) {
        const ctx = document.getElementById(chartId) as HTMLCanvasElement;
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

export function resetChart(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.reset();
}

export function renderChart(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.render();
}

export function stopChart(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.stop();
}

export function setDatasetVisibility(chartId: string, datasetIndex: number, value: boolean) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.setDatasetVisibility(datasetIndex, value);
    chart.update();
}

export function toggleDataVisibility(chartId: string, index: number) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    chart.toggleDataVisibility(index);
    chart.update();
}

export function getDataVisibility(chartId: string, index: number): boolean {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return false;
    }
    return chart.getDataVisibility(index);
}

function resolveDatasetIndex(chart: any, datasetIdOrIndex: string | number): number {
    if (typeof datasetIdOrIndex === "number") {
        return datasetIdOrIndex;
    }

    return chart.data.datasets.findIndex((dataset: any) => dataset.id === datasetIdOrIndex);
}

function hasDatasetElement(chart: any, datasetIndex: number, dataIndex?: number): boolean {
    if (datasetIndex < 0 || datasetIndex >= chart.data.datasets.length) {
        return false;
    }

    if (dataIndex == undefined) {
        return true;
    }

    const dataset = chart.data.datasets[datasetIndex];
    return dataIndex >= 0 && dataIndex < (dataset.data?.length ?? 0);
}

export function hideDataset(chartId: string, datasetIdOrIndex: string | number, dataIndex?: number) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    const datasetIndex = resolveDatasetIndex(chart, datasetIdOrIndex);
    if (!hasDatasetElement(chart, datasetIndex, dataIndex)) {
        return;
    }

    if (dataIndex == undefined) {
        chart.hide(datasetIndex);
    } else {
        chart.hide(datasetIndex, dataIndex);
    }
}

export function showDataset(chartId: string, datasetIdOrIndex: string | number, dataIndex?: number) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }
    const datasetIndex = resolveDatasetIndex(chart, datasetIdOrIndex);
    if (!hasDatasetElement(chart, datasetIndex, dataIndex)) {
        return;
    }

    if (dataIndex == undefined) {
        chart.show(datasetIndex);
    } else {
        chart.show(datasetIndex, dataIndex);
    }
}

export function getLabels(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return [];
    }
    const items = chart.options.plugins.legend.labels.generateLabels(chart);
    return items;
}

export function isDatasetVisible(chartId: string, datasetIndex: number): boolean {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return false;
    }
    const isVisible = chart.isDatasetVisible(datasetIndex);
    return isVisible;
}

export function setDatasetPointsActive(chartId: string, datasetIndex: number) {
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

export function disposeChart(chartId: string) {
    destroyExistingChart(chartId);
    ChartJsInteropModule.disposeChart(chartId);
}

type ChartInitResult =
    | {
        success: true;
        height: number;
        width: number;
        windowHeight: number;
        windowWidth: number;
    }
    | {
        success: false;
    };
