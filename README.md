![Nuget](https://img.shields.io/nuget/v/pax.BlazorChartJs)
[![Playwright Tests](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml/badge.svg)](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml) [TestPage](https://ipax77.github.io/pax.BlazorChartJs/)

# pax.BlazorChartJs

`pax.BlazorChartJs` is a Blazor wrapper for [Chart.js](https://github.com/chartjs/Chart.js). It renders charts from typed .NET configuration models through a reusable `ChartComponent` and a JavaScript isolation module.

## Compatibility

Release | Chart.js | Tested Chart.js
---|---|---
&gt;= 0.5.0 | **4.x** | 4.5.1

## Highlights

- Typed chart configuration models for common Chart.js charts, datasets, scales, plugins, callbacks, and scriptable/indexable options.
- A Blazor `ChartComponent` that owns chart lifecycle, event forwarding, image capture, resizing, visibility, and legend helpers.
- Targeted chart updates for labels, data, datasets, and options, including one batched smooth dataset synchronization path.
- Registered JavaScript callbacks referenced from C# through `ChartJsFunction` instead of raw JavaScript in serialized chart configs.
- App-wide Chart.js defaults through `AddChartJs(...)` and per-chart options when a project needs shared styling or callbacks.
- Browser-tested sample pages that show runnable charts together with the C# and JavaScript needed for the Chart.js sample behavior.

## New in v0.9

- `ChartJsFunction` and callback module support for tooltip, legend, tick, datalabel, dataset, padding, and other scriptable option paths.
- Expanded typed coverage for Chart.js core options, element options, dataset defaults, tooltip callbacks, decimation, and remaining v4.5.1 option gaps.
- `ChartJsSetupOptions.Defaults`, `ChartJsDefaultsOptions`, and `ChartJsOptionsDatasets` for app-wide `Chart.defaults` configuration.
- `ChartJsConfig.SetDatasetsSmooth(...)` to add, update, remove, reorder, and optionally relabel datasets in one smooth update.
- `ChartJsConfig.SetDatasetBinaryData(...)` and `ChartJsBinaryPayload` helpers for large Y and XY dataset updates without JSON-serializing the data arrays.
- A Chart.js sample gallery with official sample sections, sample actions, visible C#/JavaScript code, and Playwright coverage.

## Samples

Start with the live [TestPage](https://ipax77.github.io/pax.BlazorChartJs/) or the reusable [sample library](https://github.com/ipax77/pax.BlazorChartJs/tree/main/src/pax.BlazorChartJs.samplelib).

The Chart.js sample gallery mirrors the official sample set as runnable Blazor components. It covers bar, line, area, other chart types, scales, scale options, legends, titles, subtitles, tooltips, scriptable options, animations, advanced samples, and plugin samples. Library-specific demos cover events, HTML legends, datalabels, callback modules, plugin registration, update flows, and multiple-chart scenarios.

## Installation

```powershell
dotnet add package pax.BlazorChartJs
```

Program.cs:
```csharp
    builder.Services.AddChartJs(options =>
    {
        // default
        options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
        options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
    });
```
If you want to serve Chart.js locally, you can provide your own URLs:
```csharp
    builder.Services.AddChartJs(options =>
    {
        var version = "4.5.1";
        options.ChartJsLocation = $"/_content/dsstats.weblib/js/chart.umd.min.js?v={version}";
        options.ChartJsPluginDatalabelsLocation = "/_content/dsstats.weblib/js/chartjs-plugin-datalabels.min.js";
    });
```

The wrapper uses [Blazor JavaScript isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-6.0#javascript-isolation-in-javascript-modules-1), so consumers do not need to add a global interop script to the page and the module does not pollute the global JavaScript namespace.

## Minimal chart

```razor
@using pax.BlazorChartJs

<div class="chart-container w-75">
    <ChartComponent ChartJsConfig="chartJsConfig" />
</div>

@code {
    private readonly ChartJsConfig chartJsConfig = new()
    {
        Type = ChartType.bar,
        Data = new ChartJsData()
        {
            Labels = ["Jan", "Feb", "Mar"],
            Datasets =
            [
                new BarDataset()
                {
                    Label = "Dataset 1",
                    Data = [1, 2, 3]
                }
            ]
        }
    };
}
```

See the sample [minimal chart](https://ipax77.github.io/pax.BlazorChartJs/minchart) for a complete component and the rest of the sample library for chart-type examples.

## Updates and performance

Prefer targeted updates when an existing chart can be changed safely:

- `ChartJsConfig.SetLabels(...)`
- `ChartJsConfig.AddData(...)` and `ChartJsConfig.SetData(...)`
- `ChartJsConfig.AddDatasetSmooth(...)` and `ChartJsConfig.SetDatasetsSmooth(...)`
- `ChartJsConfig.UpdateChartOptions()` for options-only changes
- `ChartJsConfig.ReinitializeChart()` when a full chart reinitialization is actually needed

`SetDatasetsSmooth(...)` adds, updates, removes, and reorders datasets by id in one smooth chart update. Optional labels and current options can be applied in the same pass:

```csharp
chartJsConfig.Options ??= new ChartJsOptions();
chartJsConfig.Options.Responsive = false;

chartJsConfig.SetDatasetsSmooth(
    datasets:
    [
        new BarDataset
        {
            Id = "dataset-2",
            Label = "Dataset 2",
            Data = [ 4, 5, 6 ]
        },
        new BarDataset
        {
            Id = "dataset-1",
            Label = "Dataset 1",
            Data = [ 3, 2, 1 ]
        }
    ],
    labels: ["Apr", "May", "Jun"],
    updateOptions: true);
```

### Binary dataset updates

Use `ChartJsConfig.SetDatasetBinaryData(...)` to update a dataset by id from a packed binary payload without serializing large data arrays as JSON. `ChartJsBinaryPayload` creates compact payloads for common Y and XY layouts:

```csharp
chartJsConfig.SetDatasetBinaryData(
    ChartJsBinaryPayload.FromY("dataset-1", new float[] { 10, 12, 14 }));

var points = new ChartJsPoint[]
{
    new(1, 20),
    new(2, 21),
    new(3, 22)
};

chartJsConfig.SetDatasetBinaryData(
    ChartJsBinaryPayload.FromXY("dataset-2", points));
```

`FromY(...)` accepts `int`, `float`, or `double` spans. `FromXY(...)` accepts `ChartJsPoint` spans or interleaved `double` values. Custom strided binary layouts can still be passed with `ChartJsBinaryDatasetPayload` directly.

## ChartJsFunction callbacks

`ChartJsFunction.FromName(...)` can be used to reference JavaScript callbacks from C# chart configuration without serializing raw JavaScript into the config. Configure a callback module that exports a `chartJsCallbacks` object:

```csharp
builder.Services.AddChartJs(options =>
{
    options.ChartJsCallbacksModuleLocation = $"{builder.HostEnvironment.BaseAddress}_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
});
```

Then reference registered callback names from chart options or datasets:

```csharp
new BarDataset()
{
    Label = "Dataset 1",
    Data = new List<object>() { 1, 2, 3 },
    BackgroundColor = ChartJsFunction.FromName("createRepeatFillPattern")
}

new Legend()
{
    Labels = new Labels()
    {
        Filter = ChartJsFunction.FromName("showLegendItem")
    }
}

new LinearAxisTick()
{
    Callback = ChartJsFunction.FromName("formatCurrency")
}
```

Callback names are validated and resolved from the configured module, which avoids raw JavaScript serialization in the chart config. See the full [ChartJsFunction callback sample](https://github.com/ipax77/pax.BlazorChartJs/blob/main/src/pax.BlazorChartJs.samplelib/EventcallbackChartComp.razor).

## Global defaults

Beyond script locations and callback modules, `AddChartJs(...)` can configure app-wide Chart.js defaults.

```csharp
builder.Services.AddChartJs(options =>
{
    options.ChartJsCallbacksModuleLocation = $"{builder.HostEnvironment.BaseAddress}_content/my-app/chartJsCallbacks.js";

    options.Defaults = new ChartJsDefaultsOptions()
    {
        Color = "#1f2937",
        BorderColor = "#d1d5db",
        Font = new Font
        {
            Family = "Inter, system-ui, sans-serif",
            Size = 12
        },
        Datasets = new ChartJsOptionsDatasets()
        {
            Bar = new
            {
                barPercentage = 0.8,
                categoryPercentage = 0.9
            },
            Line = new
            {
                tension = 0.25
            }
        },
        OnClick = ChartJsFunction.FromName("globalChartClick")
    };
});
```

`Defaults` maps to `Chart.defaults` and is applied after Chart.js is loaded and before the first chart is constructed. Per-chart `ChartJsConfig.Options` still override the global defaults. `ChartJsFunction` values in defaults use the same callback registry configured by `ChartJsCallbacksModuleLocation`.

## Chart events

Several chart events are available. By default only the init event is emitted; enable the others in `ChartJsConfig.Options`. See the [events sample](https://github.com/ipax77/pax.BlazorChartJs/blob/main/src/pax.BlazorChartJs.samplelib/EventsComp.razor).

- click
- hover
- leave
- progress
- complete
- resize

## Supported Plugins

- [chartjs-plugin-datalabels](https://github.com/chartjs/chartjs-plugin-datalabels)
- [ArbitraryLines](https://www.youtube.com/watch?v=7ZZ_XfaJQbM&t=379s) (YouTube)
- Custom plugins through the [custom plugin sample](https://github.com/ipax77/pax.BlazorChartJs/blob/main/src/pax.BlazorChartJs.samplelib/CustomPluginComp.razor)

## ChartComponent

Several chart functions are available in the ChartComponent, e.g.:

- `ChartComponent.ResizeChart(...)`
- `ChartComponent.GetChartImage(...)`
- `ChartComponent.ToggleDataVisibility(...)`

## Contributing

We really like people helping us with the project. Nevertheless, take your time to read our contributing guidelines [here](https://github.com/ipax77/pax.BlazorChartJs/blob/main/CONTRIBUTING.md).

### TypeScript interop

The TypeScript source for the packaged JavaScript isolation module lives in `src/pax.BlazorChartJs/TypeScript`.
When those sources are newer than the tracked bundle, `dotnet build` restores the local Node packages and regenerates
`wwwroot/chartJsInterop.js` before the library static web assets are resolved. Node and npm are required for that
regeneration path.

To regenerate the bundled browser asset directly:

```powershell
cd src\pax.BlazorChartJs
npm install
npm run bundle
```

The bundle command writes the single shipped module to `wwwroot/chartJsInterop.js`.

## Changelog

<details open="open"><summary>v0.9.1</summary>

>- 

</details>

<details><summary>v0.9.0</summary>

>- **Breaking change:** font option properties that now support scriptable values use `IndexableOption<Font>` in those contexts. Target-typed `Font = new()` no longer binds there; use `Font = new Font { ... }` or a `ChartJsFunction` callback.
>- Added `ChartJsFunction` to reference registered JavaScript callbacks from C# chart configuration without serializing raw JavaScript.
>- Added callback module configuration and marker revival for chart initialization, option updates, and dataset add/update/set interop calls.
>- Added scriptable callback support for datalabel formatters, axis ticks, tooltip callbacks, legend callbacks, and indexable color options.
>- Expanded `IndexableOption<T>` to support single values, indexed values, and `ChartJsFunction` callback values.
>- Expanded `Padding` to support Chart.js numeric padding, `{x, y}` shorthand padding, and scriptable padding callbacks while preserving existing `Padding?` property types.
>- Added a scriptable padding sample with a sample-only `Latest` label plugin.
>- Hardened callback resolution with flat JavaScript identifier validation and reserved-name checks.
>- Updated sample callback charts to use a shared `chartJsCallbacks.js` callback registry.
>- Completed the Chart.js v4.5.1 option coverage pass with expanded core, element, dataset, tooltip, label, scale, and decimation option support.
>- Added missing global/core Chart.js options to `ChartJsOptions`: `BackgroundColor`, `BorderColor`, `Clip`, `Color`, `Datasets`, `Font`, `Hover`, `HoverBackgroundColor`, `HoverBorderColor`, `Normalized`, `OnClick`, `OnHover`, and `OnResize`.
>- Added `ChartJsSetupOptions.Defaults` / `ChartJsDefaultsOptions` to configure app-wide `Chart.defaults` values through `AddChartJs(...)`.
>- Added `ChartJsOptionsDatasets` for `options.datasets` and `Chart.defaults.datasets` chart-type defaults.
>- Chart.js native `OnClick`, `OnHover`, and `OnResize` callbacks are preserved when the Blazor/C# event bridge flags are enabled.
>- Added `ChartJsConfig.SetDatasetsSmooth(...)` to add, update, remove, and reorder datasets by id in one smooth batched chart update, with optional labels and current options update.
>- Added binary dataset updates through `ChartJsConfig.SetDatasetBinaryData(...)` and compact `ChartJsBinaryPayload` helper factories for large Y and XY data updates.
>- Added the official Chart.js sample gallery as runnable Blazor sample pages with visible C#/JavaScript code, sample actions, and Playwright coverage.

</details>

<details><summary>v0.8.8</summary>

>- Dataset interop calls are ignored safely when the target chart was already disposed.
>- Reduced allocation and lookup work while resolving and disposing Chart.js instances.
>- Refactored library and sample code for .NET 11 analyzer/style guidance.

</details>

<details><summary>v0.8.7</summary>

>- Hardened chart initialization to better handle rapid reinitialization and existing Chart.js instances for the same canvas.
>- Resize events now include browser viewport dimensions via `WindowWidth` and `WindowHeight`.
>- `ChartJsResizeEvent.Width` and `Height` remain chart/container dimensions; use `WindowWidth` and `WindowHeight` for viewport breakpoint logic.
>- `ChartJsInitEvent` now includes initial chart and viewport dimensions via `Width`, `Height`, `WindowWidth`, and `WindowHeight`.
>- Dataset updates now match by dataset id across all chart datasets, including hidden datasets.
>- Update Microsoft.TypeScript.MSBuild to v6.0.3

</details>

<details><summary>v0.8.6</summary>

>- Updated to .NET 10
>- Full JavaScript generation from TypeScript
>- Chart.js v4.5.1 test coverage
>- Improved JSON serialization to be more AOT‑friendly

</details>

<details><summary>v0.8.5</summary>

>- Microsoft.AspNetCore.Components.Web v8.0.*
>- ChartJs v4.4.5 tests
>- Test/Sample projects update to dotnet v8.0.10

</details>

<details><summary>v0.8.4</summary>

>- Microsoft.AspNetCore.Components.Web v8.0.8
>- ChartJs v4.4.4 tests

</details>

<details><summary>v0.8.3</summary>

>- Microsoft.AspNetCore.Components.Web v8.0.2
>- Added ChartJsConfig.UpdateDatasetsSmooth updates the ChartJs dataset(s), instead of replacing.
>- Added BlazorLegendBase that can be used for a [ChartJs Html Legend](https://www.chartjs.org/docs/latest/samples/legend/html.html) - [Sample][https://ipax77.github.io/pax.BlazorChartJs/htmllegendchart]
>- Added ChartComponent.GetLegendItems()
>- Added ChartComponent.IsDatasetVisible(int datasetIndex)
>- Added ChartComponent.SetDatasetPointsActive(int datasetIndex)
>- BarDataset.BarPercentage changed from int? to double?
>- Renamed Layout to ChartJsLayout (CA1724)
>- `IndexableOption` now supports Collection Expressions e.g.
```csharp
    BorderColor = ["rgba(255, 99, 132, 1)", "rgba(54, 162, 235, 1)"],
    BorderWidth = [1, 2]
```
>- ChartJs v4.4.2 Tests
>- Blazor App sample 

</details>

<details><summary>v0.8.2</summary>

>- ChartJs v4.4.1 tests
>- Catching (more) dispose exeptions when switching from SSR to CSR (rendermode auto - AggregateException, JSDisconnectedException)

</details>

<details><summary>v0.8.1</summary>

>- dotnet 8 **Breaking Change**
>- Added missing pie/doughnut dataset options (Cutout, Radius, Animation)
>- The `IndexableOption` now supports implicit operators, allowing a more concise syntax for initialization.

**New Syntax:**
```csharp
BorderColor = new List<string>()
{
    "rgba(255, 99, 132, 1)",
    "rgba(54, 162, 235, 1)"
},
BorderWidth = 1
```

Old Syntax (still possible):
```csharp
BorderColor = new IndexableOption<string>(new List<string>()
{
    "rgba(255, 99, 132, 1)",
    "rgba(54, 162, 235, 1)"
}),
BorderWidth = new IndexableOption<double>(1)
```

</details>

<details><summary>v0.6.3</summary>

>- Reverted Microsoft.TypeScript.MSBuild to v5.2.2
Microsoft.TypeScript.MSBuild v5.3.2 not working for Blazor projects (only working for wasm)

</details>

<details><summary>v0.6.2</summary>

>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.25
>- Added missing pie/doughnut dataset options (Cutout, Radius, Animation)
>- The `IndexableOption` now supports implicit operators, allowing a more concise syntax for initialization.

**New Syntax:**
```csharp
BorderColor = new List<string>()
{
    "rgba(255, 99, 132, 1)",
    "rgba(54, 162, 235, 1)"
},
BorderWidth = 1
```

Old Syntax (still possible):
```csharp
BorderColor = new IndexableOption<string>(new List<string>()
{
    "rgba(255, 99, 132, 1)",
    "rgba(54, 162, 235, 1)"
}),
BorderWidth = new IndexableOption<double>(1)
```

</details>

<details><summary>v0.6.1</summary>

>- ChartJsLabelClickEvent and ChartJsLabelHoverEvent with 'nearest' DatasetLabel and DatasetIndex
>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.21

</details>

<details><summary>v0.6.0</summary>

>- Fix typo for AngleLines in LinearRadialAxis **Breaking Change**

</details>

<details><summary>v0.5.2</summary>

>- Datalabels per dataset contributed by pjvinson
>- ChartJs v4.4.0 tests

</details>

<details><summary>v0.5.1</summary>

>- Marked ChartJsGrid border options as obsolete for v4.x - use ChartJsAxisBorder instead.
>- TimeSeriesAxis Min, Max, SuggestedMin and SuggestedMax are of type ```StringOrDoubleValue```, now.
>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.16
>- ChartJs v4.3.0 tests
>- ChartJs v4.3.1 tests
>- ChartJs v4.3.3 tests

</details>

<details><summary>v0.5.0</summary>

>- **Breaking Changes**
>- Update to [ChartJs v4.x](https://www.chartjs.org/docs/latest/migration/v4-migration.html)
>- Removed ChartJs javascript files - defaults to cdn links, now. Use ``` options.ChartJsLocation = "mychart.js"``` to use a custom/local ChartJs version.
>- Removed chartjs-plugin-labels (you can still register it as [custom plugin](https://github.com/ipax77/pax.BlazorChartJs/blob/main/src/pax.BlazorChartJs.samplelib/CustomPluginComp.razor))
>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.13
>- Added ScaleAxis X1 and Y1 (override ChartJsOptionsScales for other names)
>- ```ChartJsConfig.UpdateChartOptions()``` (will replace ```ChartComponent.UpdateChartOptions```)
>- ```ChartJsConfig.ReinitializeChart()``` (will replace ```ChartComponent.DrawChart```)

</details>

