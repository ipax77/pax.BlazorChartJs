using Microsoft.Extensions.DependencyInjection;

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
                        Action<ChartJsSetupOptions> setupAction = default!)
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
    public string ChartJsLocation { get; set; } = "https://cdn.jsdelivr.net/npm/chart.js";
    /// <summary>
    /// Optional chartjs-plugin-datalabsle.js location (e.g. cdn)
    /// </summary>
    public string ChartJsPluginDatalabelsLocation { get; set; } = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
}