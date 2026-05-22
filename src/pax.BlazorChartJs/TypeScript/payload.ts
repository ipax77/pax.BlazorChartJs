export function parsePayload<T = any>(value: T | string | null | undefined): T | null | undefined {
    if (value == undefined || typeof value !== "string") {
        return value as T | null | undefined;
    }

    return JSON.parse(value) as T;
}

export function parseArrayPayload<T = any>(value: T[] | string | null | undefined): T[] | null | undefined {
    return parsePayload<T[]>(value);
}

export function isSetupOptions(value: any): boolean {
    return value != undefined
        && typeof value === "object"
        && !Array.isArray(value)
        && ("chartJsLocation" in value
            || "chartJsPluginLabelsLocation" in value
            || "chartJsPluginDatalabelsLocation" in value
            || "chartJsCallbacksModuleLocation" in value
            || "defaults" in value);
}