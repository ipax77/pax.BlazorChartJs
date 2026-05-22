import { resolveChartJsFunctions } from "./chartCallbacks";
import { registerEvents } from "./chartEvents";
import { getLiveChart } from "./chartLifecycle";
import { parsePayload } from "./payload";
import type { ChartInstance } from "./types";

export async function updateChartOptions(chartId: string, setupOptionsOrOptions: any, options?: any, hasChartJsFunctions?: boolean) {
    const hasSetupOptions = arguments.length >= 3 && (arguments.length >= 4 || typeof options !== "boolean");
    const setupOptions = hasSetupOptions ? setupOptionsOrOptions : undefined;
    const resolvedOptions = parsePayload(hasSetupOptions ? options : setupOptionsOrOptions);
    const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctions : options;
    await resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, resolvedHasChartJsFunctions === true);

    const chart = getLiveChart(chartId);
    if (chart != undefined) {
        chart.options = resolvedOptions;
        chart.update();
        registerEvents(resolvedOptions, chartId, chart);
    }
}

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
}

export function getChartImage(chartId: string, type?: string, quality?: number, width?: number, height?: number) {
    const chart = getLiveChart(chartId);
    if (!chart) {
        return "";
    }

    const canvas = chart.canvas;
    const shouldResize = width != undefined && height != undefined && canvas.parentNode != undefined;
    const currentWidth = canvas.width;
    const currentHeight = canvas.height;
    const currentAnimation = chart.options.animation;

    try {
        if (shouldResize) {
            chart.options.animation = false;
            chart.resize(width, height);
        }

        return type != undefined && quality != undefined
            ? chart.toBase64Image(type, quality)
            : chart.toBase64Image();
    } finally {
        if (shouldResize) {
            canvas.width = currentWidth;
            canvas.height = currentHeight;
            chart.options.animation = currentAnimation;
            chart.resize();
        }
    }
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

function resolveDatasetIndex(chart: ChartInstance, datasetIdOrIndex: string | number): number {
    if (typeof datasetIdOrIndex === "number") {
        return datasetIdOrIndex;
    }

    for (let i = 0; i < chart.data.datasets.length; i++) {
        if (chart.data.datasets[i].id === datasetIdOrIndex) {
            return i;
        }
    }

    return -1;
}

function hasDatasetElement(chart: ChartInstance, datasetIndex: number, dataIndex?: number): boolean {
    if (datasetIndex < 0 || datasetIndex >= chart.data.datasets.length) {
        return false;
    }

    if (dataIndex == undefined) {
        return true;
    }

    const dataset = chart.data.datasets[datasetIndex];
    const datasetData = dataset.data as ArrayLike<unknown> | undefined;
    return dataIndex >= 0 && dataIndex < (datasetData?.length ?? 0);
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
    const items = (chart.options as any).plugins.legend.labels.generateLabels(chart);
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
    const datasetData = dataset.data as ArrayLike<unknown>;
    for (let i = 0; i < datasetData.length; i++) {
        elements.push({ datasetIndex: datasetIndex, index: i });
    }

    chart.setActiveElements(elements);
    chart.update();
}
