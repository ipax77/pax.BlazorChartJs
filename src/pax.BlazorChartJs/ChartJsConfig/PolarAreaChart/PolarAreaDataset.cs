using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs;
public record PolarAreaDataset : ChartJsDataset
{
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>           
    public object? BackgroundColor { get; set; }
    /// <summary>
    /// BorderAlign - 'center'|'inner'
    /// </summary>      
    public string? BorderAlign { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/charts/doughnut.html#border-radius">ChartJs docs</see>
    /// </summary>
    public object? BorderColor { get; set; }
    public double? BorderWidth { get; set; }
    /// <summary>
    /// can be number|object|false
    /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels inside chartArea. 0 = clip at chartArea.
    /// Clipping can also be configured per side: clip: {left: 5, top: false, right: -2, bottom: 0}
    /// </summary>    
    public object? Clip { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBackgroundColor { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public object? HoverBorderColor { get; set; }
    /// <summary>
    /// HoverBorderJoinStyle - 'round'|'bevel'|'miter'
    /// </summary>          
    public string? HoverBorderJoinStyle { get; set; }
    /// <summary>
    /// Color can either be a single string (css Color) or a list <see href="https://www.chartjs.org/docs/latest/general/colors.html">ChartJs docs</see>
    /// </summary>      
    public double? HoverBorderWidth { get; set; }
    /// <summary>
    /// By default the Arc is curved. If circular: false the Arc will be flat.
    /// </summary>
    public bool? Circular { get; set; }
}
