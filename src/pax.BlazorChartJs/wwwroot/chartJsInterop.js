// TypeScript/chartInteropState.ts
var ChartJsInteropState = class {
  constructor() {
    this.dotnetRefs = /* @__PURE__ */ new Map();
    this.charts = /* @__PURE__ */ new Map();
    this.chartInitPromises = /* @__PURE__ */ new Map();
    this.chartJsCallbackRegistryPromises = /* @__PURE__ */ new Map();
    this.appliedDefaultsKey = null;
  }
};
var chartJsInterop = new ChartJsInteropState();

// TypeScript/chartCallbacks.ts
var chartJsFunctionMarker = "__chartJsFunction";
var chartJsFunctionNamePattern = /^[A-Za-z_$][A-Za-z0-9_$]*$/;
var reservedChartJsFunctionNames = /* @__PURE__ */ new Set([
  "__proto__",
  "constructor",
  "prototype",
  "toString",
  "valueOf",
  "hasOwnProperty"
]);
function objectHasOwn(value, key) {
  const hasOwn = Object.hasOwn;
  return typeof hasOwn === "function" ? hasOwn(value, key) : Object.prototype.hasOwnProperty.call(value, key);
}
function describeChartJsFunctionName(name) {
  if (typeof name === "string") {
    return name;
  }
  if (name === null) {
    return "null";
  }
  return typeof name;
}
function validateChartJsFunctionName(name) {
  if (typeof name !== "string" || !chartJsFunctionNamePattern.test(name)) {
    throw new Error(`Invalid Chart.js callback name: ${describeChartJsFunctionName(name)}. Names must match ${chartJsFunctionNamePattern}.`);
  }
  if (reservedChartJsFunctionNames.has(name)) {
    throw new Error(`Reserved Chart.js callback name: ${name}`);
  }
}
function createChartJsCallbackRegistry(moduleLocation, callbacks) {
  if (callbacks == void 0 || typeof callbacks !== "object") {
    throw new Error(`Chart.js callback module '${moduleLocation}' must export a chartJsCallbacks object.`);
  }
  const registry = /* @__PURE__ */ Object.create(null);
  const callbackNames = Object.keys(callbacks);
  for (let i = 0; i < callbackNames.length; i++) {
    const callbackName = callbackNames[i];
    validateChartJsFunctionName(callbackName);
    const callback = callbacks[callbackName];
    if (typeof callback !== "function") {
      throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
    }
    registry[callbackName] = callback;
  }
  return Object.freeze(registry);
}
async function getChartJsCallbackRegistry(moduleLocation) {
  let registryPromise = chartJsInterop.chartJsCallbackRegistryPromises.get(moduleLocation);
  if (!registryPromise) {
    registryPromise = import(moduleLocation).then((module) => {
      const callbacks = module?.chartJsCallbacks;
      return createChartJsCallbackRegistry(moduleLocation, callbacks);
    }).catch((error) => {
      chartJsInterop.chartJsCallbackRegistryPromises.delete(moduleLocation);
      throw error;
    });
    chartJsInterop.chartJsCallbackRegistryPromises.set(moduleLocation, registryPromise);
  }
  return await registryPromise;
}
async function getChartJsCallbackRegistryIfConfigured(setupOptions) {
  const moduleLocation = setupOptions?.chartJsCallbacksModuleLocation;
  if (typeof moduleLocation !== "string" || moduleLocation.length === 0) {
    return void 0;
  }
  return await getChartJsCallbackRegistry(moduleLocation);
}
async function resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions) {
  if (hasChartJsFunctions !== true) {
    return;
  }
  const callbacks = await getChartJsCallbackRegistryIfConfigured(setupOptions);
  reviveChartJsFunctions(config, callbacks, "$", null, null, null);
}
function shouldSkipChartJsFunctionValue(key, parentKey, grandparentKey) {
  return grandparentKey === "datasets" && key === "data" || parentKey === "data" && key === "labels";
}
function resolveChartJsFunction(reference, callbacks, path) {
  const callbackName = reference[chartJsFunctionMarker];
  if (typeof callbackName !== "string" || callbackName.length === 0) {
    throw new Error(`Invalid Chart.js callback marker at ${path}.`);
  }
  validateChartJsFunctionName(callbackName);
  if (!objectHasOwn(callbacks, callbackName)) {
    throw new Error(`Unknown Chart.js callback '${callbackName}' at ${path}.`);
  }
  const callback = callbacks[callbackName];
  if (typeof callback !== "function") {
    throw new Error(`Chart.js callback '${callbackName}' is not a function.`);
  }
  return callback;
}
function reviveChartJsFunctions(value, callbacks, path, key, parentKey, grandparentKey) {
  if (value == null || typeof value !== "object") {
    return value;
  }
  if (shouldSkipChartJsFunctionValue(key, parentKey, grandparentKey)) {
    return value;
  }
  if (Array.isArray(value)) {
    for (let i = 0; i < value.length; i++) {
      value[i] = reviveChartJsFunctions(value[i], callbacks, `${path}[${i}]`, String(i), key, parentKey);
    }
    return value;
  }
  if (isChartJsFunctionMarker(value, path)) {
    if (callbacks == void 0) {
      throw new Error(`Chart.js callback marker at ${path} requires ChartJsSetupOptions.ChartJsCallbacksModuleLocation.`);
    }
    return resolveChartJsFunction(value, callbacks, path);
  }
  const keys = Object.keys(value);
  for (let i = 0; i < keys.length; i++) {
    const childKey = keys[i];
    value[childKey] = reviveChartJsFunctions(value[childKey], callbacks, `${path}.${childKey}`, childKey, key, parentKey);
  }
  return value;
}
function isChartJsFunctionMarker(value, path) {
  if (value == null || typeof value !== "object" || Array.isArray(value) || !objectHasOwn(value, chartJsFunctionMarker)) {
    return false;
  }
  const keys = Object.keys(value);
  if (keys.length !== 1) {
    throw new Error(`Invalid Chart.js callback marker at ${path}: '${chartJsFunctionMarker}' marker must not contain other properties.`);
  }
  return true;
}

