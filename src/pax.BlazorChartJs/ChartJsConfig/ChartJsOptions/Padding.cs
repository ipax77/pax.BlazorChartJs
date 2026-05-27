namespace pax.BlazorChartJs;

/// <summary>
/// Chart.js padding option value.
/// See <see href="https://www.chartjs.org/docs/latest/general/padding.html">Chart.js padding documentation</see>.
/// Padding can be serialized as a number, a directional object, an <c>{ x, y }</c> shorthand object,
/// or a scriptable <see cref="ChartJsFunction"/> callback.
/// </summary>
public record Padding
{
    internal PaddingKind Kind { get; init; }

    internal double? NumericValue { get; init; }

    internal ChartJsFunction? FunctionValue { get; init; }

    public Padding()
    {

    }

    /// <summary>
    /// Creates a directional object padding value with all four sides set to the same value.
    /// This is kept for compatibility and serializes as <c>{ left, top, right, bottom }</c>.
    /// Use <see cref="FromNumber(double)"/> or assign a <see cref="double"/> to serialize Chart.js numeric padding.
    /// </summary>
    [Obsolete("Use Padding.FromNumber(value) or assign a double value to serialize Chart.js numeric padding. This constructor remains compatible and serializes as a directional object.", false)]
    public Padding(double allSides)
    {
        Left = allSides;
        Top = allSides;
        Right = allSides;
        Bottom = allSides;
    }

    private Padding(double value, PaddingKind kind)
    {
        NumericValue = value;
        Kind = kind;
    }

    private Padding(ChartJsFunction function)
    {
        FunctionValue = function ?? throw new ArgumentNullException(nameof(function));
        Kind = PaddingKind.Function;
    }

    /// <summary>
    /// Left padding in pixels for the directional object form.
    /// </summary>
    public double? Left { get; set; }

    /// <summary>
    /// Top padding in pixels for the directional object form.
    /// </summary>
    public double? Top { get; set; }

    /// <summary>
    /// Right padding in pixels for the directional object form.
    /// </summary>
    public double? Right { get; set; }

    /// <summary>
    /// Bottom padding in pixels for the directional object form.
    /// </summary>
    public double? Bottom { get; set; }

    /// <summary>
    /// Horizontal shorthand padding in pixels. Chart.js applies this to left and right padding.
    /// </summary>
    public double? X { get; set; }

    /// <summary>
    /// Vertical shorthand padding in pixels. Chart.js applies this to top and bottom padding.
    /// </summary>
    public double? Y { get; set; }

    /// <summary>
    /// Z-index for Chart.js options that support it.
    /// </summary>
    public double? Z { get; set; }

    /// <summary>
    /// Creates a Chart.js numeric padding value, applied to all sides by Chart.js.
    /// </summary>
    public static Padding FromNumber(double value)
    {
        return new(value, PaddingKind.Number);
    }

    /// <summary>
    /// Creates a Chart.js numeric padding value, applied to all sides by Chart.js.
    /// </summary>
    public static Padding FromDouble(double value)
    {
        return FromNumber(value);
    }

    /// <summary>
    /// Creates a scriptable padding value resolved from the configured Chart.js callback registry.
    /// </summary>
    public static Padding FromFunction(ChartJsFunction function)
    {
        return new(function);
    }

    /// <summary>
    /// Creates a scriptable padding value resolved from the configured Chart.js callback registry.
    /// </summary>
    public static Padding FromChartJsFunction(ChartJsFunction function)
    {
        return FromFunction(function);
    }

    /// <summary>
    /// Converts a number to Chart.js numeric padding.
    /// </summary>
    public static implicit operator Padding(double value)
    {
        return FromNumber(value);
    }

    /// <summary>
    /// Converts a callback reference to scriptable Chart.js padding.
    /// </summary>
    public static implicit operator Padding(ChartJsFunction function)
    {
        return FromFunction(function);
    }
}

internal enum PaddingKind
{
    Object,
    Number,
    Function
}

