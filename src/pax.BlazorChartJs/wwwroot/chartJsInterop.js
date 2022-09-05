// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

// import * as Chart from './dist/chart.js';
// import Chart from './dist/chart.js/auto';
// import { Chart, registerables } from './chart.min.js';

import './chart.min.js';
//import './chartjs-plugin-labels.min.js';

// todo: this only stops the first call which might not be good enough
class AsyncLock {
    constructor() {
        this.disable = () => { }
        this.promise = Promise.resolve()
    }

    enable() {
        this.promise = new Promise(resolve => this.disable = resolve)
    }
}

const lock = new AsyncLock()

export async function initChart(chartId, dotnetConfig, dotnetRef) {

    await lock.promise
    lock.enable();

    try {
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
                await import('./chartjs-plugin-labels.min.js');
                // require('./chartjs-plugin-labels.min.js');
            }

            if (dotnetConfig.options.plugins.datalabels != undefined) {
                await import('./chartjs-plugin-datalabels.min.js');
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
            triggerEvent(chartId, "click", "label", { Label: label, Value: value, DataX: dataX, DataY: dataY });
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

async function reportChartClick(chartid, label) {
    if (window.dotnetrefs[chartid]) {
        await window.dotnetrefs[chartid].invokeMethodAsync("ChartClicked", label);
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
            var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === datasetId);
            window.charts[chartId].data.datasets.splice(datasetIndex + 1, 0, dataset);
        }
        window.charts[chartId].update();
    }
}

export function addChartDataToDatasets(chartId, label, data, backgroundColors, borderColors, pos) {
    if (window.charts[chartId]) {

        var chart = window.charts[chartId];

        if (pos == undefined) {
            pos = chart.data.labels.length;
        }

        chart.data.labels.splice(pos, 0, label);

        for (var index = 0; index < data.length; ++index) {
            let dataset = window.charts[chartId].data.datasets[index];
            dataset.data.splice(pos, 0, data[index]);

            if (backgroundColors != undefined && backgroundColors.length >= index
                && Array.isArray(Array.isArray) && dataset.backgroundColor.length >= index) {
                dataset.backgroundColor.splice(pos, 0, backgroundColors[index]);
            }

            if (borderColors != undefined && borderColors.length >= index
                && Array.isArray(dataset.borderColor) && dataset.borderColor.length >= index) {
                dataset.borderColor.splice(pos, 0, borderColors[index]);
            }
        }
        chart.update();
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
            pos = chart.data.labels.length - 1;
        }

        if (Array.isArray(chart.data.labels) && chart.data.labels.length >= pos) {
            chart.data.labels.splice(pos, 1);
        }

        chart.data.datasets.forEach(dataset => {
            dataset.data.splice(pos, 1);
            if (Array.isArray(dataset.borderColor) && dataset.borderColor.length >= pos) {
                dataset.borderColor.splice(pos, 1);
            }
            if (Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length >= pos) {
                dataset.backgroundColor.splice(pos, 1);
            }
        });
        chart.update();
    }
}

export function setLabels(chartId, labels) {
    if (window.charts[chartId]) {
        var chart = window.charts[chartId];
        chart.data.labels = labels;
        chart.update();
    }
}

export function testChart(canvasId) {
    if (window.charts == undefined) {
        return false;
    }
    if (window.charts[canvasId] == undefined) {
        return false;
    }
    return true;
}

export function resizeChart(chartId, width, height) {
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

export function getChartImage(chartId, type, quality, width, height) {

    const chart = Chart.getChart(chartId);
    let currentWidth = 0;
    let currentHeight = 0;
    if (!(width == undefined || height == undefined)) {
        var ctx = document.getElementById(chartId);
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

function arbitaryLinesPlugin() {
    return {
        id: 'arbitraryLines',
        // beforeDraw(chart, args, options) {
        afterDraw(chart, args, options) {
            const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

            ctx.save();

            for (let i = 0; i < options.length; i++) {
                var option = options[i];
                ctx.fillStyle = option.arbitraryLineColor;
                const xWidth = option.xWidth;
                let x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                let y0 = top;
                let x1 = xWidth;
                let y1 = height;
                ctx.fillRect(x0, y0, x1, y1);
            }

            for (let i = 0; i < options.length; i++) {
                var option = options[i];
                ctx.fillStyle = option.arbitraryLineColor;
                const xWidth = option.xWidth;
                let x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                let y0 = top;
                let x1 = xWidth;
                let y1 = height;

                ctx.fillStyle = 'white';
                ctx.font = '14px arial';
                ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
            }

            ctx.restore();
        }
    };
}

function barAvatarPlugin() {
    const barAvatar = {
        id: 'barAvatar',
        afterDatasetDraw(chart, args, options) {
            const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

            ctx.save();

            for (let i = 0; i < options.length; i++) {
                var option = options[i];
                // const xWidth = option.xWidth;
                let barWidth = chart.getDatasetMeta(1).data[0]._model.width;
                let x0 = x.getPixelForValue(option.xPosition) - (barWidth / 2);
                let y0 = y.getPixelForValue(option.yPosition);
                let x1 = barWidth;
                let y1 = height;

                let img1 = new Image();
                img1.src = options.image;
                ctx.drawImage(img1, x0, y0, x1, y1);
            }

        }
    }
}

