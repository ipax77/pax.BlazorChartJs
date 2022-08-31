using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs;
public record ScatterDataset : LineDataset
{
}

public record DataPoint
{
    public double X { get; set; }
    public double Y { get; set; }
}
