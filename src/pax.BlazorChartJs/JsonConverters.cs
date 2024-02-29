
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

internal sealed class IndexableOptionStringConverter : JsonConverter<IndexableOption<string>?>
{
    public override IndexableOption<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IndexableOption<string>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }
}

internal sealed class IndexableOptionDoubleConverter : JsonConverter<IndexableOption<double>?>
{
    public override IndexableOption<double>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IndexableOption<double>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }
}

internal sealed class IndexableOptionIntConverter : JsonConverter<IndexableOption<int>?>
{
    public override IndexableOption<int>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IndexableOption<int>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }
}

internal sealed class IndexableOptionBoolConverter : JsonConverter<IndexableOption<bool>?>
{
    public override IndexableOption<bool>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IndexableOption<bool>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }
}

internal sealed class StringOrDoubleValueConverter : JsonConverter<StringOrDoubleValue>
{
    public override StringOrDoubleValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, StringOrDoubleValue? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
    }

}

internal sealed class IndexableOptionObjectConverter : JsonConverter<IndexableOption<object>?>
{
    public override IndexableOption<object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IndexableOption<object>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>(value.GetJsonObject()), true);
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
                    new JsonStringEnumConverter(),
                    new IndexableOptionStringConverter(),
                    new IndexableOptionDoubleConverter(),
                    new IndexableOptionIntConverter(),
                    new IndexableOptionBoolConverter(),
                    new IndexableOptionObjectConverter(),
                    new StringOrDoubleValueConverter()
                }
    };

    public override ChartJsDataset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsDataset? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, writeOptions), true);
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
                    new ChartJsAxisTickJsonConverter(),
                    new StringOrDoubleValueConverter()
                }
    };

    public override ChartJsAxis? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsAxis? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, writeOptoins), true);
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
                    new IndexableOptionObjectConverter()
                }
    };

    public override ChartJsAxisTick? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsAxisTick? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, writeOptions), true);
    }
}