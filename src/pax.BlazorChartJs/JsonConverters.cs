
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

/// <summary>
/// Serializes the contents of a string value as raw JSON.  The string is validated as being an RFC 8259-compliant JSON payload
/// </summary>
public class RawJsonConverter : JsonConverter<object>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.GetRawText();
    }

    protected virtual bool SkipInputValidation => false;

#pragma warning disable CA1062
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        string valueJson = JsonSerializer.Serialize(value);
        valueJson = valueJson.Substring(3, valueJson.Length - 3);

        writer.WriteRawValue(valueJson, skipInputValidation : SkipInputValidation);
    }

//     public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) =>
//         // skipInputValidation : true will improve performance, but only do this if you are certain the value represents well-formed JSON!
//         writer.WriteRawValue(value, skipInputValidation : SkipInputValidation);
#pragma warning restore CA1062        
}

/// <summary>
/// Serializes the contents of a string value as raw JSON.  The string is NOT validated as being an RFC 8259-compliant JSON payload
/// </summary>
public class UnsafeRawJsonConverter : RawJsonConverter
{
    protected override bool SkipInputValidation => true;
}

public class ChartJsConfigConverter : JsonConverter<ChartJsConfig>
{
    public override ChartJsConfig Read(ref Utf8JsonReader reader,
    Type typeToConvert, JsonSerializerOptions options)
     => Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer,
        ChartJsConfig value, JsonSerializerOptions options)
    { }
}