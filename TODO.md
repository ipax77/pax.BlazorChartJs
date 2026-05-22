# Chart.js v4.5.1 Option Gaps

This list was built by comparing the local C# option records with the current Chart.js `latest` API docs, which currently identify themselves as Chart.js v4.5.1.

Sources:

- https://www.chartjs.org/docs/latest/api/
- https://www.chartjs.org/docs/latest/general/options.html
- https://www.chartjs.org/docs/latest/api/interfaces/CoreChartOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/BarControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/LineControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/DoughnutControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/PolarAreaControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/RadarControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/BubbleControllerDatasetOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/TooltipOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/LegendOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/TitleOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/GridLineOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/BorderOptions.html
- https://www.chartjs.org/docs/latest/api/interfaces/TickOptions.html

## Missing Properties Still Outstanding

None currently listed from the v4.5.1 comparison pass.

## Missing Properties Implemented In Global Options Pass

### Global Chart Options

- Added to `ChartJsOptions`: `BackgroundColor`, `BorderColor`, `Clip`, `Color`, `Datasets`, `Font`, `Hover`, `HoverBackgroundColor`, `HoverBorderColor`, `Normalized`, `OnClick`, `OnHover`, `OnResize`.
- Added app-wide `ChartJsSetupOptions.Defaults`, applied to `Chart.defaults` after Chart.js is loaded and before chart construction.
- Added `ChartJsOptionsDatasets` for `options.datasets` / `Chart.defaults.datasets` with chart-type keys and flexible option-object values.

## Missing Properties Implemented In Non-Global Pass

### Dataset-Level Common Options

- Added to `ChartJsDataset`: `Animation`, `Animations`, `Transitions`, `Normalized`.

### Dataset-Specific Options

- Added to `LineDataset`: `CapBezierPoints`.
- Added to `BubbleDataset`: `IndexAxis`, `Stack`, `XAxisID`, `YAxisID`.
- Added to `PieDataset`: `BorderDash`, `BorderDashOffset`, `Circular`, `HoverBorderDash`, `HoverBorderDashOffset`, `IndexAxis`, `Label`, `Order`, `SelfJoin`, `Stack`.
- Added to `PolarAreaDataset`: `Angle`, `BorderDash`, `BorderDashOffset`, `BorderJoinStyle`, `BorderRadius`, `Circumference`, `HoverBorderDash`, `HoverBorderDashOffset`, `HoverOffset`, `IndexAxis`, `Label`, `Offset`, `Order`, `Rotation`, `SelfJoin`, `Spacing`, `Stack`, `Weight`.
- Added to `RadarDataset`: `CapBezierPoints`, `CubicInterpolationMode`, `DrawActiveElementsOnTop`, `HitRadius`, `HoverRadius`, `IndexAxis`, `Radius`, `Rotation`, `Segment`, `ShowLine`, `Stack`, `Stepped`, `XAxisID`, `YAxisID`.

### Other Non-Global Options

- Added to `ChartJsAxis`: `Clip`; inherited by cartesian axes.
- Added to `Tooltip`: `Animation`, `Animations`, `Axis`, `IncludeInvisible`.
- Added to `Labels`: `BorderRadius`, `UseBorderRadius`.

## Existing Properties Missing Scriptable Support

Chart.js scriptable options accept a JavaScript function in addition to their normal scalar/object value. The local library generally supports this through `IndexableOption<T>`, `ChartJsFunction`, or specialized wrappers such as `Padding`.

### Global Chart Options

None currently listed from the v4.5.1 comparison pass.

## Scriptable Support Implemented In Non-Global Pass

- Added scriptable support to the listed line, radar, bar, pie/doughnut, scale border, grid, tick, title, legend label, and tooltip properties.
- Added `IndexableOption<Font>` serialization support for scriptable font options.
- Completed the remaining global-options pass, including scriptable `ChartJsOptions.Layout` support through the existing `Padding` wrapper and `ChartJsFunction` callback resolution path.

## Performance Notes For Follow-Up

- There are no database interactions in this library surface, so the relevant performance concerns are allocation and CPU during option construction and JSON serialization.
- Prefer strongly typed reusable option wrappers that serialize through the existing source-generated JSON path.
- Avoid reflection-heavy runtime shims for scriptable support.
- Be careful with wrappers around hot dataset fields so scalar-only use remains allocation-light.

## Official Chart.js Sample Porting Notes

This section tracks the Blazor sample-library work for the official Chart.js docs samples.

