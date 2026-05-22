declare const ChartDataLabels: any;

export async function loadPlugins(setupOptions: any, dotnetConfig: any): Promise<any[]> {
    const plugins: any[] = [];

    if (dotnetConfig['options'] != undefined
        && dotnetConfig['options'].plugins != undefined) {

        if (dotnetConfig['options'].plugins.arbitraryLines != undefined) {
            const arbitraryLines = arbitraryLinesPlugin();
            plugins.push(arbitraryLines);
        }

        if (dotnetConfig['options'].plugins.labels != undefined) {
            if (setupOptions?.['chartJsPluginLabelsLocation']) {
                await import(setupOptions['chartJsPluginLabelsLocation']);
            }
        }

        if (dotnetConfig['options'].plugins.datalabels != undefined) {
            if (setupOptions?.['chartJsPluginDatalabelsLocation']) {
                await import(setupOptions['chartJsPluginDatalabelsLocation']);
            }
            plugins.push(ChartDataLabels);
        }

    }
    return plugins;
}

function arbitraryLinesPlugin(): any {
    return {
        id: 'arbitraryLines',
        // beforeDraw(chart, args, options) {
        afterDraw(chart: any, args: any, options: any) {
            const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

            ctx.save();

            for (let i = 0; i < options.length; i++) {
                const option = options[i];
                ctx.fillStyle = option.arbitraryLineColor;
                const xWidth = option.xWidth;
                const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                const y0 = top;
                const x1 = xWidth;
                const y1 = height;
                ctx.fillRect(x0, y0, x1, y1);
            }

            for (let i = 0; i < options.length; i++) {
                const option = options[i];
                ctx.fillStyle = option.arbitraryLineColor;
                const xWidth = option.xWidth;
                const x0 = x.getPixelForValue(option.xPosition) - (xWidth / 2);
                const y0 = top;
                const x1 = xWidth;
                const y1 = height;

                ctx.fillStyle = 'white';
                ctx.font = '14px arial';
                ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
            }

            ctx.restore();
        }
    };
}