using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PlaywrightTests;

public static class Startup
{
    internal static readonly TimeSpan WasmLoadDelay = TimeSpan.FromSeconds(10);
    internal static readonly TimeSpan ChartJsLoadDelay = TimeSpan.FromMilliseconds(1500);
    internal static readonly TimeSpan ChartJsComputeDelay = TimeSpan.FromMilliseconds(10);

    private const string ExternalSampleBaseUrlVariable = "PWTESTS_SampleBaseUrl";
    private static readonly TimeSpan HostStartupTimeout = TimeSpan.FromMinutes(2);
    private static readonly SemaphoreSlim hostLock = new(1, 1);
    private static readonly StringBuilder hostOutput = new();

    private static Process? sampleHost;
    private static string? sampleBaseUrl;

    public static string GetSampleBaseUrl()
    {
        if (sampleBaseUrl is null)
        {
            Init().GetAwaiter().GetResult();
        }

        return sampleBaseUrl!;
    }

    public static async Task Init()
    {
        await hostLock.WaitAsync();
        try
        {
            if (sampleBaseUrl is not null)
            {
                return;
            }

            var externalBaseUrl = Environment.GetEnvironmentVariable(ExternalSampleBaseUrlVariable);
            if (!string.IsNullOrWhiteSpace(externalBaseUrl))
            {
                sampleBaseUrl = externalBaseUrl.TrimEnd('/');
                return;
            }

            sampleBaseUrl = $"http://127.0.0.1:{GetAvailableLoopbackPort()}";
            sampleHost = StartSampleHost(sampleBaseUrl);
            await WaitForSampleHostAsync(sampleBaseUrl, sampleHost);
        }
        finally
        {
            hostLock.Release();
        }
    }

    public static async Task Stop()
    {
        await hostLock.WaitAsync();
        try
        {
            if (sampleHost is null)
            {
                return;
            }

            if (!sampleHost.HasExited)
            {
                sampleHost.Kill(entireProcessTree: true);
                await sampleHost.WaitForExitAsync();
            }

            sampleHost.Dispose();
            sampleHost = null;
        }
        finally
        {
            hostLock.Release();
        }
    }

    private static Process StartSampleHost(string baseUrl)
    {
        var repositoryRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "pax.BlazorChartJs.wasmtest",
            "pax.BlazorChartJs.wasmtest.csproj");

        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.Environment["ASPNETCORE_URLS"] = baseUrl;

        var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start the Playwright WASM sample host.");

        process.OutputDataReceived += CaptureHostOutput;
        process.ErrorDataReceived += CaptureHostOutput;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }

    private static async Task WaitForSampleHostAsync(string baseUrl, Process process)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(2)
        };

        var timeoutAt = DateTimeOffset.UtcNow + HostStartupTimeout;
        while (DateTimeOffset.UtcNow < timeoutAt)
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException(
                    $"Playwright WASM sample host exited with code {process.ExitCode}.{Environment.NewLine}{GetHostOutput()}");
            }

            try
            {
                using var response = await client.GetAsync(baseUrl);
                if (response.StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.BadRequest)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(250);
        }

        throw new TimeoutException(
            $"Timed out waiting for the Playwright WASM sample host at {baseUrl}.{Environment.NewLine}{GetHostOutput()}");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            var sampleProjectPath = Path.Combine(
                directory.FullName,
                "src",
                "pax.BlazorChartJs.wasmtest",
                "pax.BlazorChartJs.wasmtest.csproj");

            if (File.Exists(sampleProjectPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the pax.BlazorChartJs repository root for Playwright tests.");
    }

    private static int GetAvailableLoopbackPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static void CaptureHostOutput(object sender, DataReceivedEventArgs args)
    {
        if (args.Data is null)
        {
            return;
        }

        lock (hostOutput)
        {
            hostOutput.AppendLine(args.Data);
        }
    }

    private static string GetHostOutput()
    {
        lock (hostOutput)
        {
            return hostOutput.ToString();
        }
    }
}

[SetUpFixture]
public sealed class PlaywrightTestRun
{
    [OneTimeSetUp]
    public Task Init()
    {
        return Startup.Init();
    }

    [OneTimeTearDown]
    public Task Cleanup()
    {
        return Startup.Stop();
    }
}
