class AsyncLock {
    constructor() {
        this.disable = () => { };
        this.promise = Promise.resolve();
    }
    enable() {
        this.promise = new Promise(resolve => this.disable = resolve);
    }
}

const lock = new AsyncLock()
let isLoaded = false;

export async function initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {

    await lock.promise
    lock.enable();

    try {

        if (!isLoaded) {
            if (setupOptions?.chartJsLocation) {
                await import(setupOptions.chartJsLocation);
            }
            else {
                await import('./chart.min.js');
            }
            isLoaded = true;
        }

        if (window.charts == undefined) {
            window.charts = {};
        }

        if (window.dotnetrefs == undefined) {
            window.dotnetrefs = {};
        }

        window.dotnetrefs[chartId] = dotnetRef;

        if (dotnetConfig.options == undefined) {
            dotnetConfig.options = {};
        }

        // todo: dynamic loading of plugins
        let plugins = [];
        if (dotnetConfig.options != undefined
            && dotnetConfig.options.plugins != undefined) {

            if (dotnetConfig.options.plugins.arbitraryLines != undefined) {
                const arbitraryLines = arbitaryLinesPlugin();

                plugins.push(arbitraryLines);
            }

            if (dotnetConfig.options.plugins.labels != undefined) {
                if (setupOptions?.chartJsPluginLabelsLocation) {
                    await import(setupOptions?.chartJsPluginLabelsLocation);
                }
                else {
                    await import('./chartjs-plugin-labels.min.js');
                }
                // require('./chartjs-plugin-labels.min.js');
            }

            if (dotnetConfig.options.plugins.datalabels != undefined) {
                if (setupOptions?.chartJsPluginDatalabelsLocation) {
                    await import(setupOptions.chartJsPluginDatalabelsLocation);
                } else {
                    await import('./chartjs-plugin-datalabels.min.js');
                }
                // require('./chartjs-plugin-datalabels.min.js');
                plugins.push(ChartDataLabels);
            }
        }

        const config = {
            type: dotnetConfig.type,
            data: dotnetConfig.data,
            options: dotnetConfig.options,
            plugins: plugins
        }

        if (window.charts[chartId]) {
            window.charts[chartId].destroy();
        }

        const ctx = document.getElementById(chartId).getContext('2d');
        window.charts[chartId] = new Chart(ctx, config);

        // window.charts[chartId].options.animation.onComplete = () => {
        //     console.log('chart animation complete');
        // };
        registerEvents(dotnetConfig.options, chartId, window.charts[chartId]);

    } catch { }
    finally {
        lock.disable();
    }
}

function registerEvents(dotnetConfigOptions, chartId, chart) {
    // chart events
    if (dotnetConfigOptions.onClickEvent == true) {
        chart.options.onClick = (e) => {
            const points = window.charts[chartId].getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;

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
                label = window.charts[chartId].data.labels[firstPoint.index];
                value = window.charts[chartId].data.datasets[firstPoint.datasetIndex].data[firstPoint.index];
            }
            triggerEvent(chartId, "click", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY });
        }
    }

    if (dotnetConfigOptions.onHoverEvent == true) {
        chart.options.onHover = (e) => {
            const points = window.charts[chartId].getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);
            let label = "";
            let value = 0;
            let dataX = 0;
            let dataY = 0;

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
                label = window.charts[chartId].data.labels[firstPoint.index];
                value = window.charts[chartId].data.datasets[firstPoint.datasetIndex].data[firstPoint.index];
            }
            triggerEvent(chartId, "hover", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY });
        }
    }

    if (dotnetConfigOptions.onResizeEvent == true) {
        chart.options.onResize = (chart, size) => {
            triggerEvent(chartId, "resize", "chart", { Height: size.height, Width: size.width });
        };
    }

    // legend events
    if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {

        chart.options.plugins.legend.onClick = (event, legendItem, legend) => {
            triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {

        chart.options.plugins.legend.onHover = (event, legendItem, legend) => {
            triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
        };
    }

    if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {

        chart.options.plugins.legend.onLeave = (event, legendItem, legend) => {
            triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
        };
    }

    // animation events
    if (dotnetConfigOptions.animation?.onProgressEvent == true) {

        chart.options.animation.onProgress = (context) => {
            triggerEvent(chartId, "progress", "animation", { CurrentStep: context.currentStep, NumSteps: context.numSteps });
        };
    }

    if (dotnetConfigOptions.animation?.onCompleteEvent == true) {

        chart.options.animation.onComplete = (context) => {
            triggerEvent(chartId, "complete", "animation", { Initial: context.initial });
        };
    }
}

async function triggerEvent(chartid, event, source, data) {
    if (window.dotnetrefs[chartid]) {
        await window.dotnetrefs[chartid].invokeMethodAsync("EventTriggered", event, source, data);
    }
}

export function updateChartOptions(chartId, options) {
    if (window.charts[chartId]) {
        window.charts[chartId].options = options;
        window.charts[chartId].update();

        registerEvents(options, chartId, window.charts[chartId]);
    }
}

export function updateChartDatasets(chartId, datasets) {
    if (window.charts[chartId]) {
        window.charts[chartId].data.datasets = datasets;
        window.charts[chartId].update();
    }
}

export function setDatasetsData(chartId, data) {
    if (window.charts[chartId]) {
        var chart = window.charts[chartId];
        const datasetMetas = window.charts[chartId].getSortedVisibleDatasetMetas();
        for (var index = 0; index < data.length; ++index) {
            var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === data[index].datasetId);
            window.charts[chartId].data.datasets[datasetIndex].data = data[index].data;
        }
        chart.update();
    }
}

