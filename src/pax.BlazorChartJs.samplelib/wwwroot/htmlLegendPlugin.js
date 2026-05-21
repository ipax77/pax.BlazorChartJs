let htmlLegendPluginRegistered = false;

const htmlLegendPlugin = {
    id: 'htmlLegend',
    afterUpdate(chart, _args, options) {
        const containerId = options?.containerID;
        if (!containerId) {
            return;
        }

        const listContainer = getOrCreateHtmlLegendList(chart, containerId);
        const items = chart.options.plugins.legend.labels.generateLabels(chart);

        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            const legendItem = getOrCreateLegendItem(listContainer, i);
            updateLegendItem(legendItem, chart, item);
        }

        while (listContainer.children.length > items.length) {
            listContainer.lastElementChild?.remove();
        }
    }
};

export function registerHtmlLegendPlugin() {
    if (htmlLegendPluginRegistered) {
        return false;
    }

    Chart.register(htmlLegendPlugin);
    htmlLegendPluginRegistered = true;
    return true;
}

function getOrCreateHtmlLegendList(chart, id) {
    let legendContainer = document.getElementById(id);
    if (!legendContainer) {
        legendContainer = document.createElement('div');
        legendContainer.id = id;
        chart.canvas.parentNode?.appendChild(legendContainer);
    }

    let listContainer = legendContainer.querySelector('ul');
    if (!listContainer) {
        listContainer = document.createElement('ul');
        listContainer.style.display = 'flex';
        listContainer.style.flexDirection = 'row';
        listContainer.style.margin = '0';
        listContainer.style.padding = '0';
        legendContainer.appendChild(listContainer);
    }

    return listContainer;
}

function getOrCreateLegendItem(listContainer, index) {
    let item = listContainer.children[index];
    if (item) {
        return item;
    }

    item = document.createElement('li');
    item.style.alignItems = 'center';
    item.style.cursor = 'pointer';
    item.style.display = 'flex';
    item.style.flexDirection = 'row';
    item.style.marginLeft = '10px';
    item.onclick = handleLegendItemClick;

    const boxSpan = document.createElement('span');
    boxSpan.style.display = 'inline-block';
    boxSpan.style.flexShrink = '0';
    boxSpan.style.height = '20px';
    boxSpan.style.marginRight = '10px';
    boxSpan.style.width = '20px';

    const textContainer = document.createElement('p');
    textContainer.style.margin = '0';
    textContainer.style.padding = '0';

    item.appendChild(boxSpan);
    item.appendChild(textContainer);
    listContainer.appendChild(item);
    return item;
}

function updateLegendItem(element, chart, item) {
    element._chart = chart;
    element._legendItem = item;

    const boxSpan = element.children[0];
    boxSpan.style.background = item.fillStyle;
    boxSpan.style.borderColor = item.strokeStyle;
    boxSpan.style.borderWidth = `${item.lineWidth}px`;

    const textContainer = element.children[1];
    textContainer.style.color = item.fontColor;
    textContainer.style.textDecoration = item.hidden ? 'line-through' : '';
    if (textContainer.textContent !== item.text) {
        textContainer.textContent = item.text;
    }
}

function handleLegendItemClick() {
    const chart = this._chart;
    const item = this._legendItem;
    if (!chart || !item) {
        return;
    }

    const { type } = chart.config;
    if (type === 'pie' || type === 'doughnut') {
        chart.toggleDataVisibility(item.index);
    } else {
        chart.setDatasetVisibility(item.datasetIndex, !chart.isDatasetVisible(item.datasetIndex));
    }

    chart.update();
}
