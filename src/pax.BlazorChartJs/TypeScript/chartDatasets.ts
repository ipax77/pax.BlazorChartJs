import { decodeBinaryDatasetData } from "./binaryDatasets";
import { resolveChartJsFunctions } from "./chartCallbacks";
import { registerEvents } from "./chartEvents";
import { getLiveChart } from "./chartLifecycle";
import { isSetupOptions, parseArrayPayload, parsePayload } from "./payload";
import type { UpdateMode } from "chart.js";
import type {
    AddDataPayloadMap,
    BinaryDataFormat,
    BinaryDatasetPayloadMetadata,
    ChartInstance,
    ChartJsDatasetPayload,
    ChartJsOptionsPayload,
    ChartSetupOptionsPayload,
    DatasetDataPayload,
    InteropChartDataset,
    SetDataPayloadMap
} from "./types";

type DatasetListPayload = ChartJsDatasetPayload[] | string | null | undefined;
type DatasetPayload = ChartJsDatasetPayload | string | null | undefined;
type DatasetListOrCallbackFlag = DatasetListPayload | boolean;
type DatasetOrCallbackFlag = DatasetPayload | boolean;
type ChartUpdateAnimation = string | null | undefined;

type ResolvedDatasetListArguments = {
    setupOptions: ChartSetupOptionsPayload | null | undefined;
    datasets: ChartJsDatasetPayload[] | null | undefined;
    hasChartJsFunctions: boolean;
};

type MutableInteropDatasetData = {
    length: number;
    pop(): unknown;
    push(value: unknown): number;
    splice(start: number, deleteCount: number, value: unknown): unknown[];
};

const builtInUpdateAnimations = new Set<string>(["default", "active", "hide", "show", "reset", "resize", "none"]);

function resolveDatasetListArguments(
    setupOptionsOrDatasets: ChartSetupOptionsPayload | DatasetListPayload,
    datasetsOrHasChartJsFunctions?: DatasetListOrCallbackFlag,
    hasChartJsFunctions?: boolean
): ResolvedDatasetListArguments {
    if (Array.isArray(setupOptionsOrDatasets) || typeof setupOptionsOrDatasets === "string") {
        return {
            setupOptions: undefined,
            datasets: parseArrayPayload<ChartJsDatasetPayload>(setupOptionsOrDatasets),
            hasChartJsFunctions: datasetsOrHasChartJsFunctions === true
        };
    }

    const datasetPayload = typeof datasetsOrHasChartJsFunctions === "boolean"
        ? undefined
        : datasetsOrHasChartJsFunctions;

    return {
        setupOptions: setupOptionsOrDatasets,
        datasets: parseArrayPayload<ChartJsDatasetPayload>(datasetPayload),
        hasChartJsFunctions: hasChartJsFunctions === true
    };
}

function insertLabel(chart: ChartInstance, label: unknown, pos?: number | null) {
    const labels = chart.data.labels;
    if (labels == undefined) {
        return;
    }

    if (pos == undefined) {
        labels.push(label);
    } else {
        labels.splice(pos, 0, label);
    }
}

