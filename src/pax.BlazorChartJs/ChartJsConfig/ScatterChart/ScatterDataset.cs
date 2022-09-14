namespace pax.BlazorChartJs;
public record ScatterDataset : LineDataset
{
}

public record DataPoint
{
    public double X { get; set; }
    public double Y { get; set; }
}
