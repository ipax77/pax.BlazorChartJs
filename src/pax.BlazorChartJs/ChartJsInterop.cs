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
    /// Add last Dataset
    /// </summary>
    public async ValueTask AddLastDataset(ChartJsConfig config, DotNetObjectReference<ChartComponent> dotnetRef)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(dotnetRef);

        var module = await moduleTask.Value.ConfigureAwait(false);
        var data = SerializeLastConfigDataset(config);
        await module.InvokeVoidAsync("addChartDataset", config.ChartJsConfigGuid, data)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Add last data to all datasets
    /// </summary>
    public async ValueTask AddDataToDataset(ChartJsConfig config)
    {
        if (config.Data.Labels.Any())
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            (var data, var backgourndColors, var borderColors) = GetAddData(config);
            await module.InvokeVoidAsync("addChartDataToDatasets", config.ChartJsConfigGuid, config.Data.Labels.Last(), data, backgourndColors, borderColors)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// RemoveLastDataset
    /// </summary>
    public async ValueTask RemoveLastDataset(ChartJsConfig config)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeLastDataset", config.ChartJsConfigGuid)
                .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes last data from all datasets
    /// </summary>
    public async ValueTask RemoveLastDataFromDatasets(ChartJsConfig config)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("removeLastData", config.ChartJsConfigGuid)
                .ConfigureAwait(false);
    }

    private (List<object>, List<string>, List<string>) GetAddData(ChartJsConfig config)
    {
        List<object> datas = new();
        List<string> backgroundColors = new();
        List<string> borderColors = new();

        if (config.Type == ChartType.bar)
        {
            foreach (BarDataset dataset in config.Data.Datasets.Cast<BarDataset>())
            {
                if (dataset.Data.Any())
                {
                    datas.Add(dataset.Data.Last());
                    if (dataset.BackgroundColor != null && dataset.BackgroundColor.GetType() == typeof(List<string>))
                    {
                        if (((List<string>)dataset.BackgroundColor).Any())
                        {
                            backgroundColors.Add(((List<string>)dataset.BackgroundColor).Last());
                        }
                    }
                    if (dataset.BorderColor != null && dataset.BorderColor.GetType() == typeof(List<string>))
                    {
                        if (((List<string>)dataset.BorderColor).Any())
                        {
                            borderColors.Add(((List<string>)dataset.BorderColor).Last());
                        }
                    }
                }
            }
        }
        return (datas, backgroundColors, borderColors);
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

    private JsonObject? SerializeLastConfigDataset(ChartJsConfig config)
    {
        var json = JsonSerializer.Serialize(config.Data.Datasets.Last(), jsonOptions);
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