export function addLabel(chart: ChartInstance, label: string | null | undefined, pos?: number | null) {
    if (label != undefined) {
        insertLabel(chart, label, pos);
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

export function addData(chartId: string, label: string | null | undefined, pos: number | null | undefined, datas: AddDataPayloadMap) {
    const chart = getLiveChart(chartId);
    if (chart == undefined) {
        return;
    }

    addLabel(chart, label, pos);

    for (let i = 0; i < chart.data.datasets.length; i++) {
        const dataset = chart.data.datasets[i];
        const datasetId = dataset.id;
        if (datasetId == undefined) {
            continue;
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
    }
    chart.update();
}

export function addChartDataToDatasets(
    chartId: string,
    label: string | null | undefined,
    data: unknown[],
    backgroundColors?: string[] | null,
    borderColors?: string[] | null,
    pos?: number | null
) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    insertLabel(chart, label, pos);

    const datasetCount = Math.min(data.length, chart.data.datasets.length);
    for (let i = 0; i < datasetCount; i++) {
        const dataset = chart.data.datasets[i];
        addDatasetData(dataset, data[i], pos);

        const backgroundColor = backgroundColors?.[i];
        if (backgroundColor != undefined) {
            addBackgroundColor(dataset, backgroundColor, pos);
        }

        const borderColor = borderColors?.[i];
        if (borderColor != undefined) {
            addBorderColor(dataset, borderColor, pos);
        }
    }

    chart.update();
}

function removeDataCore(chart: ChartInstance) {
    if (!chart || !chart.data) {
        return;
    }

    const labels = chart.data.labels;
    if (labels != undefined && !(labels.length == 0)) {
        labels.pop();
    }

    for (let i = 0; i < chart.data.datasets.length; i++) {
        const dataset = chart.data.datasets[i];
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
    }
    chart.update();
}

export function removeData(chartId: string) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    removeDataCore(chart);
}

function setDataCore(chart: ChartInstance, labels: string[] | null | undefined, datas: SetDataPayloadMap) {
    if (!chart || !chart.data) {
        return;
    }

    if (labels != undefined) {
        chart.data.labels = labels;
    }

    for (let i = 0; i < chart.data.datasets.length; i++) {
        const dataset = chart.data.datasets[i];
        const datasetId = dataset.id;
        if (datasetId == undefined) {
            continue;
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
    }
    chart.update();
}

export function setData(chartId: string, labels: string[] | null | undefined, datas: SetDataPayloadMap) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    setDataCore(chart, labels, datas);
}

export function setDatasetsData(chartId: string, data: DatasetDataPayload[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    const datasetsById = createDatasetMap(chart.data.datasets);
    for (let i = 0; i < data.length; i++) {
        const payload = data[i];
        const dataset = datasetsById.get(payload.datasetId);
        if (dataset != undefined) {
            dataset.data = payload.data;
        }
    }

    chart.update();
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

function hasOwnProperty(value: unknown, propertyName: string): boolean {
    return value != undefined
        && typeof value === "object"
        && Object.prototype.hasOwnProperty.call(value, propertyName);
}

function getStringProperty(value: unknown, propertyName: string): string | undefined {
    if (value == undefined || typeof value !== "object") {
        return undefined;
    }

    const propertyValue = (value as Record<string, unknown>)[propertyName];
    return typeof propertyValue === "string" ? propertyValue : undefined;
}

function hasDatasetTypeTransition(chart: ChartInstance, updateAnimation: string): boolean {
    const datasetOptions = chart.options?.datasets;
    if (datasetOptions == undefined || typeof datasetOptions !== "object") {
        return false;
    }

    const checkedTypes = new Set<string>();
    const chartType = getStringProperty(chart.config, "type");
    if (chartType != undefined) {
        checkedTypes.add(chartType);
    }

    for (let i = 0; i < chart.data.datasets.length; i++) {
        const datasetType = getStringProperty(chart.data.datasets[i], "type");
        if (datasetType != undefined) {
            checkedTypes.add(datasetType);
        }
    }

    for (const datasetType of checkedTypes) {
        const typedDatasetOptions = (datasetOptions as Record<string, unknown>)[datasetType];
        if (typedDatasetOptions != undefined
            && typeof typedDatasetOptions === "object"
            && hasOwnProperty((typedDatasetOptions as Record<string, unknown>).transitions, updateAnimation)) {
            return true;
        }
    }

    return false;
}

function validateUpdateAnimation(chart: ChartInstance, updateAnimation: ChartUpdateAnimation): string | undefined {
    if (updateAnimation == undefined) {
        return undefined;
    }

    if (typeof updateAnimation !== "string" || updateAnimation.length === 0) {
        throw new Error("Dataset smooth update animation must be a non-empty string.");
    }

    if (builtInUpdateAnimations.has(updateAnimation)
        || hasOwnProperty(chart.options?.transitions, updateAnimation)
        || hasDatasetTypeTransition(chart, updateAnimation)) {
        return updateAnimation;
    }

    throw new Error(`Dataset smooth update animation '${updateAnimation}' is not a built-in update mode and was not found in chart transitions.`);
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
    updateMode: UpdateMode = "none") {
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
    updateMode: UpdateMode,
    ...binaryPayloads: Uint8Array[]) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return;
    }

    setDatasetsBinaryDataCore(chart, payloads, updateMode, binaryPayloads);
}

function setDatasetsBinaryDataCore(chart: ChartInstance, payloads: BinaryDatasetPayloadMetadata[], updateMode: UpdateMode, binaryPayloads: Uint8Array[]) {
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

function addDatasetsCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    for (let i = 0; i < datasets.length; i++) {
        chart.data.datasets.push(datasets[i]);
    }
    chart.update();
}

export async function addDatasets(
    chartId: string,
    setupOptionsOrDatasets: ChartSetupOptionsPayload | DatasetListPayload,
    datasetsOrHasChartJsFunctions?: DatasetListOrCallbackFlag,
    hasChartJsFunctions?: boolean
) {
    const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
    await resolveChartJsFunctions(
        resolvedArguments.setupOptions,
        { data: { datasets: resolvedArguments.datasets } },
        resolvedArguments.hasChartJsFunctions);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedArguments.datasets) {
        return;
    }

    addDatasetsCore(chart, resolvedArguments.datasets);
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

function removeDatasetsCore(chart: ChartInstance, datasetIds: string[]) {
    if (!chart || !chart.data) {
        return;
    }

    const datasetIdSet = new Set(datasetIds);
    for (let index = chart.data.datasets.length - 1; index >= 0; index--) {
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

export function removeDataset(chartId: string, datasetId: string) {
    removeDatasets(chartId, [datasetId]);
}

function updateDatasetsSmoothCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    const existingDatasetsById = createDatasetMap(chart.data.datasets);
    for (let i = 0; i < datasets.length; i++) {
        const newDataset = datasets[i];
        const existingDataset = existingDatasetsById.get(newDataset.id);
        if (existingDataset != undefined) {
            assignDatasetSmooth(existingDataset, newDataset);
        }
    }
    chart.update();
}

export async function updateDatasetsSmooth(
    chartId: string,
    setupOptionsOrDatasets: ChartSetupOptionsPayload | DatasetListPayload,
    datasetsOrHasChartJsFunctions?: DatasetListOrCallbackFlag,
    hasChartJsFunctions?: boolean
) {
    const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
    await resolveChartJsFunctions(
        resolvedArguments.setupOptions,
        { data: { datasets: resolvedArguments.datasets } },
        resolvedArguments.hasChartJsFunctions);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedArguments.datasets) {
        return;
    }

    updateDatasetsSmoothCore(chart, resolvedArguments.datasets);
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

    for (let i = 0; i < datasets.length; i++) {
        const dataset = datasets[i];
        const datasetIndex = datasetIndexesById.get(dataset.id);
        if (datasetIndex != undefined) {
            chart.data.datasets[datasetIndex] = dataset;
        }
    }
    chart.update();
}

export async function updateDatasets(
    chartId: string,
    setupOptionsOrDatasets: ChartSetupOptionsPayload | DatasetListPayload,
    datasetsOrHasChartJsFunctions?: DatasetListOrCallbackFlag,
    hasChartJsFunctions?: boolean
) {
    const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
    await resolveChartJsFunctions(
        resolvedArguments.setupOptions,
        { data: { datasets: resolvedArguments.datasets } },
        resolvedArguments.hasChartJsFunctions);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedArguments.datasets) {
        return;
    }

    updateDatasetsCore(chart, resolvedArguments.datasets);
}

