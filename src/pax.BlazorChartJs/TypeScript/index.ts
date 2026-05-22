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