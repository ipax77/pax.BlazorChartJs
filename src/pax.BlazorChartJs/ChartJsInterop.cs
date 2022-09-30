using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

/// <summary>
/// ChartJsInterop
/// </summary>
public class ChartJsInterop : IAsyncDisposable
{
    /// <summary>
    /// ChartJsInterop
    /// </summary>
    public ChartJsInterop(IJSRuntime jsRuntime,
                          // ILogger<ChartJsInterop> logger,
                          IOptions<ChartJsSetupOptions>? options)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/pax.BlazorChartJs/chartJsInterop.js").AsTask());

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

        var module = await moduleTask.Value.ConfigureAwait(false);
        var serializedConfig = SerializeConfig(config);
        return await module.InvokeAsync<bool>("initChart", setupOptions, config.ChartJsConfigGuid, serializedConfig, dotnetRef)
            .ConfigureAwait(false);
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

    public async ValueTask ResetChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("resetChart", configGuid)
            .ConfigureAwait(false);
    }

    public async ValueTask RenderChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("renderChart", configGuid)
            .ConfigureAwait(false);
    }

    public async ValueTask StopChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("stopChart", configGuid)
            .ConfigureAwait(false);
    }

    public async ValueTask SetDatasetVisibility(Guid configGuid, int datasetIndex, bool value)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setDatasetVisibility", configGuid, datasetIndex, value)
            .ConfigureAwait(false);
    }

    public async ValueTask ToggleDataVisibility(Guid configGuid, int index)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("toggleDataVisibility", configGuid, index)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> GetDataVisibility(Guid configGuid, int index)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<bool>("getDataVisibility", configGuid, index)
            .ConfigureAwait(false);
    }

    public async ValueTask HideDataset(Guid configGuid, ChartJsDataset dataset, int? index)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("hideDataset", configGuid, dataset.Id, index)
            .ConfigureAwait(false);
    }

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

    internal async ValueTask UpdateDatasets(Guid configGuid, IList<ChartJsDataset> datasets)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("updateDatasets", configGuid, SerializeDatasets(datasets)).ConfigureAwait(false);
    }

    internal async ValueTask SetDatasets(Guid configGuid, IList<ChartJsDataset> datasets)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("setDatasets", configGuid, SerializeDatasets(datasets)).ConfigureAwait(false);
    }

    internal async Task DisposeChart(Guid configGuid)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("disposeChart", configGuid).ConfigureAwait(false);
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

    private List<JsonObject?> SerializeConfigDatasets(ChartJsConfig config)
    {
        List<JsonObject?> jsonObjects = new();
        for (int i = 0; i < config.Data.Datasets.Count; i++)
        {
            var json = JsonSerializer.Serialize(config.Data.Datasets.ElementAt(i), jsonOptions);
            jsonObjects.Add(JsonSerializer.Deserialize<JsonObject>(json));
        }
        return jsonObjects;
    }

    private JsonObject? SerializeConfigDataset(object dataset)
    {
        var json = JsonSerializer.Serialize(dataset, jsonOptions);
        return JsonSerializer.Deserialize<JsonObject>(json);
    }

    private List<JsonObject?> SerializeDatasets(IList<ChartJsDataset> datasets)
    {
        List<JsonObject?> jsonObjects = new();
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
            var module = await moduleTask.Value.ConfigureAwait(false);
            await module.DisposeAsync().ConfigureAwait(false);
        }
        // logger.LogInformation("dispose");
    }
}
