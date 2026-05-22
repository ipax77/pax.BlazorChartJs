import { resolveChartJsFunctions } from "./chartCallbacks";
import { registerEvents } from "./chartEvents";
import { getLiveChart } from "./chartLifecycle";
import { parsePayload } from "./payload";

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
    } else {
        chartImg = chart.toBase64Image();
    }

    if (currentWidth > 0 && currentHeight > 0) {
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