// TypeScript/version.ts
var chartJsInteropVersion = "0.9.0-preview";

// TypeScript/chartEvents.ts
async function triggerEvent(chartId, event, source, data) {
  const dotnetRef = chartJsInterop.dotnetRefs.get(chartId);
  if (dotnetRef) {
    await dotnetRef.invokeMethodAsync("EventTriggered", event, source, data);
  }
}
function getChartPointEventArgs(e, chart) {
  const points = chart.getElementsAtEventForMode(e, "nearest", { intersect: true }, true);
  let label = "";
  let value = 0;
  let dataX = 0;
  let dataY = 0;
  let datasetLabel = null;
  let datasetIndex = null;
  const canvasPosition = Chart.helpers.getRelativePosition(e, chart);
  try {
    dataX = chart.scales.x.getValueForPixel(canvasPosition.x);
  } catch {
  }
  try {
    dataY = chart.scales.y.getValueForPixel(canvasPosition.y);
  } catch {
  }
  if (points.length) {
    const firstPoint = points[0];
    const currentDatasetIndex = firstPoint.datasetIndex;
    const currentDataset = chart.data.datasets[currentDatasetIndex];
    label = chart.data.labels?.[firstPoint.index] ?? "";
    datasetIndex = currentDatasetIndex;
    value = currentDataset.data[firstPoint.index];
    datasetLabel = currentDataset.label ?? null;
  }
  return {
    Label: label,
    Value: value,
    DataX: dataX,
    DataY: dataY,
    DatasetLabel: datasetLabel,
    DatasetIndex: datasetIndex
  };
}
function registerChartPointEvent(chart, chartId, eventName, optionName) {
  const nativeCallback = typeof chart.options[optionName] === "function" ? chart.options[optionName] : void 0;
  chart.options[optionName] = (e, elements, chartInstance) => {
    nativeCallback?.call(chart, e, elements, chartInstance ?? chart);
    triggerEvent(chartId, eventName, "label", getChartPointEventArgs(e, chart));
  };
}
function registerEvents(dotnetConfigOptions, chartId, chart) {
  if (dotnetConfigOptions.onClickEvent == true) {
    registerChartPointEvent(chart, chartId, "click", "onClick");
  }
  if (dotnetConfigOptions.onHoverEvent == true) {
    registerChartPointEvent(chart, chartId, "hover", "onHover");
  }
  if (dotnetConfigOptions.onResizeEvent == true) {
    const nativeOnResize = typeof chart.options.onResize === "function" ? chart.options.onResize : void 0;
    chart.options.onResize = (_chart, size) => {
      nativeOnResize?.call(chart, _chart, size);
      triggerEvent(chartId, "resize", "chart", {
        Height: size.height,
        Width: size.width,
        WindowHeight: window.innerHeight,
        WindowWidth: window.innerWidth
      });
    };
  }
  if (dotnetConfigOptions.plugins?.legend?.onClickEvent == true) {
    chart.options.plugins.legend.onClick = (_event, legendItem, _legend) => {
      triggerEvent(chartId, "click", "legend", { Label: legendItem.text });
    };
  }
  if (dotnetConfigOptions.plugins?.legend?.onHoverEvent == true) {
    chart.options.plugins.legend.onHover = (_event, legendItem, _legend) => {
      triggerEvent(chartId, "hover", "legend", { Label: legendItem.text });
    };
  }
  if (dotnetConfigOptions.plugins?.legend?.onLeaveEvent == true) {
    chart.options.plugins.legend.onLeave = (_event, legendItem, _legend) => {
      triggerEvent(chartId, "leave", "legend", { Label: legendItem.text });
    };
  }
  if (dotnetConfigOptions.animation?.onProgressEvent == true) {
    chart.options.animation.onProgress = (context) => {
      triggerEvent(chartId, "progress", "animation", {
        CurrentStep: context.currentStep,
        NumSteps: context.numSteps
      });
    };
  }
  if (dotnetConfigOptions.animation?.onCompleteEvent == true) {
    chart.options.animation.onComplete = (context) => {
      triggerEvent(chartId, "complete", "animation", {
        Initial: context.initial
      });
    };
  }
}

// TypeScript/chartPlugins.ts
async function loadPlugins(setupOptions, dotnetConfig) {
  const plugins = [];
  if (dotnetConfig["options"] != void 0 && dotnetConfig["options"].plugins != void 0) {
    if (dotnetConfig["options"].plugins.arbitraryLines != void 0) {
      const arbitraryLines = arbitraryLinesPlugin();
      plugins.push(arbitraryLines);
    }
    if (dotnetConfig["options"].plugins.labels != void 0) {
      if (setupOptions?.["chartJsPluginLabelsLocation"]) {
        await import(setupOptions["chartJsPluginLabelsLocation"]);
      }
    }
    if (dotnetConfig["options"].plugins.datalabels != void 0) {
      if (setupOptions?.["chartJsPluginDatalabelsLocation"]) {
        await import(setupOptions["chartJsPluginDatalabelsLocation"]);
      }
      plugins.push(ChartDataLabels);
    }
  }
  return plugins;
}
function arbitraryLinesPlugin() {
  return {
    id: "arbitraryLines",
    // beforeDraw(chart, args, options) {
    afterDraw(chart, args, options) {
      const { ctx, chartArea: { top, right, bottom, left, width, height }, scales: { x, y } } = chart;
      ctx.save();
      for (let i = 0; i < options.length; i++) {
        const option = options[i];
        ctx.fillStyle = option.arbitraryLineColor;
        const xWidth = option.xWidth;
        const x0 = x.getPixelForValue(option.xPosition) - xWidth / 2;
        const y0 = top;
        const x1 = xWidth;
        const y1 = height;
        ctx.fillRect(x0, y0, x1, y1);
      }
      for (let i = 0; i < options.length; i++) {
        const option = options[i];
        ctx.fillStyle = option.arbitraryLineColor;
        const xWidth = option.xWidth;
        const x0 = x.getPixelForValue(option.xPosition) - xWidth / 2;
        const y0 = top;
        const x1 = xWidth;
        const y1 = height;
        ctx.fillStyle = "white";
        ctx.font = "14px arial";
        ctx.fillText(option.text, x0 + 4, y0 + 10 * (i + 1));
      }
      ctx.restore();
    }
  };
}

