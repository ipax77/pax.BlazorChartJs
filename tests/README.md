# Playwright Tests

The Playwright test project starts the local `pax.BlazorChartJs.wasmtest` app for the test run and stops it after the run.

## One-time setup

Build the Playwright project and install its Chromium browser assets:

```powershell
dotnet build .\tests\pax.BlazorChartJs.pwtests\pax.BlazorChartJs.pwtests.csproj
pwsh .\tests\pax.BlazorChartJs.pwtests\bin\Debug\net10.0\playwright.ps1 install
```

## Fast local run

Run the default regression suite from the repository root:

```powershell
dotnet test .\tests\pax.BlazorChartJs.pwtests\pax.BlazorChartJs.pwtests.csproj
```

The project run settings exclude the large `FullSamples` Chart.js sample gallery category so the default run stays focused on day-to-day regression coverage.

## Full sample gallery run

Run the full sample coverage with a positive NUnit category selection:

```powershell
dotnet test .\tests\pax.BlazorChartJs.pwtests\pax.BlazorChartJs.pwtests.csproj -- NUnit.Where="cat == FullSamples"
```

## External target override

By default the test harness self-hosts the local WASM test app on a loopback HTTP port. To debug against an already running or deployed target, set `PWTESTS_SampleBaseUrl` before the run:

```powershell
$env:PWTESTS_SampleBaseUrl = "https://localhost:7082"
dotnet test .\tests\pax.BlazorChartJs.pwtests\pax.BlazorChartJs.pwtests.csproj
```

Clear that environment variable to return to the self-hosted path.

```powershell
Remove-Item Env:PWTESTS_SampleBaseUrl -ErrorAction SilentlyContinue
```

## Readiness and intermittent failures

Page-title and chart-readiness checks allow up to 30 seconds for cold WASM startup;
they finish as soon as the expected state appears. A title timeout can indicate
that the app has not finished starting, rather than a Chart.js failure.

The event callback tests wait for the initial animation-complete event to reach
Blazor before interacting with the chart. Chart registration alone is too early:
animation events can overwrite a click event in the sample's shared latest-event
display. Keep this readiness check when adding interaction tests.
