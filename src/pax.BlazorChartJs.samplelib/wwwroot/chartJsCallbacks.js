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

function ensureBottomTooltipPositioner() {
    if (!globalThis.Chart?.Tooltip?.positioners || globalThis.Chart.Tooltip.positioners.bottom) {
        return;
    }

    globalThis.Chart.Tooltip.positioners.bottom = function (items) {
        const pos = globalThis.Chart.Tooltip.positioners.average(items);
        if (pos === false) {
            return false;
        }

        return {
            x: pos.x,
            y: this.chart.chartArea.bottom,
            xAlign: 'center',
            yAlign: 'bottom'
        };
    };
}

function colorByDataset(datasetIndex) {
    const colors = ['#ff6384', '#36a2eb', '#ffcd56', '#4bc0c0', '#9966ff', '#ff9f40'];
    return colors[datasetIndex % colors.length];
}

function transparentizeHex(hex, alpha = 0.5) {
    const value = hex.replace('#', '');
    const r = parseInt(value.substring(0, 2), 16);
    const g = parseInt(value.substring(2, 4), 16);
    const b = parseInt(value.substring(4, 6), 16);
    return `rgba(${r}, ${g}, ${b}, ${alpha})`;
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
    defaultChartClick(event, elements, chart) {
        window.chartJsDefaultClickCount = (window.chartJsDefaultClickCount ?? 0) + 1;
        window.chartJsDefaultClickArgs = {
            eventType: event?.type ?? null,
            elements: elements?.length ?? 0,
            chartId: chart?.canvas?.id ?? null
        };

        return true;
    },
    chartEventBridgeClick(event, elements, chart) {
        window.chartJsNativeClickCount = (window.chartJsNativeClickCount ?? 0) + 1;
        window.chartJsNativeClickArgs = {
            eventType: event?.type ?? null,
            elements: elements?.length ?? 0,
            chartId: chart?.canvas?.id ?? null
        };
    },
    chartEventBridgeResize(chart, size) {
        window.chartJsNativeResizeCount = (window.chartJsNativeResizeCount ?? 0) + 1;
        window.chartJsNativeResizeArgs = {
            chartId: chart?.canvas?.id ?? null,
            width: size?.width ?? null,
            height: size?.height ?? null
        };
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
    linePointStyleTitle(context) {
        return `Point Style: ${context.chart.data.datasets[0].pointStyle}`;
    },
    lineSteppedTitle(context) {
        return `Step ${context.chart.data.datasets[0].stepped} Interpolation`;
    },
    lineSegmentBorderColor(context) {
        if (context.p0.skip || context.p1.skip) {
            return 'rgb(0,0,0,0.2)';
        }

        return context.p0.parsed.y > context.p1.parsed.y
            ? 'rgb(192,75,75)'
            : undefined;
    },
    lineSegmentBorderDash(context) {
        return context.p0.skip || context.p1.skip
            ? [6, 6]
            : undefined;
    },
    areaFillTitle(context) {
        return `Fill: ${context.chart.data.datasets[0].fill}`;
    },
    areaDrawTimeTitle(context) {
        return `drawTime: ${context.chart.options.plugins.filler.drawTime}`;
    },
    areaStackedTitle(context) {
        return `Chart.js Line Chart - stacked=${context.chart.options.scales.y.stacked}`;
    },
    timeMaxSpanMajorTickFont(context) {
        return context.tick && context.tick.major
            ? { weight: 'bold' }
            : undefined;
    },
    scaleOptionsGridColor(context) {
        if (context.tick.value > 0) {
            return '#4bc0c0';
        }

        if (context.tick.value < 0) {
            return '#ff6384';
        }

        return '#000000';
    },
    scaleOptionsTickLabel(value, index) {
        const label = this.getLabelForValue(value);
        return index % 2 === 0
            ? (Array.isArray(label) ? label : `${label}`.split('\n'))
            : '';
    },
    legendHandleHover(_event, item, legend) {
        legend.chart.data.datasets[0].backgroundColor.forEach((color, index, colors) => {
            colors[index] = index === item.index || color.length === 9 ? color : `${color}4D`;
        });
        legend.chart.update();
    },
    legendHandleLeave(_event, _item, legend) {
        legend.chart.data.datasets[0].backgroundColor.forEach((color, index, colors) => {
            colors[index] = color.length === 9 ? color.slice(0, -2) : color;
        });
        legend.chart.update();
    },
    tooltipContentFooter(items) {
        return `Sum: ${getTooltipTotal(items)}`;
    },
    tooltipPositionTitle(context) {
        ensureBottomTooltipPositioner();
        return `Tooltip position mode: ${context.chart.options.plugins.tooltip.position}`;
    },
    scriptableColor(context) {
        return colorByDataset(context.datasetIndex ?? 0);
    },
    scriptableTransparentColor(context) {
        return transparentizeHex(colorByDataset(context.datasetIndex ?? 0), 0.5);
    },
    scriptableBorderColor(context) {
        const value = Number(context.parsed?.y ?? context.raw?.v ?? context.raw ?? 0);
        return value < 0 ? '#ff6384' : '#36a2eb';
    },
    scriptablePointStyle(context) {
        return context.dataIndex % 2 === 0 ? 'circle' : 'rect';
    },
    scriptableRadius(context) {
        const value = Number(context.parsed?.y ?? context.raw?.v ?? context.raw ?? 0);
        return value < 10 ? 5 : value < 25 ? 7 : value < 50 ? 9 : value < 75 ? 11 : 15;
    },
    scriptableBubbleRadius(context) {
        return Math.abs(Number(context.raw?.v ?? context.raw?.r ?? 5));
    },
    scriptableArcColor(context) {
        return colorByDataset(context.dataIndex ?? 0);
    },
    animationDelay(context) {
        let delay = 0;
        if (context.type === 'data' && context.mode === 'default' && !context.chart.$delayStarted) {
            delay = context.dataIndex * 300 + context.datasetIndex * 100;
        }

        return delay;
    },
    animationComplete(context) {
        context.chart.$delayStarted = true;
    },
    animationDropFrom(context) {
        if (context.type === 'data' && context.mode === 'default' && !context.dropped) {
            context.dropped = true;
            return 0;
        }

        return undefined;
    },
    animationLoopRadius(context) {
        return context.active ? 12 : 6;
    },
    animationProgressiveFromNaN() {
        return NaN;
    },
    animationProgressivePreviousY(context) {
        if (context.index === 0) {
            return context.chart.scales.y.getPixelForValue(100);
        }

        return context.chart.getDatasetMeta(context.datasetIndex).data[context.index - 1].getProps(['y'], true).y;
    },
    animationProgressiveDelay(context) {
        if (context.type !== 'data' || context.xStarted) {
            return 0;
        }

        context.xStarted = true;
        return context.index * (10000 / context.chart.data.datasets[0].data.length);
    },
    animationProgressiveYDelay(context) {
        if (context.type !== 'data' || context.yStarted) {
            return 0;
        }

        context.yStarted = true;
        return context.index * (10000 / context.chart.data.datasets[0].data.length);
    },
    multiSeriesPieGenerateLabels(chart) {
        const original = Chart.overrides.pie.plugins.legend.labels.generateLabels;
        const labelsOriginal = original.call(this, chart);
        const datasetColors = chart.data.datasets.flatMap(dataset => dataset.backgroundColor);

        labelsOriginal.forEach(label => {
            label.datasetIndex = (label.index - label.index % 2) / 2;
            label.hidden = !chart.isDatasetVisible(label.datasetIndex);
            label.fillStyle = datasetColors[label.index];
        });

        return labelsOriginal;
    },
    multiSeriesPieLegendClick(mouseEvent, legendItem, legend) {
        const meta = legend.chart.getDatasetMeta(legendItem.datasetIndex);
        meta.hidden = legend.chart.isDatasetVisible(legendItem.datasetIndex);
        legend.chart.update();
    },
    multiSeriesPieTooltipTitle(context) {
        const first = context?.[0];
        if (!first) {
            return '';
        }

        const labelIndex = first.datasetIndex * 2 + first.dataIndex;
        return `${first.chart.data.labels[labelIndex]}: ${first.formattedValue}`;
    },
    responsiveLatestLabelPadding(context) {
        return context.chart.width < 480
            ? latestLabelPaddingSmall
            : latestLabelPaddingLarge;
    }
});

export const chartJsCallbacks = Object.freeze(callbacks);
