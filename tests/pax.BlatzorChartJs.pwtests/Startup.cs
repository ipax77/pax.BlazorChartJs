
using System.Diagnostics;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public static class Startup
{
    private static SemaphoreSlim ssStart = new(1, 1);
    private static SemaphoreSlim ssStop = new(1, 1);
    private static int runners;
    private static TimeSpan delay = TimeSpan.FromMilliseconds(30000);

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