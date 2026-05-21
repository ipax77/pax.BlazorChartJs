namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Sets one dataset's data from a packed binary payload without changing the C# dataset's Data property.
    /// </summary>
    public void SetDatasetBinaryData(ChartJsBinaryDatasetPayload payload, string updateMode = "none")
    {
        ArgumentNullException.ThrowIfNull(payload);

        SetDatasetsBinaryData([payload], updateMode);
    }

    /// <summary>
    /// Sets one or more datasets' data from packed binary payloads without changing the C# datasets' Data properties.
    /// </summary>
    public void SetDatasetsBinaryData(IReadOnlyList<ChartJsBinaryDatasetPayload> payloads, string updateMode = "none")
    {
        ArgumentNullException.ThrowIfNull(payloads);
        ArgumentNullException.ThrowIfNull(updateMode);

        if (payloads.Count == 0)
        {
            return;
        }

        ValidateBinaryDatasetPayloads(payloads);
        OnDatasetsBinaryDataSet(new DatasetsBinaryDataSetEventArgs(payloads, updateMode));
    }

    internal static void ValidateBinaryDatasetPayloads(IReadOnlyList<ChartJsBinaryDatasetPayload> payloads)
    {
        for (int i = 0; i < payloads.Count; i++)
        {
            ValidateBinaryDatasetPayload(payloads[i]);
        }
    }

    internal static void ValidateBinaryDatasetPayload(ChartJsBinaryDatasetPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentException.ThrowIfNullOrEmpty(payload.DatasetId);
        ArgumentNullException.ThrowIfNull(payload.Bytes);

        if (payload.Count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(payload), "Payload count cannot be negative.");
        }

        if (payload.XOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(payload), "Payload X offset cannot be negative.");
        }

        if (payload.YOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(payload), "Payload Y offset cannot be negative.");
        }

        var requiredBytes = GetRequiredByteCount(payload);
        if (payload.Bytes.Length < requiredBytes)
        {
            throw new ArgumentException("Binary dataset payload is too small for the requested format, count, offsets, and stride.", nameof(payload));
        }
    }

    private static int GetRequiredByteCount(ChartJsBinaryDatasetPayload payload)
    {
        if (payload.Count == 0)
        {
            return 0;
        }

        var valueSize = GetBinaryValueSize(payload.Format);
        var compactStride = GetCompactByteStride(payload.Format);
        var byteStride = payload.ByteStride ?? compactStride;
        if (byteStride < compactStride)
        {
            throw new ArgumentOutOfRangeException(nameof(payload), "Payload byte stride is too small for the requested format.");
        }

        var yOffset = GetEffectiveYOffset(payload);
        var maxOffset = payload.Format is ChartJsBinaryDataFormat.Float64XY or ChartJsBinaryDataFormat.Float32XY
            ? Math.Max(payload.XOffset, yOffset)
            : yOffset;

        if (maxOffset + valueSize > byteStride)
        {
            throw new ArgumentOutOfRangeException(nameof(payload), "Payload offsets must fit inside the byte stride.");
        }

        return checked(((payload.Count - 1) * byteStride) + maxOffset + valueSize);
    }

    private static int GetBinaryValueSize(ChartJsBinaryDataFormat format)
    {
        return format switch
        {
            ChartJsBinaryDataFormat.Float64XY or ChartJsBinaryDataFormat.Float64Y => sizeof(double),
            ChartJsBinaryDataFormat.Float32XY or ChartJsBinaryDataFormat.Float32Y => sizeof(float),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported binary dataset format.")
        };
    }

    internal static int GetEffectiveYOffset(ChartJsBinaryDatasetPayload payload)
    {
        var valueSize = GetBinaryValueSize(payload.Format);
        return payload.Format is ChartJsBinaryDataFormat.Float64XY or ChartJsBinaryDataFormat.Float32XY
            && payload.XOffset == 0
            && payload.YOffset == 0
            ? valueSize
            : payload.YOffset;
    }

    private static int GetCompactByteStride(ChartJsBinaryDataFormat format)
    {
        return format switch
        {
            ChartJsBinaryDataFormat.Float64XY => 2 * sizeof(double),
            ChartJsBinaryDataFormat.Float32XY => 2 * sizeof(float),
            ChartJsBinaryDataFormat.Float64Y => sizeof(double),
            ChartJsBinaryDataFormat.Float32Y => sizeof(float),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported binary dataset format.")
        };
    }
}