export function addChartDataset(chartId, dataset, afterDatasetId) {
    if (window.charts[chartId]) {
        if (afterDatasetId == undefined) {
            window.charts[chartId].data.datasets.push(dataset);
        } else {
            const datasetMetas = window.charts[chartId].getSortedVisibleDatasetMetas();
            var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === afterDatasetId);
            window.charts[chartId].data.datasets.splice(datasetIndex + 1, 0, dataset);
        }
        window.charts[chartId].update();
    }
}

export function addChartDataToDatasets(chartId, label, data, backgroundColors, borderColors, pos) {
    if (window.charts[chartId]) {

        var chart = window.charts[chartId];

        if (pos == undefined) {
            if (label != undefined) {
                chart.data.labels.push(label);
            }

            for (var index = 0; index < data.length; ++index) {
                let dataset = window.charts[chartId].data.datasets[index];
                dataset.data.push(data[index]);

                if (backgroundColors != undefined && backgroundColors.length >= index
                    && Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length >= index) {
                    dataset.backgroundColor.push(backgroundColors[index]);
                }

                if (borderColors != undefined && borderColors.length >= index
                    && Array.isArray(dataset.borderColor) && dataset.borderColor.length >= index) {
                    dataset.borderColor.push(borderColors[index]);
                }
            }
        }
        else {

            if (label != undefined) {
                chart.data.labels.splice(pos, 0, label);
            }

            for (var index = 0; index < data.length; ++index) {
                let dataset = window.charts[chartId].data.datasets[index];
                dataset.data.splice(pos, 0, data[index]);

                if (backgroundColors != undefined && backgroundColors.length >= index
                    && Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length >= index) {
                    dataset.backgroundColor.splice(pos, 0, backgroundColors[index]);
                }

                if (borderColors != undefined && borderColors.length >= index
                    && Array.isArray(dataset.borderColor) && dataset.borderColor.length >= index) {
                    dataset.borderColor.splice(pos, 0, borderColors[index]);
                }
            }
        }
        chart.update();
    }
}

export function addData(chartId, label, pos, datasetDatas) {
    const chart = Chart.getChart(chartId);

    if (chart == undefined) {
        return;
    }

    addLabel(chart, label, pos);

    chart.data.datasets.forEach(dataset => {
        if (datasetDatas[dataset.id] != undefined) {
            let addData = datasetDatas[dataset.id];
            addDatasetData(dataset, addData.data, addData.atPosition);

            if (addData.backgroundColor != undefined) {
                addBackgroundColor(dataset, addData.backgroundColor, adddata.atPosition);
            }

            if (addData.borderColor != undefined) {
                addBorderColor(dataset, addData.borderColor, adddata.atPosition);
            }
        }
    });
    chart.update();
}

function addLabel(chart, label, pos) {
    if (label != undefined) {
        if (pos == undefined) {
            chart.data.labels.push(label);
        } else {
            chart.data.labels.splice(pos, 0, label);
        }
    }
}

function addDatasetData(dataset, data, pos) {
    if (pos == undefined) {
        dataset.data.push(data);
    } else {
        dataset.data.splice(pos, 0, data);
    }
}

function addBackgroundColor(dataset, backgroundColor, pos) {
    if (Array.isArray(dataset.backgroundColor)) {
        if (pos == undefined) {
            dataset.backgroundColor.push(backgroundColor);
        } else {
            dataset.backgroundColor.splice(pos, 0, backgroundColor);
        }
    }
}

function addBorderColor(dataset, borderColor, pos) {
    if (Array.isArray(dataset.borderColor)) {
        if (pos == undefined) {
            dataset.borderColor.push(borderColor);
        } else {
            dataset.borderColor.splice(pos, 0, borderColor);
        }
    }
}

export function removeDataset(chartId, datasetId) {
    if (window.charts[chartId]) {
        const datasetMetas = window.charts[chartId].getSortedVisibleDatasetMetas();
        var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === datasetId);
        window.charts[chartId].data.datasets.splice(datasetIndex, 1);
        window.charts[chartId].update();
    }
}

export function removeData(chartId, pos) {
    if (window.charts[chartId]) {
        var chart = window.charts[chartId];

        if (pos == undefined) {
            if (Array.isArray(chart.data.labels) && chart.data.labels.length > 0) {
                chart.data.labels.pop();
            }

            chart.data.datasets.forEach(dataset => {
                dataset.data.pop();
                if (Array.isArray(dataset.borderColor) && dataset.borderColor.length > 0) {
                    dataset.borderColor.pop();
                }
                if (Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length > 0) {
                    dataset.backgroundColor.pop();
                }
            });
        } else {
            if (Array.isArray(chart.data.labels) && chart.data.labels.length > pos) {
                chart.data.labels.splice(pos, 1);
            }

            chart.data.datasets.forEach(dataset => {
                dataset.data.splice(pos, 1);
                if (Array.isArray(dataset.borderColor) && dataset.borderColor.length > pos) {
                    dataset.borderColor.splice(pos, 1);
                }
                if (Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length > pos) {
                    dataset.backgroundColor.splice(pos, 1);
                }
            });
        }
        chart.update();
    }
}

export function setLabels(chartId, labels) {
    if (window.charts[chartId]) {
        var chart = window.charts[chartId];
        chart.data.labels = labels;
        chart.update();
    }
    async loadModules(moduleNames) {
        console.log("loading chartJs...");
        for (let i = 0; i < moduleNames.length; i++) {
            await import(moduleNames[i]);
        }
    }
}
window[ChartJsInterop.name] = new ChartJsInterop();
export async function initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
    await window[ChartJsInterop.name].initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
}

