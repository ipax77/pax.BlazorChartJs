const fillPatternCache = new WeakMap();
const externalTooltipCache = new WeakMap();
const latestLabelPaddingSmall = Object.freeze({ top: 20, left: 8, bottom: 8, right: 70 });
const latestLabelPaddingLarge = Object.freeze({ top: 20, left: 8, bottom: 8, right: 30 });

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

function getTooltipRawValue(item) {
    return Number(item?.raw ?? item?.parsed?.y ?? 0);
}

function getTooltipTotal(items) {
    return items.reduce((total, item) => total + getTooltipRawValue(item), 0);
}

function getCustomTooltipPoint(context) {
    return context?.dataset?.tooltipPoints?.[context.dataIndex];
}

function getOrCreateExternalTooltip(chart) {
    let tooltipElement = externalTooltipCache.get(chart);
    if (tooltipElement) {
        return tooltipElement;
    }

    tooltipElement = document.createElement('div');
    tooltipElement.className = 'chartjs-external-tooltip';
    tooltipElement.style.background = '#111827';
    tooltipElement.style.border = '1px solid #38bdf8';
    tooltipElement.style.borderRadius = '4px';
    tooltipElement.style.color = '#f8fafc';
    tooltipElement.style.font = '12px sans-serif';
    tooltipElement.style.opacity = '0';
    tooltipElement.style.padding = '8px 10px';
    tooltipElement.style.pointerEvents = 'none';
    tooltipElement.style.position = 'absolute';
    tooltipElement.style.transform = 'translate(-50%, calc(-100% - 10px))';
    tooltipElement.style.transition = 'opacity 120ms ease';
    tooltipElement.style.whiteSpace = 'nowrap';
    tooltipElement.style.zIndex = '10';

    const parent = chart.canvas.parentNode;
    if (parent instanceof HTMLElement) {
        if (getComputedStyle(parent).position === 'static') {
            parent.style.position = 'relative';
        }
        parent.appendChild(tooltipElement);
    }

    externalTooltipCache.set(chart, tooltipElement);
    return tooltipElement;
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
    updateDatasetColor(context) {
        return context.dataIndex === 0 ? '#dc2626' : '#fca5a5';
    },
    updateDatasetsColor(context) {
        return context.dataIndex === 0 ? '#16a34a' : '#86efac';
    },
    updateDatasetSmoothColor(context) {
        return context.dataIndex === 0 ? '#7c3aed' : '#c4b5fd';
    },
    labelWithIndex(value, context) {
        return `${context.dataIndex}: ${value}`;
    },
    formatTooltipContentTitle(items) {
        const first = items?.[0];
        return `Sales ${first?.label ?? ''}`;
    },
    formatTooltipColorTitle(items) {
        const first = items?.[0];
        return `Color ${first?.label ?? ''}`;
    },
    formatTooltipPointTitle(items) {
        const first = items?.[0];
        return `Point ${first?.label ?? ''}`;
    },
    formatTooltipExternalTitle(items) {
        const first = items?.[0];
        return `External ${first?.label ?? ''}`;
    },
    formatTooltipCustomDataTitle(items) {
        const first = items?.[0];
        return `Synergy ${first?.dataset?.label ?? ''} ${first?.label ?? ''}`;
    },
    formatTooltipContentLabel(context) {
        return `${context.dataset.label}: ${context.formattedValue ?? context.raw} units`;
    },
    formatTooltipCustomDataLabel(context) {
        const point = getCustomTooltipPoint(context);
        return point?.text ?? `${context.dataset.label}: ${context.formattedValue ?? context.raw}`;
    },
    formatTooltipContentFooter(items) {
        return `Total: ${getTooltipTotal(items)} units`;
    },
    tooltipLabelColor(context) {
        const backgroundColor = context.datasetIndex === 0 ? '#60a5fa' : '#fdba74';
        const borderColor = context.datasetIndex === 0 ? '#1d4ed8' : '#c2410c';

        return {
            backgroundColor,
            borderColor,
            borderWidth: 2,
            borderRadius: 2
        };
    },
    tooltipLabelTextColor(context) {
        return context.datasetIndex === 0 ? '#1e3a8a' : '#7c2d12';
    },
    tooltipLabelPointStyle(context) {
        return {
            pointStyle: context.datasetIndex === 0 ? 'triangle' : 'rectRounded',
            rotation: context.datasetIndex === 0 ? 0 : 45
        };
    },
    renderExternalTooltip(context) {
        const { chart, tooltip } = context;
        const tooltipElement = getOrCreateExternalTooltip(chart);

        if (!tooltip || tooltip.opacity === 0) {
            tooltipElement.style.opacity = '0';
            return;
        }

        const title = tooltip.title?.join(' ') ?? '';
        const body = tooltip.body?.map((bodyItem) => bodyItem.lines.join(' ')).join(' | ') ?? '';
        const footer = tooltip.footer?.join(' ') ?? '';

        tooltipElement.textContent = [title, body, footer].filter(Boolean).join(' - ');
        tooltipElement.style.left = `${tooltip.caretX}px`;
        tooltipElement.style.top = `${tooltip.caretY}px`;
        tooltipElement.style.opacity = '1';
    },
    createRepeatFillPattern(context) {
        return createFillPattern(context.chart);
    },
    responsiveLatestLabelPadding(context) {
        return context.chart.width < 480
            ? latestLabelPaddingSmall
            : latestLabelPaddingLarge;
    }
});

export const chartJsCallbacks = Object.freeze(callbacks);
