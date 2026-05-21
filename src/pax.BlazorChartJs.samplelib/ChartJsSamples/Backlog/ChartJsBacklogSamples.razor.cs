using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.Backlog;

public sealed partial class ChartJsBacklogSamples : ChartJsBacklogSamplesBase
{
}

public abstract class ChartJsBacklogSamplesBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const string CallbackModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
    private const string HtmlLegendPluginModuleLocation = "/_content/pax.BlazorChartJs.samplelib/htmlLegendPlugin.js";
    private const string HtmlLegendPluginRegisterFunction = "registerHtmlLegendPlugin";
    private const string Red = "rgb(255, 99, 132)";
    private const string RedTransparent = "rgba(255, 99, 132, 0.5)";
    private const string Blue = "rgb(54, 162, 235)";
    private const string BlueTransparent = "rgba(54, 162, 235, 0.5)";
    private const string Yellow = "rgb(255, 205, 86)";
    private const string Green = "rgb(75, 192, 192)";
    private const string Purple = "rgb(153, 102, 255)";
    private const string Orange = "rgb(255, 159, 64)";
    private static readonly string[] Months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    private static readonly string[] PieColors = ["#CB4335", "#1F618D", "#F1C40F", "#27AE60", "#884EA0", "#D35400"];
    private static readonly string[] Palette = [Red, Blue, Yellow, Green, Purple, Orange];
    private static readonly Lazy<Dictionary<string, OfficialSampleDefinition>> Definitions = new(CreateDefinitions);

    private IJSObjectReference? externalPluginModule;
    private IJSObjectReference? callbacksModule;
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

    private void SetTooltipMode(string mode, string? axis = null)
    {
        Config.Options!.Interaction ??= new Interactions { Intersect = false };
        Config.Options.Interaction.Mode = mode;
        Config.Options.Interaction.Axis = axis;
        Config.UpdateChartOptions();
    }

    private void ToggleTooltipIntersect()
    {
        Config.Options!.Interaction ??= new Interactions { Mode = "index", Intersect = false };
        Config.Options.Interaction.Intersect = !(Config.Options.Interaction.Intersect ?? false);
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
        ], csharpCode: ScaleOptionsCenterCSharp, javascriptCode: ScaleOptionsCenterJavaScript);

        definitions[GetKey("scale-options", "grid")] = Definition("scale-options", "grid", "Grid Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/grid.html", CreateGridConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScaleOptionsGridCallbacksCode, csharpCode: ScaleOptionsGridCSharp, javascriptCode: ScaleOptionsGridJavaScript);
        definitions[GetKey("scale-options", "ticks")] = Definition("scale-options", "ticks", "Tick Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/ticks.html", CreateTicksConfig, c =>
        [
            c.CreateAction("align-start", "Tick Alignment: start", () => c.SetTickAlign("start")),
            c.CreateAction("align-center", "Tick Alignment: center (default)", () => c.SetTickAlign("center")),
            c.CreateAction("align-end", "Tick Alignment: end", () => c.SetTickAlign("end")),
        ], callbacks: ScaleOptionsTicksCallbacksCode, csharpCode: ScaleOptionsTicksCSharp, javascriptCode: ScaleOptionsTicksJavaScript);
        definitions[GetKey("scale-options", "titles")] = Definition("scale-options", "titles", "Title Configuration", "https://www.chartjs.org/docs/latest/samples/scale-options/titles.html", CreateScaleTitlesConfig, _ => [], csharpCode: ScaleOptionsTitlesCSharp, javascriptCode: ScaleOptionsTitlesJavaScript);
    }

    private static void AddLegend(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("legend", "events")] = Definition("legend", "events", "Events", "https://www.chartjs.org/docs/latest/samples/legend/events.html", CreateLegendEventsConfig, _ => [], callbacks: LegendEventsCallbacksCode);
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
        definitions[GetKey("tooltip", "content")] = Definition("tooltip", "content", "Custom Tooltip Content", "https://www.chartjs.org/docs/latest/samples/tooltip/content.html", CreateTooltipContentConfig, _ => [], callbacks: TooltipContentCallbacksCode);
        definitions[GetKey("tooltip", "html")] = Definition("tooltip", "html", "External HTML Tooltip", "https://www.chartjs.org/docs/latest/samples/tooltip/html.html", CreateTooltipHtmlConfig, _ => [], callbacks: TooltipHtmlCallbacksCode);
        definitions[GetKey("tooltip", "interactions")] = Definition("tooltip", "interactions", "Interaction Modes", "https://www.chartjs.org/docs/latest/samples/tooltip/interactions.html", CreateTooltipInteractionsConfig, c =>
        [
            c.CreateAction("mode-index", "Mode: index", () => c.SetTooltipMode("index", "xy")),
            c.CreateAction("mode-dataset", "Mode: dataset", () => c.SetTooltipMode("dataset", "xy")),
            c.CreateAction("mode-point", "Mode: point", () => c.SetTooltipMode("point", "xy")),
            c.CreateAction("mode-nearest-xy", "Mode: nearest, axis: xy", () => c.SetTooltipMode("nearest", "xy")),
            c.CreateAction("mode-nearest-x", "Mode: nearest, axis: x", () => c.SetTooltipMode("nearest", "x")),
            c.CreateAction("mode-nearest-y", "Mode: nearest, axis: y", () => c.SetTooltipMode("nearest", "y")),
            c.CreateAction("mode-x", "Mode: x", () => c.SetTooltipMode("x")),
            c.CreateAction("mode-y", "Mode: y", () => c.SetTooltipMode("y")),
            c.CreateAction("toggle-intersect", "Toggle Intersect", c.ToggleTooltipIntersect),
        ], callbacks: TooltipInteractionsCallbacksCode);
        definitions[GetKey("tooltip", "point-style")] = Definition("tooltip", "point-style", "Point Style", "https://www.chartjs.org/docs/latest/samples/tooltip/point-style.html", CreateTooltipPointStyleConfig, c => [c.CreateAction("toggle-use-point-style", "Toggle Use Point Style", c.ToggleTooltipPointStyle)]);
        definitions[GetKey("tooltip", "position")] = Definition("tooltip", "position", "Position", "https://www.chartjs.org/docs/latest/samples/tooltip/position.html", CreateTooltipPositionConfig, c =>
        [
            c.CreateAction("position-average", "Position: average", () => c.SetTooltipPosition("average")),
            c.CreateAction("position-nearest", "Position: nearest", () => c.SetTooltipPosition("nearest")),
            c.CreateAction("position-bottom", "Position: bottom (custom)", () => c.SetTooltipPosition("bottom")),
        ], callbacks: TooltipPositionCallbacksCode);
    }

    private static void AddScriptable(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("scriptable", "bar")] = Definition("scriptable", "bar", "Bar Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/bar.html", CreateScriptableBarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScriptableBarCallbacksCode);
        definitions[GetKey("scriptable", "bubble")] = Definition("scriptable", "bubble", "Bubble Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/bubble.html", CreateScriptableBubbleConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScriptableBubbleCallbacksCode);
        definitions[GetKey("scriptable", "line")] = Definition("scriptable", "line", "Line Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/line.html", CreateScriptableLineConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScriptableLineCallbacksCode);
        definitions[GetKey("scriptable", "pie")] = Definition("scriptable", "pie", "Pie Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/pie.html", CreateScriptablePieConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize), c.CreateAction("toggle-doughnut", "Toggle Doughnut View", c.TogglePieCutout)], callbacks: ScriptableArcCallbacksCode);
        definitions[GetKey("scriptable", "polar")] = Definition("scriptable", "polar", "Polar Area Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/polar.html", CreateScriptablePolarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScriptableArcCallbacksCode);
        definitions[GetKey("scriptable", "radar")] = Definition("scriptable", "radar", "Radar Chart", "https://www.chartjs.org/docs/latest/samples/scriptable/radar.html", CreateScriptableRadarConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], callbacks: ScriptableRadarCallbacksCode);
    }

    private static void AddAnimations(Dictionary<string, OfficialSampleDefinition> definitions)
    {
        definitions[GetKey("animations", "delay")] = Definition("animations", "delay", "Delay", "https://www.chartjs.org/docs/latest/samples/animations/delay.html", CreateAnimationDelayConfig, c => [c.CreateAction("randomize", "Randomize", c.Randomize)], animationTabs: true, callbacks: AnimationDelayCallbacksCode);
        definitions[GetKey("animations", "drop")] = Definition("animations", "drop", "Drop", "https://www.chartjs.org/docs/latest/samples/animations/drop.html", CreateAnimationDropConfig, c => FullDatasetActions(c), animationTabs: true, callbacks: AnimationDropCallbacksCode);
        definitions[GetKey("animations", "loop")] = Definition("animations", "loop", "Loop", "https://www.chartjs.org/docs/latest/samples/animations/loop.html", CreateAnimationLoopConfig, c => FullDatasetActions(c), animationTabs: true, callbacks: AnimationLoopCallbacksCode);
        definitions[GetKey("animations", "progressive-line")] = Definition("animations", "progressive-line", "Progressive Line", "https://www.chartjs.org/docs/latest/samples/animations/progressive-line.html", CreateProgressiveLineConfig, _ => [], animationTabs: true, callbacks: AnimationProgressiveCallbacksCode);
        definitions[GetKey("animations", "progressive-line-easing")] = Definition("animations", "progressive-line-easing", "Progressive Line With Easing", "https://www.chartjs.org/docs/latest/samples/animations/progressive-line-easing.html", CreateProgressiveLineEasingConfig, c =>
        [
            c.CreateAction("easing-ease-out-quad", "easeOutQuad", () => SetProgressiveEasing(c, "easeOutQuad")),
            c.CreateAction("easing-ease-out-cubic", "easeOutCubic", () => SetProgressiveEasing(c, "easeOutCubic")),
            c.CreateAction("easing-ease-out-quart", "easeOutQuart", () => SetProgressiveEasing(c, "easeOutQuart")),
            c.CreateAction("easing-ease-out-quint", "easeOutQuint", () => SetProgressiveEasing(c, "easeOutQuint")),
            c.CreateAction("easing-ease-in-quad", "easeInQuad", () => SetProgressiveEasing(c, "easeInQuad")),
            c.CreateAction("easing-ease-in-cubic", "easeInCubic", () => SetProgressiveEasing(c, "easeInCubic")),
            c.CreateAction("easing-ease-in-quart", "easeInQuart", () => SetProgressiveEasing(c, "easeInQuart")),
            c.CreateAction("easing-ease-in-quint", "easeInQuint", () => SetProgressiveEasing(c, "easeInQuint")),
        ], animationTabs: true, callbacks: AnimationProgressiveEasingCallbacksCode);
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
        var csharp = csharpCode ?? (animationTabs ? AnimationCSharpCode(createConfig, title) : DefaultCSharpCode(createConfig));
        var js = javascriptCode ?? (animationTabs ? AnimationJavaScriptCode(createConfig, title) : DefaultJavaScriptCode(createConfig));
        return new(category, id, title, docsUrl, createConfig, createActions, csharp, js, callbacks, externalPluginModuleLocation, externalPluginRegisterFunction);
    }

    private static string GetKey(string category, string sampleId) => $"{category}:{sampleId}";

    private static ChartJsDocsCodeSet DefaultCSharpCode(Func<ChartJsConfig> createConfig)
    {
        var config = createConfig();
        return new(
            CreateCSharpConfigCode(config),
            CreateCSharpDataCode(config),
            GeneralCSharpActionsCode);
    }

    private static ChartJsDocsCodeSet DefaultJavaScriptCode(Func<ChartJsConfig> createConfig)
    {
        var config = createConfig();
        return new(
            CreateJavaScriptConfigCode(config),
            CreateJavaScriptDataCode(config),
            GeneralJavaScriptActionsCode);
    }

    private static readonly ChartJsDocsCodeSet ScaleOptionsCenterCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.scatter,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Axis Center Positioning" },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis { Min = -100, Max = 100 },
                    Y = new CartesianAxis { Min = -100, Max = 100 },
                },
            },
        };
        """,
        """
        const int DataCount = 6;

        var data = new ChartJsData
        {
            Datasets =
            [
                new ScatterDataset
                {
                    Label = "Dataset 1",
                    Data = ScatterPoints(DataCount),
                    Fill = false,
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                },
                new ScatterDataset
                {
                    Label = "Dataset 2",
                    Data = ScatterPoints(DataCount),
                    Fill = false,
                    BorderColor = "rgb(54, 162, 235)",
                    BackgroundColor = "rgba(54, 162, 235, 0.5)",
                },
            ],
        };

        static List<object> ScatterPoints(int count)
        {
            List<object> data = new(count);
            for (var i = 0; i < count; i++)
            {
                data.Add(new DataPoint { X = Random.Shared.Next(-100, 101), Y = Random.Shared.Next(-100, 101) });
            }

            return data;
        }
        """,
        """
        void SetScalePositions(object x, object y)
        {
            if (config.Options?.Scales?.X is CartesianAxis xAxis &&
                config.Options.Scales.Y is CartesianAxis yAxis)
            {
                xAxis.Position = x;
                yAxis.Position = y;
                config.UpdateChartOptions();
            }
        }

        void DefaultPositions() => SetScalePositions("bottom", "left");
        void PositionCenter() => SetScalePositions("center", "center");
        void PositionValues() => SetScalePositions(new ChartJsScalePosition { Y = 30 }, new ChartJsScalePosition { X = -60 });
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsCenterJavaScript = new(
        """
        const config = {
          type: 'scatter',
          data,
          options: {
            responsive: true,
            plugins: {
              title: {
                display: true,
                text: 'Axis Center Positioning'
              }
            },
            scales: {
              x: {
                min: -100,
                max: 100
              },
              y: {
                min: -100,
                max: 100
              }
            }
          }
        };
        """,
        """
        const DATA_COUNT = 6;
        const NUMBER_CFG = {count: DATA_COUNT, min: -100, max: 100};
        const data = {
          datasets: [
            {
              label: 'Dataset 1',
              data: Utils.points(NUMBER_CFG),
              fill: false,
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
            },
            {
              label: 'Dataset 2',
              data: Utils.points(NUMBER_CFG),
              fill: false,
              borderColor: Utils.CHART_COLORS.blue,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5),
            }
          ]
        };
        """,
        """
        const actions = [
          {
            name: 'Default Positions',
            handler(chart) {
              chart.options.scales.x.position = 'bottom';
              chart.options.scales.y.position = 'left';
              chart.update();
            }
          },
          {
            name: 'Position: center',
            handler(chart) {
              chart.options.scales.x.position = 'center';
              chart.options.scales.y.position = 'center';
              chart.update();
            }
          },
          {
            name: 'Position: Vertical: x=-60, Horizontal: y=30',
            handler(chart) {
              chart.options.scales.x.position = {y: 30};
              chart.options.scales.y.position = {x: -60};
              chart.update();
            }
          }
        ];
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsGridCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Grid Line Settings" },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis
                    {
                        Border = new ChartJsAxisBorder { Display = true },
                        Grid = new ChartJsGrid
                        {
                            Display = true,
                            DrawOnChartArea = true,
                            DrawTicks = true,
                        },
                    },
                    Y = new CartesianAxis
                    {
                        Border = new ChartJsAxisBorder { Display = false },
                        Grid = new ChartJsGrid
                        {
                            Color = ChartJsFunction.FromName("scaleOptionsGridColor"),
                        },
                    },
                },
            },
        };
        """,
        """
        var data = new ChartJsData
        {
            Labels = ["January", "February", "March", "April", "May", "June", "July"],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset 1",
                    Data = [10, 30, 39, 20, 25, 34, -10],
                    Fill = false,
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                },
                new LineDataset
                {
                    Label = "Dataset 2",
                    Data = [18, 33, 22, 19, 11, -39, 30],
                    Fill = false,
                    BorderColor = "rgb(54, 162, 235)",
                    BackgroundColor = "rgba(54, 162, 235, 0.5)",
                },
            ],
        };

        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
        """,
        """
        void Randomize()
        {
            Dictionary<ChartJsDataset, SetDataObject> data = new(config.Data.Datasets.Count);
            foreach (var dataset in config.Data.Datasets)
            {
                data[dataset] = new SetDataObject(RandomNumbers(config.Data.Labels.Count, -100, 100));
            }

            config.SetData(data);
        }
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsGridJavaScript = new(
        """
        // Change these settings to change the display for different parts of the X axis
        // grid configuration.
        const DISPLAY = true;
        const BORDER = true;
        const CHART_AREA = true;
        const TICKS = true;

        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: {
              title: {
                display: true,
                text: 'Grid Line Settings'
              }
            },
            scales: {
              x: {
                border: {
                  display: BORDER
                },
                grid: {
                  display: DISPLAY,
                  drawOnChartArea: CHART_AREA,
                  drawTicks: TICKS
                }
              },
              y: {
                border: {
                  display: false
                },
                grid: {
                  color(context) {
                    if (context.tick.value > 0) {
                      return Utils.CHART_COLORS.green;
                    } else if (context.tick.value < 0) {
                      return Utils.CHART_COLORS.red;
                    }
                    return '#000000';
                  }
                }
              }
            }
          }
        };
        """,
        """
        const DATA_COUNT = 7;
        const data = {
          labels: Utils.months({count: DATA_COUNT}),
          datasets: [
            {
              label: 'Dataset 1',
              data: [10, 30, 39, 20, 25, 34, -10],
              fill: false,
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
            },
            {
              label: 'Dataset 2',
              data: [18, 33, 22, 19, 11, -39, 30],
              fill: false,
              borderColor: Utils.CHART_COLORS.blue,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5),
            }
          ]
        };
        """,
        """
        const actions = [
          {
            name: 'Randomize',
            handler(chart) {
              chart.data.datasets.forEach(dataset => {
                dataset.data = Utils.numbers({count: chart.data.labels.length, min: -100, max: 100});
              });
              chart.update();
            }
          }
        ];
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsTicksCSharp = new(
        """
        var config = new ChartJsConfig
        {
            Type = ChartType.line,
            Data = data,
            Options = new ChartJsOptions
            {
                Responsive = true,
                Plugins = new Plugins
                {
                    Title = new Title { Display = true, Text = "Chart with Tick Configuration" },
                },
                Scales = new ChartJsOptionsScales
                {
                    X = new CartesianAxis
                    {
                        Ticks = new CartesianAxisTick
                        {
                            Callback = ChartJsFunction.FromName("scaleOptionsTickLabel"),
                            Color = "red",
                            Align = "center",
                        },
                    },
                },
            },
        };
        """,
        """
        string[] labels =
        [
            "June\n2015", "July", "August", "September", "October", "November",
            "December", "January\n2016", "February", "March", "April", "May",
        ];

        var data = new ChartJsData
        {
            Labels = [.. labels],
            Datasets =
            [
                new LineDataset
                {
                    Label = "Dataset 1",
                    Data = RandomNumbers(labels.Length, 0, 100),
                    Fill = false,
                    BorderColor = "rgb(255, 99, 132)",
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",
                },
                new LineDataset
                {
                    Label = "Dataset 2",
                    Data = RandomNumbers(labels.Length, 0, 100),
                    Fill = false,
                    BorderColor = "rgb(54, 162, 235)",
                    BackgroundColor = "rgba(54, 162, 235, 0.5)",
                },
            ],
        };

        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "/_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
        """,
        """
        void SetTickAlign(string align)
        {
            if (config.Options?.Scales?.X?.Ticks is CartesianAxisTick ticks)
            {
                ticks.Align = align;
                config.UpdateChartOptions();
            }
        }

        void AlignStart() => SetTickAlign("start");
        void AlignCenter() => SetTickAlign("center");
        void AlignEnd() => SetTickAlign("end");
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsTicksJavaScript = new(
        """
        const config = {
          type: 'line',
          data,
          options: {
            responsive: true,
            plugins: {
              title: {
                display: true,
                text: 'Chart with Tick Configuration'
              }
            },
            scales: {
              x: {
                ticks: {
                  // For a category axis, val is the index, so use getLabelForValue.
                  callback(val, index) {
                    return index % 2 === 0 ? this.getLabelForValue(val) : '';
                  },
                  color: 'red'
                }
              }
            }
          }
        };
        """,
        """
        const DATA_COUNT = 12;
        const NUMBER_CFG = {count: DATA_COUNT, min: 0, max: 100};
        const data = {
          labels: [['June', '2015'], 'July', 'August', 'September', 'October', 'November', 'December', ['January', '2016'], 'February', 'March', 'April', 'May'],
          datasets: [
            {
              label: 'Dataset 1',
              data: Utils.numbers(NUMBER_CFG),
              fill: false,
              borderColor: Utils.CHART_COLORS.red,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
            },
            {
              label: 'Dataset 2',
              data: Utils.numbers(NUMBER_CFG),
              fill: false,
              borderColor: Utils.CHART_COLORS.blue,
              backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5),
            }
          ]
        };
        """,
        """
        const actions = [
          {
            name: 'Alignment: start',
            handler(chart) {
              chart.options.scales.x.ticks.align = 'start';
              chart.update();
            }
          },
          {
            name: 'Alignment: center (default)',
            handler(chart) {
              chart.options.scales.x.ticks.align = 'center';
              chart.update();
            }
          },
          {
            name: 'Alignment: end',
            handler(chart) {
              chart.options.scales.x.ticks.align = 'end';
              chart.update();
            }
          }
        ];
        """);

    private static readonly ChartJsDocsCodeSet ScaleOptionsTitlesCSharp = new(
        [
            new("config", "Config",
                """
                var config = new ChartJsConfig
                {
                    Type = ChartType.line,
                    Data = data,
                    Options = new ChartJsOptions
                    {
                        Responsive = true,
                        Scales = new ChartJsOptionsScales
                        {
                            X = new CartesianAxis
                            {
                                Display = true,
                                Title = new Title
                                {
                                    Display = true,
                                    Text = "Month",
                                    Color = "#911",
                                    Font = new Font
                                    {
                                        Family = "Comic Sans MS",
                                        Size = 20,
                                        Weight = "bold",
                                        LineHeight = 1.2,
                                    },
                                    Padding = new Padding { Top = 20, Left = 0, Right = 0, Bottom = 0 },
                                },
                            },
                            Y = new CartesianAxis
                            {
                                Display = true,
                                Title = new Title
                                {
                                    Display = true,
                                    Text = "Value",
                                    Color = "#191",
                                    Font = new Font
                                    {
                                        Family = "Times",
                                        Size = 20,
                                        Style = "normal",
                                        LineHeight = 1.2,
                                    },
                                    Padding = new Padding { Top = 30, Left = 0, Right = 0, Bottom = 0 },
                                },
                            },
                        },
                    },
                };
                """),
            new("setup", "Setup",
                """
                const int DataCount = 7;

                var data = new ChartJsData
                {
                    Labels = ["January", "February", "March", "April", "May", "June", "July"],
                    Datasets =
                    [
                        new LineDataset
                        {
                            Label = "Dataset 1",
                            Data = RandomNumbers(DataCount, 0, 100),
                            Fill = false,
                            BorderColor = "rgb(255, 99, 132)",
                            BackgroundColor = "rgba(255, 99, 132, 0.5)",
                        },
                        new LineDataset
                        {
                            Label = "Dataset 2",
                            Data = RandomNumbers(DataCount, 0, 100),
                            Fill = false,
                            BorderColor = "rgb(54, 162, 235)",
                            BackgroundColor = "rgba(54, 162, 235, 0.5)",
                        },
                    ],
                };
                """),
        ]);

    private static readonly ChartJsDocsCodeSet ScaleOptionsTitlesJavaScript = new(
        [
            new("config", "Config",
                """
                const config = {
                  type: 'line',
                  data,
                  options: {
                    responsive: true,
                    scales: {
                      x: {
                        display: true,
                        title: {
                          display: true,
                          text: 'Month',
                          color: '#911',
                          font: {
                            family: 'Comic Sans MS',
                            size: 20,
                            weight: 'bold',
                            lineHeight: 1.2
                          },
                          padding: {top: 20, left: 0, right: 0, bottom: 0}
                        }
                      },
                      y: {
                        display: true,
                        title: {
                          display: true,
                          text: 'Value',
                          color: '#191',
                          font: {
                            family: 'Times',
                            size: 20,
                            style: 'normal',
                            lineHeight: 1.2
                          },
                          padding: {top: 30, left: 0, right: 0, bottom: 0}
                        }
                      }
                    }
                  }
                };
                """),
            new("setup", "Setup",
                """
                const DATA_COUNT = 7;
                const NUMBER_CFG = {count: DATA_COUNT, min: 0, max: 100};
                const data = {
                  labels: Utils.months({count: DATA_COUNT}),
                  datasets: [
                    {
                      label: 'Dataset 1',
                      data: Utils.numbers(NUMBER_CFG),
                      fill: false,
                      borderColor: Utils.CHART_COLORS.red,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
                    },
                    {
                      label: 'Dataset 2',
                      data: Utils.numbers(NUMBER_CFG),
                      fill: false,
                      borderColor: Utils.CHART_COLORS.blue,
                      backgroundColor: Utils.transparentize(Utils.CHART_COLORS.blue, 0.5),
                    }
                  ]
                };
                """),
        ]);

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

    private static ChartJsDocsCodeSet AnimationCSharpCode(Func<ChartJsConfig> createConfig, string title)
    {
        var config = createConfig();
        return new(
        [
            new("config", "Config", CreateCSharpConfigCode(config)),
            new("animation", "Animation", CreateCSharpAnimationCode(config, title)),
            new("data", "Data", CreateCSharpDataCode(config)),
            new("actions", "Actions", GeneralCSharpActionsCode),
        ]);
    }

    private static ChartJsDocsCodeSet AnimationJavaScriptCode(Func<ChartJsConfig> createConfig, string title)
    {
        var config = createConfig();
        return new(
        [
            new("config", "Config", CreateJavaScriptConfigCode(config)),
            new("animation", "Animation", CreateJavaScriptAnimationCode(config, title)),
            new("data", "Data", CreateJavaScriptDataCode(config)),
            new("actions", "Actions", GeneralJavaScriptActionsCode),
        ]);
    }

    private const string GeneralCSharpActionsCode =
        """
        // Actions mutate the live typed config and use targeted updates:
        // - SetData(...) for batched data changes
        // - UpdateChartOptions() for option-only changes
        // - ReinitializeChart() only when Chart.js needs a new chart instance
        """;

    private const string GeneralJavaScriptActionsCode =
        """
        // Official actions mutate chart.data or chart.options, then call chart.update().
        const actions = [];
        """;

    private static string CreateCSharpConfigCode(ChartJsConfig config)
    {
        StringBuilder builder = new();
        builder.AppendLine("var config = new ChartJsConfig");
        builder.AppendLine("{");
        if (config.Type is not null)
        {
            builder.Append("    Type = ").Append(WriteCSharpValue(config.Type, 1)).AppendLine(",");
        }

        builder.AppendLine("    Data = data,");
        if (config.Options is not null)
        {
            builder.Append("    Options = ").Append(WriteCSharpValue(config.Options, 1)).AppendLine(",");
        }

        builder.AppendLine("};");
        return builder.ToString();
    }

    private static string CreateCSharpDataCode(ChartJsConfig config)
    {
        return $"var data = {WriteCSharpValue(config.Data, 0)};";
    }

    private static string CreateCSharpAnimationCode(ChartJsConfig config, string title)
    {
        StringBuilder builder = new();
        builder.Append("// ").Append(title).AppendLine(": animation-related options from the typed chart config.");
        if (config.Options?.Animation is not null)
        {
            builder.Append("var animation = ").Append(WriteCSharpValue(config.Options.Animation, 0)).AppendLine(";");
        }

        if (config.Options?.Animations is not null)
        {
            builder.Append("var animations = ").Append(WriteCSharpValue(config.Options.Animations, 0)).AppendLine(";");
        }

        if (config.Data.Datasets.Any(dataset => dataset.Animations is not null))
        {
            builder.AppendLine("// Dataset-specific Animations values are shown in the Data tab.");
        }

        return builder.ToString();
    }

    private static string CreateJavaScriptConfigCode(ChartJsConfig config)
    {
        StringBuilder builder = new();
        builder.AppendLine("const config = {");
        if (config.Type is not null)
        {
            builder.Append("  type: '").Append(config.Type.Value).AppendLine("',");
        }

        builder.AppendLine("  data,");
        if (config.Options is not null)
        {
            builder.Append("  options: ").Append(WriteJavaScriptValue(config.Options, 1)).AppendLine();
        }

        builder.AppendLine("};");
        return builder.ToString();
    }

    private static string CreateJavaScriptDataCode(ChartJsConfig config)
    {
        return $"const data = {WriteJavaScriptValue(config.Data, 0)};";
    }

    private static string CreateJavaScriptAnimationCode(ChartJsConfig config, string title)
    {
        StringBuilder builder = new();
        builder.Append("// ").Append(title).AppendLine(": animation-related options mirrored from the official sample.");
        if (config.Options?.Animation is not null)
        {
            builder.Append("const animation = ").Append(WriteJavaScriptValue(config.Options.Animation, 0)).AppendLine(";");
        }

        if (config.Options?.Animations is not null)
        {
            builder.Append("const animations = ").Append(WriteJavaScriptValue(config.Options.Animations, 0)).AppendLine(";");
        }

        if (config.Data.Datasets.Any(dataset => dataset.Animations is not null))
        {
            builder.AppendLine("// Dataset-specific animations are shown in the Data tab.");
        }

        return builder.ToString();
    }

    private static string WriteCSharpValue(object? value, int indent)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is ChartJsFunction function)
        {
            return $"ChartJsFunction.FromName(\"{EscapeCSharpString(function.Name)}\")";
        }

        if (IsIndexableOption(value))
        {
            return WriteIndexableOption(value, indent);
        }

        return value switch
        {
            string text => $"\"{EscapeCSharpString(text)}\"",
            bool boolean => boolean ? "true" : "false",
            int or long or short or byte => Convert.ToString(value, CultureInfo.InvariantCulture)!,
            double number => number.ToString("G", CultureInfo.InvariantCulture),
            float number => number.ToString("G", CultureInfo.InvariantCulture),
            decimal number => number.ToString(CultureInfo.InvariantCulture),
            Enum enumValue => $"{enumValue.GetType().Name}.{enumValue}",
            DateOnly date => $"\"{date:yyyy-MM-dd}\"",
            IEnumerable enumerable when value is not string => WriteCSharpEnumerable(enumerable, indent),
            _ => WriteCSharpObject(value, indent),
        };
    }

    private static string WriteIndexableOption(object value, int indent)
    {
        var type = value.GetType();
        var kind = Convert.ToString(type.GetProperty("Kind")?.GetValue(value), CultureInfo.InvariantCulture);
        return kind switch
        {
            "SingleValue" => WriteCSharpValue(type.GetProperty("SingleValue")?.GetValue(value), indent),
            "Indexed" => WriteCSharpValue(type.GetProperty("IndexedValues")?.GetValue(value), indent),
            "Function" => WriteCSharpValue(type.GetProperty("FunctionValue")?.GetValue(value), indent),
            _ => "null",
        };
    }

    private static string WriteCSharpEnumerable(IEnumerable enumerable, int indent)
    {
        var values = enumerable.Cast<object?>().ToList();
        if (values.Count == 0)
        {
            return "[]";
        }

        StringBuilder builder = new();
        builder.AppendLine("[");
        var nextIndent = indent + 1;
        var visibleCount = Math.Min(values.Count, 24);
        for (var i = 0; i < visibleCount; i++)
        {
            builder.Append(GetIndent(nextIndent))
                .Append(WriteCSharpValue(values[i], nextIndent))
                .AppendLine(",");
        }

        if (values.Count > visibleCount)
        {
            builder.Append(GetIndent(nextIndent))
                .Append("/* ")
                .Append(values.Count - visibleCount)
                .AppendLine(" generated values omitted for readability */");
        }

        builder.Append(GetIndent(indent)).Append(']');
        return builder.ToString();
    }

    private static string WriteCSharpObject(object value, int indent)
    {
        var type = value.GetType();
        var properties = GetSerializableProperties(type)
            .Select(property => new { Property = property, Value = property.GetValue(value) })
            .Where(item => ShouldWriteCSharpProperty(value, item.Property, item.Value))
            .ToList();

        if (properties.Count == 0)
        {
            return IsAnonymousType(type) ? "new { }" : $"new {GetCSharpTypeName(type)}()";
        }

        StringBuilder builder = new();
        builder.Append(IsAnonymousType(type) ? "new" : $"new {GetCSharpTypeName(type)}");
        builder.AppendLine();
        builder.Append(GetIndent(indent)).AppendLine("{");
        var nextIndent = indent + 1;
        foreach (var item in properties)
        {
            builder.Append(GetIndent(nextIndent))
                .Append(item.Property.Name)
                .Append(" = ")
                .Append(WriteCSharpValue(item.Value, nextIndent))
                .AppendLine(",");
        }

        builder.Append(GetIndent(indent)).Append('}');
        return builder.ToString();
    }

    private static IEnumerable<PropertyInfo> GetSerializableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is not null && property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.DeclaringType == type ? 0 : 1)
            .ThenBy(property => property.MetadataToken);
    }

    private static bool ShouldWriteCSharpProperty(object owner, PropertyInfo property, object? value)
    {
        if (value is null
            || property.Name is nameof(ChartJsConfig.ChartJsConfigGuid)
            || owner is ChartJsDataset && property.Name == nameof(ChartJsDataset.Id))
        {
            return false;
        }

        return IsIndexableOption(value)
            || value is not IEnumerable enumerable
            || value is string
            || enumerable.Cast<object?>().Any();
    }

    private static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var tickIndex = type.Name.IndexOf('`', StringComparison.Ordinal);
        return tickIndex < 0 ? type.Name : type.Name[..tickIndex];
    }

    private static bool IsIndexableOption(object value)
    {
        var type = value.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IndexableOption<>);
    }

    private static bool IsAnonymousType(Type type)
    {
        return type.Name.Contains("AnonymousType", StringComparison.Ordinal);
    }

    private static string GetIndent(int indent)
    {
        return new string(' ', indent * 4);
    }

    private static string EscapeCSharpString(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private static string WriteJavaScriptValue(object? value, int indent)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is ChartJsFunction function)
        {
            return function.Name;
        }

        if (IsIndexableOption(value))
        {
            return WriteJavaScriptIndexableOption(value, indent);
        }

        return value switch
        {
            string text => $"'{EscapeJavaScriptString(text)}'",
            bool boolean => boolean ? "true" : "false",
            int or long or short or byte => Convert.ToString(value, CultureInfo.InvariantCulture)!,
            double number => number.ToString("G", CultureInfo.InvariantCulture),
            float number => number.ToString("G", CultureInfo.InvariantCulture),
            decimal number => number.ToString(CultureInfo.InvariantCulture),
            Enum enumValue => $"'{enumValue}'",
            DateOnly date => $"'{date:yyyy-MM-dd}'",
            IEnumerable enumerable when value is not string => WriteJavaScriptEnumerable(enumerable, indent),
            _ => WriteJavaScriptObject(value, indent),
        };
    }

    private static string WriteJavaScriptIndexableOption(object value, int indent)
    {
        var type = value.GetType();
        var kind = Convert.ToString(type.GetProperty("Kind")?.GetValue(value), CultureInfo.InvariantCulture);
        return kind switch
        {
            "SingleValue" => WriteJavaScriptValue(type.GetProperty("SingleValue")?.GetValue(value), indent),
            "Indexed" => WriteJavaScriptValue(type.GetProperty("IndexedValues")?.GetValue(value), indent),
            "Function" => WriteJavaScriptValue(type.GetProperty("FunctionValue")?.GetValue(value), indent),
            _ => "null",
        };
    }

    private static string WriteJavaScriptEnumerable(IEnumerable enumerable, int indent)
    {
        var values = enumerable.Cast<object?>().ToList();
        if (values.Count == 0)
        {
            return "[]";
        }

        StringBuilder builder = new();
        builder.AppendLine("[");
        var nextIndent = indent + 1;
        var visibleCount = Math.Min(values.Count, 24);
        for (var i = 0; i < visibleCount; i++)
        {
            builder.Append(GetIndent(nextIndent))
                .Append(WriteJavaScriptValue(values[i], nextIndent))
                .AppendLine(",");
        }

        if (values.Count > visibleCount)
        {
            builder.Append(GetIndent(nextIndent))
                .Append("/* ")
                .Append(values.Count - visibleCount)
                .AppendLine(" generated values omitted for readability */");
        }

        builder.Append(GetIndent(indent)).Append(']');
        return builder.ToString();
    }

    private static string WriteJavaScriptObject(object value, int indent)
    {
        var type = value.GetType();
        var properties = GetSerializableProperties(type)
            .Select(property => new { Property = property, Value = property.GetValue(value) })
            .Where(item => ShouldWriteCSharpProperty(value, item.Property, item.Value))
            .ToList();

        if (properties.Count == 0)
        {
            return "{}";
        }

        StringBuilder builder = new();
        builder.AppendLine("{");
        var nextIndent = indent + 1;
        foreach (var item in properties)
        {
            builder.Append(GetIndent(nextIndent))
                .Append(ToCamelCase(item.Property.Name))
                .Append(": ")
                .Append(WriteJavaScriptValue(item.Value, nextIndent))
                .AppendLine(",");
        }

        builder.Append(GetIndent(indent)).Append('}');
        return builder.ToString();
    }

    private static string ToCamelCase(string value)
    {
        return string.IsNullOrEmpty(value) || char.IsLower(value[0])
            ? value
            : char.ToLowerInvariant(value[0]) + value[1..];
    }

    private static string EscapeJavaScriptString(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\\'", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

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
        var labels = new List<string> { "June\n2015", "July", "August", "September", "October", "November", "December", "January\n2016", "February", "March", "April", "May" };
        return CreateLineConfig("Chart with Tick Configuration", [Line("Dataset 1", Red, RandomNumbers(labels.Count, 0, 100)), Line("Dataset 2", Blue, RandomNumbers(labels.Count, 0, 100))], new ChartJsOptionsScales { X = new CartesianAxis { Ticks = new CartesianAxisTick { Callback = ChartJsFunction.FromName("scaleOptionsTickLabel"), Color = "red", Align = "center" } } }, labels);
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

    private static ChartJsConfig CreateTooltipInteractionsConfig() => CreateLineConfig("Tooltip Interaction Modes", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Title = new Title { Display = true, Text = ChartJsFunction.FromName("tooltipInteractionTitle") } });

    private static ChartJsConfig CreateTooltipPointStyleConfig() => CreateLineConfig("Tooltip Point Style", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100), pointStyle: "circle"), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100), pointStyle: "rectRot")], null, null, new Plugins { Tooltip = new Tooltip { UsePointStyle = true }, Title = new Title { Display = true, Text = "Tooltip Point Style" } });

    private static ChartJsConfig CreateTooltipPositionConfig() => CreateLineConfig("Tooltip Position", [Line("Dataset 1", Red, RandomNumbers(7, -100, 100)), Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, new Plugins { Tooltip = new Tooltip { Position = "average" }, Title = new Title { Display = true, Text = ChartJsFunction.FromName("tooltipPositionTitle") } });

    private static ChartJsConfig CreateScriptableBarConfig() => new() { Type = ChartType.bar, Data = new ChartJsData { Labels = MonthLabels(16), Datasets = [new BarDataset { Data = RandomNumbers(16, -100, 100) }] }, Options = new ChartJsOptions { Plugins = new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = true } }, Elements = new ChartJsElementsOptions { Bar = new ChartJsBarElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableBarBackgroundColor"), BorderColor = ChartJsFunction.FromName("scriptableBarBorderColor"), BorderWidth = 2 } } } };

    private static ChartJsConfig CreateScriptableBubbleConfig() => new() { Type = ChartType.bubble, Data = new ChartJsData { Datasets = [new BubbleDataset { Data = BubbleScriptableData(16) }, new BubbleDataset { Data = BubbleScriptableData(16) }] }, Options = new ChartJsOptions { AspectRatio = 1, Plugins = new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = false } }, Elements = new ChartJsElementsOptions { Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableBubbleBackgroundColor"), BorderColor = ChartJsFunction.FromName("scriptableBubbleBorderColor"), BorderWidth = ChartJsFunction.FromName("scriptableBubbleBorderWidth"), HoverBackgroundColor = "transparent", HoverBorderColor = ChartJsFunction.FromName("scriptableBubbleHoverBorderColor"), HoverBorderWidth = ChartJsFunction.FromName("scriptableBubbleHoverBorderWidth"), Radius = ChartJsFunction.FromName("scriptableBubbleRadius") } } } };

    private static ChartJsConfig CreateScriptableLineConfig() => CreateLineConfig("Scriptable Line", [new LineDataset { Data = RandomNumbers(12, 0, 100) }], null, MonthLabels(12), new Plugins { Legend = new Legend { Display = false }, Tooltip = new Tooltip { Enabled = true } }, new ChartJsElementsOptions { Line = new ChartJsLineElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), BorderColor = ChartJsFunction.FromName("scriptableColor"), Fill = false }, Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), HoverBackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), Radius = ChartJsFunction.FromName("scriptableRadius"), PointStyle = ChartJsFunction.FromName("scriptablePointStyle"), HoverRadius = 15 } });

    private static ChartJsConfig CreateScriptablePieConfig() => new() { Type = ChartType.pie, Data = new ChartJsData { Labels = ["Red", "Blue", "Yellow", "Green", "Purple"], Datasets = [new PieDataset { Data = RandomNumbers(5, 0, 100) }] }, Options = new ChartJsOptions { Cutout = 0, Elements = new ChartJsElementsOptions { Arc = new ChartJsArcElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableArcColor") } } } };

    private static ChartJsConfig CreateScriptablePolarConfig() => new() { Type = ChartType.polarArea, Data = new ChartJsData { Labels = ["Red", "Blue", "Yellow", "Green", "Purple"], Datasets = [new PolarAreaDataset { Data = RandomNumbers(5, 0, 100) }] }, Options = new ChartJsOptions { Elements = new ChartJsElementsOptions { Arc = new ChartJsArcElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableArcColor") } } } };

    private static ChartJsConfig CreateScriptableRadarConfig() => new() { Type = ChartType.radar, Data = new ChartJsData { Labels = MonthLabels(12), Datasets = [new RadarDataset { Data = RandomNumbers(12, 0, 100) }] }, Options = new ChartJsOptions { Elements = new ChartJsElementsOptions { Line = new ChartJsLineElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableTransparentColor"), BorderColor = ChartJsFunction.FromName("scriptableColor") }, Point = new ChartJsPointElementOptions { BackgroundColor = ChartJsFunction.FromName("scriptableColor"), Radius = ChartJsFunction.FromName("scriptableRadius"), PointStyle = ChartJsFunction.FromName("scriptablePointStyle") } } } };

    private static ChartJsConfig CreateAnimationDelayConfig() => new() { Type = ChartType.bar, Data = new ChartJsData { Labels = MonthLabels(7), Datasets = [new BarDataset { Label = "Dataset 1", Data = RandomNumbers(7, -100, 100), BackgroundColor = Red }, new BarDataset { Label = "Dataset 2", Data = RandomNumbers(7, -100, 100), BackgroundColor = Blue }, new BarDataset { Label = "Dataset 3", Data = RandomNumbers(7, -100, 100), BackgroundColor = Green }] }, Options = new ChartJsOptions { Animation = new Animation { Delay = ChartJsFunction.FromName("animationDelay"), OnComplete = ChartJsFunction.FromName("animationComplete") }, Scales = new ChartJsOptionsScales { X = new CartesianAxis { Stacked = true }, Y = new CartesianAxis { Stacked = true } } } };

    private static ChartJsConfig CreateAnimationDropConfig() => CreateLineConfig("Drop Animation", [new LineDataset { Label = "Dataset 1", Animations = new Animations { Y = new Animations { Duration = 2000, Delay = 500 } }, Data = RandomNumbers(7, -100, 100), BorderColor = Red, BackgroundColor = RedTransparent, Fill = 1, Tension = 0.5 }, Line("Dataset 2", Blue, RandomNumbers(7, -100, 100))], null, null, null, animation: null, animations: new Animations { Y = new Animations { Easing = "easeInOutElastic", From = ChartJsFunction.FromName("animationDropFrom") } });

    private static ChartJsConfig CreateAnimationLoopConfig() => new() { Type = ChartType.line, Data = new ChartJsData { Labels = MonthLabels(7), Datasets = [new LineDataset { Label = "Dataset 1", Data = RandomNumbers(7, -100, 100), BorderColor = Red, BackgroundColor = RedTransparent, Tension = 0.4 }, new LineDataset { Label = "Dataset 2", Data = RandomNumbers(7, -100, 100), BorderColor = Blue, BackgroundColor = BlueTransparent, Tension = 0.2 }] }, Options = new ChartJsOptions { Animations = new Animations { Radius = new Animations { Duration = 400, Easing = "linear", Loop = ChartJsFunction.FromName("animationLoopRadius") } }, Elements = new ChartJsElementsOptions { Point = new ChartJsPointElementOptions { HoverRadius = 12, HoverBackgroundColor = "yellow" } }, Interaction = new Interactions { Mode = "nearest", Intersect = false, Axis = "x" }, Plugins = new Plugins { Tooltip = new Tooltip { Enabled = false } } } };

    private static ChartJsConfig CreateProgressiveLineConfig() => ProgressiveLineConfig("linear");

    private static ChartJsConfig CreateProgressiveLineEasingConfig() => ProgressiveLineConfig("easeOutQuad", includeTitle: true);

    private static ChartJsConfig ProgressiveLineConfig(string easing, bool includeTitle = false)
    {
        var data = ProgressiveData(1000, 100);
        var data2 = ProgressiveData(1000, 80);
        return new ChartJsConfig { Type = ChartType.line, Data = new ChartJsData { Datasets = [new LineDataset { BorderColor = Red, BorderWidth = 1, PointRadius = 0, Data = data, Parsing = false, Normalized = true }, new LineDataset { BorderColor = Blue, BorderWidth = 1, PointRadius = 0, Data = data2, Parsing = false, Normalized = true }] }, Options = new ChartJsOptions { Animation = new Animation { Duration = 5000, Easing = easing }, Animations = new Animations { X = new Animations { Type = "number", Easing = "linear", Duration = 10, From = ChartJsFunction.FromName("animationProgressiveFromNaN"), Delay = ChartJsFunction.FromName(includeTitle ? "animationProgressiveEasingDelay" : "animationProgressiveDelay") }, Y = new Animations { Type = "number", Easing = "linear", Duration = 10, From = ChartJsFunction.FromName("animationProgressivePreviousY"), Delay = ChartJsFunction.FromName(includeTitle ? "animationProgressiveEasingYDelay" : "animationProgressiveYDelay") } }, Interaction = new Interactions { Intersect = false }, Plugins = new Plugins { Legend = new Legend { Display = false }, Title = includeTitle ? new Title { Display = true, Text = ChartJsFunction.FromName("animationProgressiveEasingTitle") } : null }, Scales = new ChartJsOptionsScales { X = new CartesianAxis { Type = "linear" } } } };
    }

    private static async Task SetProgressiveEasing(ChartJsBacklogSamplesBase c, string easing)
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

        c.callbacksModule ??= await c.JSRuntime.InvokeAsync<IJSObjectReference>("import", CallbackModuleLocation).ConfigureAwait(false);
        await c.callbacksModule.InvokeVoidAsync("restartProgressiveLineEasing", c.Config.ChartJsConfigGuid.ToString(), easing).ConfigureAwait(false);
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
            data.Add(new BubbleScriptablePoint { X = Random.Shared.Next(-150, 101), Y = Random.Shared.Next(-150, 101), V = Random.Shared.Next(0, 1001) });
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

    private static string CallbackModuleCode(string callbackMembers, string helpers = "", string exports = "")
    {
        return $$"""
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "{{CallbackModuleLocation}}";

        // chartJsCallbacks.js
        {{helpers}}
        const callbacks = Object.assign(Object.create(null), {
        {{callbackMembers}}
        });

        {{exports}}
        export const chartJsCallbacks = Object.freeze(callbacks);
        """;
    }

    private static readonly string LegendEventsCallbacksCode = CallbackModuleCode(
        """
          legendHandleHover(_event, item, legend) {
            legend.chart.data.datasets[0].backgroundColor.forEach((color, index, colors) => {
              colors[index] = index === item.index || color.length === 9 ? color : `${color}4D`;
            });
            legend.chart.update();
          },
          legendHandleLeave(_event, _item, legend) {
            legend.chart.data.datasets[0].backgroundColor.forEach((color, index, colors) => {
              colors[index] = color.length === 9 ? color.slice(0, -2) : color;
            });
            legend.chart.update();
          }
        """);

    private static readonly string TooltipContentCallbacksCode = CallbackModuleCode(
        """
          tooltipContentFooter(items) {
            return `Sum: ${getTooltipTotal(items)}`;
          }
        """,
        """
        function getTooltipTotal(items) {
          return items.reduce((total, item) => total + Number(item?.raw ?? item?.parsed?.y ?? 0), 0);
        }
        """);

    private static readonly string TooltipHtmlCallbacksCode = CallbackModuleCode(
        """
          renderExternalTooltip(context) {
            const { chart, tooltip } = context;
            const tooltipElement = getOrCreateExternalTooltip(chart);

            if (!tooltip || tooltip.opacity === 0) {
              tooltipElement.style.opacity = '0';
              return;
            }

            const title = tooltip.title?.join(' ') ?? '';
            const body = tooltip.body?.map(item => item.lines.join(' ')).join(' | ') ?? '';
            const footer = tooltip.footer?.join(' ') ?? '';
            tooltipElement.textContent = [title, body, footer].filter(Boolean).join(' - ');
            tooltipElement.style.left = `${tooltip.caretX}px`;
            tooltipElement.style.top = `${tooltip.caretY}px`;
            tooltipElement.style.opacity = '1';
          }
        """,
        """
        const externalTooltipCache = new WeakMap();

        function getOrCreateExternalTooltip(chart) {
          let tooltipElement = externalTooltipCache.get(chart);
          if (tooltipElement) {
            return tooltipElement;
          }

          tooltipElement = document.createElement('div');
          tooltipElement.className = 'chartjs-external-tooltip';
          tooltipElement.style.opacity = '0';
          tooltipElement.style.pointerEvents = 'none';
          tooltipElement.style.position = 'absolute';
          tooltipElement.style.transform = 'translate(-50%, calc(-100% - 10px))';

          const parent = chart.canvas.parentNode;
          if (parent instanceof HTMLElement) {
            if (getComputedStyle(parent).position === 'static') {
              parent.style.position = 'relative';
            }
            parent.appendChild(tooltipElement);
          }

          externalTooltipCache.set(chart, tooltipElement);
          return tooltipElement;
        }
        """);

    private static readonly string TooltipInteractionsCallbacksCode = CallbackModuleCode(
        """
          tooltipInteractionTitle(context) {
            const { axis = 'xy', intersect, mode } = context.chart.options.interaction;
            return `Mode: ${mode}, axis: ${axis}, intersect: ${intersect}`;
          }
        """);

    private static readonly string TooltipPositionCallbacksCode = CallbackModuleCode(
        """
          tooltipPositionTitle(context) {
            ensureBottomTooltipPositioner();
            return `Tooltip position mode: ${context.chart.options.plugins.tooltip.position}`;
          }
        """,
        """
        function ensureBottomTooltipPositioner() {
          if (!globalThis.Chart?.Tooltip?.positioners || globalThis.Chart.Tooltip.positioners.bottom) {
            return;
          }

          globalThis.Chart.Tooltip.positioners.bottom = function (items) {
            const pos = globalThis.Chart.Tooltip.positioners.average(items);
            return pos === false
              ? false
              : { x: pos.x, y: this.chart.chartArea.bottom, xAlign: 'center', yAlign: 'bottom' };
          };
        }
        """);

    private static readonly string ScriptableBarCallbacksCode = CallbackModuleCode(
        """
          scriptableBarBackgroundColor(context) {
            const value = Number(context.parsed?.y ?? context.raw ?? 0);
            return transparentizeHex(scriptableBarColor(context), Math.min(Math.abs(value / 150), 1));
          },
          scriptableBarBorderColor(context) {
            return scriptableBarColor(context);
          }
        """,
        """
        function transparentizeHex(hex, alpha = 0.5) {
          const value = hex.replace('#', '');
          const r = parseInt(value.substring(0, 2), 16);
          const g = parseInt(value.substring(2, 4), 16);
          const b = parseInt(value.substring(4, 6), 16);
          return `rgba(${r}, ${g}, ${b}, ${alpha})`;
        }

        function scriptableBarColor(context) {
          const value = Number(context.parsed?.y ?? context.raw ?? 0);
          return value < -50 ? '#D60000' : value < 0 ? '#F46300' : value < 50 ? '#0358B6' : '#44DE28';
        }
        """);

    private static readonly string ScriptableBubbleCallbacksCode = CallbackModuleCode(
        """
          scriptableBubbleRadius(context) {
            const size = context.chart.width;
            const base = Math.abs(Number(context.raw?.v ?? context.raw?.r ?? 5)) / 1000;
            return (size / 24) * base;
          },
          scriptableBubbleBackgroundColor(context) {
            return scriptableBubbleColor(false, context);
          },
          scriptableBubbleBorderColor(context) {
            return scriptableBubbleColor(true, context);
          },
          scriptableBubbleBorderWidth(context) {
            return Math.min(Math.max(1, (context.datasetIndex ?? 0) + 1), 8);
          },
          scriptableBubbleHoverBorderColor(context) {
            return colorByDataset(context.datasetIndex ?? 0);
          },
          scriptableBubbleHoverBorderWidth(context) {
            return Math.round(8 * Number(context.raw?.v ?? 0) / 1000);
          }
        """,
        """
        function colorByDataset(datasetIndex) {
          const colors = ['#ff6384', '#36a2eb', '#ffcd56', '#4bc0c0', '#9966ff', '#ff9f40'];
          return colors[datasetIndex % colors.length];
        }

        function scriptableBubbleColor(opaque, context) {
          const value = context.raw ?? {};
          const x = Number(value.x ?? 0) / 100;
          const y = Number(value.y ?? 0) / 100;
          const r = x < 0 && y < 0 ? 250 : x < 0 ? 150 : y < 0 ? 50 : 0;
          const g = x < 0 && y < 0 ? 0 : x < 0 ? 50 : y < 0 ? 150 : 250;
          const b = x < 0 && y < 0 ? 0 : x < 0 ? 150 : y < 0 ? 150 : 250;
          const alpha = opaque ? 1 : 0.5 * Number(value.v ?? 0) / 1000;
          return `rgba(${r}, ${g}, ${b}, ${alpha})`;
        }
        """);

    private static readonly string ScriptableLineCallbacksCode = CallbackModuleCode(
        """
          scriptableColor(context) {
            return colorByDataset(context.datasetIndex ?? 0);
          },
          scriptableTransparentColor(context) {
            return transparentizeHex(colorByDataset(context.datasetIndex ?? 0), 0.5);
          },
          scriptableRadius(context) {
            const value = Number(context.parsed?.y ?? context.raw?.v ?? context.raw ?? 0);
            return value < 10 ? 5 : value < 25 ? 7 : value < 50 ? 9 : value < 75 ? 11 : 15;
          },
          scriptablePointStyle(context) {
            return context.dataIndex % 2 === 0 ? 'circle' : 'rect';
          }
        """,
        ScriptableColorHelpersCode);

    private static readonly string ScriptableRadarCallbacksCode = CallbackModuleCode(
        """
          scriptableTransparentColor(context) {
            return transparentizeHex(colorByDataset(context.datasetIndex ?? 0), 0.5);
          },
          scriptableColor(context) {
            return colorByDataset(context.datasetIndex ?? 0);
          },
          scriptableRadius(context) {
            const value = Number(context.parsed?.y ?? context.raw?.v ?? context.raw ?? 0);
            return value < 10 ? 5 : value < 25 ? 7 : value < 50 ? 9 : value < 75 ? 11 : 15;
          },
          scriptablePointStyle(context) {
            return context.dataIndex % 2 === 0 ? 'circle' : 'rect';
          }
        """,
        ScriptableColorHelpersCode);

    private static readonly string ScriptableArcCallbacksCode = CallbackModuleCode(
        """
          scriptableArcColor(context) {
            return colorByDataset(context.dataIndex ?? 0);
          }
        """,
        """
        function colorByDataset(datasetIndex) {
          const colors = ['#ff6384', '#36a2eb', '#ffcd56', '#4bc0c0', '#9966ff', '#ff9f40'];
          return colors[datasetIndex % colors.length];
        }
        """);

    private const string ScriptableColorHelpersCode =
        """
        function colorByDataset(datasetIndex) {
          const colors = ['#ff6384', '#36a2eb', '#ffcd56', '#4bc0c0', '#9966ff', '#ff9f40'];
          return colors[datasetIndex % colors.length];
        }

        function transparentizeHex(hex, alpha = 0.5) {
          const value = hex.replace('#', '');
          const r = parseInt(value.substring(0, 2), 16);
          const g = parseInt(value.substring(2, 4), 16);
          const b = parseInt(value.substring(4, 6), 16);
          return `rgba(${r}, ${g}, ${b}, ${alpha})`;
        }
        """;

    private static readonly string AnimationDelayCallbacksCode = CallbackModuleCode(
        """
          animationDelay(context) {
            let delay = 0;
            if (context.type === 'data' && context.mode === 'default' && !context.chart.$delayStarted) {
              delay = context.dataIndex * 300 + context.datasetIndex * 100;
            }
            return delay;
          },
          animationComplete(context) {
            context.chart.$delayStarted = true;
          }
        """);

    private static readonly string AnimationDropCallbacksCode = CallbackModuleCode(
        """
          animationDropFrom(context) {
            if (context.type === 'data' && context.mode === 'default' && !context.dropped) {
              context.dropped = true;
              return 0;
            }

            return undefined;
          }
        """);

    private static readonly string AnimationLoopCallbacksCode = CallbackModuleCode(
        """
          animationLoopRadius(context) {
            return context.active;
          }
        """);

    private static readonly string AnimationProgressiveCallbacksCode = CallbackModuleCode(
        """
          animationProgressiveFromNaN() {
            return NaN;
          },
          animationProgressivePreviousY(context) {
            if (context.index === 0) {
              return context.chart.scales.y.getPixelForValue(100);
            }

            return context.chart.getDatasetMeta(context.datasetIndex).data[context.index - 1].getProps(['y'], true).y;
          },
          animationProgressiveDelay(context) {
            if (context.type !== 'data' || context.xStarted) {
              return 0;
            }

            context.xStarted = true;
            return context.index * (10000 / context.chart.data.datasets[0].data.length);
          },
          animationProgressiveYDelay(context) {
            if (context.type !== 'data' || context.yStarted) {
              return 0;
            }

            context.yStarted = true;
            return context.index * (10000 / context.chart.data.datasets[0].data.length);
          }
        """);

    private static readonly string AnimationProgressiveEasingCallbacksCode = CallbackModuleCode(
        """
          animationProgressiveFromNaN() {
            return NaN;
          },
          animationProgressivePreviousY(context) {
            if (context.index === 0) {
              return context.chart.scales.y.getPixelForValue(100);
            }

            return context.chart.getDatasetMeta(context.datasetIndex).data[context.index - 1].getProps(['y'], true).y;
          },
          animationProgressiveEasingTitle(context) {
            return context.chart.options.animation?.easing ?? 'easeOutQuad';
          },
          animationProgressiveEasingDelay(context) {
            if (context.type !== 'data' || context.xStarted) {
              return 0;
            }

            context.xStarted = true;
            return progressiveDelay(context);
          },
          animationProgressiveEasingYDelay(context) {
            if (context.type !== 'data' || context.yStarted) {
              return 0;
            }

            context.yStarted = true;
            return progressiveDelay(context);
          }
        """,
        """
        function getProgressiveEasing(context) {
          const easingName = context.chart.options.animation?.easing ?? 'linear';
          const effects = Chart.helpers?.easingEffects ?? {};
          return effects[easingName] ?? effects.linear ?? (value => value);
        }

        function progressiveDelay(context) {
          const dataLength = context.chart.data.datasets[0]?.data?.length || 1;
          const totalDuration = context.chart.options.animation?.duration ?? 10000;
          return getProgressiveEasing(context)(context.index / dataLength) * totalDuration;
        }
        """,
        """
        export function restartProgressiveLineEasing(chartId, easingName) {
          const chart = Chart.getChart(chartId);
          if (!chart) {
            return;
          }

          chart.stop();
          chart.options.animation ??= {};
          chart.options.animation.easing = easingName;
          chart.update();
        }
        """);

    private const string ScaleOptionsGridCallbacksCode =
        $$"""
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "{{CallbackModuleLocation}}";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          scaleOptionsGridColor(context) {
            if (context.tick.value > 0) {
              return '#4bc0c0';
            }

            if (context.tick.value < 0) {
              return '#ff6384';
            }

            return '#000000';
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
        """;

    private const string ScaleOptionsTicksCallbacksCode =
        $$"""
        // Register this once with AddChartJs in the host app.
        options.ChartJsCallbacksModuleLocation = "{{CallbackModuleLocation}}";

        // chartJsCallbacks.js
        const callbacks = Object.assign(Object.create(null), {
          scaleOptionsTickLabel(value, index) {
            const label = this.getLabelForValue(value);
            return index % 2 === 0
              ? (Array.isArray(label) ? label : `${label}`.split('\n'))
              : '';
          }
        });

        export const chartJsCallbacks = Object.freeze(callbacks);
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

        if (callbacksModule is not null)
        {
            await callbacksModule.DisposeAsync().ConfigureAwait(false);
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
