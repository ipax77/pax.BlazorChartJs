namespace pax.BlazorChartJs;

public record Animations
{
    /// <summary>
    /// The property names this configuration applies to. Defaults to the key name of this object.
    /// </summary>
    public IList<string>? Properties { get; set; }
    /// <summary>
    /// Type of property, determines the interpolator used. Possible values: 'number', 'color' and 'boolean'.
    /// Only really needed for 'color', because typeof does not get that right.
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// Start value for the animation. Current value is used when undefined
    /// </summary>
    public object? From { get; set; }
    /// <summary>
    /// End value for the animation. Updated value is used when undefined
    /// </summary>
    public object? To { get; set; }
    /// <summary>
    /// disables animation defined by the collection of 'colors' properties
    /// </summary>
    public bool? Colors { get; set; }
    /// <summary>
    /// disables animation defined by the 'x' property 
    /// </summary>
    public bool? X { get; set; }
}