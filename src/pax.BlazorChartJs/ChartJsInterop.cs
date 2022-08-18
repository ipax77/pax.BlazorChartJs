using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
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
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly ILogger<ChartJsInterop> logger;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// ChartJsInterop
    /// </summary>
    public ChartJsInterop(IJSRuntime jsRuntime, ILogger<ChartJsInterop> logger)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/pax.BlazorChartJs/chartJsInterop.js").AsTask());

        this.logger = logger;
        // this.logger.LogInformation("init");
    }

    /// <summary>
    /// (Re-)initializes chart
    /// </summary>
    public async ValueTask InitChart(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dotnetRef);

        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("initChart", config.ChartJsConfigGuid, SerializeConfig(config), dotnetRef)
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
    /// Update Chart datasets
    /// </summary>
    public async ValueTask UpdateChartDatasets(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dotnetRef);

        var module = await moduleTask.Value.ConfigureAwait(false);
        var data = SerializeConfigDatasets(config);
        await module.InvokeVoidAsync("updateChartDatasets", config.ChartJsConfigGuid, data)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Add last data to all datasets
    /// </summary>
    public async ValueTask AddDataToDataset(Guid configGuid, string label, IList<object> data, IList<string>? backgroundColors, IList<string>? borderColors, int? atPosition)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
            await module.InvokeVoidAsync("addChartDataToDatasets", configGuid , label, data, backgroundColors, borderColors, atPosition)
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

    private JsonObject? SerializeConfig(ChartJsConfig config)
    {
        var json = JsonSerializer.Serialize(config, jsonOptions);

        if (json == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        return JsonSerializer.Deserialize<JsonObject>(json);
    }

    private JsonObject? SerializeConfigOptions(ChartJsConfig config)
    {
        var json = JsonSerializer.Serialize(config.Options, jsonOptions);

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
