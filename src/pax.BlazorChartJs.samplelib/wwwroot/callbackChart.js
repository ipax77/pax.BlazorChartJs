
export function setTickCallback(chartId) {
    const chart = Chart.getChart(chartId);

    if (chart == undefined) {
        return;
    }

    chart.options.scales.y.ticks.callback = function(value, index, ticks) {
        return '$' + value;
    }

    chart.update();
}