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

### Global Chart Options

- `ChartJsOptions.BackgroundColor`
- `ChartJsOptions.BorderColor`
- `ChartJsOptions.Clip`
- `ChartJsOptions.Color`
- `ChartJsOptions.Datasets`
- `ChartJsOptions.Font`
- `ChartJsOptions.Hover`
- `ChartJsOptions.HoverBackgroundColor`
- `ChartJsOptions.HoverBorderColor`
- `ChartJsOptions.Normalized`
- `ChartJsOptions.OnClick`
- `ChartJsOptions.OnHover`
- `ChartJsOptions.OnResize`

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

- `ChartJsOptions.Layout`

### Line Dataset

- `LineDataset.BorderCapStyle`
- `LineDataset.BorderDash`
- `LineDataset.BorderDashOffset`
- `LineDataset.BorderJoinStyle`
- `LineDataset.BorderWidth`
- `LineDataset.CubicInterpolationMode`
- `LineDataset.Fill`
- `LineDataset.HoverBorderCapStyle`
- `LineDataset.HoverBorderDash`
- `LineDataset.HoverBorderDashOffset`
- `LineDataset.HoverBorderJoinStyle`
- `LineDataset.HoverBorderWidth`
- `LineDataset.Segment`
- `LineDataset.Stepped`
- `LineDataset.Tension`

### Radar Dataset

- `RadarDataset.BorderCapStyle`
- `RadarDataset.BorderDash`
- `RadarDataset.BorderDashOffset`
- `RadarDataset.BorderJoinStyle`
- `RadarDataset.BorderWidth`
- `RadarDataset.Fill`
- `RadarDataset.HoverBorderCapStyle`
- `RadarDataset.HoverBorderDash`
- `RadarDataset.HoverBorderDashOffset`
- `RadarDataset.HoverBorderJoinStyle`
- `RadarDataset.HoverBorderWidth`
- `RadarDataset.Tension`

### Bar Dataset

- `BarDataset.BorderSkipped`

### Pie/Doughnut Dataset

- `PieDataset.BorderRadius`

### Scale Border and Grid

- `ChartJsAxisBorder.Dash`
- `ChartJsAxisBorder.DashOffset`
- `ChartJsGrid.LineWidth`
- `ChartJsGrid.TickBorderDash`
- `ChartJsGrid.TickBorderDashOffset`

### Tick Options

- `ChartJsAxisTick.Font`
- `ChartJsAxisTick.ShowLabelBackdrop`
- `ChartJsAxisTick.TextStrokeWidth`

### Title and Legend Label Fonts

- `Title.Font`
- `Labels.Font`

### Tooltip Options

- `Tooltip.BodyAlign`
- `Tooltip.BodyFont`
- `Tooltip.BodySpacing`
- `Tooltip.BorderWidth`
- `Tooltip.BoxHeight`
- `Tooltip.BoxWidth`
- `Tooltip.CaretPadding`
- `Tooltip.CaretSize`
- `Tooltip.CornerRadius`
- `Tooltip.DisplayColors`
- `Tooltip.Enabled`
- `Tooltip.FooterAlign`
- `Tooltip.FooterFont`
- `Tooltip.FooterMarginTop`
- `Tooltip.FooterSpacing`
- `Tooltip.Position`
- `Tooltip.Rtl`
- `Tooltip.TextDirection`
- `Tooltip.TitleAlign`
- `Tooltip.TitleFont`
- `Tooltip.TitleMarginBottom`
- `Tooltip.TitleSpacing`
- `Tooltip.UsePointStyle`
- `Tooltip.XAlign`
- `Tooltip.YAlign`

## Performance Notes For Follow-Up

- There are no database interactions in this library surface, so the relevant performance concerns are allocation and CPU during option construction and JSON serialization.
- Prefer strongly typed reusable option wrappers that serialize through the existing source-generated JSON path.
- Avoid reflection-heavy runtime shims for scriptable support.
- Be careful with wrappers around hot dataset fields so scalar-only use remains allocation-light.