// TypeScript/payload.ts
function parsePayload(value) {
  if (value == void 0 || typeof value !== "string") {
    return value;
  }
  return JSON.parse(value);
}
function parseArrayPayload(value) {
  return parsePayload(value);
}
function isSetupOptions(value) {
  return value != void 0 && typeof value === "object" && !Array.isArray(value) && ("chartJsLocation" in value || "chartJsPluginLabelsLocation" in value || "chartJsPluginDatalabelsLocation" in value || "chartJsCallbacksModuleLocation" in value || "defaults" in value);
}

// TypeScript/chartLifecycle.ts
var chartJsLoadPromise = null;
function getLiveChart(chartId) {
  const mappedChart = chartJsInterop.charts.get(chartId);
  if (mappedChart && mappedChart.data) {
    return mappedChart;
  }
  if (typeof Chart === "undefined") {
    return void 0;
  }
  const chart = Chart.getChart(chartId);
  return chart && chart.data ? chart : void 0;
}
async function ensureChartJsLoaded(setupOptions) {
  if (!setupOptions?.chartJsLocation) {
    return;
  }
  if (!chartJsLoadPromise) {
    chartJsLoadPromise = import(setupOptions.chartJsLocation).then(() => {
    });
  }
  await chartJsLoadPromise;
}
async function initChart(setupOptions, chartId, dotnetConfig, hasChartJsFunctions, dotnetRef, defaults, hasDefaultChartJsFunctions, defaultsKey) {
  const runningInit = chartJsInterop.chartInitPromises.get(chartId) ?? Promise.resolve({ success: true });
  const initPromise = runningInit.catch(() => ({ success: true })).then(() => initChartCore(
    setupOptions,
    chartId,
    dotnetConfig,
    hasChartJsFunctions,
    dotnetRef,
    defaults,
    hasDefaultChartJsFunctions,
    defaultsKey
  ));
  chartJsInterop.chartInitPromises.set(chartId, initPromise);
  try {
    return await initPromise;
  } finally {
    if (chartJsInterop.chartInitPromises.get(chartId) === initPromise) {
      chartJsInterop.chartInitPromises.delete(chartId);
    }
  }
}
function buildChartConfig(dotnetConfig) {
  return {
    type: dotnetConfig.type,
    data: dotnetConfig.data,
    options: dotnetConfig.options ?? {},
    plugins: []
  };
}
async function applyDefaults(setupOptions, defaults, hasChartJsFunctions, defaultsKey) {
  if (defaults == void 0) {
    return;
  }
  const resolvedDefaultsKey = typeof defaultsKey === "string" && defaultsKey.length > 0 ? defaultsKey : JSON.stringify(defaults);
  if (chartJsInterop.appliedDefaultsKey === resolvedDefaultsKey) {
    return;
  }
  await resolveChartJsFunctions(setupOptions, defaults, hasChartJsFunctions);
  Chart.defaults.set(defaults);
  chartJsInterop.appliedDefaultsKey = resolvedDefaultsKey;
}
function destroyExistingChart(chartId, element) {
  const mappedChart = chartJsInterop.charts.get(chartId);
  let destroyedChart;
  let destroyedElementChart;
  if (mappedChart != void 0) {
    mappedChart.destroy();
    destroyedChart = mappedChart;
  }
  if (typeof Chart !== "undefined") {
    const elementChart = element ? Chart.getChart(element) : void 0;
    if (elementChart != void 0 && elementChart !== destroyedChart) {
      elementChart.destroy();
      destroyedElementChart = elementChart;
    }
    const idChart = Chart.getChart(chartId);
    if (idChart != void 0 && idChart !== destroyedChart && idChart !== destroyedElementChart) {
      idChart.destroy();
    }
  }
  chartJsInterop.charts.delete(chartId);
  chartJsInterop.dotnetRefs.delete(chartId);
}
async function initChartCore(setupOptions, chartId, dotnetConfig, hasChartJsFunctions, dotnetRef, defaults, hasDefaultChartJsFunctions, defaultsKey) {
  try {
    const resolvedDotnetConfig = parsePayload(dotnetConfig);
    const resolvedDefaults = parsePayload(defaults);
    await ensureChartJsLoaded(setupOptions);
    await applyDefaults(setupOptions, resolvedDefaults, hasDefaultChartJsFunctions === true, defaultsKey);
    const element = document.getElementById(chartId);
    if (!element) {
      return { success: false };
    }
    destroyExistingChart(chartId, element);
    const config = buildChartConfig(resolvedDotnetConfig);
    await resolveChartJsFunctions(setupOptions, config, hasChartJsFunctions);
    config.plugins = await loadPlugins(setupOptions, resolvedDotnetConfig);
    const ctx = element.getContext("2d");
    if (!ctx) {
      return { success: false };
    }
    const chart = new Chart(ctx, config);
    chartJsInterop.charts.set(chartId, chart);
    chartJsInterop.dotnetRefs.set(chartId, dotnetRef);
    if (resolvedDotnetConfig["options"] != void 0) {
      registerEvents(resolvedDotnetConfig.options, chartId, chart);
    }
    const result = {
      success: true,
      height: chart.height,
      width: chart.width,
      windowHeight: window.innerHeight,
      windowWidth: window.innerWidth
    };
    return result;
  } finally {
  }
}
function disposeChart(chartId) {
  destroyExistingChart(chartId);
}

