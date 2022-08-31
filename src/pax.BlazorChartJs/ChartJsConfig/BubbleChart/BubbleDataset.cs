using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs;
public record BubbleDataset : ChartJsDataset
{
    public new IList<BubbleDataPoint> Data { get; set; } = new List<BubbleDataPoint> { new BubbleDataPoint() { X = 10, Y = 20, R = 4 }, new BubbleDataPoint() { X = 15, Y = 10, R = 6 } };

    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public object? BackgroundColor { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? BorderColor { get; set; }
    public double? BorderWidth { get; set; }
    /// <summary>
    /// Clip - number|object|false
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea. 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>    
    public object? Clip { get; set; }
    /// <summary>
    /// Draw the active bubbles of a dataset over the other bubbles of the dataset
    /// </summary>
    public bool? DrawActiveElementsOnTop { get; set; }
    public object? HoverBackgroundColor { get; set; }
    public object? HoverBorderColor { get; set; }
    public double? HoverBorderWidth { get; set; }
    public double? HoverRadius { get; set; }
    public double? HitRadius { get; set; }
    public string? Label { get; set; }
    public int? Order { get; set; }
    /// <summary>
    /// Types
    ///    The pointStyle argument accepts the following type of inputs: string, Image and HTMLCanvasElement
    ///
    ///    #Info
    ///    When a string is provided, the following values are supported:
    ///
    ///    'circle'
    ///    'cross'
    ///    'crossRot'
    ///    'dash'
    ///    'line'
    ///    'rect'
    ///    'rectRounded'
    ///    'rectRot'
    ///    'star'
    ///    'triangle'
    ///    If the value is an image or a canvas element, that image or canvas element is drawn on the canvas usin
    /// </summary>    
    public string? PointStyle { get; set; }
    public double? Rotation { get; set; }
    public double? Radius { get; set; }

}

public record BubbleDataPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public double R { get; set; }
}