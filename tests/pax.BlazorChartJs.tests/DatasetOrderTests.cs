namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class DatasetOrderTests
{
    [TestMethod]
    public void AddDatasetsAfterInsertsAfterAnchor()
    {
        var config = CreateConfig();
        BarDataset add1 = CreateDataset("add-1");
        BarDataset add2 = CreateDataset("add-2");

        config.AddDatasetsAfter("a", [add1, add2]);

        CollectionAssert.AreEqual(
            new[] { "a", "add-1", "add-2", "b" },
            config.Data.Datasets.Select(dataset => dataset.Id).ToArray());
    }

    [TestMethod]
    public void AddDatasetsBeforeInsertsBeforeAnchor()
    {
        var config = CreateConfig();
        BarDataset add1 = CreateDataset("add-1");
        BarDataset add2 = CreateDataset("add-2");

        config.AddDatasetsBefore("b", [add1, add2]);

        CollectionAssert.AreEqual(
            new[] { "a", "add-1", "add-2", "b" },
            config.Data.Datasets.Select(dataset => dataset.Id).ToArray());
    }

    [TestMethod]
    public void AddDatasetAfterAppendsWhenAnchorIsMissing()
    {
        var config = CreateConfig();

        config.AddDatasetAfter("missing", CreateDataset("add-1"));

        CollectionAssert.AreEqual(
            new[] { "a", "b", "add-1" },
            config.Data.Datasets.Select(dataset => dataset.Id).ToArray());
    }

    [TestMethod]
    public void AddDatasetBeforeAppendsWhenAnchorIsMissing()
    {
        var config = CreateConfig();

        config.AddDatasetBefore("missing", CreateDataset("add-1"));

        CollectionAssert.AreEqual(
            new[] { "a", "b", "add-1" },
            config.Data.Datasets.Select(dataset => dataset.Id).ToArray());
    }

    private static ChartJsConfig CreateConfig()
    {
        return new()
        {
            Type = ChartType.bar,
            Data = new ChartJsData
            {
                Labels = ["One", "Two"],
                Datasets =
                [
                    CreateDataset("a"),
                    CreateDataset("b")
                ]
            }
        };
    }

    private static BarDataset CreateDataset(string id)
    {
        return new()
        {
            Id = id,
            Label = id,
            Data = [1, 2]
        };
    }
}
