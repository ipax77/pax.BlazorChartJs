using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// References a JavaScript callback registered in the configured Chart.js callback module.
/// </summary>
public sealed record ChartJsFunction
{
    private ChartJsFunction(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Callback name used to resolve the function from the JavaScript callback registry.
    /// </summary>
    [JsonPropertyName("__chartJsFunction")]
    public string Name { get; }

    /// <summary>
    /// Creates a reference to a JavaScript callback by name.
    /// </summary>
    public static ChartJsFunction FromName(string name)
    {
        return String.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Chart.js callback name must not be empty.", nameof(name))
            : new ChartJsFunction(name);
    }
}
