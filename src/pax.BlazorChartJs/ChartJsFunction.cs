using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// References a JavaScript callback registered in the configured Chart.js callback module.
/// </summary>
public sealed partial record ChartJsFunction
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
            : !ChartJsFunctionNameRegex().IsMatch(name)
            ? throw new ArgumentException("Chart.js callback name must be a flat JavaScript identifier.", nameof(name))
            : IsReservedName(name)
            ? throw new ArgumentException($"Chart.js callback name '{name}' is reserved.", nameof(name))
            : new ChartJsFunction(name);
    }

    private static Boolean IsReservedName(string name)
    {
        return name switch
        {
            "__proto__" or
            "constructor" or
            "prototype" or
            "toString" or
            "valueOf" or
            "hasOwnProperty" => true,
            _ => false
        };
    }

    [GeneratedRegex("^[A-Za-z_$][A-Za-z0-9_$]*$")]
    private static partial Regex ChartJsFunctionNameRegex();
}
