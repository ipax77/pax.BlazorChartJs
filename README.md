[![Playwright Tests](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml/badge.svg)](https://github.com/ipax77/pax.BlazorChartJs/actions/workflows/pwtests.yml) [TestPage](https://ipax77.github.io/pax.BlazorChartJs/)

Blazor dotnet wrapper library for [ChartJs](https://github.com/chartjs/Chart.js) (v3.9.1)

# Getting started
## Prerequisites
dotnet 6
## Installation

```
dotnet add package pax.BlazorChartJs
```

Program.cs: (You can set optional javascript locations here)
``` cs
    builder.Services.AddChartJs();
```

## Usage

Sample Project [pax.BlazorChartJs.samplelib](https://github.com/ipax77/pax.BlazorChartJs/tree/master/src/pax.BlazorChartJs.samplelib)

```razor
<div class="btn-group">
    <button type="button" class="btn btn-primary" @onclick="Randomize">Randomize</button>
    <button type="button" class="btn btn-primary" @onclick="AddDataset">Add Dataset</button>
    <button type="button" class="btn btn-primary" @onclick="AddData">Add Data</button>
    <button type="button" class="btn btn-primary" @onclick="RemoveLastDataset">Remove Dataset</button>
    <button type="button" class="btn btn-primary" @onclick="RemoveLastDataFromDatasets">Remove Data</button>
</div>

<div class="w-75">
    <ChartComponent @ref="chartComponent" OnEventTriggered="LabelClicked" ChartJsConfig="chartJsConfig"></ChartComponent>
</div>

<div>
    @if (!String.IsNullOrEmpty(labelClicked))
    {
        <p>
            Label clicked: @labelClicked
        </p>
    }
</div>

@code {
    ChartComponent? chartComponent;
    ChartJsConfig chartJsConfig = null!;
    private string? labelClicked;

    protected override void OnInitialized()
    {
        chartJsConfig = new()
            {
                Type = ChartType.bar,
                Data = new ChartJsData()
                {
                    Labels = new List<string>()
                    {
                        "Red", "Blue", "Yellow", "Green", "Purple", "Orange"
                    },
                    Datasets = new List<ChartJsDataset>()
                    {
                        new BarDataset()
                        {
                            Label = "# of Votes",
                            Data = new List<object>() { 12, 19, 3, 5, 2, 3 },
                            BackgroundColor = new IndexableOption<string>(new List<string>()
                            {
                                "rgba(255, 99, 132, 0.2)",
                                "rgba(54, 162, 235, 0.2)",
                                "rgba(255, 206, 86, 0.2)",
                                "rgba(75, 192, 192, 0.2)",
                                "rgba(153, 102, 255, 0.2)",
                                "rgba(255, 159, 64, 0.2)",
                            }),
                            BorderColor = new IndexableOption<string>(new List<string>()
                            {
                                "rgba(255, 99, 132, 1)",
                                "rgba(54, 162, 235, 1)",
                                "rgba(255, 206, 86, 1)",
                                "rgba(75, 192, 192, 1)",
                                "rgba(153, 102, 255, 1)",
                                "rgba(255, 159, 64, 1)",
                            }),
                            BorderWidth = new IndexableOption<double>(1)
                        }
                    }
                },
                Options = new ChartJsOptions()
                {
                    Responsive = true,
                    MaintainAspectRatio = true,
                    OnClickEvent = true,
                    Scales = new ChartJsOptionsScales()
                    {
                        Y = new LinearAxis()
                        {
                            SuggestedMax = 25
                        }
                    }
                }
            };
        base.OnInitialized();
    }


    private void ShowChart()
    {
        chartComponent?.DrawChart();
    }

    private void LabelClicked(ChartJsEvent chartJsEvent)
    {
        if (chartJsEvent is ChartJsLabelClickEvent labelClickEvent)
        {
            labelClicked = labelClickEvent.Label;
        }
    }

    private void AddData()
    {
        var dataAddEventArgs = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count);

        Dictionary<ChartJsDataset, AddDataObject> datas = new();
        for (int i = 0; i < chartJsConfig.Data.Datasets.Count; i++)
        {
            ChartJsDataset dataset = chartJsConfig.Data.Datasets[i];
            datas.Add(dataset,
                new AddDataObject(dataAddEventArgs.Data[i],
                                  null,
                                  dataAddEventArgs.BackgroundColors?[i],
                                  dataAddEventArgs.BorderColors?[i]));
        }
        chartJsConfig.AddData(dataAddEventArgs.Label, null, datas);
    }

    private void Randomize()
    {
        var data = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count,
         chartJsConfig.Data.Labels.Count, -100, 100);

        Dictionary<ChartJsDataset, SetDataObject> chartData = new();

        for (int i = 0; i < chartJsConfig.Data.Datasets.Count; i++)
        {
            var dataset = chartJsConfig.Data.Datasets.ElementAt(i);
            var dataList = data.ElementAt(i);
            chartData.Add(dataset, new SetDataObject(dataList));
        }
        chartJsConfig.SetData(chartData);
    }

    private void AddDataset()
    {
        var dataset = ChartUtils
         .GetRandomDataset(chartJsConfig.Type == null ? ChartType.bar : chartJsConfig.Type.Value,
                           chartJsConfig.Data.Datasets.Count + 1,
                           chartJsConfig.Data.Labels.Count);
        chartJsConfig.AddDataset(dataset);
    }

    private void RemoveLastDataset()
    {
        if (chartJsConfig.Data.Datasets.Any())
        {
            chartJsConfig.RemoveDataset(chartJsConfig.Data.Datasets.Last());
        }
    }

    private void RemoveLastDataFromDatasets()
    {
        chartJsConfig.RemoveData();
    }
```
## Supported Plugins
* [chartjs-plugin-datalabels](https://github.com/chartjs/chartjs-plugin-datalabels)
* [chartjs-plugin-labels](https://github.com/DavideViolante/chartjs-plugin-labels)
* [ArbitraryLines](https://www.youtube.com/watch?v=7ZZ_XfaJQbM&t=379s) (YouTube)
* Custom Plugins [Sample](https://github.com/ipax77/pax.BlazorChartJs/blob/master/src/pax.BlazorChartJs.samplelib/CustomPluginComp.razor)

## Known Limitations / ToDo

* Callbacks
* InteractionModes
* DataDecimation

## ChangeLog

<details open="open"><summary>v0.4.0</summary>

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
