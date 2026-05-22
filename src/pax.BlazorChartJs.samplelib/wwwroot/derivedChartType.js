let controllerRegistrationPromise;

export function registerController(chartJsLocation) {
    controllerRegistrationPromise ??= registerControllerCore(chartJsLocation);
    return controllerRegistrationPromise;
}

async function registerControllerCore(chartJsLocation) {
    const chartJs = await import(chartJsLocation);
    const Chart = chartJs.Chart ?? chartJs.default ?? globalThis.Chart;
    const BubbleController = chartJs.BubbleController ?? Chart.registry.getController('bubble');

    class DerivedBubbleController extends BubbleController {
        draw() {
            super.draw();

            const firstPoint = this.getMeta().data[0];
            if (!firstPoint) {
                return;
            }

            const { x, y } = firstPoint.getProps(['x', 'y']);
            const { radius } = firstPoint.options;
            const ctx = this.chart.ctx;
            ctx.save();
            ctx.strokeStyle = this.options.boxStrokeStyle;
            ctx.lineWidth = 1;
            ctx.strokeRect(x - radius, y - radius, 2 * radius, 2 * radius);
            ctx.restore();
        }
    }

    DerivedBubbleController.id = 'derivedBubble';
    DerivedBubbleController.defaults = { boxStrokeStyle: 'red' };
    Chart.register(DerivedBubbleController);
}
