using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pax.BlazorChartJs;
using pax.BlazorChartJs.pwatest;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddChartJs(options =>
{
    options.ChartJsCallbacksModuleLocation = $"{builder.HostEnvironment.BaseAddress}_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
    options.Defaults = new ChartJsDefaultsOptions
    {
        Color = "#123456",
        BorderColor = "#654321",
        Datasets = new ChartJsOptionsDatasets
        {
            Bar = new { barPercentage = 0.72 }
        },
        OnClick = ChartJsFunction.FromName("defaultChartClick")
    };
});

await builder.Build().RunAsync();
