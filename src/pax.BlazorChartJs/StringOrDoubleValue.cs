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

    public string? AsString()
    {
        return StringValue;
    }

    public double? AsDouble()
    {
        return DoubleValue;
    }

    public bool IsString => StringValue != null;
    public bool IsDouble => DoubleValue != null;

    public static StringOrDoubleValue FromString(string stringValue)
    {
        return new(stringValue);
    }

    public static StringOrDoubleValue FromDouble(double doubleValue)
    {
        return new(doubleValue);
    }

    public static implicit operator StringOrDoubleValue(string stringValue)
    {
        return new(stringValue);
    }

    public static implicit operator StringOrDoubleValue(double doubleValue)
    {
        return new(doubleValue);
    }

    public override string ToString()
    {
        return IsString ? AsString()! : AsDouble()?.ToString(CultureInfo.InvariantCulture)! ?? "";
    }

    internal object GetJsonObject()
    {
        return StringValue ?? (DoubleValue != null ? (object)DoubleValue.Value
        : throw new ArgumentNullException("String and Double values are null"));
    }
}