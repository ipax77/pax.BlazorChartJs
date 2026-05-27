let latestValueLabelRegistered = false;

const latestValueLabelPlugin = Object.freeze({
    id: 'latestValueLabel',
    afterDatasetsDraw(chart, _args, options) {
        if (options?.display !== true) {
            return;
        }

        const meta = chart.getDatasetMeta(0);
        const dataset = chart.data.datasets[0];
        const lastIndex = dataset?.data?.length - 1;
        const lastBar = lastIndex >= 0 ? meta?.data?.[lastIndex] : undefined;
        if (!lastBar) {
            return;
        }

        const ctx = chart.ctx;
        ctx.save();
        ctx.font = options.font ?? '12px sans-serif';
        ctx.fillStyle = options.color ?? '#111827';
        ctx.textBaseline = 'middle';
        ctx.fillText(options.text ?? 'Latest', lastBar.x + 12, lastBar.y);
        ctx.restore();
    }
});

export function registerLatestValueLabelPlugin() {
    if (latestValueLabelRegistered) {
        return true;
    }

    if (typeof Chart === 'undefined') {
        return false;
    }

    Chart.register(latestValueLabelPlugin);
    latestValueLabelRegistered = true;
    return true;
}
