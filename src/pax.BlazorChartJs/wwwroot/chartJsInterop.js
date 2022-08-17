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

        config.options.onClick = (e) => {
            const points = window.charts[chartId].getElementsAtEventForMode(e, 'nearest', { intersect: true }, true);

            if (points.length) {
                const firstPoint = points[0];
                const label = window.charts[chartId].data.labels[firstPoint.index];
                const value = window.charts[chartId].data.datasets[firstPoint.datasetIndex].data[firstPoint.index];
                reportChartClick(chartId, label);
            }
        }

        async function reportChartClick(chartid, label) {
            if (window.dotnetrefs[chartid]) {
                await window.dotnetrefs[chartid].invokeMethodAsync("ChartClicked", label);
            }
        }

        if (window.charts[chartId]) {
            window.charts[chartId].destroy();
        }

        const ctx = document.getElementById(chartId).getContext('2d');
        window.charts[chartId] = new Chart(ctx, config);
    } catch { }
    finally {
        lock.disable();
    }
}

export function updateChartOptions(chartId, options) {
    if (window.charts[chartId]) {
        window.charts[chartId].options = options;
        window.charts[chartId].update();
    }
}

export function addDataToDataset(chartId, datasetId, data) {
    // var datasetIndex = (window.charts[chartId].data).map(object => object.id).indexOf(datasetId);

    //var datasetIndex = window.charts[chartId].data.findIndex(function (item, i) {
    //    return item.id === datasetId;
    //});

    var datasetMetas = window.charts[chartId].getSortedVisibleDatasetMetas();

    var datasetIndex = datasetMetas.findIndex(obj => obj._dataset.id === datasetId);
    console.log(datasetIndex);

    window.charts[chartId].data.datasets[datasetIndex].data.push(77);
    window.charts[chartId].data.datasets[datasetIndex].backgroundColor.push('rgba(255, 206, 86, 1)');
    window.charts[chartId].data.datasets[datasetIndex].borderColor.push('rgba(255, 206, 86, 1)');

    window.charts[chartId].data.labels.push(data);
    window.charts[chartId].update();
}

export function removeDataset() {

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

