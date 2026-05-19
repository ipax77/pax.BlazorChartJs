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
    labelWithIndex(value, context) {
        return `${context.dataIndex}: ${value}`;
    }
});

export const chartJsCallbacks = Object.freeze(callbacks);