// TypeScript/chartCommands.ts
async function updateChartOptions(chartId, setupOptionsOrOptions, options, hasChartJsFunctions) {
  const hasSetupOptions = arguments.length >= 3 && (arguments.length >= 4 || typeof options !== "boolean");
  const setupOptions = hasSetupOptions ? setupOptionsOrOptions : void 0;
  const resolvedOptions = parsePayload(hasSetupOptions ? options : setupOptionsOrOptions);
  const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctions : options;
  await resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, resolvedHasChartJsFunctions === true);
  const chart = getLiveChart(chartId);
  if (chart != void 0) {
    chart.options = resolvedOptions;
    chart.update();
    registerEvents(resolvedOptions, chartId, chart);
  }
}
function setLabels(chartId, labels) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.data.labels = labels;
  chart.update();
}
function resizeChart(chartId, width, height) {
  const chart = getLiveChart(chartId);
  if (chart == void 0) {
    return;
  }
  if (width == void 0 || height == void 0) {
    chart.resize();
  } else {
    chart.resize(width, height);
  }
  chart.options.onResize?.(chart, { height: chart.height, width: chart.width });
}
function getChartImage(chartId, type, quality, width, height) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return "";
  }
  let currentWidth = 0;
  let currentHeight = 0;
  if (!(width == void 0 || height == void 0)) {
    const ctx = document.getElementById(chartId);
    if (ctx.parentNode) {
      currentHeight = ctx.width;
      currentHeight = ctx.height;
      ctx.width = width;
      ctx.height = height;
      chart.options.animation = false;
      chart.resize(width, height);
    }
  }
  let chartImg;
  if (!(type == void 0 || quality == void 0)) {
    chartImg = chart.toBase64Image(type, quality);
  } else {
    chartImg = chart.toBase64Image();
  }
  if (currentWidth > 0 && currentHeight > 0) {
    chart.resize();
  }
  chart.options.animation = true;
  return chartImg;
}
function resetChart(chartId) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.reset();
}
function renderChart(chartId) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.render();
}
function stopChart(chartId) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.stop();
}
function setDatasetVisibility(chartId, datasetIndex, value) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.setDatasetVisibility(datasetIndex, value);
  chart.update();
}
function toggleDataVisibility(chartId, index) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  chart.toggleDataVisibility(index);
  chart.update();
}
function getDataVisibility(chartId, index) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return false;
  }
  return chart.getDataVisibility(index);
}
function hideDataset(chartId, datasetIdOrIndex, dataIndex) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  const datasetIndex = resolveDatasetIndex(chart, datasetIdOrIndex);
  if (!hasDatasetElement(chart, datasetIndex, dataIndex)) {
    return;
  }
  if (dataIndex == void 0) {
    chart.hide(datasetIndex);
  } else {
    chart.hide(datasetIndex, dataIndex);
  }
}
function resolveDatasetIndex(chart, datasetIdOrIndex) {
  if (typeof datasetIdOrIndex === "number") {
    return datasetIdOrIndex;
  }
  return chart.data.datasets.findIndex((dataset) => dataset.id === datasetIdOrIndex);
}
function hasDatasetElement(chart, datasetIndex, dataIndex) {
  if (datasetIndex < 0 || datasetIndex >= chart.data.datasets.length) {
    return false;
  }
  if (dataIndex == void 0) {
    return true;
  }
  const dataset = chart.data.datasets[datasetIndex];
  return dataIndex >= 0 && dataIndex < (dataset.data?.length ?? 0);
}
function showDataset(chartId, datasetIdOrIndex, dataIndex) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  const datasetIndex = resolveDatasetIndex(chart, datasetIdOrIndex);
  if (!hasDatasetElement(chart, datasetIndex, dataIndex)) {
    return;
  }
  if (dataIndex == void 0) {
    chart.show(datasetIndex);
  } else {
    chart.show(datasetIndex, dataIndex);
  }
}
function getLabels(chartId) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return [];
  }
  const items = chart.options.plugins.legend.labels.generateLabels(chart);
  return items;
}
function isDatasetVisible(chartId, datasetIndex) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return false;
  }
  const isVisible = chart.isDatasetVisible(datasetIndex);
  return isVisible;
}
function setDatasetPointsActive(chartId, datasetIndex) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  if (chart.getActiveElements().length > 0) {
    chart.setActiveElements([]);
    chart.update();
  }
  if (datasetIndex == -1 || chart.data.datasets.length <= datasetIndex) {
    return;
  }
  const dataset = chart.data.datasets[datasetIndex];
  const elements = [];
  const datasetData = dataset.data;
  for (let i = 0; i < datasetData.length; i++) {
    elements.push({ datasetIndex, index: i });
  }
  chart.setActiveElements(elements);
  chart.update();
}

