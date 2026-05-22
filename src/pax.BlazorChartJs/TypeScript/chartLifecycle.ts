import { resolveChartJsFunctions } from "./chartCallbacks";
import { registerEvents } from "./chartEvents";
import { chartJsInterop } from "./chartInteropState";
import { loadPlugins } from "./chartPlugins";
import { parsePayload } from "./payload";
import { ChartInitResult } from "./types";

declare const Chart: any;


let chartJsLoadPromise: Promise<void> | null = null;

export function getLiveChart(chartId: string): any | undefined {
    const mappedChart = chartJsInterop.charts.get(chartId);
    if (mappedChart && mappedChart.data) {
        return mappedChart;
    }

    if (typeof Chart === "undefined") {
        return undefined;
    }

    const chart = Chart.getChart(chartId);
    return chart && chart.data ? chart : undefined;
}

async function ensureChartJsLoaded(setupOptions: any): Promise<void> {
    if (!setupOptions?.chartJsLocation) {
        return;
    }

    if (!chartJsLoadPromise) {
        chartJsLoadPromise = import(setupOptions.chartJsLocation).then(() => { });
    }

    await chartJsLoadPromise;
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
    const runningInit = chartJsInterop.chartInitPromises.get(chartId) ?? Promise.resolve({ success: true });
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

    chartJsInterop.chartInitPromises.set(chartId, initPromise);

    try {
        return await initPromise;
    } finally {
        if (chartJsInterop.chartInitPromises.get(chartId) === initPromise) {
            chartJsInterop.chartInitPromises.delete(chartId);
        }
    }
}

function buildChartConfig(dotnetConfig: any): any {
    return {
        type: dotnetConfig.type,
        data: dotnetConfig.data,
        options: dotnetConfig.options ?? {},
        plugins: []
    };
}

async function applyDefaults(
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

    if (chartJsInterop.appliedDefaultsKey === resolvedDefaultsKey) {
        return;
    }

    await resolveChartJsFunctions(setupOptions, defaults, hasChartJsFunctions);
    Chart.defaults.set(defaults);
    chartJsInterop.appliedDefaultsKey = resolvedDefaultsKey;
}

function destroyExistingChart(chartId: string, element?: HTMLCanvasElement | null) {
    const mappedChart = chartJsInterop.charts.get(chartId);
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

    chartJsInterop.charts.delete(chartId);
    chartJsInterop.dotnetRefs.delete(chartId);
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
        await applyDefaults(setupOptions, resolvedDefaults, hasDefaultChartJsFunctions === true, defaultsKey);

        const element = document.getElementById(chartId) as HTMLCanvasElement;
        if (!element) {
            return { success: false };
        }

        destroyExistingChart(chartId, element);

        const config = buildChartConfig(resolvedDotnetConfig);
        await resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions);
        config.plugins = await loadPlugins(setupOptions, resolvedDotnetConfig);

        const ctx = element.getContext('2d');
        if (!ctx) {
            return { success: false };
        }

        const chart = new Chart(ctx, config);
        chartJsInterop.charts.set(chartId, chart);
        chartJsInterop.dotnetRefs.set(chartId, dotnetRef);

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

export function disposeChart(chartId: string) {
    destroyExistingChart(chartId);
}
