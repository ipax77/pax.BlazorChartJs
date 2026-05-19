
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

internal sealed class IndexableOptionStringConverter : JsonConverter<IndexableOption<string>?>
{
    public override IndexableOption<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexableOption<string>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        IndexableOptionJsonWriter.WriteString(writer, value, options);
    }
}

internal sealed class IndexableOptionDoubleConverter : JsonConverter<IndexableOption<double>?>
{
    public override IndexableOption<double>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexableOption<double>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        IndexableOptionJsonWriter.WriteDouble(writer, value, options);
    }
}

internal sealed class IndexableOptionIntConverter : JsonConverter<IndexableOption<int>?>
{
    public override IndexableOption<int>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexableOption<int>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        IndexableOptionJsonWriter.WriteInt(writer, value, options);
    }
}

internal sealed class IndexableOptionBoolConverter : JsonConverter<IndexableOption<bool>?>
{
    public override IndexableOption<bool>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexableOption<bool>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        IndexableOptionJsonWriter.WriteBool(writer, value, options);
    }
}

internal sealed class StringOrDoubleValueConverter : JsonConverter<StringOrDoubleValue>
{
    public override StringOrDoubleValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, StringOrDoubleValue? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }

}

internal sealed class PaddingJsonConverter : JsonConverter<Padding>
{
    public override Padding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Padding? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        switch (value.Kind)
        {
            case PaddingKind.Number:
                writer.WriteNumberValue(value.NumericValue ?? throw new InvalidOperationException("Numeric padding value is null."));
                break;

            case PaddingKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            case PaddingKind.Object:
                WriteObject(writer, value);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(PaddingKind)}: {value.Kind}.");
        }
    }

    private static void WriteFunction(Utf8JsonWriter writer, ChartJsFunction? value)
    {
        var function = value ?? throw new InvalidOperationException("Function padding value is null.");
        writer.WriteStartObject();
        writer.WriteString("__chartJsFunction", function.Name);
        writer.WriteEndObject();
    }

    private static void WriteObject(Utf8JsonWriter writer, Padding value)
    {
        writer.WriteStartObject();
        WriteNumber(writer, "left", value.Left);
        WriteNumber(writer, "top", value.Top);
        WriteNumber(writer, "right", value.Right);
        WriteNumber(writer, "bottom", value.Bottom);
        WriteNumber(writer, "x", value.X);
        WriteNumber(writer, "y", value.Y);
        WriteNumber(writer, "z", value.Z);
        writer.WriteEndObject();
    }

    private static void WriteNumber(Utf8JsonWriter writer, string propertyName, double? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(propertyName, value.Value);
        }
    }
}

internal sealed class IndexableOptionObjectConverter : JsonConverter<IndexableOption<object>?>
{
    public override IndexableOption<object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexableOption<object>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }

        IndexableOptionJsonWriter.WriteObject(writer, value, options);
    }
}

internal static class IndexableOptionJsonWriter
{
    public static void WriteString(Utf8JsonWriter writer, IndexableOption<string> value, JsonSerializerOptions options)
    {
        switch (value.Kind)
        {
            case IndexableOptionKind.SingleValue:
                writer.WriteStringValue(value.SingleValue ?? throw new InvalidOperationException("Single value is null."));
                break;

            case IndexableOptionKind.Indexed:
                JsonSerializer.Serialize(writer, value.IndexedValues ?? throw new InvalidOperationException("Indexed values are null."), options);
                break;

            case IndexableOptionKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {value.Kind}.");
        }
    }

    public static void WriteDouble(Utf8JsonWriter writer, IndexableOption<double> value, JsonSerializerOptions options)
    {
        switch (value.Kind)
        {
            case IndexableOptionKind.SingleValue:
                writer.WriteNumberValue(value.SingleValue);
                break;

            case IndexableOptionKind.Indexed:
                JsonSerializer.Serialize(writer, value.IndexedValues ?? throw new InvalidOperationException("Indexed values are null."), options);
                break;

            case IndexableOptionKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {value.Kind}.");
        }
    }

