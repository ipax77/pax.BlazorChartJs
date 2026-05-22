import { decodeBinaryDatasetData } from "./binaryDatasets";
import { resolveChartJsFunctions } from "./chartCallbacks";
import { registerEvents } from "./chartEvents";
import { getLiveChart } from "./chartLifecycle";
import { isSetupOptions, parseArrayPayload, parsePayload } from "./payload";
import type {
    AddDataPayloadMap,
    BinaryDataFormat,
    BinaryDatasetPayloadMetadata,
    ChartInstance,
    ChartJsDatasetPayload,
    InteropChartDataset,
    SetDataPayloadMap
} from "./types";

type MutableInteropDatasetData = {
    length: number;
    pop(): unknown;
    push(value: unknown): number;
    splice(start: number, deleteCount: number, value: unknown): unknown[];
};

export function addLabel(chart: ChartInstance, label: string, pos: number) {
    if (label != undefined) {
        const labels = chart.data.labels as string[];
        if (pos == undefined) {
            labels.push(label);
        } else {
            labels.splice(pos, 0, label);
        }
    }
}

export function addDatasetData(dataset: InteropChartDataset, data: unknown, pos?: number | null) {
    const datasetData = dataset.data as MutableInteropDatasetData;
    if (pos == undefined) {
        datasetData.push(data);
    } else {
        datasetData.splice(pos, 0, data);
    }
}

export function addBackgroundColor(dataset: InteropChartDataset, backgroundColor: string, pos?: number | null) {
    if (Array.isArray(dataset.backgroundColor)) {
        if (pos == undefined) {
            dataset.backgroundColor.push(backgroundColor);
        } else {
            dataset.backgroundColor.splice(pos, 0, backgroundColor);
        }
    }
}

export function addBorderColor(dataset: InteropChartDataset, borderColor: string, pos?: number | null) {
    if (Array.isArray(dataset.borderColor)) {
        if (pos == undefined) {
            dataset.borderColor.push(borderColor);
        } else {
            dataset.borderColor.splice(pos, 0, borderColor);
        }
    }
}

export function addData(chartId: string, label: string, pos: number, datas: AddDataPayloadMap) {
    const chart = getLiveChart(chartId);
    if (chart == undefined) {
        return;
    }

    addLabel(chart, label, pos);

    chart.data.datasets.forEach((dataset) => {
        const datasetId = dataset.id;
        if (datasetId == undefined) {
            return;
        }

        const addData = datas[datasetId];
        if (addData != undefined) {
            addDatasetData(dataset, addData.data, addData.atPosition);

            if (addData.backgroundColor != undefined) {
                addBackgroundColor(dataset, addData.backgroundColor, addData.atPosition);
            }

            if (addData.borderColor != undefined) {
                addBorderColor(dataset, addData.borderColor, addData.atPosition);
            }
        }
    });
    chart.update();
}