// TypeScript/binaryDatasets.ts
var binaryTypedArraysReadLittleEndian = new Uint8Array(new Uint16Array([1]).buffer)[0] === 1;
function validateBinaryLayout(bytes, payload, compactStride, valueSize, usesX) {
  if (!Number.isInteger(payload.count) || payload.count < 0) {
    throw new Error("Binary dataset payload count must be a non-negative integer.");
  }
  const xOffset = payload.xOffset ?? 0;
  const rawYOffset = payload.yOffset ?? 0;
  const yOffset = usesX && xOffset === 0 && rawYOffset === 0 ? valueSize : rawYOffset;
  const byteStride = payload.byteStride ?? compactStride;
  if (!Number.isInteger(xOffset) || xOffset < 0 || !Number.isInteger(yOffset) || yOffset < 0 || !Number.isInteger(byteStride) || byteStride < compactStride) {
    throw new Error("Binary dataset payload has invalid offsets or byte stride.");
  }
  const maxOffset = usesX ? Math.max(xOffset, yOffset) : yOffset;
  if (maxOffset + valueSize > byteStride) {
    throw new Error("Binary dataset payload offsets must fit inside the byte stride.");
  }
  const requiredBytes = payload.count === 0 ? 0 : (payload.count - 1) * byteStride + maxOffset + valueSize;
  if (bytes.byteLength < requiredBytes) {
    throw new Error(`Binary dataset payload for '${payload.datasetId}' is too small.`);
  }
  return {
    xOffset,
    yOffset,
    byteStride
  };
}
function tryGetBinaryFloatArray(bytes, valueSize, valueKind, byteStride, ...offsets) {
  if (!binaryTypedArraysReadLittleEndian || bytes.byteOffset % valueSize !== 0 || byteStride % valueSize !== 0 || offsets.some((offset) => offset % valueSize !== 0)) {
    return void 0;
  }
  const valueCount = Math.floor(bytes.byteLength / valueSize);
  return valueKind === "float64" ? new Float64Array(bytes.buffer, bytes.byteOffset, valueCount) : new Float32Array(bytes.buffer, bytes.byteOffset, valueCount);
}
function tryGetBinaryInt32Array(bytes, byteStride, yOffset) {
  const valueSize = Int32Array.BYTES_PER_ELEMENT;
  if (!binaryTypedArraysReadLittleEndian || bytes.byteOffset % valueSize !== 0 || byteStride % valueSize !== 0 || yOffset % valueSize !== 0) {
    return void 0;
  }
  const valueCount = Math.floor(bytes.byteLength / valueSize);
  return new Int32Array(bytes.buffer, bytes.byteOffset, valueCount);
}
function readBinaryFloat(view, byteOffset, valueKind) {
  return valueKind === "float64" ? view.getFloat64(byteOffset, true) : view.getFloat32(byteOffset, true);
}
function decodeBinaryDatasetData(bytes, payload) {
  if (!(bytes instanceof Uint8Array)) {
    throw new Error("Binary dataset payload must be a Uint8Array.");
  }
  switch (payload.format) {
    case "Float64XY":
      return decodeBinaryXY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");
    case "Float32XY":
      return decodeBinaryXY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");
    case "Float64Y":
      return decodeBinaryY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");
    case "Float32Y":
      return decodeBinaryY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");
    case "Int32Y":
      return decodeBinaryInt32Y(bytes, payload);
    default:
      throw new Error(`Unsupported binary dataset format '${payload.format}'.`);
  }
}
function decodeBinaryXY(bytes, payload, valueSize, valueKind) {
  const layout = validateBinaryLayout(bytes, payload, valueSize * 2, valueSize, true);
  const data = new Array(payload.count);
  const typedValues = tryGetBinaryFloatArray(bytes, valueSize, valueKind, layout.byteStride, layout.xOffset, layout.yOffset);
  if (typedValues) {
    const valueStride = layout.byteStride / valueSize;
    const xOffset = layout.xOffset / valueSize;
    const yOffset = layout.yOffset / valueSize;
    for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
      data[i] = {
        x: typedValues[valueIndex + xOffset],
        y: typedValues[valueIndex + yOffset]
      };
    }
    return data;
  }
  const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
    data[i] = {
      x: readBinaryFloat(view, recordOffset + layout.xOffset, valueKind),
      y: readBinaryFloat(view, recordOffset + layout.yOffset, valueKind)
    };
  }
  return data;
}
function decodeBinaryY(bytes, payload, compactStride, valueKind) {
  const layout = validateBinaryLayout(bytes, payload, compactStride, compactStride, false);
  const data = new Array(payload.count);
  const typedValues = tryGetBinaryFloatArray(bytes, compactStride, valueKind, layout.byteStride, layout.yOffset);
  if (typedValues) {
    const valueStride = layout.byteStride / compactStride;
    const yOffset = layout.yOffset / compactStride;
    for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
      data[i] = typedValues[valueIndex + yOffset];
    }
    return data;
  }
  const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
    data[i] = readBinaryFloat(view, recordOffset + layout.yOffset, valueKind);
  }
  return data;
}
function decodeBinaryInt32Y(bytes, payload) {
  const compactStride = Int32Array.BYTES_PER_ELEMENT;
  const layout = validateBinaryLayout(bytes, payload, compactStride, compactStride, false);
  const data = new Array(payload.count);
  const typedValues = tryGetBinaryInt32Array(bytes, layout.byteStride, layout.yOffset);
  if (typedValues) {
    const valueStride = layout.byteStride / compactStride;
    const yOffset = layout.yOffset / compactStride;
    for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
      data[i] = typedValues[valueIndex + yOffset];
    }
    return data;
  }
  const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
    data[i] = view.getInt32(recordOffset + layout.yOffset, true);
  }
  return data;
}

