using System.Globalization;
namespace pax.BlazorChartJs;

public record StringOrDoubleValue
{
    public string? StringValue { get; init; }
    public double? DoubleValue { get; init; }

    public StringOrDoubleValue(string stringValue)
    {
        StringValue = stringValue;
        DoubleValue = null;
    }

    public StringOrDoubleValue(double doubleValue)
    {
        StringValue = null;
        DoubleValue = doubleValue;
    }

    public string? AsString() => StringValue;
    public double? AsDouble() => DoubleValue;

    public bool IsString => StringValue != null;
    public bool IsDouble => DoubleValue != null;

    public static StringOrDoubleValue FromString(string stringValue)
        => new(stringValue);

    public static StringOrDoubleValue FromDouble(double doubleValue)
        => new(doubleValue);

    public static implicit operator StringOrDoubleValue(string stringValue)
        => new(stringValue);

    public static implicit operator StringOrDoubleValue(double doubleValue)
        => new(doubleValue);

    public override string ToString()
        => IsString ? AsString()! : AsDouble()?.ToString(CultureInfo.InvariantCulture)! ?? "";

    internal object GetJsonObject()
    {
        if (StringValue != null)
        {
            return StringValue;
        }
        else if (DoubleValue != null)
        {
            return DoubleValue.Value;
        }
        else
        {
            throw new ArgumentNullException("String and Double values are null");
        }
    }
}