const fillPatternCache = new WeakMap();

function createFillPattern(chart) {
    let pattern = fillPatternCache.get(chart);
    if (pattern) {
        return pattern;
    }

    const patternCanvas = document.createElement('canvas');
    patternCanvas.width = 8;
    patternCanvas.height = 8;

    const patternContext = patternCanvas.getContext('2d');
    patternContext.fillStyle = '#2563eb';
    patternContext.fillRect(0, 0, 8, 8);
    patternContext.fillStyle = '#facc15';
    patternContext.fillRect(0, 0, 4, 4);
    patternContext.fillRect(4, 4, 4, 4);

    pattern = chart.ctx.createPattern(patternCanvas, 'repeat');
    fillPatternCache.set(chart, pattern);

    return pattern;
}

const callbacks = Object.assign(Object.create(null), {
    formatPercent(value, context) {
        return `${Math.round(value * 100)}%`;
    },
    formatCurrency(value) {
        return `$${value}`;
    },
    formatTooltipLabel(context) {
        return `tooltip:${context.raw}`;
    },
    showLegendItem() {
        return true;
    },
    labelWithIndex(value, context) {
        return `${context.dataIndex}: ${value}`;
    },
    createRepeatFillPattern(context) {
        return createFillPattern(context.chart);
    }
});

export const chartJsCallbacks = Object.freeze(callbacks);
