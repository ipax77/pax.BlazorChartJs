using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pax.BlazorChartJs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddChartJs();

await builder.Build().RunAsync();
