// v1.1

window.Prism = window.Prism || {};
Prism.manual = true;
Prism.disableWorkerMessageHandler = true;

function getChartVersion() {
    if (Chart !== undefined) {
        return Chart.version;
    } else {
        return "0.0"
    }
}

function highlightCode() {

    Prism.highlightAll();
}