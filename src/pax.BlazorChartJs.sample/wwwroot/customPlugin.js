import * as annotationPlugin from './chartjs-plugin-annotation.min.js';

export async function regsisterPlugin() {
    await import('./chartjs-plugin-annotation.min.js');
    Chart.register(annotationPlugin);
};

