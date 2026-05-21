using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Backlog;

public sealed partial class ChartJsBacklogSamples : ChartJsBacklogSamplesBase
{
}

public abstract class ChartJsBacklogSamplesBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const string HtmlLegendPluginModuleLocation = "/_content/pax.BlazorChartJs.samplelib/htmlLegendPlugin.js";
    private const string HtmlLegendPluginRegisterFunction = "registerHtmlLegendPlugin";
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string Yellow = "rgb(255, 205, 86)";
    private const string Green = "rgb(75, 192, 192)";
    private const string Purple = "rgb(153, 102, 255)";
    private const string Orange = "rgb(255, 159, 64)";
    private static readonly string[] Months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    private static readonly string[] PieColors = ["#CB4335", "#1F618D", "#F1C40F", "#27AE60", "#884EA0", "#D35400"];
    private static readonly string[] Palette = [Red, Blue, Yellow, Green, Purple, Orange];
    private static readonly Lazy<Dictionary<string, OfficialSampleDefinition>> Definitions = new(CreateDefinitions);

    private IJSObjectReference? externalPluginModule;
    private string? registeredExternalPluginModuleLocation;
    private ChartJsConfig? config;
    private IReadOnlyList<ChartJsDocsAction> actions = [];

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter, EditorRequired]
    public string Category { get; set; } = string.Empty;

    [Parameter]
    public string? SampleId { get; set; }

    protected OfficialSampleDefinition ResolvedSample { get; private set; } = default!;

    protected ChartJsConfig Config => config ?? throw new InvalidOperationException("Sample config has not been initialized.");

    protected IReadOnlyList<ChartJsDocsAction> Actions => actions;

    public static string ResolveDefaultSample(string category)
    {
        return category switch
        {
            "scale-options" => "center",
            "legend" => "events",
            "title" => "alignment",
            "tooltip" => "content",
            "scriptable" => "bar",
            "animations" => "delay",
            _ => "center",
        };
    }

    public static bool IsKnownSample(string category, string sampleId)
    {
        return Definitions.Value.ContainsKey(GetKey(category, sampleId));
    }

    protected override void OnParametersSet()
    {
        var sampleId = string.IsNullOrWhiteSpace(SampleId) ? ResolveDefaultSample(Category) : SampleId!;
        ResolvedSample = Definitions.Value.TryGetValue(GetKey(Category, sampleId), out var definition)
            ? definition
            : Definitions.Value[GetKey(Category, ResolveDefaultSample(Category))];

        config = ResolvedSample.CreateConfig();
        actions = ResolvedSample.CreateActions(this);
    }

    protected async Task ChartEventTriggered(ChartJsEvent chartJsEvent)
    {
        if (chartJsEvent is not ChartJsInitEvent
            || ResolvedSample.ExternalPluginModuleLocation is not { Length: > 0 } moduleLocation
            || ResolvedSample.ExternalPluginRegisterFunction is not { Length: > 0 } registerFunction
            || string.Equals(registeredExternalPluginModuleLocation, moduleLocation, StringComparison.Ordinal))
        {
            return;
        }

        if (externalPluginModule is not null
            && !string.Equals(registeredExternalPluginModuleLocation, moduleLocation, StringComparison.Ordinal))
        {
            await externalPluginModule.DisposeAsync().ConfigureAwait(false);
            externalPluginModule = null;
            registeredExternalPluginModuleLocation = null;
        }

        externalPluginModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", moduleLocation).ConfigureAwait(false);
        await externalPluginModule.InvokeVoidAsync(registerFunction).ConfigureAwait(false);
        registeredExternalPluginModuleLocation = moduleLocation;
        Config.ReinitializeChart();
    }

    private void Randomize()
    {
        var datasets = Config.Data.Datasets;
        Dictionary<ChartJsDataset, SetDataObject> data = new(datasets.Count);
        for (var i = 0; i < datasets.Count; i++)
        {
            data[datasets[i]] = new SetDataObject(CreateRandomData(datasets[i], i));
        }

        Config.SetData(data);
    }

    private void AddDataset()
    {
        var index = Config.Data.Datasets.Count;
        Config.AddDataset(new LineDataset
        {
            Label = $"Dataset {index + 1}",
            Data = RandomNumbers(Config.Data.Labels.Count == 0 ? 7 : Config.Data.Labels.Count, -100, 100),
            BorderColor = Palette[index % Palette.Length],
            BackgroundColor = ToTransparent(Palette[index % Palette.Length]),
        });
    }

    private void AddData()
    {
        if (Config.Data.Datasets.Count == 0)
        {
            return;
        }

        Dictionary<ChartJsDataset, AddDataObject> data = new(Config.Data.Datasets.Count);
        foreach (var dataset in Config.Data.Datasets)
        {
            data[dataset] = new AddDataObject(Random.Shared.Next(-100, 101));
        }

        Config.AddData(Months[Math.Min(Config.Data.Labels.Count, Months.Length - 1)], null, data);
    }

    private void RemoveDataset()
    {
        if (Config.Data.Datasets.Count > 0)
        {
            Config.RemoveDataset(Config.Data.Datasets[^1]);
        }
    }

    private void RemoveData()
    {
        if (Config.Data.Labels.Count > 0)
        {
            Config.RemoveData();
        }
    }

    private void SetScalePositions(object x, object y)
    {
        if (Config.Options?.Scales?.X is CartesianAxis xAxis && Config.Options.Scales.Y is CartesianAxis yAxis)
        {
            xAxis.Position = x;
            yAxis.Position = y;
            Config.UpdateChartOptions();
        }
    }

    private void SetTitleAlign(string align)
    {
        Config.Options!.Plugins!.Title!.Align = align;
        Config.UpdateChartOptions();
    }

    private void SetLegendPosition(string position)
    {
        Config.Options!.Plugins!.Legend!.Position = position;
        Config.UpdateChartOptions();
    }

    private void SetLegendTitle(string align, string titlePosition)
    {
        var legend = Config.Options!.Plugins!.Legend!;
        legend.Align = align;
        legend.Title!.Position = titlePosition;
        Config.UpdateChartOptions();
    }

    private void SetTooltipMode(string mode, bool intersect, string? axis = null)
    {
        Config.Options!.Interaction = new Interactions { Mode = mode, Intersect = intersect, Axis = axis };
        Config.UpdateChartOptions();
    }

    private void ToggleTooltipPointStyle()
    {
        var tooltip = Config.Options!.Plugins!.Tooltip!;
        tooltip.UsePointStyle = !(tooltip.UsePointStyle?.SingleValue ?? false);
        Config.UpdateChartOptions();
    }

    private void SetTooltipPosition(string position)
    {
        Config.Options!.Plugins!.Tooltip ??= new Tooltip();
        Config.Options.Plugins.Tooltip.Position = position;
        Config.UpdateChartOptions();
    }

    private void TogglePieCutout()
    {
        Config.Options!.Cutout = Config.Options.Cutout?.StringValue == "50%" ? 0 : "50%";
        Config.UpdateChartOptions();
    }

    private void SetTickAlign(string align)
    {
        if (Config.Options?.Scales?.X?.Ticks is CartesianAxisTick ticks)
        {
            ticks.Align = align;
            Config.UpdateChartOptions();
        }
    }

    private static List<object> CreateRandomData(ChartJsDataset dataset, int datasetIndex)
    {
        var count = dataset.Data.Count == 0 ? 7 : dataset.Data.Count;
        if (dataset.Data.FirstOrDefault() is BubbleScriptablePoint)
        {
            return BubbleScriptableData(count);
        }

        if (dataset.Data.FirstOrDefault() is BubbleDataPoint)
        {
            return BubbleData(count);
        }

        if (dataset.Data.FirstOrDefault() is DataPoint)
        {
            return ProgressiveData(count, datasetIndex == 0 ? 100 : 80);
        }

        return RandomNumbers(count, -100, 100);
    }

    private static Dictionary<string, OfficialSampleDefinition> CreateDefinitions()
    {
        Dictionary<string, OfficialSampleDefinition> definitions = [];
        AddScaleOptions(definitions);
        AddLegend(definitions);
        AddTitle(definitions);
        AddTooltip(definitions);
        AddScriptable(definitions);
        AddAnimations(definitions);
        return definitions;
    }

    private static void AddScaleOptions(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("scale-options", "center")] = Definition("scale-options", "center", "Center Positioning", "https://www.chartjs.org/docs/latest/samples/scale-options/center.html", CreateCenterConfig, c =>
        [
            c.CreateAction("default-positions", "Default Positions", () => c.SetScalePositions("bottom", "left")),
            c.CreateAction("position-center", "Position: center", () => c.SetScalePositions("center", "center")),
            c.CreateAction("position-values", "Position: Vertical: x=-60, Horizontal: y=30", () => c.SetScalePositions(new ChartJsScalePosition { Y = 30 }, new ChartJsScalePosition { X = -60 })),
        ], callbacks: CallbackCode);

        definitions[GetKey("scale-options", "grid")] = Definition("scale-options", "grid", "Grid Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/grid.html", CreateGridConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
        definitions[GetKey("scale-options", "ticks")] = Definition("scale-options", "ticks", "Tick Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/ticks.html", CreateTicksConfig, c =>
        [
            c.CreateAction("align-start", "Tick Alignment: start", () => c.SetTickAlign("start")),
            c.CreateAction("align-center", "Tick Alignment: center (default)", () => c.SetTickAlign("center")),
            c.CreateAction("align-end", "Tick Alignment: end", () => c.SetTickAlign("end")),
        ], callbacks: CallbackCode);
        definitions[GetKey("scale-options", "titles")] = Definition("scale-options", "titles", "Title Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/titles.html", CreateScaleTitlesConfig, _ => []);
    }

    private static void AddLegend(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("legend", "events")] = Definition("legend", "events", "Events", "https://www.chartjs.org/docs/latest/samples/legend/events.html", CreateLegendEventsConfig, _ => [], callbacks: CallbackCode);
        definitions[GetKey("legend", "html")] = Definition(
            "legend",
            "html",
            "HTML Legend",
            "https://www.chartjs.org/docs/latest/samples/legend/html.html",
            CreateHtmlLegendConfig,
            _ => [],
            javascriptCode: HtmlLegendJavaScriptCode(),
            callbacks: HtmlLegendPluginCode,
            externalPluginModuleLocation: HtmlLegendPluginModuleLocation,
            externalPluginRegisterFunction: HtmlLegendPluginRegisterFunction);
        definitions[GetKey("legend", "point-style")] = Definition("legend", "point-style", "Point Style", "https://www.chartjs.org/docs/latest/samples/legend/point-style.html", CreateLegendPointStyleConfig, c => [c.CreateAction("toggle-point-style", "Toggle Point Style", () => { var labels = c.Config.Options!.Plugins!.Legend!.Labels!; labels.UsePointStyle = !(labels.UsePointStyle ?? false); c.Config.UpdateChartOptions(); })]);
        definitions[GetKey("legend", "position")] = Definition("legend", "position", "Position", "https://www.chartjs.org/docs/latest/samples/legend/position.html", CreateLegendPositionConfig, c =>
        [
            c.CreateAction("position-top", "Position: top", () => c.SetLegendPosition("top")),
            c.CreateAction("position-left", "Position: left", () => c.SetLegendPosition("left")),
            c.CreateAction("position-bottom", "Position: bottom", () => c.SetLegendPosition("bottom")),
            c.CreateAction("position-right", "Position: right", () => c.SetLegendPosition("right")),
            c.CreateAction("position-chart-area", "Position: chartArea", () => c.SetLegendPosition("chartArea")),
        ]);
        definitions[GetKey("legend", "title")] = Definition("legend", "title", "Alignment and Title Position", "https://www.chartjs.org/docs/latest/samples/legend/title.html", CreateLegendTitleConfig, c =>
        [
            c.CreateAction("title-start", "Align start / title start", () => c.SetLegendTitle("start", "start")),
            c.CreateAction("title-center", "Align center / title center", () => c.SetLegendTitle("center", "center")),
            c.CreateAction("title-end", "Align end / title end", () => c.SetLegendTitle("end", "end")),
        ]);
    }

    private static void AddTitle(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("title", "alignment")] = Definition("title", "alignment", "Alignment", "https://www.chartjs.org/docs/latest/samples/title/alignment.html", CreateTitleAlignmentConfig, c =>
        [
            c.CreateAction("title-alignment-start", "Title Alignment: start", () => c.SetTitleAlign("start")),
            c.CreateAction("title-alignment-center", "Title Alignment: center (default)", () => c.SetTitleAlign("center")),
            c.CreateAction("title-alignment-end", "Title Alignment: end", () => c.SetTitleAlign("end")),
        ]);
    }

    private static void AddTooltip(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("tooltip", "content")] = Definition("tooltip", "content", "Custom Tooltip Content", "https://www.chartjs.org/docs/latest/samples/tooltip/content.html", CreateTooltipContentConfig, _ => [], callbacks: CallbackCode);
        definitions[GetKey("tooltip", "html")] = Definition("tooltip", "html", "External HTML Tooltip", "https://www.chartjs.org/docs/latest/samples/tooltip/html.html", CreateTooltipHtmlConfig, _ => [], callbacks: CallbackCode);
        definitions[GetKey("tooltip", "interactions")] = Definition("tooltip", "interactions", "Interaction Modes", "https://www.chartjs.org/docs/latest/samples/tooltip/interactions.html", CreateTooltipInteractionsConfig, c =>
        [
            c.CreateAction("mode-index", "Mode: index", () => c.SetTooltipMode("index", false)),
            c.CreateAction("mode-dataset", "Mode: dataset", () => c.SetTooltipMode("dataset", false)),
            c.CreateAction("mode-point", "Mode: point", () => c.SetTooltipMode("point", true)),
            c.CreateAction("mode-nearest-x", "Mode: nearest, axis x", () => c.SetTooltipMode("nearest", false, "x")),
            c.CreateAction("mode-x", "Mode: x", () => c.SetTooltipMode("x", false)),
            c.CreateAction("mode-y", "Mode: y", () => c.SetTooltipMode("y", false)),
        ]);
        definitions[GetKey("tooltip", "point-style")] = Definition("tooltip", "point-style", "Point Style", "https://www.chartjs.org/docs/latest/samples/tooltip/point-style.html", CreateTooltipPointStyleConfig, c => [c.CreateAction("toggle-use-point-style", "Toggle Use Point Style", c.ToggleTooltipPointStyle)]);
        definitions[GetKey("tooltip", "position")] = Definition("tooltip", "position", "Position", "https://www.chartjs.org/docs/latest/samples/tooltip/position.html", CreateTooltipPositionConfig, c =>
        [
            c.CreateAction("position-average", "Position: average", () => c.SetTooltipPosition("average")),
            c.CreateAction("position-nearest", "Position: nearest", () => c.SetTooltipPosition("nearest")),
            c.CreateAction("position-bottom", "Position: bottom (custom)", () => c.SetTooltipPosition("bottom")),
        ], callbacks: CallbackCode);
    }

    private static void AddScriptable(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("scriptable", "bar")] = Definition("scriptable", "bar", "Bar Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/bar.html", CreateScriptableBarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
        definitions[GetKey("scriptable", "bubble")] = Definition("scriptable", "bubble", "Bubble Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/bubble.html", CreateScriptableBubbleConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
        definitions[GetKey("scriptable", "line")] = Definition("scriptable", "line", "Line Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/line.html", CreateScriptableLineConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
        definitions[GetKey("scriptable", "pie")] = Definition("scriptable", "pie", "Pie Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/pie.html", CreateScriptablePieConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize), c.CreateAction("toggle-doughnut", "Toggle Doughnut View", c.TogglePieCutout)], callbacks: CallbackCode);
        definitions[GetKey("scriptable", "polar")] = Definition("scriptable", "polar", "Polar Area Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/polar.html", CreateScriptablePolarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
        definitions[GetKey("scriptable", "radar")] = Definition("scriptable", "radar", "Radar Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/radar.html", CreateScriptableRadarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: CallbackCode);
    }

    private static void AddAnimations(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("animations", "delay")] = Definition("animations", "delay", "Delay", "https://www.chartjs.org/docs/latest/samples/animations/delay.html", CreateAnimationDelayConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], animationTabs: true, callbacks: CallbackCode);
        definitions[GetKey("animations", "drop")] = Definition("animations", "drop", "Drop", "https://www.chartjs.org/docs/latest/samples/animations/drop.html", CreateAnimationDropConfig, c => FullDatasetActions(c), animationTabs: true, callbacks: CallbackCode);
        definitions[GetKey("animations", "loop")] = Definition("animations", "loop", "Loop", "https://www.chartjs.org/docs/latest/samples/animations/loop.html", CreateAnimationLoopConfig, _ => [], animationTabs: true, callbacks: CallbackCode);
        definitions[GetKey("animations", "progressive-line")] = Definition("animations", "progressive-line", "Progressive Line", "https://www.chartjs.org/docs/latest/samples/animations/progressive-line.html", CreateProgressiveLineConfig, _ => [], animationTabs: true, callbacks: CallbackCode);
        definitions[GetKey("animations", "progressive-line-easing")] = Definition("animations", "progressive-line-easing", "Progressive Line With Easing", "https://www.chartjs.org/docs/latest/samples/animations/progressive-line-easing.html", CreateProgressiveLineEasingConfig, c =>
        [
            c.CreateAction("easing-linear", "linear", () => SetProgressiveEasing(c, "linear")),
            c.CreateAction("easing-ease-in-quad", "easeInQuad", () => SetProgressiveEasing(c, "easeInQuad")),
            c.CreateAction("easing-ease-out-quad", "easeOutQuad", () => SetProgressiveEasing(c, "easeOutQuad")),
            c.CreateAction("easing-ease-in-out-quad", "easeInOutQuad", () => SetProgressiveEasing(c, "easeInOutQuad")),
        ], animationTabs: true, callbacks: CallbackCode);
    }

    private static ChartJsDocsAction[] FullDatasetActions(ChartJsBacklogSamplesBase c)
    {
        return
        [
            c.CreateAction("randomize", "Randomize", c.Randomize),
            c.CreateAction("add-dataset", "Add Dataset", c.AddDataset),
            c.CreateAction("add-data", "Add Data", c.AddData),
            c.CreateAction("remove-dataset", "Remove Dataset", c.RemoveDataset),
            c.CreateAction("remove-data", "Remove Data", c.RemoveData),
        ];
    }

    private static OfficialSampleDefinition Definition(
        string category,
        string id,
        string title,
        string docsUrl,
        Func<ChartJsConfig> createConfig,
        Func<ChartJsBacklogSamplesBase, IReadOnlyList<ChartJsDocsAction>> createActions,
        bool animationTabs = false,
        string? callbacks = null,
        ChartJsDocsCodeSet? csharpCode = null,
        ChartJsDocsCodeSet? javascriptCode = null,
        string? externalPluginModuleLocation = null,
        string? externalPluginRegisterFunction = null)
    {
        var csharp = csharpCode ?? (animationTabs ? AnimationCSharpCode(title) : DefaultCSharpCode(title));
        var js = javascriptCode ?? (animationTabs ? AnimationJavaScriptCode(title) : DefaultJavaScriptCode(title));
        return new(category, id, title, docsUrl, createConfig, createActions, csharp, js, callbacks, externalPluginModuleLocation, externalPluginRegisterFunction);
    }

    private static string GetKey(string category, string sampleId) => $"{category}:{sampleId}";

    private static ChartJsDocsCodeSet DefaultCSharpCode(string title) => new(
        """
        var config = new ChartJsConfig { Data = data, Options = options };
        """,
        $"""
        // {title}: build typed C# data/options equivalent to the official Chart.js sample.
        """,
        """
        // Official actions mutate data/options and call SetData or UpdateChartOptions.
        """);

    private static ChartJsDocsCodeSet DefaultJavaScriptCode(string title) => new(
        """
        const config = { type, data, options };
        """,
        $"""
        // {title}: official setup is mirrored with typed C# and named callbacks.
        """,
        """
        const actions = [];
        """);

    private static ChartJsDocsCodeSet HtmlLegendJavaScriptCode() => new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            plugins: {
              htmlLegend: {
                containerID: 'legend-container'
              },
              legend: {
                display: false
              }
            }
          }
        };
        """,
        $$"""
        // The sample registers this after the first chart init loads Chart.js, then reinitializes the chart.
        const module = await import('{{HtmlLegendPluginModuleLocation}}');
        await module.{{HtmlLegendPluginRegisterFunction}}();
        """,
        """
        const actions = [];
        """);

    private static ChartJsDocsCodeSet AnimationCSharpCode(string title) => new(
        [
            new("config", "Config", "var config = new ChartJsConfig { Data = data, Options = options };"),
            new("animation", "Animation", $"// {title}: animation settings use typed Animation/Animations and named callbacks."),
            new("data", "Data", "// Data is generated once per sample instance and updated with batched helpers."),
            new("actions", "Actions", "// Actions call SetData/AddData/AddDataset/UpdateChartOptions as appropriate."),
        ]);

    private static ChartJsDocsCodeSet AnimationJavaScriptCode(string title) => new(
        [
            new("config", "Config", "const config = { type: 'line', data, options };"),
            new("animation", "Animation", $"// {title}: mirrors the official animation block."),
            new("data", "Data", "const data = { datasets: [] };"),
            new("actions", "Actions", "const actions = [];"),
        ]);

    private static ChartJsConfig CreateCenterConfig()
    {
        return new()
        {
            Type = ChartType.scatter,
            Data = new ChartJsData { Datasets = [CreateScatter("Dataset 1", Red), CreateScatter("Dataset 2", Blue)] },
            Options = new ChartJsOptions { Responsive = true, Plugins = TitlePlugin("Axis Center Positioning"), Scales = new ChartJsOptionsScales { X = new CartesianAxis { Min = -100, Max = 100 }, Y = new CartesianAxis { Min = -100, Max = 100 } } },
        };
    }

    private static ChartJsConfig CreateGridConfig() => CreateLineConfig("Grid Line Settings", [Line("Dataset 1", Red, [10, 30, 39, 20, 25, 34, -10]), Line("Dataset 2", Blue, [18, 33, 22, 19, 11, -39, 30])], new ChartJsOptionsScales { X = new CartesianAxis { Border = new ChartJsAxisBorder { Display = true }, Grid = new ChartJsGrid { Display = true, DrawOnChartArea = true, DrawTicks = true } }, Y = new CartesianAxis { Border = new ChartJsAxisBorder { Display = false }, Grid = new ChartJsGrid { Color = ChartJsFunction.FromName("scaleOptionsGridColor") } } });

    private static ChartJsConfig CreateTicksConfig()
    {
        var labels = new List<string> { "January\n2015", "February", "March", "April", "May", "June\n2015", "July", "August", "September", "October", "November", "December", "January\n2016", "February", "March", "April" };
        return CreateLineConfig("Tick Configuration", [Line("Dataset 1", Red, RandomNumbers(labels.Count, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(labels.Count, -100, 100))], new ChartJsOptionsScales { X = new CartesianAxis { Ticks = new CartesianAxisTick { Callback = ChartJsFunction.FromName("scaleOptionsTickLabel"), Align = "center" } }, Y = new CartesianAxis { Title = new Title { Display = true, Text = "Value" } } }, labels);
    }

    private static ChartJsConfig CreateScaleTitlesConfig() => CreateLineConfig("Scale Title Configuration", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100))], new ChartJsOptionsScales { X = new CartesianAxis { Title = new Title { Display = true, Text = "Month", Color = Blue, Font = new Font { Size = 18, Weight = "bold" }, Padding = new Padding { Top = 20 } } }, Y = new CartesianAxis { Title = new Title { Display = true, Text = "Value", Color = Red, Font = new Font { Size = 18, Style = "italic" }, Padding = new Padding { Bottom = 10 } } } });

    private static ChartJsConfig CreateLegendEventsConfig() => new() { Type = ChartType.pie, Data = new ChartJsData { Labels = ["Red", "Blue", "Yellow", "Green", "Purple", "Orange"], Datasets = [new PieDataset { Label = "# of Votes", Data = [12, 19, 3, 5, 2, 3], BorderWidth = 1, BackgroundColor = PieColors.ToList() }] }, Options = new ChartJsOptions { Plugins = new Plugins { Legend = new Legend { OnHover = ChartJsFunction.FromName("legendHandleHover"), OnLeave = ChartJsFunction.FromName("legendHandleLeave") } } } };

    private static ChartJsConfig CreateHtmlLegendConfig() => CreateLineConfig("HTML Legend", [Line("Dataset: 1", Red, RandomNumbers(7, 0, 100)), Line("Dataset: 2", Blue, RandomNumbers(7, 0, 100))], null, null, new Plugins { HtmlLegend = new HtmlLegendOptions { ContainerID = "legend-container" }, Legend = new Legend { Display = false } });

    private static ChartJsConfig CreateLegendPointStyleConfig() => CreateLineConfig("Legend Point Style", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100), pointStyle: "circle"), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100), pointStyle: "rectRot")], null, null, new Plugins { Legend = new Legend { Labels = new Labels { UsePointStyle = true, PointStyle = "circle" } }, Title = new Title { Display = true, Text = "Legend Point Style" } });

    private static ChartJsConfig CreateLegendPositionConfig() => CreateLineConfig("Legend Position", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Legend = new Legend { Position = "top" }, Title = new Title { Display = true, Text = "Legend Position" } });

    private static ChartJsConfig CreateLegendTitleConfig() => CreateLineConfig("Legend Alignment and Title", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Legend = new Legend { Align = "center", Title = new Title { Display = true, Text = "Legend Title", Position = "center" } }, Title = new Title { Display = true, Text = "Legend Title Position" } });

    private static ChartJsConfig CreateTitleAlignmentConfig() => CreateLineConfig("Chart Title", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100))]);

    private static ChartJsConfig CreateTooltipContentConfig() => CreateLineConfig("Tooltip Content", [Line("Dataset 1", Red, RandomNumbers(7, 0, 100)), Line("Dataset 2", Blue, RandomNumbers(7, 0, 100))], null, null, new Plugins { Tooltip = new Tooltip { Mode = "index", Callbacks = new TooltipCallbacks { Footer = ChartJsFunction.FromName("tooltipContentFooter") } }, Title = new Title { Display = true, Text = "Tooltip Content" } });

    private static ChartJsConfig CreateTooltipHtmlConfig() => CreateLineConfig("External HTML Tooltip", [Line("Dataset 1", Red, RandomNumbers(7, 0, 100)), Line("Dataset 2", Blue, RandomNumbers(7, 0, 100))], null, null, new Plugins { Tooltip = new Tooltip { Enabled = false, Position = "nearest", External = ChartJsFunction.FromName("renderExternalTooltip") }, Title = new Title { Display = true, Text = "External Tooltip" } });

    private static ChartJsConfig CreateTooltipInteractionsConfig() => CreateLineConfig("Tooltip Interaction Modes", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Title = new Title { Display = true, Text = "Tooltip Interactions" } });

    private static ChartJsConfig CreateTooltipPointStyleConfig() => CreateLineConfig("Tooltip Point Style", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100), pointStyle: "circle"), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100), pointStyle: "rectRot")], null, null, new Plugins { Tooltip = new Tooltip { UsePointStyle = true }, Title = new Title { Display = true, Text = "Tooltip Point Style" } });

    private static ChartJsConfig CreateTooltipPositionConfig() => CreateLineConfig("Tooltip Position", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Tooltip = new Tooltip { Position = "average" }, Title = new Title { Display = true, Text = ChartJsFunction.FromName("tooltipPositionTitle") } });

    private static ChartJsConfig CreateScriptableBarConfig() => new() { Type = ChartType.bar, Data = new ChartJsData { Labels = MonthLabels(16), Datasets = [new BarDataset { Data = RandomNumbers(16, -100, 100), BorderWidth = 2 }] }, Options = new ChartJsOptions { Plugins = new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = true } }, Elements = new ChartJsElementsOptions { Bar = new ChartJsBarElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), BorderColor = ChartJsFunction.FromName("scriptableBorderColor"), BorderWidth = 2 } } } };

    private static ChartJsConfig CreateScriptableBubbleConfig() => new() { Type = ChartType.bubble, Data = new ChartJsData { Datasets = [new BubbleDataset { Data = BubbleScriptableData(16) }] }, Options = new ChartJsOptions { Plugins = new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = true } }, Elements = new ChartJsElementsOptions { Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), BorderColor = ChartJsFunction.FromName("scriptableBorderColor"), BorderWidth = ChartJsFunction.FromName("scriptableRadius"), Radius = ChartJsFunction.FromName("scriptableBubbleRadius"), HoverBackgroundColor = ChartJsFunction.FromName("scriptableColor") } } } };

    private static ChartJsConfig CreateScriptableLineConfig() => CreateLineConfig("Scriptable Line", [new LineDataset { Data = RandomNumbers(12, 0, 100) }], null, MonthLabels(12), new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = true } }, new ChartJsElementsOptions { Line = new ChartJsLineElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), BorderColor = ChartJsFunction.FromName("scriptableColor"), Fill = false }, Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), HoverBackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), Radius = ChartJsFunction.FromName("scriptableRadius"), PointStyle = ChartJsFunction.FromName("scriptablePointStyle"), HoverRadius = 15 } });

    private static ChartJsConfig CreateScriptablePieConfig() => new() { Type = ChartType.pie, Data = new ChartJsData { Labels = ["Red", "Blue", "Yellow", "Green", "Purple"], Datasets = [new PieDataset { Data = RandomNumbers(5, 0, 100) }] }, Options = new ChartJsOptions { Cutout = 0, Elements = new ChartJsElementsOptions { Arc = new ChartJsArcElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableArcColor") } } } };

    private static ChartJsConfig CreateScriptablePolarConfig() => new() { Type = ChartType.polarArea, Data = new ChartJsData { Labels = ["Red", "Blue", "Yellow", "Green", "Purple"], Datasets = [new PolarAreaDataset { Data = RandomNumbers(5, 0, 100) }] }, Options = new ChartJsOptions { Elements = new ChartJsElementsOptions { Arc = new ChartJsArcElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableArcColor") } } } };

    private static ChartJsConfig CreateScriptableRadarConfig() => new() { Type = ChartType.radar, Data = new ChartJsData { Labels = MonthLabels(12), Datasets = [new RadarDataset { Data = RandomNumbers(12, 0, 100) }] }, Options = new ChartJsOptions { Elements = new ChartJsElementsOptions { Line = new ChartJsLineElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), BorderColor = ChartJsFunction.FromName("scriptableColor") }, Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), Radius = ChartJsFunction.FromName("scriptableRadius"), PointStyle = ChartJsFunction.FromName("scriptablePointStyle") } } } };

    private static ChartJsConfig CreateAnimationDelayConfig() => CreateLineConfig("Animation Delay", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Title = new Title { Display = true, Text = "Delay Animation" } }, animation: new Animation { Delay = ChartJsFunction.FromName("animationDelay"), OnComplete = ChartJsFunction.FromName("animationComplete") });

    private static ChartJsConfig CreateAnimationDropConfig() => CreateLineConfig("Drop Animation", [new LineDataset { Label = "Dataset 1", Animations = new Animations { Y = new Animations { Duration = 2000, Delay = 500 } }, Data = RandomNumbers(7, -100, 100), BorderColor = Red, BackgroundColor = RedTransparent, Fill = 1, Tension = 0.5 }, Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, null, animation: null, animations: new Animations { Y = new Animations { Easing = "easeInOutElastic", From = ChartJsFunction.FromName("animationDropFrom") } });

    private static ChartJsConfig CreateAnimationLoopConfig() => new() { Type = ChartType.scatter, Data = new ChartJsData { Datasets = [new BubbleDataset { Data = BubbleData(8), BackgroundColor = RedTransparent, BorderColor = Red }] }, Options = new ChartJsOptions { Plugins = new Plugins { Tooltip = new Tooltip { Enabled = false } }, Animations = new Animations { Radius = new Animations { Duration = 400, Easing = "linear", Loop = ChartJsFunction.FromName("animationLoopRadius") } }, Hover = new Interactions { Mode = "nearest", Intersect = true } } };

    private static ChartJsConfig CreateProgressiveLineConfig() => ProgressiveLineConfig("linear");

    private static ChartJsConfig CreateProgressiveLineEasingConfig() => ProgressiveLineConfig("easeInOutQuad");

    private static ChartJsConfig ProgressiveLineConfig(string easing)
    {
        var data = ProgressiveData(1000, 100);
        var data2 = ProgressiveData(1000, 80);
        return new ChartJsConfig { Type = ChartType.line, Data = new ChartJsData { Datasets = [new LineDataset { BorderColor = Red, BorderWidth = 1, PointRadius = 0, Data = data, Parsing = false, Normalized = true }, new LineDataset { BorderColor = Blue, BorderWidth = 1, PointRadius = 0, Data = data2, Parsing = false, Normalized = true }] }, Options = new ChartJsOptions { Animation = new Animation { Duration = 10000, Easing = easing }, Animations = new Animations { X = new Animations { Type = "number", Easing = easing, Duration = 10, From = ChartJsFunction.FromName("animationProgressiveFromNaN"), Delay = ChartJsFunction.FromName("animationProgressiveDelay") }, Y = new Animations { Type = "number", Easing = easing, Duration = 10, From = ChartJsFunction.FromName("animationProgressivePreviousY"), Delay = ChartJsFunction.FromName("animationProgressiveYDelay") } }, Interaction = new Interactions { Intersect = false }, Plugins = new Plugins { Legend = new Legend { Display = false } }, Scales = new ChartJsOptionsScales { X = new CartesianAxis { Type = "linear" } } } };
    }

    private static void SetProgressiveEasing(ChartJsBacklogSamplesBase c, string easing)
    {
        if (c.Config.Options?.Animation is Animation animation)
        {
            animation.Easing = easing;
        }

        if (c.Config.Options?.Animations?.X is Animations x)
        {
            x.Easing = easing;
        }

        if (c.Config.Options?.Animations?.Y is Animations y)
        {
            y.Easing = easing;
        }

        c.Config.UpdateChartOptions();
    }

    private static ChartJsConfig CreateLineConfig(string title, IList<ChartJsDataset> datasets, ChartJsOptionsScales? scales = null, IList<string>? labels = null, Plugins? plugins = null, ChartJsElementsOptions? elements = null, Animation? animation = null, Animations? animations = null)
    {
        return new ChartJsConfig { Type = ChartType.line, Data = new ChartJsData { Labels = labels ?? MonthLabels(7), Datasets = datasets }, Options = new ChartJsOptions { Responsive = true, Interaction = new Interactions { Mode = "index", Intersect = false }, Plugins = plugins ?? TitlePlugin(title), Scales = scales, Elements = elements, Animation = animation, Animations = animations } };
    }

    private static Plugins TitlePlugin(string title) => new() { Title = new Title { Display = true, Text = title } };

    private static LineDataset Line(string label, string color, IList<object> data, string? pointStyle = null)
    {
        var dataset = new LineDataset { Label = label, Data = data, BorderColor = color, BackgroundColor = ToTransparent(color) };
        if (pointStyle is not null)
        {
            dataset.PointStyle = pointStyle;
        }

        return dataset;
    }

    private static ScatterDataset CreateScatter(string label, string color) => new() { Label = label, Data = ScatterPoints(6), BorderColor = color, BackgroundColor = ToTransparent(color), Fill = false };

    private static List<object> RandomNumbers(int count, int min, int max)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(min, max + 1));
        }

        return data;
    }

    private static List<object> ScatterPoints(int count)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(new DataPoint { X = Random.Shared.Next(-100, 101), Y = Random.Shared.Next(-100, 101) });
        }

        return data;
    }

    private static List<object> BubbleData(int count)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(new BubbleDataPoint { X = Random.Shared.Next(-100, 101), Y = Random.Shared.Next(-100, 101), R = Random.Shared.Next(5, 20) });
        }

        return data;
    }

    private static List<object> BubbleScriptableData(int count)
    {
        List<object> data = new(count);
        for (var i = 0; i < count; i++)
        {
            data.Add(new BubbleScriptablePoint { X = Random.Shared.Next(-100, 101), Y = Random.Shared.Next(-100, 101), V = Random.Shared.Next(5, 25) });
        }

        return data;
    }

    private static List<object> ProgressiveData(int count, double start)
    {
        List<object> data = new(count);
        var previous = start;
        for (var i = 0; i < count; i++)
        {
            previous += 5 - Random.Shared.NextDouble() * 10;
            data.Add(new DataPoint { X = i, Y = previous });
        }

        return data;
    }

    private static List<string> MonthLabels(int count)
    {
        List<string> labels = new(count);
        for (var i = 0; i < count; i++)
        {
            labels.Add(Months[i % Months.Length]);
        }

        return labels;
    }

    private static string ToTransparent(string rgb)
    {
        return rgb.Replace("rgb(", "rgba(", StringComparison.Ordinal).Replace(")", ", 0.5)", StringComparison.Ordinal);
    }

    private const string CallbackCode =
        """
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";

        // The callback module contains the named Chart.js callbacks used by these official samples.
        """;

    private const string HtmlLegendPluginCode =
        """
        // htmlLegendPlugin.js is sample-owned. Register it after Chart.js is loaded, before reinitializing the chart.
        export function registerHtmlLegendPlugin() {
            Chart.register(htmlLegendPlugin);
        }

        const htmlLegendPlugin = {
            id: 'htmlLegend',
            afterUpdate(chart, _args, options) {
                // The sample plugin reuses existing legend DOM nodes across chart updates.
            }
        };
        """;

    public async ValueTask DisposeAsync()
    {
        if (externalPluginModule is not null)
        {
            await externalPluginModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }
}

#pragma warning disable CA1054, CA1056
public sealed record OfficialSampleDefinition(
    string Category,
    string Id,
    string Title,
    string DocsUrl,
    Func<ChartJsConfig> CreateConfig,
    Func<ChartJsBacklogSamplesBase, IReadOnlyList<ChartJsDocsAction>> CreateActions,
    ChartJsDocsCodeSet CSharpCode,
    ChartJsDocsCodeSet JavaScriptCode,
    string? CallbacksCode,
    string? ExternalPluginModuleLocation,
    string? ExternalPluginRegisterFunction);
#pragma warning restore CA1054, CA1056

public sealed record BubbleScriptablePoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double V { get; set; }
}