function setDatasetsCore(chart: ChartInstance, datasets: ChartJsDatasetPayload[]) {
    if (!chart || !chart.data) {
        return;
    }

    chart.data.datasets = datasets;
    chart.update();
}

export async function setDatasets(
    chartId: string,
    setupOptionsOrDatasets: ChartSetupOptionsPayload | DatasetListPayload,
    datasetsOrHasChartJsFunctions?: DatasetListOrCallbackFlag,
    hasChartJsFunctions?: boolean
) {
    const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
    await resolveChartJsFunctions(
        resolvedArguments.setupOptions,
        { data: { datasets: resolvedArguments.datasets } },
        resolvedArguments.hasChartJsFunctions);

    const chart = getLiveChart(chartId);
    if (!chart || !resolvedArguments.datasets) {
        return;
    }

    setDatasetsCore(chart, resolvedArguments.datasets);
}

function applyDatasetChangesSmoothCore(
    chart: ChartInstance,
    desiredDatasetIds: string[],
    datasetsToAdd: ChartJsDatasetPayload[],
    datasetsToUpdateSmooth: ChartJsDatasetPayload[],
    datasetIdsToRemove: string[],
    labels: string[] | null | undefined,
    options: ChartJsOptionsPayload | null | undefined,
    updateAnimation: ChartUpdateAnimation,
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
    const validatedUpdateAnimation = validateUpdateAnimation(chart, updateAnimation);
    chart.update(validatedUpdateAnimation as UpdateMode | undefined);
}

