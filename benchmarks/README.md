# Benchmarks

This folder contains BenchmarkDotNet benchmarks for performance-sensitive library paths.

## ChartJsConfig Serialization

The `pax.BlazorChartJs.benchmarks` project benchmarks serialization of a `ChartJsConfig` shaped like the advanced data decimation sample: one line dataset with 100,000 `DataPoint` values.

Run from the repository root:

```powershell
dotnet build benchmarks\pax.BlazorChartJs.benchmarks\pax.BlazorChartJs.benchmarks.csproj -c Release
dotnet run -c Release --project benchmarks\pax.BlazorChartJs.benchmarks\pax.BlazorChartJs.benchmarks.csproj -- --filter *ChartJsConfigSerializationBenchmarks*
```

For a quick harness check, use one warmup and one measured iteration:

```powershell
dotnet run -c Release --project benchmarks\pax.BlazorChartJs.benchmarks\pax.BlazorChartJs.benchmarks.csproj -- --filter *ChartJsConfigSerializationBenchmarks* --warmupCount 1 --iterationCount 1
```

BenchmarkDotNet writes reports under `BenchmarkDotNet.Artifacts/results`. The project is intentionally not included in the normal solution files so routine builds and CI stay unchanged.
