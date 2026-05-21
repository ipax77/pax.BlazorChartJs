using System.Diagnostics.CodeAnalysis;

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
    Float32Y
}
