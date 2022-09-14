using pax.BlazorChartJs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 128 * 1024); // for chart images (only server side)

builder.Services.AddChartJs(
//options =>
//{
//    options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
//    options.ChartJsPluginDatalabelsLocation = "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
//    options.ChartJsPluginLabelsLocation = "https://unpkg.com/chart.js-plugin-labels-dv/dist/chartjs-plugin-labels.min.js";
//}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
