// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

// import * as Chart from './dist/chart.js';
// import Chart from './dist/chart.js/auto';
// import { Chart, registerables } from './chart.min.js';
import './chart.min.js';



export function showChart(chartId, config) {

    if (window.charts == undefined) {
        window.charts = {};
    }

    console.log("indahouse1");
    const ctx = document.getElementById(chartId).getContext('2d');

    // const chartJson = JSON.parse(config);
    window.charts[chartId] = new Chart(ctx, config);

    //const myChart = new Chart(ctx, {
    //    type: 'bar',
    //    data: {
    //        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
    //        datasets: [{
    //            label: '# of Votes',
    //            data: [12, 19, 3, 5, 2, 3],
    //            backgroundColor: [
    //                'rgba(255, 99, 132, 0.2)',
    //                'rgba(54, 162, 235, 0.2)',
    //                'rgba(255, 206, 86, 0.2)',
    //                'rgba(75, 192, 192, 0.2)',
    //                'rgba(153, 102, 255, 0.2)',
    //                'rgba(255, 159, 64, 0.2)'
    //            ],
    //            borderColor: [
    //                'rgba(255, 99, 132, 1)',
    //                'rgba(54, 162, 235, 1)',
    //                'rgba(255, 206, 86, 1)',
    //                'rgba(75, 192, 192, 1)',
    //                'rgba(153, 102, 255, 1)',
    //                'rgba(255, 159, 64, 1)'
    //            ],
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        scales: {
    //            y: {
    //                beginAtZero: true
    //            }
    //        }
    //    }
    //});

    console.log("indahouse3");
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