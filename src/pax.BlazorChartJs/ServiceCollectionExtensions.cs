using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace pax.BlazorChartJs;

/// <summary>
/// ServiceCollectionExtensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// AddChartJs
    /// </summary>
    public static IServiceCollection AddChartJs(this IServiceCollection services,
                        Action<ChartJsSetupOptions>? setupAction = null)
    {
        if (setupAction != null)
        {
            services.Configure(setupAction);
        }
        return services.AddScoped<ChartJsInterop, ChartJsInterop>();
    }


}

public record ChartJsSetupOptions
{
    /// <summary>
    /// Optional chart.js Javascript location (e.g. cdn)
    /// </summary>
    public string? ChartJsLocation { get; set; }
    /// <summary>
    /// Optional chartjs-plugin-datalabsle.js location (e.g. cdn)
    /// </summary>
    public string? ChartJsPluginDatalabelsLocation { get; set; }
    /// <summary>
    /// Optional chartjs-plugin-labels.js location (e.g. cdn)
    /// </summary>    
    public string? ChartJsPluginLabelsLocation { get; set; }
}