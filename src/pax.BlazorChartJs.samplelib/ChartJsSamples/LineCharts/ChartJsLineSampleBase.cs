using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.LineCharts;

public abstract class ChartJsLineSampleBase : ChartJsDocsBaseComponent
{
    protected const int DataCount = 7;
    protected const int RandomMin = -100;
    protected const int RandomMax = 100;
    protected const string Red = "rgb(255, 99, 132)";
    protected const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    protected const string Blue = "rgb(54, 162, 235)";
    protected const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    protected const string Green = "rgb(75, 192, 192)";
    protected const string GreenTransparent = "rgba(75, 192, 192, 0.5)";

    protected static readonly string[] MonthLabels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];

    protected static readonly string[] InitialMonthLabels = MonthLabels[..DataCount];

    protected static readonly string[] DayLabels =
    [
        "Day 1",
        "Day 2",
        "Day 3",
        "Day 4",
        "Day 5",
        "Day 6",
    ];

    protected static readonly string[] InterpolationLabels =
    [
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
    ];

    protected static readonly IReadOnlyList<ChartJsDocsAction> NoActions = [];

    private static readonly string[] ColorPalette =
    [
        Red,
        Blue,
        Green,
        "rgb(255, 159, 64)",
        "rgb(153, 102, 255)",
        "rgb(255, 205, 86)",
        "rgb(201, 203, 207)",
    ];

    private static readonly string[] TransparentColorPalette =
    [
        RedTransparent,
        BlueTransparent,
        GreenTransparent,
        "rgba(255, 159, 64, 0.5)",
        "rgba(153, 102, 255, 0.5)",
        "rgba(255, 205, 86, 0.5)",
        "rgba(201, 203, 207, 0.5)",
    ];

    protected abstract string SampleId { get; }

    protected abstract string Title { get; }

    protected abstract string DocsHref { get; }

    protected abstract ChartJsConfig Config { get; }

    protected abstract IReadOnlyList<ChartJsDocsAction> Actions { get; }

    protected abstract ChartJsDocsCodeSet CSharpCode { get; }

    protected abstract ChartJsDocsCodeSet JavaScriptCode { get; }

    protected virtual string? CallbacksCode => null;

    protected ChartJsDocsAction[] CreateFullDatasetActions()
    {
        return
        [
            CreateAction("randomize", "Randomize", RandomizeNumbers),
            CreateAction("add-dataset", "Add Dataset", AddDataset),
            CreateAction("add-data", "Add Data", AddData),
            CreateAction("remove-dataset", "Remove Dataset", RemoveDataset),
            CreateAction("remove-data", "Remove Data", RemoveData),
        ];
    }

    protected ChartJsDocsAction[] CreateRandomizeNumbersActions()
    {
        return [CreateAction("randomize", "Randomize", RandomizeNumbers)];
    }

    protected ChartJsDocsAction[] CreatePointStyleActions()
    {
        return
        [
            CreateAction("point-style-circle", "pointStyle: circle (default)", () => SetPointStyle("circle")),
            CreateAction("point-style-cross", "pointStyle: cross", () => SetPointStyle("cross")),
            CreateAction("point-style-cross-rot", "pointStyle: crossRot", () => SetPointStyle("crossRot")),
            CreateAction("point-style-dash", "pointStyle: dash", () => SetPointStyle("dash")),
            CreateAction("point-style-line", "pointStyle: line", () => SetPointStyle("line")),
            CreateAction("point-style-rect", "pointStyle: rect", () => SetPointStyle("rect")),
            CreateAction("point-style-rect-rounded", "pointStyle: rectRounded", () => SetPointStyle("rectRounded")),
            CreateAction("point-style-rect-rot", "pointStyle: rectRot", () => SetPointStyle("rectRot")),
            CreateAction("point-style-star", "pointStyle: star", () => SetPointStyle("star")),
            CreateAction("point-style-triangle", "pointStyle: triangle", () => SetPointStyle("triangle")),
            CreateAction("point-style-false", "pointStyle: false", () => SetPointStyle(false)),
        ];
    }

    protected ChartJsDocsAction[] CreateSteppedActions()
    {
        return
        [
            CreateAction("step-false", "Step: false (default)", () => SetStepped(false)),
            CreateAction("step-true", "Step: true", () => SetStepped(true)),
            CreateAction("step-before", "Step: before", () => SetStepped("before")),
            CreateAction("step-after", "Step: after", () => SetStepped("after")),
            CreateAction("step-middle", "Step: middle", () => SetStepped("middle")),
        ];
    }

    protected static IList<object> RandomNumbers()
    {
        return RandomNumbers(DataCount);
    }

    protected static IList<object> RandomNumbers(int count)
    {
        List<object> data = new(count);

        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(RandomMin, RandomMax));
        }

        return data;
    }

    protected static IList<object> InterpolationData()
    {
        return [0, 20, 20, 60, 60, 120, null!, 180, 120, 125, 105, 110, 170];
    }

    private void RandomizeNumbers()
    {
        Config.SetData(CreateRandomNumberData());
    }

    private void AddDataset()
    {
        var datasetIndex = Config.Data.Datasets.Count;
        Config.AddDataset(new LineDataset
        {
            Label = $"Dataset {datasetIndex + 1}",
            Data = RandomNumbers(Config.Data.Labels.Count),
            BackgroundColor = TransparentColorPalette[datasetIndex % TransparentColorPalette.Length],
            BorderColor = ColorPalette[datasetIndex % ColorPalette.Length],
        });
    }

    private void AddData()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        var label = GetMonthLabel(Config.Data.Labels.Count);
        Dictionary<ChartJsDataset, AddDataObject> data = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            data[dataset] = new AddDataObject(Random.Shared.Next(RandomMin, RandomMax));
        }

        Config.AddData(label, null, data);
    }

    private void RemoveDataset()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        Config.RemoveDataset(datasets[^1]);
    }

    private void RemoveData()
    {
        if (Config.Data.Labels.Count == 0)
        {
            return;
        }

        Config.RemoveData();
    }

    private void SetPointStyle(object pointStyle)
    {
        UpdateLineDatasets(dataset => dataset.PointStyle = pointStyle);
    }

    private void SetStepped(object stepped)
    {
        UpdateLineDatasets(dataset => dataset.Stepped = stepped);
    }

    private void UpdateLineDatasets(Action<LineDataset> update)
    {
        var datasets = Config.Data.Datasets;
        List<ChartJsDataset> updatedDatasets = new(datasets.Count);

        foreach (var dataset in datasets)
        {
            if (dataset is LineDataset lineDataset)
            {
                update(lineDataset);
                updatedDatasets.Add(lineDataset);
            }
        }

        if (updatedDatasets.Count > 0)
        {
            Config.UpdateDatasets(updatedDatasets);
        }
    }

    private Dictionary<ChartJsDataset, SetDataObject> CreateRandomNumberData()
    {
        var datasets = Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);
        var labelCount = Config.Data.Labels.Count;

        foreach (var dataset in datasets)
        {
            data[dataset] = new SetDataObject(RandomNumbers(labelCount));
        }

        return data;
    }

    private static string GetMonthLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }
}
