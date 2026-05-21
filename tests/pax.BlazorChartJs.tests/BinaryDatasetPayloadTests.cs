namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class BinaryDatasetPayloadTests
{
    [TestMethod]
    public void CompactFloat64XYPayloadUsesDefaultOffsets()
    {
        ChartJsConfig config = new();

        config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = "series-1",
            Bytes = new byte[2 * 2 * sizeof(double)],
            Count = 2,
            Format = ChartJsBinaryDataFormat.Float64XY
        });
    }

    [TestMethod]
    public void StridedFloat64XYPayloadAcceptsExplicitOffsets()
    {
        ChartJsConfig config = new();

        config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = "series-1",
            Bytes = new byte[2 * 24],
            Count = 2,
            Format = ChartJsBinaryDataFormat.Float64XY,
            XOffset = 8,
            YOffset = 16,
            ByteStride = 24
        });
    }

    [TestMethod]
    public void ShortBinaryPayloadThrows()
    {
        ChartJsConfig config = new();

        Assert.ThrowsExactly<ArgumentException>(() =>
            config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
            {
                DatasetId = "series-1",
                Bytes = new byte[24],
                Count = 2,
                Format = ChartJsBinaryDataFormat.Float64XY
            }));
    }

    [TestMethod]
    public void InvalidStrideThrows()
    {
        ChartJsConfig config = new();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
            {
                DatasetId = "series-1",
                Bytes = new byte[16],
                Count = 1,
                Format = ChartJsBinaryDataFormat.Float64XY,
                ByteStride = 8
            }));
    }
}
