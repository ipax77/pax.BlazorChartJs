using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using pax.BlazorChartJs.samplelib.ChartJsSamples;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples.PluginSamples;

public abstract class ChartJsPluginSampleBase : ChartJsDocsBaseComponent, IAsyncDisposable
{
    private const string PluginModuleLocation = "./_content/pax.BlazorChartJs.samplelib/chartJsSamplePlugins.js";

    private IJSObjectReference? pluginModule;
    private bool pluginRegistered;
    private bool pluginRegistrationStarted;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IOptions<ChartJsSetupOptions> ChartJsSetupOptions { get; set; } = default!;

    protected static IReadOnlyList<ChartJsDocsAction> NoActions { get; } = [];

    protected bool RenderChart => pluginRegistered;

    protected abstract string PluginId { get; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !pluginRegistered && !pluginRegistrationStarted)
        {
            pluginRegistrationStarted = true;
            pluginModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", PluginModuleLocation).ConfigureAwait(false);
            await pluginModule.InvokeVoidAsync(
                "registerPlugin",
                ChartJsSetupOptions.Value.ChartJsLocation,
                PluginId).ConfigureAwait(false);

            pluginRegistered = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (pluginModule is not null)
        {
            await pluginModule.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }
}

public sealed class PluginSampleChartJsConfig : ChartJsConfig
{
    public new PluginSampleChartJsOptions? Options { get; set; }
}

public sealed record PluginSampleChartJsOptions : ChartJsOptions
{
    public new PluginSamplePlugins? Plugins { get; set; }
}

public sealed record PluginSamplePlugins : Plugins
{
    public ChartAreaBorderOptions? ChartAreaBorder { get; set; }

    public EmptyDoughnutOptions? EmptyDoughnut { get; set; }

    public QuadrantsOptions? Quadrants { get; set; }
}

public sealed record ChartAreaBorderOptions
{
    public string? BorderColor { get; set; }

    public int? BorderWidth { get; set; }

    public IList<int>? BorderDash { get; set; }

    public int? BorderDashOffset { get; set; }
}

public sealed record EmptyDoughnutOptions
{
    public string? Color { get; set; }

    public int? Width { get; set; }

    public int? RadiusDecrease { get; set; }
}

public sealed record QuadrantsOptions
{
    public string? TopLeft { get; set; }

    public string? TopRight { get; set; }

    public string? BottomRight { get; set; }

    public string? BottomLeft { get; set; }
}
