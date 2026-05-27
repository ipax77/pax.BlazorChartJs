using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace pax.BlazorChartJs;

/// <summary>
/// Binary dataset payload used to update chart data without serializing large point arrays as JSON.
/// </summary>
public sealed record ChartJsBinaryDatasetPayload
{
    public required string DatasetId { get; init; }
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Blazor optimized byte-array interop requires byte[] payloads.")]
    public required byte[] Bytes { get; init; }
    public required int Count { get; init; }
    public required ChartJsBinaryDataFormat Format { get; init; }
    public int XOffset { get; init; }
    public int YOffset { get; init; }
    public int? ByteStride { get; init; }
}

public enum ChartJsBinaryDataFormat
{
    Float64XY,
    Float32XY,
    Float64Y,
    Float32Y,
    Int32Y
}

/// <summary>
/// A compact XY point for binary dataset payload creation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct ChartJsPoint(double X, double Y);

/// <summary>
/// Creates compact binary dataset payloads for common Chart.js data layouts.
/// </summary>
public static class ChartJsBinaryPayload
{
    public static ChartJsBinaryDatasetPayload FromY(string datasetId, ReadOnlySpan<int> values)
    {
        ValidateDatasetId(datasetId);

        var bytes = AllocateBytes(values.Length, sizeof(int));
        CopyInt32Values(values, bytes);

        return CreatePayload(datasetId, bytes, values.Length, ChartJsBinaryDataFormat.Int32Y);
    }

    public static ChartJsBinaryDatasetPayload FromY(string datasetId, ReadOnlySpan<float> values)
    {
        ValidateDatasetId(datasetId);

        var bytes = AllocateBytes(values.Length, sizeof(float));
        CopySingleValues(values, bytes);

        return CreatePayload(datasetId, bytes, values.Length, ChartJsBinaryDataFormat.Float32Y);
    }

    public static ChartJsBinaryDatasetPayload FromY(string datasetId, ReadOnlySpan<double> values)
    {
        ValidateDatasetId(datasetId);

        var bytes = AllocateBytes(values.Length, sizeof(double));
        CopyDoubleValues(values, bytes);

        return CreatePayload(datasetId, bytes, values.Length, ChartJsBinaryDataFormat.Float64Y);
    }

    public static ChartJsBinaryDatasetPayload FromXY(string datasetId, ReadOnlySpan<ChartJsPoint> points)
    {
        ValidateDatasetId(datasetId);

        var bytes = AllocateBytes(points.Length, 2 * sizeof(double));
        if (BitConverter.IsLittleEndian)
        {
            MemoryMarshal.AsBytes(points).CopyTo(bytes);
        }
        else
        {
            var destination = bytes.AsSpan();

            for (var i = 0; i < points.Length; i++)
            {
                var offset = i * 2 * sizeof(double);
                BinaryPrimitives.WriteDoubleLittleEndian(destination.Slice(offset, sizeof(double)), points[i].X);
                BinaryPrimitives.WriteDoubleLittleEndian(destination.Slice(offset + sizeof(double), sizeof(double)), points[i].Y);
            }
        }

        return CreatePayload(datasetId, bytes, points.Length, ChartJsBinaryDataFormat.Float64XY);
    }

    public static ChartJsBinaryDatasetPayload FromXY(string datasetId, ReadOnlySpan<double> values)
    {
        ValidateDatasetId(datasetId);

        if ((values.Length & 1) != 0)
        {
            throw new ArgumentException("XY values must contain interleaved x and y pairs.", nameof(values));
        }

        var bytes = AllocateBytes(values.Length, sizeof(double));
        CopyDoubleValues(values, bytes);

        return CreatePayload(datasetId, bytes, values.Length / 2, ChartJsBinaryDataFormat.Float64XY);
    }

    private static ChartJsBinaryDatasetPayload CreatePayload(
        string datasetId,
        byte[] bytes,
        int count,
        ChartJsBinaryDataFormat format)
    {
        return new ChartJsBinaryDatasetPayload
        {
            DatasetId = datasetId,
            Bytes = bytes,
            Count = count,
            Format = format
        };
    }

    private static byte[] AllocateBytes(int count, int size)
    {
        return GC.AllocateUninitializedArray<byte>(checked(count * size));
    }

    private static void CopyInt32Values(ReadOnlySpan<int> values, byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            MemoryMarshal.AsBytes(values).CopyTo(bytes);
            return;
        }

        var destination = bytes.AsSpan();

        for (var i = 0; i < values.Length; i++)
        {
            BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(i * sizeof(int), sizeof(int)), values[i]);
        }
    }

    private static void CopySingleValues(ReadOnlySpan<float> values, byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            MemoryMarshal.AsBytes(values).CopyTo(bytes);
            return;
        }

        var destination = bytes.AsSpan();

        for (var i = 0; i < values.Length; i++)
        {
            BinaryPrimitives.WriteSingleLittleEndian(destination.Slice(i * sizeof(float), sizeof(float)), values[i]);
        }
    }

    private static void CopyDoubleValues(ReadOnlySpan<double> values, byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            MemoryMarshal.AsBytes(values).CopyTo(bytes);
            return;
        }

        var destination = bytes.AsSpan();

        for (var i = 0; i < values.Length; i++)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(destination.Slice(i * sizeof(double), sizeof(double)), values[i]);
        }
    }

    private static void ValidateDatasetId(string datasetId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(datasetId);
    }
}
