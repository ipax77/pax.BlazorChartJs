
namespace ChartJsInterop {
    declare var dotnetRefs: Array<object>;
    declare var chartJsInitialized: boolean;

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

    export class ChartJsFunctions {
        private lock: AsyncLock;

        constructor() {
            dotnetRefs = [];
            chartJsInitialized = false;
            this.lock = new AsyncLock();
        }

        async InitChart(setupOptions: JSON, chartId: string, dotnetConfig: JSON, dotnetRef: object) {
            await this.lock.promise;
            this.lock.enable();
            
            try {
                let Chart;
                if (!chartJsInitialized) {
                    if (setupOptions['chartJsLocation']) {
                        Chart = await import(setupOptions['chartJsLocation']);
                    } else {
                        Chart = await import('./chart.min.js');
                    }
                }

                dotnetRefs[chartId] = dotnetRef;

                let plugins: Array<object>;
                plugins = await this.LoadPlugins(dotnetConfig['options'])

                const config = {
                    type: dotnetConfig["type"],
                    data: dotnetConfig["data"],
                    options: dotnetConfig["options"] ?? {},
                    plugins: plugins
                }

                const existingChart = Chart.getChart(chartId);
                if (existingChart)
                {
                    existingChart.destory();
                }

                const ctx = (<HTMLCanvasElement>document.getElementById(chartId)).getContext('2d');
                const chart = new Chart(ctx, config);
                

            } finally {
                this.lock.disable();
            }
        }

        private async LoadPlugins(options: JSON): Promise<object[]> {
            let plugins: Array<object> = [];

            if (options == undefined) {
                return plugins;
            }

            if (options['plugins']) {
                if (options['plugins']['arbitraryLines']) {
                    plugins.push(this.GetArbitaryLinesPlugin());
                }
            }
            return plugins;
        }

        private GetArbitaryLinesPlugin(): object {
            return {
                id: 'arbitraryLines',
                // beforeDraw(chart, args, options) {
                afterDraw(chart, args, options) {
                    const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

                    ctx.save();

                    for (let i = 0; i < options.length; i++) {
                        var option = options[i];
                        ctx.fillStyle = option.arbitraryLineColor;
                        const xWidth = option.xWidth;
                        let x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                        let y0 = top;
                        let x1 = xWidth;
                        let y1 = height;
                        ctx.fillRect(x0, y0, x1, y1);
                    }

                    for (let i = 0; i < options.length; i++) {
                        var option = options[i];
                        ctx.fillStyle = option.arbitraryLineColor;
                        const xWidth = option.xWidth;
                        let x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                        let y0 = top;
                        let x1 = xWidth;
                        let y1 = height;

                        ctx.fillStyle = 'white';
                        ctx.font = '14px arial';
                        ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
                    }

                    ctx.restore();
                }
            };
        }
    }

    export function Load(): void {
        window['chartJsFunctions'] = new ChartJsFunctions();
    }

    export async function initChart(setupOptions: JSON, chartId: string, dotnetConfig: JSON, dotnetRef: object) {
        await window['chartJsFunctions'].InitChart(setupOptions, chartId, dotnetConfig, dotnetRef);
    }
}

ChartJsInterop.Load();

export function initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
    ChartJsInterop.initChart(setupOptions, chartId, dotnetConfig, dotnetRef);
}