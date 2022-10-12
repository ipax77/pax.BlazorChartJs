
using System.Diagnostics;
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

    private static SemaphoreSlim ssStart = new(1, 1);
    private static SemaphoreSlim ssStop = new(1, 1);
    private static int runners;
    private static TimeSpan delay = TimeSpan.FromMilliseconds(30000);
    private static object lockobject = new();

    public static string GetSampleBaseUrl()
    {
        lock (lockobject)
        {
            if (String.IsNullOrEmpty(sampleBaseUrl))
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json", false, false)
                    .AddJsonFile($"appsettings.{environment}.json", true, false)
                    .Build();
                sampleBaseUrl = configuration["SampleBaseUrl"];
            }
        }

        return sampleBaseUrl;
    }

    public static async Task Init()
    {
        await ssStart.WaitAsync();

        try
        {
            runners++;

            if (runners > 1)
            {
                return;
            }

            var processes = Process.GetProcessesByName("pax.BlazorChartJs.sample");
            var sampleProcess = processes.FirstOrDefault();

            if (sampleProcess != null)
            {
                StopSampleProject(sampleProcess);
            }
            await RunSampleProject();
        }
        finally
        {
            ssStart.Release();
        }

    }

    public static async Task Stop(bool init = false)
    {
        await ssStop.WaitAsync();
        try
        {
            runners--;

            if (runners == 0)
            {
                var processes = Process.GetProcessesByName("pax.BlazorChartJs.sample");
                var sampleProcess = processes.FirstOrDefault();

                if (sampleProcess != null)
                {
                    StopSampleProject(sampleProcess);
                }
            }
        }
        finally
        {
            ssStop.Release();
        }
    }

    private static void StopSampleProject(Process process)
    {
        process.Kill();
    }

    private static async Task RunSampleProject()
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "dotnet",
            Arguments = "run --project ../../../../../src/pax.BlazorChartJs.sample"
        });

        // wait for dotnet
        await Task.Delay(delay);
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