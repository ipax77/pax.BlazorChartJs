import { chartJsInterop } from "./chartInteropState";
import type { ChartJsCallback, ChartJsCallbackRegistry } from "./types";

const chartJsFunctionMarker = "__chartJsFunction";
const chartJsFunctionNamePattern = /^[A-Za-z_$][A-Za-z0-9_$]*$/;
const reservedChartJsFunctionNames = new Set([
    "__proto__",
    "constructor",
    "prototype",
    "toString",
    "valueOf",
    "hasOwnProperty"
]);

function objectHasOwn(value: any, key: string): boolean {
    const hasOwn = (Object as any).hasOwn;
    return typeof hasOwn === "function"
        ? hasOwn(value, key)
        : Object.prototype.hasOwnProperty.call(value, key);
}

function describeChartJsFunctionName(name: any): string {
    if (typeof name === "string") {
        return name;
    }

    if (name === null) {
        return "null";
    }

    return typeof name;
}

export function validateChartJsFunctionName(name: any): asserts name is string {
    if (typeof name !== "string" || !chartJsFunctionNamePattern.test(name)) {
        throw new Error(`Invalid Chart.js callback name: ${describeChartJsFunctionName(name)}. Names must match ${chartJsFunctionNamePattern}.`);
    }

    if (reservedChartJsFunctionNames.has(name)) {
        throw new Error(`Reserved Chart.js callback name: ${name}`);
    }
}

function createChartJsCallbackRegistry(moduleLocation: string, callbacks: any): ChartJsCallbackRegistry {
    if (callbacks == undefined || typeof callbacks !== "object") {
        throw new Error(`Chart.js callback module '${moduleLocation}' must export a chartJsCallbacks object.`);
    }

    const registry = Object.create(null) as Record<string, ChartJsCallback>;
    const callbackNames = Object.keys(callbacks);
    for (let i = 0; i < callbackNames.length; i++) {
        const callbackName = callbackNames[i];
        validateChartJsFunctionName(callbackName);

        const callback = callbacks[callbackName];
        if (typeof callback !== "function") {
            throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
        }

        registry[callbackName] = callback;
    }

    return Object.freeze(registry);
}

async function getChartJsCallbackRegistry(moduleLocation: string): Promise<ChartJsCallbackRegistry> {
    let registryPromise = chartJsInterop.chartJsCallbackRegistryPromises.get(moduleLocation);
    if (!registryPromise) {
        registryPromise = import(moduleLocation)
            .then((module) => {
                const callbacks = module?.chartJsCallbacks;
                return createChartJsCallbackRegistry(moduleLocation, callbacks);
            })
            .catch((error) => {
                chartJsInterop.chartJsCallbackRegistryPromises.delete(moduleLocation);
                throw error;
            });

        chartJsInterop.chartJsCallbackRegistryPromises.set(moduleLocation, registryPromise);
    }

    return await registryPromise;
}

async function getChartJsCallbackRegistryIfConfigured(setupOptions: any): Promise<ChartJsCallbackRegistry | undefined> {
    const moduleLocation = setupOptions?.chartJsCallbacksModuleLocation;
    if (typeof moduleLocation !== "string" || moduleLocation.length === 0) {
        return undefined;
    }

    return await getChartJsCallbackRegistry(moduleLocation);
}

export async function resolveChartJsFunctions(setupOptions: any, config: any, hasChartJsFunctions: boolean): Promise<void> {
    if (hasChartJsFunctions !== true) {
        return;
    }

    const callbacks = await getChartJsCallbackRegistryIfConfigured(setupOptions);
    reviveChartJsFunctions(config, callbacks, "$", null, null, null);
}

function shouldSkipChartJsFunctionValue(
    key: string | null,
    parentKey: string | null,
    grandparentKey: string | null
): boolean {
    return (grandparentKey === "datasets" && key === "data")
        || (parentKey === "data" && key === "labels");
}

function resolveChartJsFunction(reference: any, callbacks: ChartJsCallbackRegistry, path: string): ChartJsCallback {
    const callbackName = reference[chartJsFunctionMarker];
    if (typeof callbackName !== "string" || callbackName.length === 0) {
        throw new Error(`Invalid Chart.js callback marker at ${path}.`);
    }

    validateChartJsFunctionName(callbackName);

    if (!objectHasOwn(callbacks, callbackName)) {
        throw new Error(`Unknown Chart.js callback '${callbackName}' at ${path}.`);
    }

    const callback = callbacks[callbackName];
    if (typeof callback !== "function") {
        throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
    }

    return callback;
}

export function reviveChartJsFunctions(
    value: any,
    callbacks: ChartJsCallbackRegistry | undefined,
    path: string,
    key: string | null,
    parentKey: string | null,
    grandparentKey: string | null
): any {
    if (value == null || typeof value !== "object") {
        return value;
    }

    if (shouldSkipChartJsFunctionValue(key, parentKey, grandparentKey)) {
        return value;
    }

    if (Array.isArray(value)) {
        for (let i = 0; i < value.length; i++) {
            value[i] = reviveChartJsFunctions(value[i], callbacks, `${path}[${i}]`, String(i), key, parentKey);
        }

        return value;
    }

    if (isChartJsFunctionMarker(value, path)) {
        if (callbacks == undefined) {
            throw new Error(`Chart.js callback marker at ${path} requires ChartJsSetupOptions.ChartJsCallbacksModuleLocation.`);
        }

        return resolveChartJsFunction(value, callbacks, path);
    }

    const keys = Object.keys(value);
    for (let i = 0; i < keys.length; i++) {
        const childKey = keys[i];
        value[childKey] = reviveChartJsFunctions(value[childKey], callbacks, `${path}.${childKey}`, childKey, key, parentKey);
    }

    return value;
}

function isChartJsFunctionMarker(value: any, path: string): boolean {
    if (value == null
        || typeof value !== "object"
        || Array.isArray(value)
        || !objectHasOwn(value, chartJsFunctionMarker)) {
        return false;
    }

    const keys = Object.keys(value);
    if (keys.length !== 1) {
        throw new Error(`Invalid Chart.js callback marker at ${path}: '${chartJsFunctionMarker}' marker must not contain other properties.`);
    }

    return true;
}
