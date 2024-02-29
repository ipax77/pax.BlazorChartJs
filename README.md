![Nuget](https://img.shields.io/nuget/v/pax.BlazorChartJs)
[![Playwright Tests](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml/badge.svg)](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml) [TestPage](https://ipax77.github.io/pax.BlazorChartJs/)

# Blazor dotnet wrapper library for [ChartJs](https://github.com/chartjs/Chart.js)
 
 The following versions of ChartJs are compatible with published releases of `pax.BlazorChartJs`
 Release | ChartJs | Tests
 ---|---------------|---------------|
 <= 0.5.0 | **3.9.1** | 3.9.1
 &gt;= 0.5.0 | **4.x**   | 4.4.2
 
 
## Getting started
This library is using [JavaScript isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-6.0#javascript-isolation-in-javascript-modules-1). JS isolation provides the following benefits:
* Imported JS no longer pollutes the global namespace.
* Consumers of a library and components aren't required to import the related JS.
## Prerequisites
* dotnet 6/7 for versions < 0.8
* dotnet 8 for versions >= 0.8
## Installation

**dotnet 8**
```
dotnet add package pax.BlazorChartJs
```

**dotnet 6/7**
```
dotnet add package pax.BlazorChartJs --version 0.6.3
```

Program.cs:
``` cs
    builder.Services.AddChartJs(options =>
    {
        // default
        options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
        options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
    });
```

## Usage

Sample Project [pax.BlazorChartJs.samplelib](https://github.com/ipax77/pax.BlazorChartJs/tree/master/src/pax.BlazorChartJs.samplelib) with
[Sample Chart](https://ipax77.github.io/pax.BlazorChartJs/minchart)
```razor
@using pax.BlazorChartJs

<div class="btn-group">
    <button type="button" class="btn btn-primary" @onclick="Randomize">Randomize</button>
</div>
<div class="chart-container w-75">
    <ChartComponent @ref="chartComponent"
                    ChartJsConfig="chartJsConfig"
                    OnEventTriggered="ChartEventTriggered">
    </ChartComponent>
</div>

@code {
    ChartJsConfig chartJsConfig = null!;
    ChartComponent? chartComponent;
    private bool chartReady;

    protected override void OnInitialized()
    {
        chartJsConfig = new ChartJsConfig()
            {
                Type = ChartType.bar,
                Data = new ChartJsData()
                {
                    Labels = ["Jan", "Feb", "Mar"],
                    Datasets = new List<ChartJsDataset>()
                    {
                        new BarDataset()
                        {
                            Label = "Dataset 1",
                            Data = [ 1, 2, 3 ]
                        }
                    }
                }
            };
        base.OnInitialized();
    }

    private void ChartEventTriggered(ChartJsEvent chartJsEvent)
    {
        if (chartJsEvent is ChartJsInitEvent initEvent)
        {
            chartReady = true;
        }
    }

    private void Randomize()
    {
        if (!chartReady)
        {
            return;
        }

        List<ChartJsDataset> updateDatasets = [];
        
        foreach (var dataset in chartJsConfig.Data.Datasets)
        {
            if (dataset is BarDataset barDataset)
            {
                List<object> newData = new();
                foreach (var data in barDataset.Data)
                {
                    newData.Add(Random.Shared.Next(1, 10));
                }
                barDataset.Data = newData;
                updateDatasets.Add(dataset);
            }
        }
        chartJsConfig.UpdateDatasets(updateDatasets);
    }
}
```

## Update Chart
* To update the chart with the current ChartJsConfig call ```ChartJsConfig.ReinitializeChart()```
* To update the chart with smooth animations there are several helper functions available, e.g.:
    - ```ChartJsConfig.SetLabels(...)```
    - ```ChartJsConfig.AddData(...)```
    - ```ChartJsConfig.AddDataset(...)```
    - ```ChartJsConfig.SetData(...)```
* use ```ChartJsConfig.UpdateChartOptions()``` to update the chart options, only (e.g. [StepSize](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/StackedChartComp.razor#L106))

## Chart Events
Several chart events are available, by default only the Init event is fired. The others can be activated in the ChartJsConfig.Options [Sample](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/EventsComp.razor)
*  click
*  hover
*  leave
*  progress
*  complete
*  resize

## Supported Plugins
* [chartjs-plugin-datalabels](https://github.com/chartjs/chartjs-plugin-datalabels)
* [ArbitraryLines](https://www.youtube.com/watch?v=7ZZ_XfaJQbM&t=379s) (YouTube)
* Custom Plugins [Sample](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/CustomPluginComp.razor)

## ChartComponent
Several chart functions are available in the ChartComponent, e.g.:
* ```ChartComponent.ResizeChart(...)```
* ```ChartComponent.GetChartImage(...)```
* ```ChartComponent.ToggleDataVisibility(...)```

## Contributing

We really like people helping us with the project. Nevertheless, take your time to read our contributing guidelines [here](https://github.com/ipax77/pax.BlazorChartJs/blob/master/CONTRIBUTING.md).

## ChangeLog

<details open="open"><summary>v0.8.3-rc1</summary>

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
>- Removed chartjs-plugin-labels (you can still register it as [custom plugin](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/CustomPluginComp.razor))
>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.13
>- Added ScaleAxis X1 and Y1 (override ChartJsOptionsScales for other names)
>- ```ChartJsConfig.UpdateChartOptions()``` (will replace ```ChartComponent.UpdateChartOptions```)
>- ```ChartJsConfig.ReinitializeChart()``` (will replace ```ChartComponent.DrawChart```)

</details>

<details><summary>v0.4.1</summary>

>- Catch ObjectDisposedException and JSException when disposing the ChartComponent while initializing
>- Microsoft.AspNetCore.Components.Web upgrade to v6.0.12

</details>

<details><summary>v0.4.0</summary>

>- Title.Text is now IndexableOptions<string> - **Breaking Change!**
>- chartComponent?.DrawChart() triggeres an InitEvent after the chart is complete
>- ChartJsInitEvent does have the ChartJsConfigGuid set correctly, now
>- RemoveDataset(s) can now handle self referencing and missing

</details>

<details><summary>v0.3.5</summary>

>- TimeCartesianAxisTicks fix
>- Interactions fix
>- Playwright tests
>- ghpages
>- ChartComponent DisposeAsync

</details>

<details><summary>v0.3.4</summary>

>- Fix #7 - Axis Ticks JsonConverter
>- Added ChartJsInitEvent which is triggered when the chart finished initializing the first time
>- [StackedChart](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/StackedChartComp.razor) Sample

</details>

<details><summary>v0.3.3</summary>

>- Fix #6
>- chartComponent.UpdateChartDatasets removed - use chartConfig.SetDatasets() instead
>- Added Hidden option for Datasets

</details>

<details><summary>v0.3.2</summary>

>- Chart update refactoring - Breaking Changes!
>- Chart events refactoring - Breaking Changes!
>- Typescript
>- NuGet udpates

</details>

<details><summary>v0.3.1</summary>

>- Time Scale Chart
>- Optional javascript location options
>- ChartJs API calls
>- bugfixes
>- refactoring

</details>

<details><summary>v0.3.0</summary>

>- IndexableOption - Breaking Change!

</details>

<details><summary>v0.2.0</summary>

>- Events
>- Custom Plugin Sample
>- ChartJs API calls

</details>

<details><summary>v0.1.3</summary>

>- Nuget Package

</details>

<details><summary>v0.1.2</summary>

>- RadarChart

</details>


<details><summary>v0.1.1</summary>

>- Readme

</details>

<details><summary>v0.1.0</summary>

>- Init

</details>
