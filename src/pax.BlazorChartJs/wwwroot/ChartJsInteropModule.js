class LoadInfo {
    constructor() {
        this.chartJsLoaded = false;
        this.chartJsDatalabels = false;
        this.chartJsLabels = false;
    }
}
class ChartJsInterop {
    constructor() {
        this.dotnetRefs = new Map();
        this.loadInfo = new LoadInfo();
    }
    async initChart(setupOptions, chartId, dotnetConfig, dotnetRef) {
        this.dotnetRefs.set(chartId, dotnetRef);
        const config = {
            'type': dotnetConfig['type'],
            data: dotnetConfig['data'],
            options: dotnetConfig['options'] ?? {},
            plugins: []
        };
        return config;
    }
    async loadPlugins(setupOptions, dotnetConfig) {
        let plugins = [];
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
    async triggerEvent(chartId, event, source, data) {
        if (this.dotnetRefs.has(chartId)) {
            await this.dotnetRefs.get(chartId).invokeMethodAsync("EventTriggered", event, source, data);
        }
    }
    addData(chart, label, pos, datas) {
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
    addLabel(chart, label, pos) {
        if (label != undefined) {
            if (pos == undefined) {
                chart.data.labels.push(label);
            }
            else {
                chart.data.labels.splice(pos, 0, label);
            }
        }
    }
    addDatasetData(dataset, data, pos) {
        if (pos == undefined) {
            dataset.data.push(data);
        }
        else {
            dataset.data.splice(pos, 0, data);
        }
    }
    addBackgroundColor(dataset, backgroundColor, pos) {
        if (Array.isArray(dataset.backgroundColor)) {
            if (pos == undefined) {
                dataset.backgroundColor.push(backgroundColor);
            }
            else {
                dataset.backgroundColor.splice(pos, 0, backgroundColor);
            }
        }
    }
    addBorderColor(dataset, borderColor, pos) {
        if (Array.isArray(dataset.borderColor)) {
            if (pos == undefined) {
                dataset.borderColor.push(borderColor);
            }
            else {
                dataset.borderColor.splice(pos, 0, borderColor);
            }
        }
    }
    *reverseKeys(arr) {
        var key = arr.length - 1;
        while (key >= 0) {
            yield key;
            key -= 1;
        }
    }
    removeData(chart) {
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
    setData(chart, labels, datas) {
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
    addDatasets(chart, datasets) {
        for (let i = 0; i < datasets.length; i++) {
            chart.data.datasets.push(datasets[i]);
        }
        chart.update();
    }
    removeDatasets(chart, datasetIds) {
        for (var index of this.reverseKeys(chart.data.datasets)) {
            var dataset = chart.data.datasets[index];
            if (datasetIds.includes(dataset['id'])) {
                chart.data.datasets.splice(index, 1);
            }
        }
        chart.update();
    }
    updateDatasetsSmooth(chart, datasets) {
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
    updateDatasets(chart, datasets) {
        const datasetMetas = chart.getSortedVisibleDatasetMetas();
        datasets.forEach(dataset => {
            const datasetIndex = datasetMetas.findIndex(obj => obj['_dataset']['id'] === dataset['id']);
            if (datasetIndex >= 0) {
                chart.data.datasets[datasetIndex] = dataset;
            }
        });
        chart.update();
    }
    setDatasets(chart, datasets) {
        chart.data.datasets = datasets;
        chart.update();
    }
    disposeChart(chartId) {
        this.dotnetRefs.delete(chartId);
    }
    arbitaryLinesPlugin() {
        return {
            id: 'arbitraryLines',
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
