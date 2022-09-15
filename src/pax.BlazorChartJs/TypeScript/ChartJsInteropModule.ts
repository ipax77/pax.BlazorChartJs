/// <reference path="./types/index.esm.d.ts" />   

import { Chart, Chart as TsChart, ChartDataset } from "./types/index.esm";

class LoadInfo {
    chartJsLoaded: boolean = false;
    chartJsDatalabels: boolean = false;
    chartJsLabels: boolean = false;
}

class ChartJsInterop {
    dotnetRefs = new Map<string, any>();
    loadInfo = new LoadInfo();

    public async initChart(setupOptions: JSON, chartId: string, dotnetConfig: JSON, dotnetRef: any): Promise<JSON> {
        this.dotnetRefs.set(chartId, dotnetRef);

        const config: any = {
            'type': dotnetConfig['type'],
            data: dotnetConfig['data'],
            options: dotnetConfig['options'] ?? {},
            plugins: await this.loadPlugins(setupOptions, dotnetConfig)
        }
        return <JSON>config;
    }

    private async loadPlugins(setupOptions: JSON, dotnetConfig: JSON): Promise<Array<object>> {
        let plugins = []

        if (dotnetConfig['options'] != undefined
            && dotnetConfig['options'].plugins != undefined) {

            if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
                const arbitraryLines = arbitaryLinesPlugin();
                plugins.push(arbitraryLines);
            }

            if (dotnetConfig['options'].plugins.labels != undefined) {
                if (setupOptions?.['chartJsPluginLabelsLocation']) {
                    await import(setupOptions['chartJsPluginLabelsLocation']);
                }
                else {
                    await import('./chartjs-plugin-labels.min.js');
                }
            }

            if (dotnetConfig['options'].plugins.datalabels != undefined) {
                if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                    const ChartDataLabels = await import(setupOptions['chartJsPluginDatalabelsLocation']);
                    plugins.push(ChartDataLabels);
                } else {
                    const ChartDataLabels = await import('./chartjs-plugin-datalabels.min.js');
                    plugins.push(ChartDataLabels);
                }
            }
        }

        return plugins;
    }

    public async triggerEvent(chartId: string, event: string, source: string, data: any) {
        if (this.dotnetRefs.has(chartId)) {
            await this.dotnetRefs.get(chartId).invokeMethodAsync("EventTriggered", event, source, data);
        }
    }

    public addData(chart: Chart, label: string, pos: number, datas: Array<JSON>) {

        if (chart == undefined)
        {
            return;
        }

        this.addLabel(chart, label, pos);
        
        chart.data.datasets.forEach(dataset => {
            if (datas[dataset['id']] != undefined) {
                let addData = datas[dataset['id']];
                this.addDatasetData(dataset, addData['data'], addData['atPosition']);

                if (addData['backgroundColor'] != undefined) {
                    this.addBackgroundColor(dataset, addData['backgroundColor'], addData['atPosition']);
                }
    
                if (addData['borderColor'] != undefined) {
                    this.addBorderColor(dataset, addData['borderColor'], addData['atPosition']);
                }
            }
        });
        chart.update();
    }

    private addLabel(chart: Chart, label: string, pos: number) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            } else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }
    
    private addDatasetData(dataset: ChartDataset, data: any, pos: number) {
        if (pos == undefined) {
            dataset.data.push(data);
        } else {
            dataset.data.splice(pos, 0, data);
        }
    }
    
    private addBackgroundColor(dataset: ChartDataset, backgroundColor: string, pos: number) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            } else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }
    
    private addBorderColor(dataset: ChartDataset, borderColor: string, pos: number) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            } else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }

    public removeData(chart: Chart) {

        if (!(chart.data.labels.length == 0)) {
            chart.data.labels.pop();
        }

        chart.data.datasets.forEach(dataset => {
            if (!(dataset.data.length == 0)) {
                dataset.data.pop();
            }

            if (Array.isArray(dataset.backgroundColor) 
                && !(dataset.backgroundColor.length == 0)) {
                dataset.backgroundColor.pop();
            }

            if (Array.isArray(dataset.borderColor)
                && !(dataset.borderColor.length == 0)) {
                dataset.borderColor.pop();
            }
        });
        chart.update();
    }

    public setData(chart: Chart, labels: Array<string>, datas: Array<JSON>) {
        if (labels != undefined) {
            chart.data.labels = labels;
        }

        chart.data.datasets.forEach(dataset => {
            if (datas[dataset['id']] != undefined) {
                let addData = datas[dataset['id']];

                dataset.data = addData['data'];

                if (addData['backgroundColor'] != undefined) {
                    dataset.backgroundColor = addData['backgroundColor'];
                }
    
                if (addData['borderColor'] != undefined) {
                    dataset.borderColor = addData['borderColor']; 
                }
            }
        });
        chart.update();
    }

    public addDatasets(chart: Chart, datasets: Array<ChartDataset>) {
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }

    public removeDatasets(chart: Chart, datasetIds: Array<string>) {
        chart.data.datasets.forEach(dataset => {
            if (datasetIds.includes(dataset['id'])) {
                const index = chart.data.datasets.indexOf(dataset);
                chart.data.datasets.splice(index, 1);
            }
        });
        chart.update();
    }

    public updateDatasets(chart: Chart, datasets: Array<ChartDataset>) {

        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach(dataset => {
            const datasetIndex = datasetMetas.findIndex(obj => obj['_dataset']['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }

    public setDatasets(chart: Chart, datasets: Array<ChartDataset>) {
        chart.data.datasets = datasets;
        chart.update();
    }

    public disposeChart(chartId) {
        this.dotnetRefs.delete(chartId);
    }
}

window[ChartJsInterop.name] = new ChartJsInterop();
export default window[ChartJsInterop.name];

function arbitaryLinesPlugin() {
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