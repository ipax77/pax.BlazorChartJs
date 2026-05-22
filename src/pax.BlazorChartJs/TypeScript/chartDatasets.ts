import { decodeBinaryDatasetData } from "./binaryDatasets";
import { resolveChartJsFunctions } from "./chartCallbacks";
import { chartJsInterop } from "./chartInteropState";
import { getLiveChart } from "./chartLifecycle";
import { parsePayload } from "./payload";
import { BinaryDataFormat, BinaryDatasetPayloadMetadata } from "./types";

export function addLabel(chart: any, label: string, pos: number) {
    if (label != undefined) {
        if (pos == undefined) {
            chart.data.labels.push(label);
        } else {
            chart.data.labels.splice(pos, 0, label);
        }
    }
}

export function addDatasetData(dataset: any, data: any, pos: number) {
    if (pos == undefined) {
        dataset.data.push(data);
    } else {
        dataset.data.splice(pos, 0, data);
    }
}

export function addBackgroundColor(dataset: any, backgroundColor: string, pos: number) {
    if (Array.isArray(dataset.backgroundColor)) {
        if (pos == undefined) {
            dataset.backgroundColor.push(backgroundColor);
        } else {
            dataset.backgroundColor.splice(pos, 0, backgroundColor);
        }
    }
}

export function addBorderColor(dataset: any, borderColor: string, pos: number) {
    if (Array.isArray(dataset.borderColor)) {
        if (pos == undefined) {
            dataset.borderColor.push(borderColor);
        } else {
            dataset.borderColor.splice(pos, 0, borderColor);
        }
    }
}

export function addData(chartId: string, label: string, pos: number, datas: any) {
    const chart = getLiveChart(chartId);
    if (chart == undefined) {
        return;
    }

    addLabel(chart, label, pos);

    chart.data.datasets.forEach((dataset: any) => {
        if (datas[dataset['id']] != undefined) {
            const addData = datas[dataset['id']];
            addDatasetData(dataset, addData['data'], addData['atPosition']);

            if (addData['backgroundColor'] != undefined) {
                addBackgroundColor(dataset, addData['backgroundColor'], addData['atPosition']);
            }

            if (addData['borderColor'] != undefined) {
                addBorderColor(dataset, addData['borderColor'], addData['atPosition']);
            }
        }
    });
    chart.update();
}

export function removeData(chart: any) {
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

export function setData(chart: any, labels: string[], datas: any) {
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

function createDatasetMap(datasets: any[]): Map<string, any> {
    const datasetsById = new Map<string, any>();
    for (let i = 0; i < datasets.length; i++) {
        datasetsById.set(datasets[i]['id'], datasets[i]);
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
        [bytes]);
}

export function setDatasetsBinaryData(chart: any, payloads: BinaryDatasetPayloadMetadata[], updateMode: string, binaryPayloads: Uint8Array[]) {
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

    chart.update(updateMode ?? "none");
}

export function addDatasets(chart: any, datasets: any[]) {
    if (!chart || !chart.data) {
        return;
    }

    for (let i = 0; i < datasets.length; i++) {
        chart.data.datasets.push(datasets[i]);
    }
    chart.update();
}

export function addDataset(chart: any, dataset: any, afterDatasetId: string | null | undefined) {
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

function* reverseKeys(arr: any[]) {
    let key = arr.length - 1;

    while (key >= 0) {
        yield key;
        key -= 1;
    }
}


export function removeDatasets(chart: any, datasetIds: string[]) {
    if (!chart || !chart.data) {
        return;
    }

    const datasetIdSet = new Set(datasetIds);
    for (const index of reverseKeys(chart.data.datasets)) {
        const dataset = chart.data.datasets[index];
        if (datasetIdSet.has(dataset['id'])) {
            chart.data.datasets.splice(index, 1);
        }
    }
    chart.update();
}

export function updateDatasetsSmooth(chart: any, datasets: any[]) {
    if (!chart || !chart.data) {
        return;
    }

    const existingDatasetsById = createDatasetMap(chart.data.datasets);
    datasets.forEach((newDataset: any) => {
        const existingDataset = existingDatasetsById.get(newDataset['id']);
        if (existingDataset != undefined) {
            assignDatasetSmooth(existingDataset, newDataset);
        }
    });
    chart.update();
}

export function updateDatasets(chart: any, datasets: any[]) {
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

export function setDatasets(chart: any, datasets: any[]) {
    if (!chart || !chart.data) {
        return;
    }

    chart.data.datasets = datasets;
    chart.update();
}

export function applyDatasetChangesSmooth(
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
    const candidateDatasetsById = createDatasetMap(chart.data.datasets);

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

export function assignDatasetSmooth(existingDataset: any, newDataset: any) {
    Object.assign(existingDataset, newDataset);

    for (const prop in existingDataset) {
        if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
            delete existingDataset[prop];
        }
    }
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

    await resolveChartJsFunctions(setupOptions, { data: { datasets: [dataset] } }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (!chart || !dataset) {
        return;
    }
    addDataset(chart, dataset, resolvedAfterDatasetId);
}