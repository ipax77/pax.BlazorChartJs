import { chartJsInterop } from "./chartInteropState";
import type {
    ActiveElement,
    AnimationEvent,
    Chart as ChartJsChart,
    ChartEvent,
    ChartType,
    LegendElement,
    LegendItem
} from "chart.js";
import type { ChartInstance, ChartJsOptionsPayload } from "./types";

declare const Chart: {
    helpers: {
        getRelativePosition(event: Event, chart: ChartInstance): { x: number; y: number };
    };
};

type ChartEventBridgeOptions = ChartJsOptionsPayload & {
    animation?: {
        onCompleteEvent?: boolean;
        onProgressEvent?: boolean;
    };
    plugins?: {
        legend?: {
            onClickEvent?: boolean;
            onHoverEvent?: boolean;
            onLeaveEvent?: boolean;
        };
    };
};

type ChartPointOptionName = "onClick" | "onHover";
type LegendEventName = "click" | "hover" | "leave";
type LegendOptionName = "onClick" | "onHover" | "onLeave";
type Size = { height: number; width: number };
type ChartLegend = LegendElement<ChartType>;

async function triggerEvent(chartId: string, event: string, source: string, data: unknown) {
    const dotnetRef = chartJsInterop.dotnetRefs.get(chartId);
    if (dotnetRef) {
        await dotnetRef.invokeMethodAsync("EventTriggered", event, source, data);
    }
}

function getChartPointEventArgs(e: ChartEvent, chart: ChartInstance) {
    const nativeEvent = e.native;
    const points = nativeEvent == undefined
        ? []
        : chart.getElementsAtEventForMode(nativeEvent, "nearest", { intersect: true }, true);

    let label: unknown = "";
    let value: unknown = 0;
    let dataX = 0;
    let dataY = 0;
    let datasetLabel: string | null = null;
    let datasetIndex: number | null = null;

    const canvasPosition = nativeEvent == undefined
        ? { x: e.x ?? 0, y: e.y ?? 0 }
        : Chart.helpers.getRelativePosition(nativeEvent, chart);

    // Substitute the appropriate scale IDs.
    // Not all chart types have x/y scales, e.g. pie/doughnut charts.
    try {
        dataX = chart.scales.x.getValueForPixel(canvasPosition.x) ?? 0;
    } catch { }

    try {
        dataY = chart.scales.y.getValueForPixel(canvasPosition.y) ?? 0;
    } catch { }

    if (points.length) {
        const firstPoint = points[0];
        const currentDatasetIndex = firstPoint.datasetIndex;
        const currentDataset = chart.data.datasets[currentDatasetIndex];

        label = chart.data.labels?.[firstPoint.index] ?? "";
        datasetIndex = currentDatasetIndex;
        value = (currentDataset.data as ArrayLike<unknown> | undefined)?.[firstPoint.index] ?? 0;
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

function registerChartPointEvent(
    chart: ChartInstance,
    chartId: string,
    eventName: "click" | "hover",
    optionName: ChartPointOptionName
) {
    const nativeCallback = typeof chart.options[optionName] === "function"
        ? chart.options[optionName]
        : undefined;

    chart.options[optionName] = (e: ChartEvent, elements: ActiveElement[], chartInstance: ChartJsChart) => {
        nativeCallback?.call(chart, e, elements, chartInstance ?? chart);
        triggerEvent(chartId, eventName, "label", getChartPointEventArgs(e, chart));
    };
}

function registerLegendEvent(
    chart: ChartInstance,
    chartId: string,
    eventName: LegendEventName,
    optionName: LegendOptionName
) {
    const legendOptions = chart.options.plugins?.legend;
    if (legendOptions == undefined) {
        return;
    }
    const nativeCallback = typeof legendOptions[optionName] === "function"
        ? legendOptions[optionName]
        : undefined;

    legendOptions[optionName] = function (event: ChartEvent, legendItem: LegendItem, legend: ChartLegend) {
        nativeCallback?.call(this, event, legendItem, legend);
        triggerEvent(chartId, eventName, "legend", { Label: legendItem.text });
    };
}

export function registerEvents(dotnetConfigOptions: ChartEventBridgeOptions, chartId: string, chart: ChartInstance) {
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

        chart.options.onResize = (_chart: ChartJsChart, size: Size) => {
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
        registerLegendEvent(chart, chartId, "click", "onClick");
    }

    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {
        registerLegendEvent(chart, chartId, "hover", "onHover");
    }

    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {
        registerLegendEvent(chart, chartId, "leave", "onLeave");
    }

    // animation events
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {
        const animation = chart.options.animation;
        if (animation && typeof animation === "object") {
            const nativeOnProgress = typeof animation.onProgress === "function"
                ? animation.onProgress
                : undefined;

            animation.onProgress = (context: AnimationEvent) => {
                nativeOnProgress?.call(chart as unknown as ChartJsChart, context);
                triggerEvent(chartId, "progress", "animation", {
                    CurrentStep: context.currentStep,
                    NumSteps: context.numSteps
                });
            };
        }
    }

    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {
        const animation = chart.options.animation;
        if (animation && typeof animation === "object") {
            const nativeOnComplete = typeof animation.onComplete === "function"
                ? animation.onComplete
                : undefined;

            animation.onComplete = (context: AnimationEvent) => {
                nativeOnComplete?.call(chart as unknown as ChartJsChart, context);
                triggerEvent(chartId, "complete", "animation", {
                    Initial: context.initial
                });
            };
        }
    }
}
