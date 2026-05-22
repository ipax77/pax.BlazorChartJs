function getChart(chartId) {
    return globalThis.Chart?.getChart(chartId);
}

export function triggerHover(chartId) {
    const chart = getChart(chartId);
    if (!chart) {
        return;
    }

    if (chart.getActiveElements().length > 0) {
        chart.setActiveElements([]);
    } else {
        chart.setActiveElements([
            { datasetIndex: 0, index: 0 },
            { datasetIndex: 1, index: 0 }
        ]);
    }

    chart.update();
}

export function triggerTooltip(chartId) {
    const chart = getChart(chartId);
    const tooltip = chart?.tooltip;
    if (!chart || !tooltip) {
        return;
    }

    if (tooltip.getActiveElements().length > 0) {
        tooltip.setActiveElements([], { x: 0, y: 0 });
    } else {
        const { chartArea } = chart;
        tooltip.setActiveElements([
            { datasetIndex: 0, index: 2 },
            { datasetIndex: 1, index: 2 }
        ], {
            x: (chartArea.left + chartArea.right) / 2,
            y: (chartArea.top + chartArea.bottom) / 2
        });
    }

    chart.update();
}