export async function applyDatasetChangesSmooth(
    chartId: string,
    setupOptions: ChartSetupOptionsPayload | null | undefined,
    desiredDatasetIds: string[],
    datasetsToAdd: ChartJsDatasetPayload[],
    datasetsToUpdateSmooth: ChartJsDatasetPayload[],
    datasetIdsToRemove: string[],
    labels?: string[] | null,
    options?: ChartJsOptionsPayload | string | null,
    updateAnimationOrHasChartJsFunctions?: ChartUpdateAnimation | boolean,
    hasChartJsFunctions?: boolean) {
    const updateAnimation = typeof updateAnimationOrHasChartJsFunctions === "boolean"
        ? undefined
        : updateAnimationOrHasChartJsFunctions;
    const resolvedHasChartJsFunctions = typeof updateAnimationOrHasChartJsFunctions === "boolean"
        ? updateAnimationOrHasChartJsFunctions
        : hasChartJsFunctions;
    const resolvedDatasetsToAdd = parseArrayPayload<ChartJsDatasetPayload>(datasetsToAdd) ?? [];
    const resolvedDatasetsToUpdateSmooth = parseArrayPayload<ChartJsDatasetPayload>(datasetsToUpdateSmooth) ?? [];
    const resolvedDatasetIdsToRemove = datasetIdsToRemove ?? [];
    const resolvedOptions = parsePayload<ChartJsOptionsPayload>(options);

    if (resolvedHasChartJsFunctions === true) {
        if (resolvedDatasetsToAdd.length > 0) {
            await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasetsToAdd } }, true);
        }

        if (resolvedDatasetsToUpdateSmooth.length > 0) {
            await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasetsToUpdateSmooth } }, true);
        }

        if (resolvedOptions != undefined) {
            await resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, true);
        }
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
        updateAnimation,
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

export async function addChartDataset(
    chartId: string,
    setupOptionsOrDataset: ChartSetupOptionsPayload | DatasetPayload,
    datasetOrHasChartJsFunctions?: DatasetOrCallbackFlag,
    hasChartJsFunctionsOrAfterDatasetId?: boolean | string | null,
    afterDatasetId?: string | null
) {
    const hasSetupOptions = arguments.length >= 5 || isSetupOptions(setupOptionsOrDataset);
    const setupOptions = hasSetupOptions ? setupOptionsOrDataset as ChartSetupOptionsPayload | null | undefined : undefined;
    const datasetPayload = hasSetupOptions && typeof datasetOrHasChartJsFunctions !== "boolean"
        ? datasetOrHasChartJsFunctions
        : setupOptionsOrDataset as DatasetPayload;
    const dataset = parsePayload<ChartJsDatasetPayload>(datasetPayload);
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