Sources:

- https://www.chartjs.org/docs/latest/samples/
- https://www.chartjs.org/docs/latest/samples/information.html
- https://www.chartjs.org/docs/latest/samples/bar/border-radius.html
- https://www.chartjs.org/docs/latest/samples/line/line.html
- https://www.chartjs.org/docs/latest/samples/other-charts/bubble.html
- https://www.chartjs.org/docs/latest/samples/area/line-boundaries.html
- https://www.chartjs.org/docs/latest/samples/scales/time-combo.html
- https://www.chartjs.org/docs/latest/samples/scale-options/center.html
- https://www.chartjs.org/docs/latest/samples/legend/events.html
- https://www.chartjs.org/docs/latest/samples/title/alignment.html
- https://www.chartjs.org/docs/latest/samples/subtitle/basic.html
- https://www.chartjs.org/docs/latest/samples/tooltip/position.html
- https://www.chartjs.org/docs/latest/samples/scriptable/line.html
- https://www.chartjs.org/docs/latest/samples/animations/drop.html
- https://www.chartjs.org/docs/latest/samples/advanced/data-decimation.html
- https://www.chartjs.org/docs/latest/samples/plugins/chart-area-border.html

### Completed Sample Sections

- [x] Bar Charts: border radius, floating bars, horizontal bar, stacked bar, stacked groups, vertical bar.
- [x] Line Charts: interpolation modes, line chart, multi-axis line, point styling, line segments, stepped line, line styling.
- [x] Other charts: bubble, combo bar/line, doughnut, multi-series pie, pie, polar area, polar area centered point labels, radar, radar skip points, scatter, scatter multi-axis, stacked bar/line.

### What We Learned

- Official docs samples are not copy/paste standalone examples. The docs hide utility/setup code, generate data through `Utils`, and turn the `actions` block into buttons at build time.
- The Blazor samples should mirror the docs surface: title, docs link, runnable chart, every official button, and visible C#/JavaScript setup/action code.
- Use `ChartJsDocsSampleLayout` and `ChartJsDocsBaseComponent` for code tabs, action buttons, and stable Playwright selectors (`data-chartjs-sample`, `data-sample-action`).
- If a sample needs direct Chart.js instance behavior, such as doughnut hide/show, expose the internal `ChartComponent` through the layout instead of duplicating layout markup.
- Prefer targeted updates: `SetData`, `AddData`, `RemoveData`, `AddDataset`, `RemoveDataset`, `UpdateDatasets`, and `UpdateChartOptions`. Reinitialize only when the Chart.js behavior truly requires a new chart.
- Keep `TypeScript/chartJsInterop.ts` and `wwwroot/chartJsInterop.js` synchronized whenever an interop method, callback revival path, payload shape, or guard changes.
- Use mutable label/data collections in samples. The core add/remove helpers now tolerate fixed-size/read-only collections, but sample setup should still avoid array-backed collections when the sample mutates data.
- Multi-axis samples need the right typed axis model. For cartesian-only options such as `Position = "right"`, use `CartesianAxis`; otherwise options can serialize but Chart.js will not behave as intended.
- Hide/show interop must handle invalid dataset/data indexes as a no-op and should not call Chart.js visibility APIs for deleted datasets.
- For callback samples, add named callbacks to `chartJsCallbacks.js`, reference them with `ChartJsFunction.FromName`, and show the callback JavaScript in the sample.
- Performance work is allocation/CPU/serialization/JS-interop focused. There is no database work in this library surface.

### How To Add A Docs Sample

1. Verify the current official docs page and record the exact title, URL, code tabs, setup block, action block, and callback/plugin behavior.
2. Add one concrete Razor component per official chart sample in the matching `ChartJsSamples` section folder. If the section has several similar samples, shared base classes or helper models are fine, but the router/section switch should still render a distinct component for each chart sample rather than one parameterized mega-sample.
3. Build the chart with typed C# configuration first. Add small library abstractions only when they make the sample fully functional and are generally useful.
4. Implement every official action button. Match the docs behavior and use targeted chart updates where practical.
5. Add visible C# and JavaScript code tabs for config/setup/actions. Add a callbacks/plugin tab when behavior depends on `chartJsCallbacks.js` or a custom plugin.
6. Wire the sample into the wasm test app route and nav under the matching docs section.
7. Add Playwright coverage for page render, canvas presence, code tabs, and representative button/callback behavior. Add regression tests for any issue found while porting.
8. Run the smallest useful verification set. For sample-only docs/components, build the wasm test app and run the focused Playwright section. 
9. Before finishing a section, take one extra performance pass: avoid avoidable list/dictionary allocations, avoid interop calls inside loops when a batched helper exists, and keep large-data samples such as decimation from doing unnecessary CPU work.

