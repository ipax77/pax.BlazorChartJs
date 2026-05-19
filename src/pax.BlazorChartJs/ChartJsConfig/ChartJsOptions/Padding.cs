namespace pax.BlazorChartJs;

public record Padding
{
    internal PaddingKind Kind { get; init; }

    internal double? NumericValue { get; init; }

    internal ChartJsFunction? FunctionValue { get; init; }

    public Padding()
    {

    }

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

    public double? Left { get; set; }
    public double? Top { get; set; }
    public double? Right { get; set; }
    public double? Bottom { get; set; }
    public double? X { get; set; }
    public double? Y { get; set; }
    public double? Z { get; set; }

    public static Padding FromNumber(double value)
    {
        return new(value, PaddingKind.Number);
    }

    public static Padding FromDouble(double value)
    {
        return FromNumber(value);
    }

    public static Padding FromFunction(ChartJsFunction function)
    {
        return new(function);
    }

    public static Padding FromChartJsFunction(ChartJsFunction function)
    {
        return FromFunction(function);
    }

    public static implicit operator Padding(double value)
    {
        return FromNumber(value);
    }

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

