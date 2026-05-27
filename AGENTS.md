# AGENTS.md

## Library Overview

`pax.BlazorChartJs` is a .NET/Blazor Razor class library that wraps Chart.js 4.x through Blazor JavaScript isolation. The package targets `net10.0` and exposes typed C# chart configuration models plus a reusable `ChartComponent` for rendering charts in Blazor applications.

The core package lives in `src/pax.BlazorChartJs`. The most important runtime pieces are:

- `ChartComponent`: the Razor component that owns the canvas, subscribes to `ChartJsConfig` events, initializes the chart, forwards chart events to Blazor, and disposes JS/.NET references.
- `ChartJsConfig`: the central configuration and update/event model. Its helper methods raise targeted update events such as labels, data, datasets, options, and reinitialization.
- `ChartJsInterop`: the C# interop service that serializes configs and invokes the isolated JS module.
- `wwwroot/chartJsInterop.js`: the packaged JavaScript interop module used by consumers.
- `TypeScript/chartJsInterop.ts`: the TypeScript source for the generated JS module.

## Repository Map

- `src/pax.BlazorChartJs`: core library, config types, converters, interop, component, and static web assets.
- `src/pax.BlazorChartJs.samplelib`: reusable sample components that demonstrate chart types, callbacks, plugins, events, legends, and updates.
- `src/pax.BlazorChartJs.wasmtest` and `src/pax.BlazorChartJs.pwatest`: Blazor test/demo apps used for manual and browser-based validation.
- `tests/pax.BlazorChartJs.tests`: MSTest serialization and model behavior tests.
- `tests/pax.BlazorChartJs.pwtests`: Playwright/NUnit browser behavior tests against the Blazor test app.
- `.github/workflows`: CI definitions for serialization tests, Playwright tests, and GitHub Pages publishing.
- The public GitHub wiki is maintained in its own repository checkout. In this workspace it is usually available as the sibling directory `../pax.BlazorChartJs.wiki`.

## Development Guidance

- Keep public API changes deliberate and compatible. This is a NuGet library, so renames, type changes, and behavior changes can affect consumers.
- Keep C# and TypeScript/JavaScript interop behavior synchronized. If an interop method, payload shape, callback marker, or config field changes on one side, update the other side in the same change.
- Prefer typed configuration models and existing converter patterns over ad hoc JSON or reflection-heavy shims.
- Preserve nullable annotations and analyzer expectations. The core project treats nullable warnings as errors.
- When adding Chart.js options or datasets, update the source-generated JSON context and converters as needed so serialization remains predictable and AOT-friendly.
- Use the sample library as the source of practical examples. Add or update samples when a user-facing feature benefits from a visible chart scenario.
- Treat public documentation as part of user-facing changes. Review the README, samples, and wiki when APIs, setup steps, sample routes, callbacks, plugins, events, legends, or helper methods change.

## Documentation Guidance

- The GitHub wiki is a separate git repository from the library checkout. Keep wiki edits in `../pax.BlazorChartJs.wiki` when that sibling checkout is available.
- The wiki explains basic and advanced usage from the public sample app, including screenshots and links back to sample or library source files.
- Update or review the wiki when a change adds a useful visible sample, changes an API that a guide demonstrates, moves linked source files or sample routes, or changes chart output shown by a screenshot.
- Prefer sample-library examples for wiki snippets so documentation stays close to executable chart scenarios.
- For wiki image updates, store assets in the wiki-local `images` folder and keep Markdown image links relative.

## Performance Guidance

The library surface has no database interactions, so database performance is generally not applicable here. The main performance concerns are memory allocation, JSON serialization, JS interop round trips, Chart.js update paths, and CPU usage in reflection or conversion code.

- Prefer targeted chart updates (`SetLabels`, `SetData`, dataset add/remove/update helpers, `UpdateChartOptions`) over full chart reinitialization when the existing chart can be updated safely.
- Avoid unnecessary allocation in hot paths such as dataset updates, event handling, JSON conversion, and JS payload construction.
- Avoid CPU-heavy reflection in serialization or frequent update paths. If reflection is needed, cache results where appropriate.
- Preserve source-generated or strongly typed serialization patterns where practical.
- Be careful with wrappers around dataset data and option values so common scalar and list-based use cases remain allocation-light.
- Minimize JS interop calls in loops. Batch data or dataset changes when the API already supports it.

## Verification

For documentation-only changes, a manual review is usually enough. Check wiki navigation, relative image links, and source links when the wiki changes. For code changes, choose the smallest test set that covers the risk:

- Build core library: `dotnet build src\pax.BlazorChartJs\pax.BlazorChartJs.csproj`
- Run serialization tests: `dotnet test tests\pax.BlazorChartJs.tests\pax.BlazorChartJs.tests.csproj`
- Run browser behavior tests: `dotnet test tests\pax.BlazorChartJs.pwtests\pax.BlazorChartJs.pwtests.csproj`

When changing TypeScript interop, ensure `TypeScript/chartJsInterop.ts` and `wwwroot/chartJsInterop.js` stay aligned. When changing runtime chart behavior, add or update Playwright coverage.
