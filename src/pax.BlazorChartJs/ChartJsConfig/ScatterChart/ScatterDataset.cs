using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs;
public record ScatterDataset : LineDataset
{
    public new IList<PointData> Data { get; set; } = new List<PointData> { new PointData() { X = 10, Y = 20 }, new PointData() { X = 15, Y = 10 } };
}

public record PointData
{
    public double X { get; set; }
    public double Y { get; set; }
}
