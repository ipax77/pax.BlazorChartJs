
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
            // plugins: await this.loadPlugins(setupOptions, dotnetConfig)
            plugins: []
        }
        return <JSON>config;
    }

    private async loadPlugins(setupOptions: JSON, dotnetConfig: JSON): Promise<Array<object>> {
        let plugins = []

        if (dotnetConfig['options'] != undefined
            && dotnetConfig['options'].plugins != undefined) {

            if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
                const arbitraryLines = this.arbitaryLinesPlugin();
                plugins.push(arbitraryLines);
            }

            if (dotnetConfig['options'].plugins.datalabels != undefined) {
                if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                    const ChartDataLabels = await import(setupOptions['chartJsPluginDatalabelsLocation']);
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

    public addData(chart: any, label: string, pos: number, datas: Array<JSON>) {

        if (chart == undefined) {
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

    private addLabel(chart: any, label: string, pos: number) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            } else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }

    private addDatasetData(dataset: any, data: any, pos: number) {
        if (pos == undefined) {
            dataset.data.push(data);
        } else {
            dataset.data.splice(pos, 0, data);
        }
    }

    private addBackgroundColor(dataset: any, backgroundColor: string, pos: number) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            } else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }

    private addBorderColor(dataset: any, borderColor: string, pos: number) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            } else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }

    private *reverseKeys(arr) {
        var key = arr.length - 1;

        while (key >= 0) {
            yield key;
            key -= 1;
        }
    }

    public removeData(chart: any) {

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

    public setData(chart: any, labels: Array<string>, datas: Array<JSON>) {
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

    public addDatasets(chart: any, datasets: Array<any>) {
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }

    public removeDatasets(chart: any, datasetIds: Array<string>) {
        for (var index of this.reverseKeys(chart.data.datasets)) {
            var dataset = chart.data.datasets[index];
            if (datasetIds.includes(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }

    public updateDatasetsSmooth(chart: any, datasets: Array<any>) {

        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach(newDataset => {
            const datasetIndex = datasetMetas.findIndex(obj => obj['_dataset']['id'] === newDataset['id']);
            if (datasetIndex >= 0) {
                const existingDataset = chart.data.datasets[datasetIndex];

                Object.assign(existingDataset, newDataset);

                for (const prop in existingDataset) {
                    if (existingDataset.hasOwnProperty(prop) && !newDataset.hasOwnProperty(prop)) {
                        delete existingDataset[prop];
                    }
                }
            }
        });
        chart.update();
    }

    public updateDatasets(chart: any, datasets: Array<any>) {

        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach(dataset => {
            const datasetIndex = datasetMetas.findIndex(obj => obj['_dataset']['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }

    public setDatasets(chart: any, datasets: Array<any>) {
        chart.data.datasets = datasets;
        chart.update();
    }

    public disposeChart(chartId) {
        this.dotnetRefs.delete(chartId);
    }

    public arbitaryLinesPlugin(): any {
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

window[ChartJsInterop.name] = new ChartJsInterop();
export default window[ChartJsInterop.name];

