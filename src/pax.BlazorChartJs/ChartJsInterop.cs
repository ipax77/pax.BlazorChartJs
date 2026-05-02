using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using pax.BlazorChartJs.BlazorLegend;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// ChartJsInterop
/// </summary>
public class ChartJsInterop : IAsyncDisposable
{
    private const string ChartJsInteropVersion = "0.8.7";
    /// <summary>
    /// ChartJsInterop
    /// </summary>
    public ChartJsInterop(IJSRuntime jsRuntime,
                          // ILogger<ChartJsInterop> logger,
                          IOptions<ChartJsSetupOptions>? options)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./_content/pax.BlazorChartJs/chartJsInterop.js?v={ChartJsInteropVersion}").AsTask());

        setupOptions = options?.Value;
        JsRuntime = jsRuntime;

        // this.logger = logger;
    }
    private readonly ChartJsSetupOptions? setupOptions;
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    // private readonly ILogger<ChartJsInterop> logger;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
            {
                new JsonStringEnumConverter(),
                new IndexableOptionStringConverter(),
                new IndexableOptionDoubleConverter(),
                new IndexableOptionIntConverter(),
                new IndexableOptionBoolConverter(),
                new IndexableOptionObjectConverter(),
                new ChartJsDatasetJsonConverter(),
                new ChartJsAxisJsonConverter(),
                new ChartJsAxisTickJsonConverter(),
            }
    };

    public IJSRuntime JsRuntime { get; }



    /// <summary>
    /// (Re-)initializes chart
    /// </summary>
    public async ValueTask<bool> InitChart(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dotnetRef);

        try
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            var serializedConfig = SerializeConfig(config);
            return await module.InvokeAsync<bool>("initChart", setupOptions, config.ChartJsConfigGuid, serializedConfig, dotnetRef)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (JSDisconnectedException)
        {
            return false;
        }
    }

    /// <summary>
    /// Update Chart options
    /// </summary>
    public async ValueTask UpdateChartOptions(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dotnetRef);

        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("updateChartOptions", config.ChartJsConfigGuid, SerializeConfigOptions(config))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// SetDatasetsData
    /// </summary>
    public async ValueTask SetDatasetsData(Guid configGuid, Dictionary<ChartJsDataset, IList<object>> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var module = await moduleTask.Value.ConfigureAwait(false);

        List<object> jsData = new();
        foreach (var ent in data)
        {
            jsData.Add(new
            {
                datasetId = ent.Key.Id,
                data = ent.Value
            });
        }

        await module.InvokeVoidAsync("setDatasetsData", configGuid, jsData)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Add last data to all datasets
    /// </summary>
    public async ValueTask AddDataToDataset(Guid configGuid, string? label, IList<object> data, IList<string>? backgroundColors, IList<string>? borderColors, int? atPosition)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("addChartDataToDatasets", configGuid, label, data, backgroundColors, borderColors, atPosition)
            .ConfigureAwait(false);
    }



    /// <summary>
    /// AddDataset
    /// </summary>
    public async ValueTask AddDataset(Guid configGuid, object dataset, string? afterDatasetId)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("addChartDataset", configGuid, SerializeConfigDataset(dataset), afterDatasetId)
                .ConfigureAwait(false);
    }


    /// <summary>
    /// RemoveDataset
    /// </summary>
    public async ValueTask RemoveDataset(Guid configGuid, string datasetId)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeDataset", configGuid, datasetId)
                .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes last data from all datasets
    /// </summary>
    public async ValueTask RemoveDataFromDatasets(Guid configGuid, int? atPosition)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeData", configGuid, atPosition)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Set chart labels
    /// </summary>
    public async ValueTask SetLabels(Guid configGuid, IList<string> labels)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setLabels", configGuid, labels)
            .ConfigureAwait(false);
    }


    public async ValueTask ResizeChart(Guid configGuid, double? width = null, double? height = null)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("resizeChart", configGuid, width, height)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// returns png/jpeg data url of the image on the canvas <see href="https://www.chartjs.org/docs/latest/developers/api.html#tobase64image-type-quality">ChartJs docs</see>
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public async ValueTask<string> GetChartImage(Guid configGuid, string? imageType = null, int? quality = null, double? width = null, double? height = null)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<string>("getChartImage", configGuid, imageType, quality, width, height)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Reset the chart to its state before the initial animation. 
    /// </summary>
    /// <param name="configGuid"></param>
    /// <returns></returns>
    public async ValueTask ResetChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("resetChart", configGuid)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers a redraw of all chart elements. Note, this does not update elements for new data. Use .update() in that case.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <returns></returns>
    public async ValueTask RenderChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("renderChart", configGuid)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Use this to stop any current animation. This will pause the chart during any current animation frame.
    /// Call .render() to re-animate.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <returns></returns>
    public async ValueTask StopChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("stopChart", configGuid)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the visibility for a given dataset. This can be used to build a chart legend in HTML.
    /// During click on one of the HTML items, you can call setDatasetVisibility to change the appropriate dataset.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="datasetIndex"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async ValueTask SetDatasetVisibility(Guid configGuid, int datasetIndex, bool value)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setDatasetVisibility", configGuid, datasetIndex, value)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Toggles the visibility of an item in all datasets. A dataset needs to explicitly support this feature for it to have an effect.
    /// From internal chart types, doughnut / pie, polar area, and bar use this.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async ValueTask ToggleDataVisibility(Guid configGuid, int index)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("toggleDataVisibility", configGuid, index)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the stored visibility state of a data index for all datasets. Set by toggleDataVisibility.
    /// A dataset controller should use this method to determine if an item should not be visible.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async ValueTask<bool> GetDataVisibility(Guid configGuid, int index)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<bool>("getDataVisibility", configGuid, index)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// If dataIndex is not specified, sets the visibility for the given dataset to false. Updates the chart and animates the dataset with 'hide' mode.
    /// If dataIndex is specified, sets the hidden flag of that element to true and updates the chart.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="dataset"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async ValueTask HideDataset(Guid configGuid, ChartJsDataset dataset, int? index)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("hideDataset", configGuid, dataset.Id, index)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// If dataIndex is not specified, sets the visibility for the given dataset to true. Updates the chart and animates the dataset with 'show' mode.
    /// If dataIndex is specified, sets the hidden flag of that element to false and updates the chart.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="datasetIndex"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async ValueTask ShowDataset(Guid configGuid, int datasetIndex, int? index)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("showDataset", configGuid, datasetIndex, index)
            .ConfigureAwait(false);
    }

    internal async ValueTask AddData(Guid configGuid, string? label, int? atPosition, Dictionary<string, AddDataObject> datas)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("addData", configGuid, label, atPosition, datas).ConfigureAwait(false);
    }

    internal async ValueTask RemoveData(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeData", configGuid).ConfigureAwait(false);
    }

    internal async ValueTask SetData(Guid configGuid, IList<string>? labels, Dictionary<string, SetDataObject> datas)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setData", configGuid, labels, datas).ConfigureAwait(false);
    }

    internal async ValueTask AddDatasets(Guid configGuid, IList<ChartJsDataset> datasets)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("addDatasets", configGuid, SerializeDatasets(datasets)).ConfigureAwait(false);
    }

    internal async ValueTask RemoveDatasets(Guid configGuid, IList<string> datasetIds)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeDatasets", configGuid, datasetIds).ConfigureAwait(false);
    }

    internal async ValueTask UpdateDatasets(Guid configGuid, IList<ChartJsDataset> datasets, bool smooth = false)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        if (smooth)
        {
            await module.InvokeVoidAsync("updateDatasetsSmooth", configGuid, SerializeDatasets(datasets))
                .ConfigureAwait(false);
        }
        else
        {
            await module.InvokeVoidAsync("updateDatasets", configGuid, SerializeDatasets(datasets))
                .ConfigureAwait(false);

        }
    }

    internal async ValueTask SetDatasets(Guid configGuid, IList<ChartJsDataset> datasets)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setDatasets", configGuid, SerializeDatasets(datasets)).ConfigureAwait(false);
    }

    internal async ValueTask<List<ChartJsLegendItem>> GetLegendItems(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<List<ChartJsLegendItem>>("getLabels", configGuid).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a boolean if a dataset at the given index is currently visible.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="datasetIndex"></param>
    /// <returns></returns>
    public async ValueTask<bool> IsDatasetVisible(Guid configGuid, int datasetIndex)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<bool>("isDatasetVisible", configGuid, datasetIndex).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the active (hovered) elements for the chart. See the "Programmatic Events" sample file to see this in action.
    /// </summary>
    /// <param name="configGuid"></param>
    /// <param name="datasetIndex"></param>
    /// <returns></returns>
    public async ValueTask SetDatasetPointsActive(Guid configGuid, int datasetIndex)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setDatasetPointsActive", configGuid, datasetIndex).ConfigureAwait(false);
    }

    internal async ValueTask DisposeChart(Guid configGuid)
    {
        try
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            await module.InvokeVoidAsync("disposeChart", configGuid).ConfigureAwait(false);
        }
        catch (AggregateException) { }
        catch (OperationCanceledException) { }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException)
        {
            // catch disposed exception for statically rendered components (e.g. RenderMode.Auto)
        }
    }

    private JsonObject? SerializeConfig(ChartJsConfig config)
    {
        var json = JsonSerializer.Serialize<object>(config, jsonOptions);

        if (json == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        return JsonSerializer.Deserialize<JsonObject>(json);
    }

    private JsonObject? SerializeConfigOptions(ChartJsConfig config)
    {
        Type configType = config.GetType();
        var options = GetLowestProperty(configType, "Options")?.GetValue(config);

        if (options == null)
        {
            return null;
        }

        var json = JsonSerializer.Serialize<object>(options, jsonOptions);

        if (json == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        return JsonSerializer.Deserialize<JsonObject>(json);
    }

    private JsonObject? SerializeConfigDataset(object dataset)
    {
        var json = JsonSerializer.Serialize(dataset, jsonOptions);
        return JsonSerializer.Deserialize<JsonObject>(json);
    }

    private List<JsonObject?> SerializeDatasets(IList<ChartJsDataset> datasets)
    {
        List<JsonObject?> jsonObjects = [];
        for (int i = 0; i < datasets.Count; i++)
        {
            var json = JsonSerializer.Serialize(datasets[i], jsonOptions);
            jsonObjects.Add(JsonSerializer.Deserialize<JsonObject>(json));
        }
        return jsonObjects;
    }

    private static PropertyInfo? GetLowestProperty(Type type, string name)
    {
        var myType = type;
        while (myType != null)
        {
            var property = myType.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                return property;
            }
            myType = myType.BaseType;
        }
        return null;
    }

    /// <summary>
    /// DisposeAsync
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (moduleTask.IsValueCreated)
        {
            try
            {
                var module = await moduleTask.Value.ConfigureAwait(false);
                await module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
            }
        }
    }
}