// TypeScript/chartDatasets.ts
function resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions) {
  if (Array.isArray(setupOptionsOrDatasets) || typeof setupOptionsOrDatasets === "string") {
    return {
      setupOptions: void 0,
      datasets: parseArrayPayload(setupOptionsOrDatasets),
      hasChartJsFunctions: datasetsOrHasChartJsFunctions === true
    };
  }
  const datasetPayload = typeof datasetsOrHasChartJsFunctions === "boolean" ? void 0 : datasetsOrHasChartJsFunctions;
  return {
    setupOptions: setupOptionsOrDatasets,
    datasets: parseArrayPayload(datasetPayload),
    hasChartJsFunctions: hasChartJsFunctions === true
  };
}
function insertLabel(chart, label, pos) {
  const labels = chart.data.labels;
  if (labels == void 0) {
    return;
  }
  if (pos == void 0) {
    labels.push(label);
  } else {
    labels.splice(pos, 0, label);
  }
}
function addLabel(chart, label, pos) {
  if (label != void 0) {
    insertLabel(chart, label, pos);
  }
}
function addDatasetData(dataset, data, pos) {
  const datasetData = dataset.data;
  if (pos == void 0) {
    datasetData.push(data);
  } else {
    datasetData.splice(pos, 0, data);
  }
}
function addBackgroundColor(dataset, backgroundColor, pos) {
  if (Array.isArray(dataset.backgroundColor)) {
    if (pos == void 0) {
      dataset.backgroundColor.push(backgroundColor);
    } else {
      dataset.backgroundColor.splice(pos, 0, backgroundColor);
    }
  }
}
function addBorderColor(dataset, borderColor, pos) {
  if (Array.isArray(dataset.borderColor)) {
    if (pos == void 0) {
      dataset.borderColor.push(borderColor);
    } else {
      dataset.borderColor.splice(pos, 0, borderColor);
    }
  }
}
function addData(chartId, label, pos, datas) {
  const chart = getLiveChart(chartId);
  if (chart == void 0) {
    return;
  }
  addLabel(chart, label, pos);
  for (let i = 0; i < chart.data.datasets.length; i++) {
    const dataset = chart.data.datasets[i];
    const datasetId = dataset.id;
    if (datasetId == void 0) {
      continue;
    }
    const addData2 = datas[datasetId];
    if (addData2 != void 0) {
      addDatasetData(dataset, addData2.data, addData2.atPosition);
      if (addData2.backgroundColor != void 0) {
        addBackgroundColor(dataset, addData2.backgroundColor, addData2.atPosition);
      }
      if (addData2.borderColor != void 0) {
        addBorderColor(dataset, addData2.borderColor, addData2.atPosition);
      }
    }
  }
  chart.update();
}
function addChartDataToDatasets(chartId, label, data, backgroundColors, borderColors, pos) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  insertLabel(chart, label, pos);
  const datasetCount = Math.min(data.length, chart.data.datasets.length);
  for (let i = 0; i < datasetCount; i++) {
    const dataset = chart.data.datasets[i];
    addDatasetData(dataset, data[i], pos);
    const backgroundColor = backgroundColors?.[i];
    if (backgroundColor != void 0) {
      addBackgroundColor(dataset, backgroundColor, pos);
    }
    const borderColor = borderColors?.[i];
    if (borderColor != void 0) {
      addBorderColor(dataset, borderColor, pos);
    }
  }
  chart.update();
}
function removeDataCore(chart) {
  if (!chart || !chart.data) {
    return;
  }
  const labels = chart.data.labels;
  if (labels != void 0 && !(labels.length == 0)) {
    labels.pop();
  }
  for (let i = 0; i < chart.data.datasets.length; i++) {
    const dataset = chart.data.datasets[i];
    const datasetData = dataset.data;
    if (!(datasetData.length == 0)) {
      datasetData.pop();
    }
    if (Array.isArray(dataset.backgroundColor) && !(dataset.backgroundColor.length == 0)) {
      dataset.backgroundColor.pop();
    }
    if (Array.isArray(dataset.borderColor) && !(dataset.borderColor.length == 0)) {
      dataset.borderColor.pop();
    }
  }
  chart.update();
}
function removeData(chartId) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  removeDataCore(chart);
}
function setDataCore(chart, labels, datas) {
  if (!chart || !chart.data) {
    return;
  }
  if (labels != void 0) {
    chart.data.labels = labels;
  }
  for (let i = 0; i < chart.data.datasets.length; i++) {
    const dataset = chart.data.datasets[i];
    const datasetId = dataset.id;
    if (datasetId == void 0) {
      continue;
    }
    const addData2 = datas[datasetId];
    if (addData2 != void 0) {
      dataset.data = addData2.data;
      if (addData2.backgroundColor != void 0) {
        dataset.backgroundColor = addData2.backgroundColor;
      }
      if (addData2.borderColor != void 0) {
        dataset.borderColor = addData2.borderColor;
      }
    }
  }
  chart.update();
}
function setData(chartId, labels, datas) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  setDataCore(chart, labels, datas);
}
function setDatasetsData(chartId, data) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  const datasetsById = createDatasetMap(chart.data.datasets);
  for (let i = 0; i < data.length; i++) {
    const payload = data[i];
    const dataset = datasetsById.get(payload.datasetId);
    if (dataset != void 0) {
      dataset.data = payload.data;
    }
  }
  chart.update();
}
function createDatasetMap(datasets) {
  const datasetsById = /* @__PURE__ */ new Map();
  for (let i = 0; i < datasets.length; i++) {
    const dataset = datasets[i];
    if (dataset.id != void 0) {
      datasetsById.set(dataset.id, dataset);
    }
  }
  return datasetsById;
}
function setDatasetBinaryData(chartId, datasetId, bytes, pointCount, format, xOffset = 0, yOffset = 0, byteStride, updateMode = "none") {
  setDatasetsBinaryData(
    chartId,
    [{
      datasetId,
      count: pointCount,
      format,
      xOffset,
      yOffset,
      byteStride
    }],
    updateMode,
    bytes
  );
}
function setDatasetsBinaryData(chartId, payloads, updateMode, ...binaryPayloads) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  setDatasetsBinaryDataCore(chart, payloads, updateMode, binaryPayloads);
}
function setDatasetsBinaryDataCore(chart, payloads, updateMode, binaryPayloads) {
  if (!chart || !chart.data) {
    return;
  }
  if (payloads.length !== binaryPayloads.length) {
    throw new Error("Binary dataset metadata count does not match binary payload count.");
  }
  const datasetsById = createDatasetMap(chart.data.datasets);
  for (let i = 0; i < payloads.length; i++) {
    const payload = payloads[i];
    const dataset = datasetsById.get(payload.datasetId);
    if (dataset == void 0) {
      throw new Error(`Dataset '${payload.datasetId}' was not found.`);
    }
    dataset.data = decodeBinaryDatasetData(binaryPayloads[i], payload);
  }
  chart.update(updateMode ?? "none");
}
function addDatasetsCore(chart, datasets) {
  if (!chart || !chart.data) {
    return;
  }
  for (let i = 0; i < datasets.length; i++) {
    chart.data.datasets.push(datasets[i]);
  }
  chart.update();
}
async function addDatasets(chartId, setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions) {
  const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
  await resolveChartJsFunctions(
    resolvedArguments.setupOptions,
    { data: { datasets: resolvedArguments.datasets } },
    resolvedArguments.hasChartJsFunctions
  );
  const chart = getLiveChart(chartId);
  if (!chart || !resolvedArguments.datasets) {
    return;
  }
  addDatasetsCore(chart, resolvedArguments.datasets);
}
function addDatasetCore(chart, dataset, afterDatasetId) {
  if (!chart || !chart.data) {
    return;
  }
  if (afterDatasetId == void 0) {
    chart.data.datasets.push(dataset);
  } else {
    const datasetIndex = chart.data.datasets.findIndex((existingDataset) => existingDataset.id === afterDatasetId);
    if (datasetIndex >= 0) {
      chart.data.datasets.splice(datasetIndex + 1, 0, dataset);
    } else {
      chart.data.datasets.push(dataset);
    }
  }
  chart.update();
}
function removeDatasetsCore(chart, datasetIds) {
  if (!chart || !chart.data) {
    return;
  }
  const datasetIdSet = new Set(datasetIds);
  for (let index = chart.data.datasets.length - 1; index >= 0; index--) {
    const dataset = chart.data.datasets[index];
    if (dataset.id != void 0 && datasetIdSet.has(dataset.id)) {
      chart.data.datasets.splice(index, 1);
    }
  }
  chart.update();
}
function removeDatasets(chartId, datasetIds) {
  const chart = getLiveChart(chartId);
  if (!chart) {
    return;
  }
  removeDatasetsCore(chart, datasetIds);
}
function removeDataset(chartId, datasetId) {
  removeDatasets(chartId, [datasetId]);
}
function updateDatasetsSmoothCore(chart, datasets) {
  if (!chart || !chart.data) {
    return;
  }
  const existingDatasetsById = createDatasetMap(chart.data.datasets);
  for (let i = 0; i < datasets.length; i++) {
    const newDataset = datasets[i];
    const existingDataset = existingDatasetsById.get(newDataset.id);
    if (existingDataset != void 0) {
      assignDatasetSmooth(existingDataset, newDataset);
    }
  }
  chart.update();
}
async function updateDatasetsSmooth(chartId, setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions) {
  const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
  await resolveChartJsFunctions(
    resolvedArguments.setupOptions,
    { data: { datasets: resolvedArguments.datasets } },
    resolvedArguments.hasChartJsFunctions
  );
  const chart = getLiveChart(chartId);
  if (!chart || !resolvedArguments.datasets) {
    return;
  }
  updateDatasetsSmoothCore(chart, resolvedArguments.datasets);
}
function updateDatasetsCore(chart, datasets) {
  if (!chart || !chart.data) {
    return;
  }
  const datasetIndexesById = /* @__PURE__ */ new Map();
  for (let i = 0; i < chart.data.datasets.length; i++) {
    const datasetId = chart.data.datasets[i].id;
    if (datasetId != void 0) {
      datasetIndexesById.set(datasetId, i);
    }
  }
  for (let i = 0; i < datasets.length; i++) {
    const dataset = datasets[i];
    const datasetIndex = datasetIndexesById.get(dataset.id);
    if (datasetIndex != void 0) {
      chart.data.datasets[datasetIndex] = dataset;
    }
  }
  chart.update();
}
async function updateDatasets(chartId, setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions) {
  const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
  await resolveChartJsFunctions(
    resolvedArguments.setupOptions,
    { data: { datasets: resolvedArguments.datasets } },
    resolvedArguments.hasChartJsFunctions
  );
  const chart = getLiveChart(chartId);
  if (!chart || !resolvedArguments.datasets) {
    return;
  }
  updateDatasetsCore(chart, resolvedArguments.datasets);
}
function setDatasetsCore(chart, datasets) {
  if (!chart || !chart.data) {
    return;
  }
  chart.data.datasets = datasets;
  chart.update();
}
async function setDatasets(chartId, setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions) {
  const resolvedArguments = resolveDatasetListArguments(setupOptionsOrDatasets, datasetsOrHasChartJsFunctions, hasChartJsFunctions);
  await resolveChartJsFunctions(
    resolvedArguments.setupOptions,
    { data: { datasets: resolvedArguments.datasets } },
    resolvedArguments.hasChartJsFunctions
  );
  const chart = getLiveChart(chartId);
  if (!chart || !resolvedArguments.datasets) {
    return;
  }
  setDatasetsCore(chart, resolvedArguments.datasets);
}
function applyDatasetChangesSmoothCore(chart, desiredDatasetIds, datasetsToAdd, datasetsToUpdateSmooth, datasetIdsToRemove, labels, options, beforeUpdate) {
  if (!chart || !chart.data) {
    return;
  }
  if (labels != void 0) {
    chart.data.labels = labels;
  }
  if (options != void 0) {
    chart.options = options;
  }
  const removeDatasetIdSet = new Set(datasetIdsToRemove ?? []);
  const candidateDatasetsById = createDatasetMap(chart.data.datasets);
  for (let i = 0; i < datasetsToAdd.length; i++) {
    const dataset = datasetsToAdd[i];
    const datasetId = dataset.id;
    if (!removeDatasetIdSet.has(datasetId)) {
      candidateDatasetsById.set(datasetId, dataset);
    }
  }
  for (let i = 0; i < datasetsToUpdateSmooth.length; i++) {
    const newDataset = datasetsToUpdateSmooth[i];
    const datasetId = newDataset.id;
    const existingDataset = candidateDatasetsById.get(datasetId);
    if (!removeDatasetIdSet.has(datasetId) && existingDataset != void 0) {
      assignDatasetSmooth(existingDataset, newDataset);
    }
  }
  const finalDatasets = [];
  for (let i = 0; i < desiredDatasetIds.length; i++) {
    const datasetId = desiredDatasetIds[i];
    const dataset = candidateDatasetsById.get(datasetId);
    if (!removeDatasetIdSet.has(datasetId) && dataset != void 0) {
      finalDatasets.push(dataset);
    }
  }
  chart.data.datasets = finalDatasets;
  beforeUpdate?.();
  chart.update();
}
async function applyDatasetChangesSmooth(chartId, setupOptions, desiredDatasetIds, datasetsToAdd, datasetsToUpdateSmooth, datasetIdsToRemove, labels, options, hasChartJsFunctions) {
  const resolvedDatasetsToAdd = parseArrayPayload(datasetsToAdd) ?? [];
  const resolvedDatasetsToUpdateSmooth = parseArrayPayload(datasetsToUpdateSmooth) ?? [];
  const resolvedDatasetIdsToRemove = datasetIdsToRemove ?? [];
  const resolvedOptions = parsePayload(options);
  if (hasChartJsFunctions === true) {
    if (resolvedDatasetsToAdd.length > 0) {
      await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasetsToAdd } }, true);
    }
    if (resolvedDatasetsToUpdateSmooth.length > 0) {
      await resolveChartJsFunctions(setupOptions, { data: { datasets: resolvedDatasetsToUpdateSmooth } }, true);
    }
    if (resolvedOptions != void 0) {
      await resolveChartJsFunctions(setupOptions, { options: resolvedOptions }, true);
    }
  }
  const chart = getLiveChart(chartId);
  if (!chart || !desiredDatasetIds) {
    return;
  }
  applyDatasetChangesSmoothCore(
    chart,
    desiredDatasetIds,
    resolvedDatasetsToAdd,
    resolvedDatasetsToUpdateSmooth,
    resolvedDatasetIdsToRemove,
    labels,
    resolvedOptions,
    () => {
      if (resolvedOptions != void 0) {
        registerEvents(resolvedOptions, chartId, chart);
      }
    }
  );
}
function assignDatasetSmooth(existingDataset, newDataset) {
  Object.assign(existingDataset, newDataset);
  for (const prop in existingDataset) {
    if (Object.prototype.hasOwnProperty.call(existingDataset, prop) && !Object.prototype.hasOwnProperty.call(newDataset, prop)) {
      delete existingDataset[prop];
    }
  }
}
async function addChartDataset(chartId, setupOptionsOrDataset, datasetOrHasChartJsFunctions, hasChartJsFunctionsOrAfterDatasetId, afterDatasetId) {
  const hasSetupOptions = arguments.length >= 5 || isSetupOptions(setupOptionsOrDataset);
  const setupOptions = hasSetupOptions ? setupOptionsOrDataset : void 0;
  const datasetPayload = hasSetupOptions && typeof datasetOrHasChartJsFunctions !== "boolean" ? datasetOrHasChartJsFunctions : setupOptionsOrDataset;
  const dataset = parsePayload(datasetPayload);
  const resolvedHasChartJsFunctions = hasSetupOptions ? hasChartJsFunctionsOrAfterDatasetId : datasetOrHasChartJsFunctions;
  const resolvedAfterDatasetId = hasSetupOptions ? afterDatasetId : typeof hasChartJsFunctionsOrAfterDatasetId === "string" || hasChartJsFunctionsOrAfterDatasetId === null ? hasChartJsFunctionsOrAfterDatasetId : void 0;
  await resolveChartJsFunctions(setupOptions, { data: { datasets: [dataset] } }, resolvedHasChartJsFunctions === true);
  const chart = getLiveChart(chartId);
  if (!chart || !dataset) {
    return;
  }
  addDatasetCore(chart, dataset, resolvedAfterDatasetId);
}

// TypeScript/index.ts
var chartJsInteropGlobal = Object.assign(chartJsInterop, { resolveChartJsFunctions });
window.ChartJsInterop = chartJsInteropGlobal;
window.chartJsInteropVersion = chartJsInteropVersion;
export {
  addChartDataToDatasets,
  addChartDataset,
  addData,
  addDatasets,
  applyDatasetChangesSmooth,
  chartJsInteropVersion,
  disposeChart,
  getChartImage,
  getDataVisibility,
  getLabels,
  hideDataset,
  initChart,
  isDatasetVisible,
  removeData,
  removeDataset,
  removeDatasets,
  renderChart,
  resetChart,
  resizeChart,
  setData,
  setDatasetBinaryData,
  setDatasetPointsActive,
  setDatasetVisibility,
  setDatasets,
  setDatasetsBinaryData,
  setDatasetsData,
  setLabels,
  showDataset,
  stopChart,
  toggleDataVisibility,
  updateChartOptions,
  updateDatasets,
  updateDatasetsSmooth
};
