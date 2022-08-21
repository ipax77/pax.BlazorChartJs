
# Blazor dotnet wrapper library for ChartJs (v3.9.1)

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

```

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
@code {
    private ChartJsConfig chartJsConfig = null!;
    private ChartComponent? chartComponent;

    protected override void OnInitialized()
    {
        chartJsConfig = new()
            {
                Type = ChartType.line,
                Data = new ChartJsData()
                {
                    Labels = labels,
                    Datasets = new List<object>()
                    {
                        new LineDataset()
                        {
                            Label = "Team 1",
                            Data = team1mid,
                            BorderColor = winnerTeam == 1 ? "green" : "red",
                            BorderWidth = 3,
                            Fill = false,
                            PointBackgroundColor = "white",
                            PointBorderColor = "yellow",
                            PointRadius = 1,
                            PointBorderWidth = 1,
                            PointHitRadius = 1,
                            Tension = 0
                        },
                        new LineDataset()
                        {
                            Label = "Team 2",
                            Data = team2mid,
                            BorderColor = winnerTeam == 2 ? "green" : "red",
                            BorderWidth = 3,
                            Fill = false,
                            PointBackgroundColor = "white",
                            PointBorderColor = "yellow",
                            PointRadius = 1,
                            PointBorderWidth = 1,
                            PointHitRadius = 1,
                            Tension = 0
                        }
                    }
            };
        base.OnInitialized();
    }

    private void AddData()
    {
        var dataAddEventArgs = ChartUtils.GetRandomData(chartJsConfig.Data.Datasets.Count);
        chartJsConfig.AddData(dataAddEventArgs.Label, dataAddEventArgs.Data);
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
        ((LineDataset)dataset).BorderWidth = 3;
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
}
```

# Known Limitations / ToDo

* RadarCharts
* Events
* Objects to Interfaces and Json handling

# ChangeLog

<details open="open"><summary>v0.1.1</summary>

>- Readme

</details>

<details><summary>v0.1.0</summary>

>- Init

</details>