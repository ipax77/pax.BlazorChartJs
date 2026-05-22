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
    addData,
    removeData,
    setData,
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

(chartJsInterop as any).resolveChartJsFunctions = resolveChartJsFunctions;
(window as any).ChartJsInterop = chartJsInterop;
(window as any).chartJsInteropVersion = chartJsInteropVersion;