function removeDataCore(chart: ChartInstance) {
    if (!chart || !chart.data) {
        return;
    }

    const labels = chart.data.labels as unknown[];
    if (!(labels.length == 0)) {
        labels.pop();
    }

    chart.data.datasets.forEach((dataset) => {
        const datasetData = dataset.data as MutableInteropDatasetData;
        if (!(datasetData.length == 0)) {
            datasetData.pop();
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

export function removeData(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    removeDataCore(chart);
}

function setDataCore(chart: ChartInstance, labels: string[], datas: SetDataPayloadMap) {
    if (!chart || !chart.data) {
        return;
    }

    if (labels != undefined) {
        chart.data.labels = labels;
    }

    chart.data.datasets.forEach((dataset) => {
        const datasetId = dataset.id;
        if (datasetId == undefined) {
            return;
        }

        const addData = datas[datasetId];
        if (addData != undefined) {

            dataset.data = addData.data;

            if (addData.backgroundColor != undefined) {
                dataset.backgroundColor = addData.backgroundColor;
            }

            if (addData.borderColor != undefined) {
                dataset.borderColor = addData.borderColor;
            }
        }
    });
    chart.update();
}

export function setData(chartId: string, labels: string[], datas: SetDataPayloadMap) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    setDataCore(chart, labels, datas);
}

function createDatasetMap(datasets: InteropChartDataset[]): Map<string, InteropChartDataset> {
    const datasetsById = new Map<string, InteropChartDataset>();
    for (let i = 0; i < datasets.length; i++) {
        const dataset = datasets[i];
        if (dataset.id != undefined) {
            datasetsById.set(dataset.id, dataset);
        }
    }

    return datasetsById;
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

    setDatasetsBinaryDataCore(chart, payloads, updateMode, binaryPayloads);
}

function setDatasetsBinaryDataCore(chart: ChartInstance, payloads: BinaryDatasetPayloadMetadata[], updateMode: string, binaryPayloads: Uint8Array[]) {
    if (!chart || !chart.data) {
        return;
    }

    if (payloads.length !== binaryPayloads.length) {
        throw new Error("Binary dataset metadata count does not match binary payload count.");
    }

    const datasetsById = createDatasetMap(chart.data.datasets);
    for (let i = 0; i < payloads.length; i++) {
        const payload = payloads[i];
        const dataset = datasetsById.get(payload.datasetId);
        if (dataset == undefined) {
            throw new Error(`Dataset '${payload.datasetId}' was not found.`);
        }

        dataset.data = decodeBinaryDatasetData(binaryPayloads[i], payload);
    }

    chart.update((updateMode ?? "none") as any);
}

function addDatasetsCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    for (let i = 0; i < datasets.length; i++) {
        chart.data.datasets.push(datasets[i]);
    }
    chart.update();
}

export async function addDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: ChartJsDatasetPayload[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload<ChartJsDatasetPayload>(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }

    addDatasetsCore(chart, resolvedDatasets);
}

function addDatasetCore(chart: ChartInstance, dataset: ChartJsDatasetPayload, afterDatasetId: string | null | undefined) {
    if (!chart || !chart.data) {
        return;
    }

    if (afterDatasetId == undefined) {
        chart.data.datasets.push(dataset);
    } else {
        const datasetIndex = chart.data.datasets.findIndex((existingDataset) => existingDataset.id === afterDatasetId);
        if (datasetIndex >= 0) {
            chart.data.datasets.splice(datasetIndex + 1, 0, dataset);
        } else {
            chart.data.datasets.push(dataset);
        }
    }

    chart.update();
}

function* reverseKeys(arr: any[]) {
    let key = arr.length - 1;

    while (key >= 0) {
        yield key;
        key -= 1;
    }
}


function removeDatasetsCore(chart: ChartInstance, datasetIds: string[]) {
    if (!chart || !chart.data) {
        return;
    }

    const datasetIdSet = new Set(datasetIds);
    for (const index of reverseKeys(chart.data.datasets)) {
        const dataset = chart.data.datasets[index];
        if (dataset.id != undefined && datasetIdSet.has(dataset.id)) {
            chart.data.datasets.splice(index, 1);
        }
    }
    chart.update();
}

export function removeDatasets(chartId: string, datasetIds: string[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    removeDatasetsCore(chart, datasetIds);
}

function updateDatasetsSmoothCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    const existingDatasetsById = createDatasetMap(chart.data.datasets);
    datasets.forEach((newDataset) => {
        const existingDataset = existingDatasetsById.get(newDataset.id);
        if (existingDataset != undefined) {
            assignDatasetSmooth(existingDataset, newDataset);
        }
    });
    chart.update();
}

export async function updateDatasetsSmooth(chartId: string, setupOptionsOrDatasets: any, datasets?: ChartJsDatasetPayload[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload<ChartJsDatasetPayload>(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }

    updateDatasetsSmoothCore(chart, resolvedDatasets);
}

function updateDatasetsCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    const datasetIndexesById = new Map<string, number>();
    for (let i = 0; i < chart.data.datasets.length; i++) {
        const datasetId = chart.data.datasets[i].id;
        if (datasetId != undefined) {
            datasetIndexesById.set(datasetId, i);
        }
    }

    datasets.forEach((dataset) => {
        const datasetIndex = datasetIndexesById.get(dataset.id);
        if (datasetIndex != undefined) {
            chart.data.datasets[datasetIndex] = dataset;
        }
    });
    chart.update();
}

export async function updateDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: ChartJsDatasetPayload[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload<ChartJsDatasetPayload>(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }

    updateDatasetsCore(chart, resolvedDatasets);
}

function setDatasetsCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    chart.data.datasets = datasets;
    chart.update();
}

export async function setDatasets(chartId: string, setupOptionsOrDatasets: any, datasets?: ChartJsDatasetPayload[], hasChartJsFunctions?: boolean) {
    const setupOptions = Array.isArray(setupOptionsOrDatasets) ? undefined : setupOptionsOrDatasets;
    const resolvedDatasets = parseArrayPayload<ChartJsDatasetPayload>(Array.isArray(setupOptionsOrDatasets) ? setupOptionsOrDatasets : datasets);
    const resolvedHasChartJsFunctions = Array.isArray(setupOptionsOrDatasets) ? datasets : hasChartJsFunctions;
    await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasets } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedDatasets) {
        return;
    }

    setDatasetsCore(chart, resolvedDatasets);
}

