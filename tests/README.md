# Playwright Tests (pwtests)

## Prerequisites

Install [Playwright](https://playwright.dev/dotnet/docs/intro) chromium assets (powershell):
```sh
dotnet build
pwsh pax.BlatzorChartJs.pwtests/bin/Debug/net6.0/playwright.ps1 install
```

To run the pwtests locally you have to start the sample project first - and restart it after changes.

To start it from this directory:
```sh
dotnet run --project ../src/pax.BlazorChartJs.wasmsample
```

## Configuration
Depending on your hardware you might have to adjust the WaitTimes

In ./pax.BlazorChartJs.pwtests/Startup.cs
```csharp
// time to wait for wasmsample to load
internal static TimeSpan WasmLoadDelay = TimeSpan.FromMilliseconds(10000);
// time to wait for chart.min.js and plugins to load
internal static TimeSpan ChartJsLoadDelay = TimeSpan.FromMilliseconds(1500);
// time to wait for ChartJs to compute actions (e.g. addData)
internal static TimeSpan ChartJsComputeDelay = TimeSpan.FromMilliseconds(10);
```

## Run local tests (powershell)
```sh
($env:ASPNETCORE_ENVIRONMENT="Development") | dotnet test
```