/// <reference path="./types/index.esm.d.ts" />   

// declare function require(
//     moduleNames: string[],
//     onLoad: (...args: any[]) => void
// ): void;


import { Chart as TsChart } from "./types/index.esm";

// todo: this locks only the first call but might be good enough?
class AsyncLock {
    disable: Function;
    promise: Promise<void>;

    constructor() {
        this.disable = () => { }
        this.promise = Promise.resolve()
    }

    enable() {
        this.promise = new Promise(resolve => this.disable = resolve)
    }
}

class LoadInfo {
    chartJsLoaded: boolean = false;
    chartJsDatalabels: boolean = false;
    chartJsLabels: boolean = false;
}

class ChartJsInterop {
    lock = new AsyncLock();
    dotnetRefs = new Map<string, any>();
    loadInfo = new LoadInfo();

    public async initChart(setupOptions: JSON, chartId: string, dotnetConfig: JSON, dotnetRef: any) {
        const chartJsPath = setupOptions['ChartJsLocation'] == undefined ?
            './chart.min.js'
            : setupOptions['ChartJsLocation'];

        await this.require([chartJsPath], async (Chart: typeof TsChart) => {
            console.log("indahouse");
            await this.lock.promise;
            this.lock.enable();

            try {
                console.log("Und es war Sommer");

                // await this.loadModules(["./chart.min.js"]);

                this.dotnetRefs.set(chartId, dotnetRef);

                const plugins = [];

                const config = {
                    type: dotnetConfig['type'],
                    data: dotnetConfig['data'],
                    options: dotnetConfig['options'] ?? {},
                    plugins: plugins
                }

                var oldChart = Chart.getChart(chartId);
                if (oldChart != undefined) {
                    oldChart.destroy();
                }

                const ctx = (<HTMLCanvasElement>document.getElementById(chartId)).getContext('2d');
                const chart = new Chart(ctx, config);

            } catch {

            } finally {
                this.lock.disable();
            }
        });
    }

    private async require(
        moduleNames: string[],
        onLoad: (...args: any[]) => void
    ): Promise<void> {
        for (let i = 0; i < moduleNames.length; i++) {
            await import(moduleNames[i]);
        }
        onLoad();
    }

    private async loadModules(moduleNames: string[]) {
        console.log("loading chartJs...");
        for (let i = 0; i < moduleNames.length; i++) {
            await import(moduleNames[i]);
        }
    }
}

window[ChartJsInterop.name] = new ChartJsInterop();

export async function initChart(setupOptions: JSON, chartId: string, dotnetConfig: JSON, dotnetRef: any) {
    await window[ChartJsInterop.name].initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
}