function applyDatasetChangesSmoothCore(
    chart: ChartInstance,
    desiredDatasetIds: string[],
    datasetsToAdd: ChartJsDatasetPayload[],
    datasetsToUpdateSmooth: ChartJsDatasetPayload[],
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
    const candidateDatasetsById = createDatasetMap(chart.data.datasets);

    for (let i = 0; i < datasetsToAdd.length; i++) {
        const dataset = datasetsToAdd[i];
        const datasetId = dataset.id;
        if (!removeDatasetIdSet.has(datasetId)) {
            candidateDatasetsById.set(datasetId, dataset);
        }
    }

    for (let i = 0; i < datasetsToUpdateSmooth.length; i++) {
        const newDataset = datasetsToUpdateSmooth[i];
        const datasetId = newDataset.id;
        const existingDataset = candidateDatasetsById.get(datasetId);
        if (!removeDatasetIdSet.has(datasetId) && existingDataset != undefined) {
            assignDatasetSmooth(existingDataset, newDataset);
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

export async function applyDatasetChangesSmooth(
    chartId: string,
    setupOptions: any,
    desiredDatasetIds: string[],
    datasetsToAdd: ChartJsDatasetPayload[],
    datasetsToUpdateSmooth: ChartJsDatasetPayload[],
    datasetIdsToRemove: string[],
    labels?: string[] | null,
    options?: any,
    hasChartJsFunctions?: boolean) {
    const resolvedDatasetsToAdd = parseArrayPayload<ChartJsDatasetPayload>(datasetsToAdd) ?? [];
    const resolvedDatasetsToUpdateSmooth = parseArrayPayload<ChartJsDatasetPayload>(datasetsToUpdateSmooth) ?? [];
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

        await resolveChartJsFunctions(setupOptions, config, true);
    }

    const chart = getLiveChart(chartId);
    if (!chart || !desiredDatasetIds) {
        return;
    }

    applyDatasetChangesSmoothCore(
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

export function assignDatasetSmooth(existingDataset: InteropChartDataset, newDataset: ChartJsDatasetPayload) {
    Object.assign(existingDataset, newDataset);

    for (const prop in existingDataset) {
        if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
            delete existingDataset[prop];
        }
    }
}

export async function addChartDataset(chartId: string, setupOptionsOrDataset: any, datasetOrHasChartJsFunctions?: any, hasChartJsFunctionsOrAfterDatasetId?: boolean | string | null, afterDatasetId?: string | null) {
    const hasSetupOptions = arguments.length >= 5 || isSetupOptions(setupOptionsOrDataset);
    const setupOptions = hasSetupOptions ? setupOptionsOrDataset : undefined;
    const dataset = parsePayload<ChartJsDatasetPayload>(hasSetupOptions ? datasetOrHasChartJsFunctions : setupOptionsOrDataset);
    const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctionsOrAfterDatasetId : datasetOrHasChartJsFunctions;
    const resolvedAfterDatasetId = hasSetupOptions
        ? afterDatasetId
        : typeof hasChartJsFunctionsOrAfterDatasetId === "string" || hasChartJsFunctionsOrAfterDatasetId === null
            ? hasChartJsFunctionsOrAfterDatasetId
            : undefined;

    await resolveChartJsFunctions(setupOptions, { data: { datasets: [dataset] } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !dataset) {
        return;
    }
    addDatasetCore(chart, dataset, resolvedAfterDatasetId);
}
