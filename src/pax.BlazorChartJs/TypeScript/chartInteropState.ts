import type { ChartInitResult, ChartInstance, ChartJsCallbackRegistry } from "./types";

export class ChartJsInteropState {
    public dotnetRefs = new Map<string, any>();
    public charts = new Map<string, ChartInstance>();
    public chartInitPromises = new Map<string, Promise<ChartInitResult>>();
    public chartJsCallbackRegistryPromises = new Map<string, Promise<ChartJsCallbackRegistry>>();
    public appliedDefaultsKey: string | null = null;
}

export const chartJsInterop = new ChartJsInteropState();
