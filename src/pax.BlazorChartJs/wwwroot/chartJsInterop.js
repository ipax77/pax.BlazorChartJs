// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

// import * as Chart from './dist/chart.js';
// import Chart from './dist/chart.js/auto';
// import { Chart, registerables } from './chart.min.js';
import './chart.min.js';

export function initChart(chartId, dotnetConfig, dotnetRef)
{
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

    let config;
    if (dotnetConfig.options != undefined
        && dotnetConfig.options.plugins != undefined
        && dotnetConfig.options.plugins.arbitraryLines != undefined) {
        const arbitraryLines = arbitaryLinesPlugin();

        config = {
            type: dotnetConfig.type,
            data: dotnetConfig.data,
            options: dotnetConfig.options,
            plugins: [arbitraryLines]
        }
    }
    else {
        config = {
            type: dotnetConfig.type,
            data: dotnetConfig.data,
            options: dotnetConfig.options
        }        
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
        if (window.dotnetrefs[chartid])
        {
            await window.dotnetrefs[chartid].invokeMethodAsync("ChartClicked", label);
        }
    }

    if (window.charts[chartId]) {
        window.charts[chartId].destroy();
    }

    const ctx = document.getElementById(chartId).getContext('2d');
    window.charts[chartId] = new Chart(ctx, config);
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