
Blazor dotnet wrapper library for [ChartJs](https://github.com/chartjs/Chart.js) (v3.9.1)



# Getting started
## Prerequisites
dotnet 6
## Installation

TBD: nuget package

Program.cs:
```
    builder.Services.AddChartJs();
```

## Usage

```cs

<div class="btn-group">
    <button type="button" class="btn btn-primary" @onclick="Randomize">Randomize</button>
    <button type="button" class="btn btn-primary" @onclick="AddDataset">Add Dataset</button>
    <button type="button" class="btn btn-primary" @onclick="AddData">Add Data</button>
    <button type="button" class="btn btn-primary" @onclick="RemoveLastDataset">Remove Dataset</button>
    <button type="button" class="btn btn-primary" @onclick="RemoveLastDataFromDatasets">Remove Data</button>
</div>

<div class="w-75">
    <ChartComponent @ref="chartComponent" ChartJsConfig="chartJsConfig"></ChartComponent>
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
                    Datasets = new List<object>()
                    {
                        new BarDataset()
                        {
                            Label = "# of Votes",
                            Data = new List<object>() { 12, 19, 3, 5, 2, 3 },
                            BackgroundColor = new List<string>()
                            {
                                "rgba(255, 99, 132, 0.2)",
                                "rgba(54, 162, 235, 0.2)",
                                "rgba(255, 206, 86, 0.2)",
                                "rgba(75, 192, 192, 0.2)",
                                "rgba(153, 102, 255, 0.2)",
                                "rgba(255, 159, 64, 0.2)",
                            },
                            BorderColor = new List<string>()
                            {
                                "rgba(255, 99, 132, 1)",
                                "rgba(54, 162, 235, 1)",
                                "rgba(255, 206, 86, 1)",
                                "rgba(75, 192, 192, 1)",
                                "rgba(153, 102, 255, 1)",
                                "rgba(255, 159, 64, 1)",
                            },
                            BorderWidth = 1
                        }
                    }
                },
                Options = new ChartJsOptions()
                {
                    Responsive = true,
                    MaintainAspectRatio = true,
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

    private void AddData()
    {
        var dataAddEventArgs = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count);
        chartJsConfig.AddData(dataAddEventArgs.Label, dataAddEventArgs.Data, dataAddEventArgs.BackgroundColors, dataAddEventArgs.BorderColors);
    }

    private void Randomize()
    {
        var data = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count, chartJsConfig.Data.Labels.Count, -100, 100);

        Dictionary<object, IList<object>> chartData = new();

        for (int i = 0; i < chartJsConfig.Data.Datasets.Count; i++)
        {
            var dataset = chartJsConfig.Data.Datasets.ElementAt(i);
            var dataList = data.ElementAt(i);
            chartData.Add(dataset, dataList);
        }
        chartJsConfig.SetData(chartData);
    }

    private void AddDataset()
    {
        var dataset = ChartUtils.GetRandomDataset(chartJsConfig.Type == null ? ChartType.bar : chartJsConfig.Type.Value, chartJsConfig.Data.Datasets.Count + 1, chartJsConfig.Data.Labels.Count);
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

    private void LabelClicked(KeyValuePair<Guid, string> report)
    {
        labelClicked = report.Value;
    }    
}
```
## Supported Plugins
* [chartjs-plugin-datalabels](https://github.com/chartjs/chartjs-plugin-datalabels)
* [chartjs-plugin-labels](https://github.com/DavideViolante/chartjs-plugin-labels)
* [ArbitraryLines](https://www.youtube.com/watch?v=7ZZ_XfaJQbM&t=379s) (YouTube)

## Known Limitations / ToDo

* BubbleChart
* ScatterChart

* Events
* Objects to Interfaces and Json handling

## ChangeLog

<details open="open"><summary>v0.1.2</summary>

>- RadarChart

</details>


<details><summary>v0.1.1</summary>

>- Readme

</details>

<details><summary>v0.1.0</summary>

>- Init

</details>