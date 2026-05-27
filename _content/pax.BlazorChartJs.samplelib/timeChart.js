let registrationPromise;

export function registerPlugin(chartJsLocation) {
    registrationPromise ??= registerPluginCore(chartJsLocation);
    return registrationPromise;
}

async function registerPluginCore(chartJsLocation) {
    if (typeof globalThis.Chart === 'undefined' && chartJsLocation) {
        await import(chartJsLocation);
    }

    await import('./chartjs-adapter-date-fns.bundle.min.js');
}
