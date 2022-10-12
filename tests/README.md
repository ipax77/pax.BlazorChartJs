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
Depending on your hardware you might have to adjust the WaitTimes in milliseconds

In ./pax.BlazorChartJs.pwtests/appsettings.Development.json
```json
{
  "SampleBaseUrl": "https://localhost:7082",
  "WasmLoadDelay": 10000,
  "ChartJsLoadDelay": 1500,
  "ChartJsComputeDelay": 10
}
```

## Run local tests (powershell)
```sh
($env:ASPNETCORE_ENVIRONMENT="Development") | dotnet test
```