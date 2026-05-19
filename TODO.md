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
