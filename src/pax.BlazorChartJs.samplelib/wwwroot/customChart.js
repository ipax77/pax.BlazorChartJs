
export function setCustomChart(chartId) {
    const chart = Chart.getChart(chartId);
    if (chart != undefined) {
        var img = new Image();
        img.src = "./images/alarak-min.png";
        img.onload = function () {
            const ctx = document.getElementById(chartId).getContext('2d');
            const fillPattern = ctx.createPattern(img, 'repeat');

            chart.data.datasets[0].backgroundColor = fillPattern;
            chart.data.datasets[0].borderColor = "darkblue";
            chart.update();
        };
    }
}