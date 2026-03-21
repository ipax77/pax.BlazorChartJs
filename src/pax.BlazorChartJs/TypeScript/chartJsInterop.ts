// v0.8.6
export const chartJsInteropVersion = "0.8.6";

declare const Chart: any;
declare const ChartDataLabels: any;

class LoadInfo {
    public chartJsLoaded: boolean = false;
    public chartJsDatalabels: boolean = false;
    public chartJsLabels: boolean = false;
}

class ChartJsInterop {
    public dotnetRefs = new Map<string, any>();
    public loadInfo = new LoadInfo();

    public async initChart(setupOptions: any, chartId: string, dotnetConfig: any, dotnetRef: any): Promise<any> {
        this.dotnetRefs.set(chartId, dotnetRef);

        const config: any = {
            'type': dotnetConfig['type'],
            data: dotnetConfig['data'],
            options: dotnetConfig['options'] ?? {},
            // plugins: await this.loadPlugins(setupOptions, dotnetConfig)
            plugins: []
        };
        return config;
    }

    private async loadPlugins(setupOptions: any, dotnetConfig: any): Promise<Array<object>> {
        const plugins: object[] = [];

        if (dotnetConfig['options'] != undefined
            && dotnetConfig['options'].plugins != undefined) {

            if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
                const arbitraryLines = this.arbitaryLinesPlugin();
                plugins.push(arbitraryLines);
            }

            if (dotnetConfig['options'].plugins.datalabels != undefined) {
                if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                    const ChartDataLabelsModule = await import(setupOptions['chartJsPluginDatalabelsLocation']);
                    plugins.push(ChartDataLabelsModule);
                }
            }
        }

        return plugins;
    }

    public async triggerEvent(chartId: string, event: string, source: string, data: any) {
        if (this.dotnetRefs.has(chartId)) {
            await this.dotnetRefs.get(chartId).invokeMethodAsync("EventTriggered", event, source, data);
        }
    }

    public addData(chart: any, label: string, pos: number, datas: any) {

        if (chart == undefined) {
            return;
        }

        this.addLabel(chart, label, pos);

        chart.data.datasets.forEach((dataset: any) => {
            if (datas[dataset['id']] != undefined) {
                const addData = datas[dataset['id']];
                this.addDatasetData(dataset, addData['data'], addData['atPosition']);

                if (addData['backgroundColor'] != undefined) {
                    this.addBackgroundColor(dataset, addData['backgroundColor'], addData['atPosition']);
                }

                if (addData['borderColor'] != undefined) {
                    this.addBorderColor(dataset, addData['borderColor'], addData['atPosition']);
                }
            }
        });
        chart.update();
    }

    private addLabel(chart: any, label: string, pos: number) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            } else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }

    private addDatasetData(dataset: any, data: any, pos: number) {
        if (pos == undefined) {
            dataset.data.push(data);
        } else {
            dataset.data.splice(pos, 0, data);
        }
    }

    private addBackgroundColor(dataset: any, backgroundColor: string, pos: number) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            } else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }

    private addBorderColor(dataset: any, borderColor: string, pos: number) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            } else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }

    private *reverseKeys(arr: any[]) {
        let key = arr.length - 1;

        while (key >= 0) {
            yield key;
            key -= 1;
        }
    }

    public removeData(chart: any) {

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

    public setData(chart: any, labels: string[], datas: any) {
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

    public addDatasets(chart: any, datasets: any[]) {
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }

    public removeDatasets(chart: any, datasetIds: string[]) {
        for (const index of this.reverseKeys(chart.data.datasets)) {
            const dataset = chart.data.datasets[index];
            if (datasetIds.includes(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }

    public updateDatasetsSmooth(chart: any, datasets: any[]) {

        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach((newDataset: any) => {
            const datasetIndex = datasetMetas.findIndex((obj: any) => obj['_dataset']['id'] === newDataset['id']);
            if (datasetIndex >= 0) {
                const existingDataset = chart.data.datasets[datasetIndex];

                Object.assign(existingDataset, newDataset);

                for (const prop in existingDataset) {
                    if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
                        delete existingDataset[prop];
                    }
                }
            }
        });
        chart.update();
    }

    public updateDatasets(chart: any, datasets: any[]) {

        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach((dataset: any) => {
            const datasetIndex = datasetMetas.findIndex((obj: any) => obj['_dataset']['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }

    public setDatasets(chart: any, datasets: any[]) {
        chart.data.datasets = datasets;
        chart.update();
    }

    public disposeChart(chartId: string) {
        this.dotnetRefs.delete(chartId);
    }

    public arbitaryLinesPlugin(): any {
        return {
            id: 'arbitraryLines',
            // beforeDraw(chart, args, options) {
            afterDraw(chart: any, args: any, options: any) {
                const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

                ctx.save();

                for (let i = 0; i < options.length; i++) {
                    const option = options[i];
                    ctx.fillStyle = option.arbitraryLineColor;
                    const xWidth = option.xWidth;
                    const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                    const y0 = top;
                    const x1 = xWidth;
                    const y1 = height;
                    ctx.fillRect(x0, y0, x1, y1);
                }

                for (let i = 0; i < options.length; i++) {
                    const option = options[i];
                    ctx.fillStyle = option.arbitraryLineColor;
                    const xWidth = option.xWidth;
                    const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                    const y0 = top;
                    const x1 = xWidth;
                    const y1 = height;

                    ctx.fillStyle = 'white';
                    ctx.font = '14px arial';
                    ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
                }

                ctx.restore();
            }
        };
    }
}

const ChartJsInteropModule = new ChartJsInterop();
(window as any)[ChartJsInterop.name] = ChartJsInteropModule;
(window as any).chartJsInteropVersion = chartJsInteropVersion;

let chartJsLoadPromise: Promise<void> | null = null;

async function ensureChartJsLoaded(setupOptions: any): Promise<void> {
    if (!setupOptions?.chartJsLocation) {
        return;
    }

    if (!chartJsLoadPromise) {
        chartJsLoadPromise = import(setupOptions.chartJsLocation).then(() => { });
    }

    await chartJsLoadPromise;
}

export async function initChart(setupOptions: any, chartId: string, dotnetConfig: any, dotnetRef: any): Promise<boolean> {
    try {
        await ensureChartJsLoaded(setupOptions);

        const oldChart = Chart.getChart(chartId);
        if (oldChart != undefined) {
            oldChart.destroy();
        }

        const config = await ChartJsInteropModule.initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
        config.plugins = await loadPlugins(setupOptions, dotnetConfig);
        const ctx = (document.getElementById(chartId) as HTMLCanvasElement).getContext('2d');
        const chart = new Chart(ctx, config);

        if (dotnetConfig['options'] != undefined) {
            registerEvents(dotnetConfig.options, chartId, chart);
        }
    } finally {
    }
    return true;
}

async function loadPlugins(setupOptions: any, dotnetConfig: any): Promise<any[]> {
    const plugins: any[] = [];

    if (dotnetConfig['options'] != undefined
        && dotnetConfig['options'].plugins != undefined) {

        if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
            const arbitraryLines = ChartJsInteropModule.arbitaryLinesPlugin();
            plugins.push(arbitraryLines);
        }

        if (dotnetConfig['options'].plugins.labels != undefined) {
            if (setupOptions?.['chartJsPluginLabelsLocation']) {
                await import(setupOptions['chartJsPluginLabelsLocation']);
            }
        }

        if (dotnetConfig['options'].plugins.datalabels != undefined) {
            if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                await import(setupOptions['chartJsPluginDatalabelsLocation']);
            }
            plugins.push(ChartDataLabels);
        }
    }
    return plugins;
}

function registerEvents(dotnetConfigOptions: any, chartId: string, chart: any) {
    // chart events
    if (dotnetConfigOptions.onClickEvent == true) {
        chart.options.onClick = (e: any) => {
            const points = chart.getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;
            let datasetLabel: string | null = null;
            let datasetIndex: number | null = null;

            const canvasPosition = Chart.helpers.getRelativePosition(e, chart);

            // Substitute the appropriate scale IDs
            // todo: not working on pie.. charts
            try {
                dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
            } catch { }
            try {
                dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
            } catch { }

            if (points.length) {
                const firstPoint = points[0];
                label = chart.data.labels[firstPoint.index];
                datasetIndex = firstPoint.datasetIndex;
                value = chart.data.datasets[datasetIndex].data[firstPoint.index];
                datasetLabel = chart.data.datasets[datasetIndex].label;
            }
            triggerEvent(chartId, "click", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY, DatasetLabel: datasetLabel, DatasetIndex: datasetIndex });
        };
    }

    if (dotnetConfigOptions.onHoverEvent == true) {
        chart.options.onHover = (e: any) => {
            const points = chart.getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;
            let datasetLabel: string | null = null;
            let datasetIndex: number | null = null;

            const canvasPosition = Chart.helpers.getRelativePosition(e, chart);

            // Substitute the appropriate scale IDs
            // todo: not working on pie.. charts
            try {
                dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
            } catch { }
            try {
                dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
            } catch { }

            if (points.length) {
                const firstPoint = points[0];
                label = chart.data.labels[firstPoint.index];
                datasetIndex = firstPoint.datasetIndex;
                value = chart.data.datasets[datasetIndex].data[firstPoint.index];
                datasetLabel = chart.data.datasets[datasetIndex].label;
            }
            triggerEvent(chartId, "hover", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY, DatasetLabel: datasetLabel, DatasetIndex: datasetIndex });
        };
    }

    if (dotnetConfigOptions.onResizeEvent == true) {
        chart.options.onResize = (chart: any, size: any) => {
            triggerEvent(chartId, "resize", "chart", { Height: size.height, Width: size.width });
        };
    }

    // legend events
    if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {

        chart.options.plugins.legend.onClick = (event: any, legendItem: any, legend: any) => {
            triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {

        chart.options.plugins.legend.onHover = (event: any, legendItem: any, legend: any) => {
            triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {

        chart.options.plugins.legend.onLeave = (event: any, legendItem: any, legend: any) => {
            triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
        };
    }

    // animation events
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {

        chart.options.animation.onProgress = (context: any) => {
            triggerEvent(chartId, "progress", "animation", { CurrentStep: context.currentStep, NumSteps: context.numSteps });
        };
    }

    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {

        chart.options.animation.onComplete = (context: any) => {
            triggerEvent(chartId, "complete", "animation", { Initial: context.initial });
        };
    }
}

async function triggerEvent(chartId: string, event: string, source: string, data: any) {
    await ChartJsInteropModule.triggerEvent(chartId, event, source, data);
}

export function updateChartOptions(chartId: string, options: any) {
    const chart = Chart.getChart(chartId);
    if (chart != undefined) {
        chart.options = options;
        chart.update();
        registerEvents(options, chartId, chart);
    }
}

export function addData(chartId: string, label: string, pos: number, datas: any) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.addData(chart, label, pos, datas);
}

export function removeData(chartId: string) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.removeData(chart);
}

export function setData(chartId: string, labels: string[], datas: any) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.setData(chart, labels, datas);
}

export function addDatasets(chartId: string, datasets: any[]) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.addDatasets(chart, datasets);
}

export function removeDatasets(chartId: string, datasets: string[]) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.removeDatasets(chart, datasets);
}

export function updateDatasetsSmooth(chartId: string, datasets: any[]) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.updateDatasetsSmooth(chart, datasets);
}

export function updateDatasets(chartId: string, datasets: any[]) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.updateDatasets(chart, datasets);
}

export function setDatasets(chartId: string, datasets: any[]) {
    const chart = Chart.getChart(chartId);
    ChartJsInteropModule.setDatasets(chart, datasets);
}

// - ts
export function setLabels(chartId: string, labels: string[]) {
    const chart = Chart.getChart(chartId);
    chart.data.labels = labels;
    chart.update();
}

export function resizeChart(chartId: string, width?: number, height?: number) {
    const chart = Chart.getChart(chartId);
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

    const chart = Chart.getChart(chartId);
    let currentWidth = 0;
    let currentHeight = 0;
    if (!(width == undefined || height == undefined)) {
        const ctx = document.getElementById(chartId) as HTMLCanvasElement;
        // var ctx = document.getElementById(chartId).getContext('2d');
        if (ctx.parentNode) {
            currentHeight = ctx.width;
            currentHeight = ctx.height;

            //ctx.parentNode.style.resize = 'both';
            //ctx.parentNode.style.width = width + 'px !important';
            //ctx.parentNode.style.height = height + 'px !important';
            //chart.resize();

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
        //ctx.parentNode.style.width = currentWidth;
        //ctx.parentNode.style.height = currentHeight;
        chart.resize();
    }
    chart.options.animation = true;
    return chartImg;
}

export function resetChart(chartId: string) {
    const chart = Chart.getChart(chartId);
    chart.reset();
}

export function renderChart(chartId: string) {
    const chart = Chart.getChart(chartId);
    chart.render();
}

export function stopChart(chartId: string) {
    const chart = Chart.getChart(chartId);
    chart.stop();
}

export function setDatasetVisibility(chartId: string, datasetIndex: number, value: boolean) {
    const chart = Chart.getChart(chartId);
    chart.setDatasetVisibility(datasetIndex, value);
    chart.update();
}

export function toggleDataVisibility(chartId: string, index: number) {
    const chart = Chart.getChart(chartId);
    chart.toggleDataVisibility(index);
    chart.update();
}

export function getDataVisibility(chartId: string, index: number): boolean {
    const chart = Chart.getChart(chartId);
    return chart.getDataVisibility(index);
}

export function hideDataset(chartId: string, datasetId: string, dataIndex?: number) {
    const chart = Chart.getChart(chartId);
    const datasetMetas = chart.getSortedVisibleDatasetMetas();
    const datasetIndex = datasetMetas.findIndex((obj: any) => obj._dataset.id === datasetId);
    if (dataIndex == undefined) {
        chart.hide(datasetIndex);
    } else {
        chart.hide(datasetIndex, dataIndex);
    }
}

export function showDataset(chartId: string, datasetIndex: number, dataIndex?: number) {
    const chart = Chart.getChart(chartId);
    if (dataIndex == undefined) {
        chart.show(datasetIndex);
    } else {
        chart.show(datasetIndex, dataIndex);
    }
}

export function getLabels(chartId: string) {
    const chart = Chart.getChart(chartId);
    const items = chart.options.plugins.legend.labels.generateLabels(chart);
    return items;
}

export function isDatasetVisible(chartId: string, datasetIndex: number): boolean {
    const chart = Chart.getChart(chartId);
    const isVisible = chart.isDatasetVisible(datasetIndex);
    return isVisible;
}

export function setDatasetPointsActive(chartId: string, datasetIndex: number) {
    const chart = Chart.getChart(chartId);

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

export function disposeChart(chartId: string) {
    ChartJsInteropModule.disposeChart(chartId);
}
