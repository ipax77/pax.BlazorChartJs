export const chartJsCallbacks = {
    formatPercent(value, context) {
        return `${Math.round(value * 100)}%`;
    }
};
