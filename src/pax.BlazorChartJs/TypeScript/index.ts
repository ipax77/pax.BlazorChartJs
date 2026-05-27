import { resolveChartJsFunctions } from "./chartCallbacks";
import { chartJsInterop } from "./chartInteropState";
import { chartJsInteropVersion } from "./version";

export { chartJsInteropVersion } from "./version";

export {
    initChart,
    disposeChart
} from "./chartLifecycle";

export {
    updateChartOptions,
    setLabels,
    resizeChart,
    getChartImage,
    resetChart,
    renderChart,
    stopChart,
    setDatasetVisibility,
    toggleDataVisibility,
    getDataVisibility,
    hideDataset,
    showDataset,
    getLabels,
    isDatasetVisible,
    setDatasetPointsActive
} from "./chartCommands";

export {
    addChartDataToDatasets,
    addData,
    removeData,
    removeDataset,
    setData,
    setDatasetsData,
    setDatasetBinaryData,
    setDatasetsBinaryData,
    addDatasets,
    addChartDataset,
    removeDatasets,
    updateDatasetsSmooth,
    updateDatasets,
    applyDatasetChangesSmooth,
    setDatasets
} from "./chartDatasets";

type ChartJsInteropGlobal = typeof chartJsInterop & {
    resolveChartJsFunctions: typeof resolveChartJsFunctions;
};

declare global {
    interface Window {
        ChartJsInterop: ChartJsInteropGlobal;
        chartJsInteropVersion: string;
    }
}

const chartJsInteropGlobal = Object.assign(chartJsInterop, { resolveChartJsFunctions });
window.ChartJsInterop = chartJsInteropGlobal;
window.chartJsInteropVersion = chartJsInteropVersion;
