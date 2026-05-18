
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public static class Startup
{
    // time to wait for wasmsample to load
    internal static TimeSpan WasmLoadDelay = TimeSpan.FromMilliseconds(10000);
    // time to wait for chart.min.js and plugins to load
    internal static TimeSpan ChartJsLoadDelay = TimeSpan.FromMilliseconds(1500);
    // time to wait for ChartJs to compute actions (e.g. addData)
    internal static TimeSpan ChartJsComputeDelay = TimeSpan.FromMilliseconds(10);


    private static string sampleBaseUrl = "";

    private static readonly object lockobject = new();

    public static string GetSampleBaseUrl()
    {
        lock (lockobject)
        {
            if (String.IsNullOrEmpty(sampleBaseUrl))
            {
                SetupConfiguration();
            }
        }
        return sampleBaseUrl;
    }

    private static void SetupConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", false, false)
            .AddJsonFile($"appsettings.{environment}.json", true, false)
            .Build();

        sampleBaseUrl = configuration["SampleBaseUrl"] ?? "";
        if (String.IsNullOrWhiteSpace(sampleBaseUrl))
        {
            throw new InvalidOperationException("SampleBaseUrl must be configured for Playwright tests.");
        }

        if (int.TryParse(configuration["WasmLoadDelay"], out int wasmMs))
        {
            WasmLoadDelay = TimeSpan.FromMilliseconds(wasmMs);
        }

        if (int.TryParse(configuration["ChartJsLoadDelay"], out int chartLoadMs))
        {
            ChartJsLoadDelay = TimeSpan.FromMilliseconds(chartLoadMs);
        }

        if (int.TryParse(configuration["ChartJsComputeDelay"], out int chartComputeDelay))
        {
            ChartJsComputeDelay = TimeSpan.FromMilliseconds(chartComputeDelay);
        }
    }

    public static Task Init()
    {
        // CI runs against the published GitHub Pages app from appsettings.json.
        // Local runs use ASPNETCORE_ENVIRONMENT=Development after the host is
        // started manually with the commands in tests/README.md.
        GetSampleBaseUrl();
        return Task.CompletedTask;
    }

    public static Task Stop()
    {
        return Task.CompletedTask;
    }
}

public class PageStartupTest : PageTest
{
    [OneTimeSetUp]
    public async Task Init()
    {
        await Startup.Init();
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        await Startup.Stop();
    }
}
