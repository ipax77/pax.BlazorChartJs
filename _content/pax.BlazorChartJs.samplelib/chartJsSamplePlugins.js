const registeredPlugins = new Set();

const samplePlugins = Object.freeze({
    chartAreaBorder: Object.freeze({
        id: 'chartAreaBorder',
        beforeDraw(chart, _args, options) {
            if (!options?.borderColor || options.borderWidth === undefined) {
                return;
            }

            const { ctx, chartArea: { left, top, width, height } } = chart;
            ctx.save();
            ctx.strokeStyle = options.borderColor;
            ctx.lineWidth = options.borderWidth;
            ctx.setLineDash(options.borderDash || []);
            ctx.lineDashOffset = options.borderDashOffset;
            ctx.strokeRect(left, top, width, height);
            ctx.restore();
        }
    }),
    emptyDoughnut: Object.freeze({
        id: 'emptyDoughnut',
        afterDraw(chart, _args, options) {
            if (chart.config.type !== 'doughnut'
                || (!options?.color && options?.width === undefined && options?.radiusDecrease === undefined)) {
                return;
            }

            const { datasets } = chart.data;
            let hasData = false;

            for (let i = 0; i < datasets.length; i += 1) {
                hasData |= datasets[i].data.length > 0;
            }

            if (hasData) {
                return;
            }

            const { chartArea: { left, top, right, bottom }, ctx } = chart;
            const centerX = (left + right) / 2;
            const centerY = (top + bottom) / 2;
            const radius = Math.min(right - left, bottom - top) / 2;
            ctx.beginPath();
            ctx.lineWidth = options.width || 2;
            ctx.strokeStyle = options.color || 'rgba(255, 128, 0, 0.5)';
            ctx.arc(centerX, centerY, radius - (options.radiusDecrease || 0), 0, 2 * Math.PI);
            ctx.stroke();
        }
    }),
    quadrants: Object.freeze({
        id: 'quadrants',
        beforeDraw(chart, _args, options) {
            const { ctx, chartArea: { left, top, right, bottom }, scales: { x, y } } = chart;
            if (!x
                || !y
                || !options?.topLeft
                || !options.topRight
                || !options.bottomRight
                || !options.bottomLeft) {
                return;
            }

            const midX = x.getPixelForValue(0);
            const midY = y.getPixelForValue(0);
            ctx.save();
            ctx.fillStyle = options.topLeft;
            ctx.fillRect(left, top, midX - left, midY - top);
            ctx.fillStyle = options.topRight;
            ctx.fillRect(midX, top, right - midX, midY - top);
            ctx.fillStyle = options.bottomRight;
            ctx.fillRect(midX, midY, right - midX, bottom - midY);
            ctx.fillStyle = options.bottomLeft;
            ctx.fillRect(left, midY, midX - left, bottom - midY);
            ctx.restore();
        }
    })
});

export async function registerPlugin(chartJsLocation, pluginId) {
    const plugin = samplePlugins[pluginId];
    if (!plugin) {
        return false;
    }

    const chartJs = await import(chartJsLocation);
    const Chart = chartJs.Chart ?? chartJs.default ?? globalThis.Chart;
    if (registeredPlugins.has(pluginId) || Chart.registry.plugins.get(pluginId)) {
        registeredPlugins.add(pluginId);
        return true;
    }

    Chart.register(plugin);
    registeredPlugins.add(pluginId);
    return true;
}
