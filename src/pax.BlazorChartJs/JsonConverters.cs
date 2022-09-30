
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

internal class IndexableOptionStringConverter : JsonConverter<IndexableOption<string>?>
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

internal class IndexableOptionDoubleConverter : JsonConverter<IndexableOption<double>?>
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

internal class IndexableOptionIntConverter : JsonConverter<IndexableOption<int>?>
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

internal class IndexableOptionBoolConverter : JsonConverter<IndexableOption<bool>?>
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


internal class IndexableOptionObjectConverter : JsonConverter<IndexableOption<object>?>
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

internal class ChartJsDatasetJsonConverter : JsonConverter<ChartJsDataset?>
{
    public override ChartJsDataset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsDataset? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, new JsonSerializerOptions()
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
                }
        }), true);
    }
}


internal class ChartJsAxisJsonConverter : JsonConverter<ChartJsAxis?>
{
    public override ChartJsAxis? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsAxis? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, new JsonSerializerOptions()
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
                }
        }), true);
    }
}

internal class ChartJsAxisTickJsonConverter : JsonConverter<ChartJsAxisTick?>
{
    public override ChartJsAxisTick? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
     => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, ChartJsAxisTick? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return;
        }
        writer.WriteRawValue(JsonSerializer.Serialize<object>((object)value, new JsonSerializerOptions()
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
        }), true);
    }
}