[![Playwright Tests](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml/badge.svg)](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml) [TestPage](https://ipax77.github.io/pax.BlazorChartJs/)

Blazor dotnet wrapper library for [ChartJs](https://github.com/chartjs/Chart.js)
 * **ChartJs v4.x support** (tested with ChartJs v4.2.0)
 
# Getting started
This library is using [JavaScript isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/?view=aspnetcore-6.0#javascript-isolation-in-javascript-modules-1). JS isolation provides the following benefits:
* Imported JS no longer pollutes the global namespace.
* Consumers of a library and components aren't required to import the related JS.
## Prerequisites
dotnet 6
## Installation

```
dotnet add package pax.BlazorChartJs
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
<div class="btn-group">
    <button type="button" class="btn btn-primary" @onclick="Randomize">Randomize</button>
</div>
<div class="w-75 h-50">
    <ChartComponent
        @ref="chartComponent"
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
                Labels = new List<string>() { "Jan", "Feb", "Mar" },
                Datasets = new List<ChartJsDataset>()
                {
                    new BarDataset()
                    {
                        Label = "Dataset 1",
                        Data = new List<object>() { 1, 2, 3 }
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

        Random random = new();
        Dictionary<ChartJsDataset, SetDataObject> chartData = new();

        foreach (var dataset in chartJsConfig.Data.Datasets)
        {
            if (dataset is BarDataset barDataset)
            {
                List<object> newData = new();
                foreach (var data in barDataset.Data)
                {
                    newData.Add(random.Next(1, 10));
                }
                chartData[dataset] = new(newData);
            }
        }
        chartJsConfig.SetData(chartData);
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

## ChangeLog

<details open="open"><summary>v0.5.0</summary>

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
