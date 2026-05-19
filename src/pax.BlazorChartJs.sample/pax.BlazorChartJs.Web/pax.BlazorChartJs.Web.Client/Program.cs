using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pax.BlazorChartJs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddChartJs(options =>
{
    options.ChartJsCallbacksModuleLocation = $"{builder.HostEnvironment.BaseAddress}_content/pax.BlazorChartJs.samplelib/chartJsCallbacks.js";
});

await builder.Build().RunAsync();
