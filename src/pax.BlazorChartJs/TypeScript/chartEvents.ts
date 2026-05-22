import { chartJsInterop } from "./chartInteropState";

declare const Chart: any;

async function triggerEvent(chartId: string, event: string, source: string, data: any) {
    const dotnetRef = chartJsInterop.dotnetRefs.get(chartId);
    if (dotnetRef) {
        await dotnetRef.invokeMethodAsync("EventTriggered", event, source, data);
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

export function registerEvents(dotnetConfigOptions: any, chartId: string, chart: any) {
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