    public static void WriteInt(Utf8JsonWriter writer, IndexableOption<int> value, JsonSerializerOptions options)
    {
        switch (value.Kind)
        {
            case IndexableOptionKind.SingleValue:
                writer.WriteNumberValue(value.SingleValue);
                break;

            case IndexableOptionKind.Indexed:
                JsonSerializer.Serialize(writer, value.IndexedValues ?? throw new InvalidOperationException("Indexed values are null."), options);
                break;

            case IndexableOptionKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {value.Kind}.");
        }
    }

    public static void WriteBool(Utf8JsonWriter writer, IndexableOption<bool> value, JsonSerializerOptions options)
    {
        switch (value.Kind)
        {
            case IndexableOptionKind.SingleValue:
                writer.WriteBooleanValue(value.SingleValue);
                break;

            case IndexableOptionKind.Indexed:
                JsonSerializer.Serialize(writer, value.IndexedValues ?? throw new InvalidOperationException("Indexed values are null."), options);
                break;

            case IndexableOptionKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {value.Kind}.");
        }
    }

    public static void WriteObject(Utf8JsonWriter writer, IndexableOption<object> value, JsonSerializerOptions options)
    {
        switch (value.Kind)
        {
            case IndexableOptionKind.SingleValue:
            case IndexableOptionKind.Indexed:
                var jsonObject = value.GetJsonObject();
                JsonSerializer.Serialize(writer, jsonObject, jsonObject.GetType(), options);
                break;

            case IndexableOptionKind.Function:
                WriteFunction(writer, value.FunctionValue);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {value.Kind}.");
        }
    }

    private static void WriteFunction(Utf8JsonWriter writer, ChartJsFunction? value)
    {
        var function = value ?? throw new InvalidOperationException("Function value is null.");
        writer.WriteStartObject();
        writer.WriteString("__chartJsFunction", function.Name);
        writer.WriteEndObject();
    }
}

internal sealed class ChartJsDatasetJsonConverter : JsonConverter<ChartJsDataset?>
{
    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
                {
                    new JsonStringEnumConverter<ChartType>(),
                    new JsonStringEnumConverter<ChartJsEventType>(),
                    new JsonStringEnumConverter<ChartJsEventSource>(),
                    new IndexableOptionStringConverter(),
                    new IndexableOptionDoubleConverter(),
                    new IndexableOptionIntConverter(),
                    new IndexableOptionBoolConverter(),
                    new IndexableOptionObjectConverter(),
                    new PaddingJsonConverter(),
                    new StringOrDoubleValueConverter()
                }
    };

    public override ChartJsDataset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ChartJsDataset? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value, writeOptions), true);
    }
}


internal sealed class ChartJsAxisJsonConverter : JsonConverter<ChartJsAxis?>
{
    private static readonly JsonSerializerOptions writeOptoins = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
                {
                    new JsonStringEnumConverter(),
                    new IndexableOptionStringConverter(),
                    new IndexableOptionDoubleConverter(),
                    new IndexableOptionIntConverter(),
                    new IndexableOptionBoolConverter(),
                    new IndexableOptionObjectConverter(),
                    new PaddingJsonConverter(),
                    new ChartJsAxisTickJsonConverter(),
                    new StringOrDoubleValueConverter()
                }
    };

    public override ChartJsAxis? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ChartJsAxis? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value, writeOptoins), true);
    }
}

internal sealed class ChartJsAxisTickJsonConverter : JsonConverter<ChartJsAxisTick?>
{
    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
                {
                    new JsonStringEnumConverter(),
                    new IndexableOptionStringConverter(),
                    new IndexableOptionDoubleConverter(),
                    new IndexableOptionIntConverter(),
                    new IndexableOptionBoolConverter(),
                    new IndexableOptionObjectConverter(),
                    new PaddingJsonConverter()
                }
    };

    public override ChartJsAxisTick? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ChartJsAxisTick? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value, writeOptions), true);
    }
}
