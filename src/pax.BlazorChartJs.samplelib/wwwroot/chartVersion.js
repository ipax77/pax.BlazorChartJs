
function getChartVersion() {
    if (Chart !== undefined) {
        return Chart.version;
    } else {
        return "0.0"
    }
}
