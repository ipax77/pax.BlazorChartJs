using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.BarCharts;

public abstract class ChartJsBarSampleBase : ChartJsDocsBaseComponent
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
    ];

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

    protected ChartJsDocsAction[] CreateRandomizeFloatingBarsActions()
    {
        return [CreateAction("randomize", "Randomize", RandomizeFloatingBars)];
    }

    protected static ChartJsConfig CreateConfig(
        string title,
        string? legendPosition,
        IList<ChartJsDataset> datasets,
        bool stacked = false,
        string? indexAxis = null,
        bool interactionIntersect = true)
    {
        return new ChartJsConfig
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = [.. MonthLabels],
                Datasets = datasets,
            },
            Options = new ChartJsOptions
            {
                Responsive = true,
                IndexAxis = indexAxis,
                Interaction = interactionIntersect
                    ? null
                    : new Interactions { Intersect = false },
                Plugins = new Plugins
                {
                    Legend = legendPosition is null ? null : new Legend { Position = legendPosition },
                    Title = new Title
                    {
                        Display = true,
                        Text = title,
                    },
                },
                Scales = stacked
                    ? new ChartJsOptionsScales
                    {
                        X = new ChartJsAxis { Stacked = true },
                        Y = new ChartJsAxis { Stacked = true },
                    }
                    : null,
            },
        };
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

    protected static IList<object> RandomFloatingBars()
    {
        return RandomFloatingBars(DataCount);
    }

    protected static IList<object> RandomFloatingBars(int count)
    {
        List<object> data = new(count);

        for (var i = 0; i < count; i++)
        {
            data.Add(new[] { Random.Shared.Next(RandomMin, RandomMax), Random.Shared.Next(RandomMin, RandomMax) });
        }

        return data;
    }

    private void RandomizeNumbers()
    {
        Config.SetData(CreateRandomNumberData());
    }

    private void RandomizeFloatingBars()
    {
        Config.SetData(CreateRandomFloatingBarData());
    }

    private void AddDataset()
    {
        var datasetIndex = Config.Data.Datasets.Count;
        Config.AddDataset(new BarDataset
        {
            Label = $"Dataset {datasetIndex + 1}",
            Data = RandomNumbers(Config.Data.Labels.Count),
            BackgroundColor = TransparentColorPalette[datasetIndex % TransparentColorPalette.Length],
            BorderColor = ColorPalette[datasetIndex % ColorPalette.Length],
            BorderWidth = 1,
        });
    }

    private void AddData()
    {
        var datasets = Config.Data.Datasets;
        if (datasets.Count == 0)
        {
            return;
        }

        var label = GetLabel(Config.Data.Labels.Count);
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

    private Dictionary<ChartJsDataset, SetDataObject> CreateRandomFloatingBarData()
    {
        var datasets = Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);
        var labelCount = Config.Data.Labels.Count;

        foreach (var dataset in datasets)
        {
            data[dataset] = new SetDataObject(RandomFloatingBars(labelCount));
        }

        return data;
    }

    private static string GetLabel(int index)
    {
        return MonthLabels[index % MonthLabels.Length];
    }
}