### Remaining Official Samples Backlog

Re-check the docs section sidebar before implementing each group because the `latest` docs can add or rename sample pages.

- [x] Area charts
  - [x] Line Chart Boundaries (`samples/area/line-boundaries.html`)
  - [x] Line Chart Datasets (`samples/area/line-datasets.html`)
  - [x] Line Chart drawTime (`samples/area/line-drawtime.html`)
  - [x] Line Chart Stacked (`samples/area/line-stacked.html`)
  - [x] Radar Chart Stacked (`samples/area/radar.html`)

- [x] Scales
  - [x] Linear Scale - Min-Max (`samples/scales/linear-min-max.html`)
  - [x] Linear Scale - Suggested Min-Max (`samples/scales/linear-min-max-suggested.html`)
  - [x] Linear Scale - Step Size (`samples/scales/linear-step-size.html`)
  - [x] Log Scale (`samples/scales/log.html`)
  - [x] Stacked Linear / Category (`samples/scales/stacked.html`)
  - [x] Time Scale (`samples/scales/time-line.html`)
  - [x] Time Scale - Max Span (`samples/scales/time-max-span.html`)
  - [x] Time Scale - Combo Chart (`samples/scales/time-combo.html`)

- [x] Scale Options
  - [x] Center Positioning (`samples/scale-options/center.html`)
  - [x] Grid Configuration (`samples/scale-options/grid.html`)
  - [x] Tick Configuration (`samples/scale-options/ticks.html`)
  - [x] Title Configuration (`samples/scale-options/titles.html`)

- [x] Legend
  - [x] Events (`samples/legend/events.html`)
  - [x] HTML Legend (`samples/legend/html.html`)
  - [x] Point Style (`samples/legend/point-style.html`)
  - [x] Position (`samples/legend/position.html`)
  - [x] Alignment and Title Position (`samples/legend/title.html`)

- [x] Title
  - [x] Alignment (`samples/title/alignment.html`)

- [x] Subtitle
  - [x] Basic (`samples/subtitle/basic.html`)

- [x] Tooltip
  - [x] Custom Tooltip Content (`samples/tooltip/content.html`)
  - [x] External HTML Tooltip (`samples/tooltip/html.html`)
  - [x] Interaction Modes (`samples/tooltip/interactions.html`)
  - [x] Point Style (`samples/tooltip/point-style.html`)
  - [x] Position (`samples/tooltip/position.html`)

- [x] Scriptable Options
  - [x] Bar Chart (`samples/scriptable/bar.html`)
  - [x] Bubble Chart (`samples/scriptable/bubble.html`)
  - [x] Line Chart (`samples/scriptable/line.html`)
  - [x] Pie Chart (`samples/scriptable/pie.html`)
  - [x] Polar Area Chart (`samples/scriptable/polar.html`)
  - [x] Radar Chart (`samples/scriptable/radar.html`)

- [x] Animations
  - [x] Delay (`samples/animations/delay.html`)
  - [x] Drop (`samples/animations/drop.html`)
  - [x] Loop (`samples/animations/loop.html`)
  - [x] Progressive Line (`samples/animations/progressive-line.html`)
  - [x] Progressive Line With Easing (`samples/animations/progressive-line-easing.html`)

- [ ] Advanced
  - [x] Data Decimation (`samples/advanced/data-decimation.html`)
  - [x] Derived Axis Type (`samples/advanced/derived-axis-type.html`)
  - [x] Derived Chart Type (`samples/advanced/derived-chart-type.html`)
  - [x] Linear Gradient (`samples/advanced/linear-gradient.html`)
  - [ ] Programmatic Event Triggers (`samples/advanced/programmatic-events.html`)
  - [ ] Animation Progress Bar (`samples/advanced/progress-bar.html`)
  - [ ] Radial Gradient (`samples/advanced/radial-gradient.html`)

- [ ] Plugins
  - [ ] Chart Area Border (`samples/plugins/chart-area-border.html`)
  - [ ] Doughnut Empty State (`samples/plugins/doughnut-empty-state.html`)
  - [ ] Quadrants (`samples/plugins/quadrants.html`)
