using System.Buffers.Binary;

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
    public void CompactInt32YPayloadAcceptsValues()
    {
        ChartJsConfig config = new();

        config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = "series-1",
            Bytes = new byte[2 * sizeof(int)],
            Count = 2,
            Format = ChartJsBinaryDataFormat.Int32Y
        });
    }

    [TestMethod]
    public void StridedInt32YPayloadAcceptsExplicitYOffset()
    {
        ChartJsConfig config = new();

        config.SetDatasetBinaryData(new ChartJsBinaryDatasetPayload
        {
            DatasetId = "series-1",
            Bytes = new byte[2 * 8],
            Count = 2,
            Format = ChartJsBinaryDataFormat.Int32Y,
            YOffset = 4,
            ByteStride = 8
        });
    }

    [TestMethod]
    public void Int32YHelperCreatesCompactPayload()
    {
        var payload = ChartJsBinaryPayload.FromY("series-1", new int[] { -30, 31 });

        Assert.AreEqual(ChartJsBinaryDataFormat.Int32Y, payload.Format);
        Assert.AreEqual(2, payload.Count);
        Assert.HasCount(2 * sizeof(int), payload.Bytes);
        Assert.AreEqual(-30, BinaryPrimitives.ReadInt32LittleEndian(payload.Bytes.AsSpan(0, sizeof(int))));
        Assert.AreEqual(31, BinaryPrimitives.ReadInt32LittleEndian(payload.Bytes.AsSpan(sizeof(int), sizeof(int))));
    }

    [TestMethod]
    public void Float32YHelperCreatesCompactPayload()
    {
        var payload = ChartJsBinaryPayload.FromY("series-1", new float[] { 30.5f, 31.5f });

        Assert.AreEqual(ChartJsBinaryDataFormat.Float32Y, payload.Format);
        Assert.AreEqual(2, payload.Count);
        Assert.HasCount(2 * sizeof(float), payload.Bytes);
        Assert.AreEqual(30.5f, BinaryPrimitives.ReadSingleLittleEndian(payload.Bytes.AsSpan(0, sizeof(float))));
        Assert.AreEqual(31.5f, BinaryPrimitives.ReadSingleLittleEndian(payload.Bytes.AsSpan(sizeof(float), sizeof(float))));
    }

    [TestMethod]
    public void Float64YHelperCreatesCompactPayload()
    {
        var payload = ChartJsBinaryPayload.FromY("series-1", new double[] { 30.25, 31.25 });

        Assert.AreEqual(ChartJsBinaryDataFormat.Float64Y, payload.Format);
        Assert.AreEqual(2, payload.Count);
        Assert.HasCount(2 * sizeof(double), payload.Bytes);
        Assert.AreEqual(30.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(0, sizeof(double))));
        Assert.AreEqual(31.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(sizeof(double), sizeof(double))));
    }

    [TestMethod]
    public void PointXYHelperCreatesCompactPayload()
    {
        var payload = ChartJsBinaryPayload.FromXY(
            "series-1",
            new ChartJsPoint[]
            {
                new(1.25, 2.25),
                new(3.25, 4.25)
            });

        Assert.AreEqual(ChartJsBinaryDataFormat.Float64XY, payload.Format);
        Assert.AreEqual(2, payload.Count);
        Assert.HasCount(4 * sizeof(double), payload.Bytes);
        Assert.AreEqual(1.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(0, sizeof(double))));
        Assert.AreEqual(2.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(sizeof(double), sizeof(double))));
        Assert.AreEqual(3.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(2 * sizeof(double), sizeof(double))));
        Assert.AreEqual(4.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(3 * sizeof(double), sizeof(double))));
    }

    [TestMethod]
    public void InterleavedXYHelperCreatesCompactPayload()
    {
        var payload = ChartJsBinaryPayload.FromXY("series-1", new double[] { 1.25, 2.25, 3.25, 4.25 });

        Assert.AreEqual(ChartJsBinaryDataFormat.Float64XY, payload.Format);
        Assert.AreEqual(2, payload.Count);
        Assert.HasCount(4 * sizeof(double), payload.Bytes);
        Assert.AreEqual(1.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(0, sizeof(double))));
        Assert.AreEqual(2.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(sizeof(double), sizeof(double))));
        Assert.AreEqual(3.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(2 * sizeof(double), sizeof(double))));
        Assert.AreEqual(4.25, BinaryPrimitives.ReadDoubleLittleEndian(payload.Bytes.AsSpan(3 * sizeof(double), sizeof(double))));
    }

    [TestMethod]
    public void HelperPayloadsPassConfigValidation()
    {
        ChartJsConfig config = new();

        config.SetDatasetsBinaryData(
        [
            ChartJsBinaryPayload.FromY("int-y", new int[] { 1, 2 }),
            ChartJsBinaryPayload.FromY("float-y", new float[] { 1, 2 }),
            ChartJsBinaryPayload.FromY("double-y", new double[] { 1, 2 }),
            ChartJsBinaryPayload.FromXY("point-xy", new ChartJsPoint[] { new(1, 2) }),
            ChartJsBinaryPayload.FromXY("double-xy", new double[] { 1, 2 })
        ]);
    }

    [TestMethod]
    public void HelperRejectsWhitespaceDatasetId()
    {
        Assert.ThrowsExactly<ArgumentException>(() => ChartJsBinaryPayload.FromY(" ", new int[] { 1 }));
    }

    [TestMethod]
    public void InterleavedXYHelperRejectsOddValueCount()
    {
        Assert.ThrowsExactly<ArgumentException>(() => ChartJsBinaryPayload.FromXY("series-1", new double[] { 1, 2, 3 }));
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
