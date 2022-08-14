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
    public static IServiceCollection AddChartJs(this IServiceCollection services)
    {
        return services.AddScoped<ChartJsInterop>();
    }
}
