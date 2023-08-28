import * as annotationPlugin from './chartjs-plugin-annotation.min.js';

const cmdrIconsMap = new Map();

export async function registerPlugin() {
    await import('./chartjs-plugin-annotation.min.js');
    Chart.register(annotationPlugin);
};

export function registerImagePlugin(xWidth, yWidth) {
    const barIcons = barIconsPlugin();
    Chart.register(barIcons);
    preloadChartIcons(xWidth, yWidth);
}

function preloadChartIcons(xWidth, yWidth) {
    if (cmdrIconsMap.size > 0) {
        return;
    }
    const cmdrs = ["abathur", "alarak", "artanis"];
    for (let i = 0; i < cmdrs.length; i++) {
        const img = new Image(xWidth, yWidth);
        img.onload = () => {
            cmdrIconsMap.set(cmdrs[i], img);
        };
        img.src = "_content/pax.BlazorChartJs.samplelib/images/" + cmdrs[i] + "-min.png";
    }
}

function barIconsPlugin() {
    return {
        id: 'barIcons',
        // beforeDraw(chart, args, options) {
        afterDatasetDraw(chart, args, options) {
            // afterDraw(chart, args, options) {
            const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;

            ctx.save();
            const meta = chart.getDatasetMeta(0);
            if (meta != undefined) {
                for (let i = 0; i < options.length; i++) {
                    var option = options[i];
                    const xWidth = option.xWidth;
                    const yWidth = option.xWidth;
                    const yOffset = option.yOffset;


                    const elem = meta.data[i];
                    if (elem != undefined) {
                        let x0 = 0;
                        let y0 = 0;
                        if (x != undefined) {
                            x0 = x.getPixelForValue(i) - (xWidth / 2);
                            y0 = elem.y - yWidth + yOffset;
                        } else {
                            if (elem.startAngle == undefined) {
                                x0 = elem.x;
                                y0 = elem.y;
                            } else {
                                const piePos = getPieIconPos(elem);
                                x0 = piePos.x;
                                y0 = piePos.y + yOffset;
                            }
                        }

                        const img = cmdrIconsMap.get(option.cmdr);

                        if (img == undefined) {

                            const img = new Image();
                            img.onload = () => {
                                ctx.drawImage(img, x0, y0, xWidth, yWidth);
                            };
                            img.src = option.imageSrc;
                        } else {
                            ctx.drawImage(img, x0, y0, img.width, img.height);
                        }
                    }
                }
            }
            ctx.restore();
        }
    };
}