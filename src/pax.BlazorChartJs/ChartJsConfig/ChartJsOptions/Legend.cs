namespace pax.BlazorChartJs;

public record Legend
{
    public bool? Display { get; set; }
    /// <summary>
    /// Position
    /// Position of the legend. Options are:
    /// 
    /// 'top'
    /// 'left'
    /// 'bottom'
    /// 'right'
    /// 'chartArea'
    /// When using the 'chartArea' option the legend position is at the moment not configurable, it will always be on the left side of the chart in the middle.    
    /// <see href="https://www.chartjs.org/docs/latest/configuration/legend.html#position">ChartJs docs</see>
    /// </summary>
    public string? Position { get; set; }
    /// <summary>
    /// Align
    /// Alignment of the legend. Options are:
    /// 
    /// 'start'
    /// 'center'
    /// 'end'
    /// Defaults to 'center' for unrecognized values   
    /// <see href="https://www.chartjs.org/docs/latest/configuration/legend.html#align">ChartJs docs</see>
    /// </summary>    
    public string? Align { get; set; }
    public double? MaxHeight { get; set; }
    public double? MaxWidth { get; set; }
    public bool? FullSize { get; set; }
    /// <summary>
    /// Registers OnClick Event - ChartJsEvent type = ChartJsLegendClickEvent
    /// </summary>   
    public bool? OnClickEvent { get; set; }
    /// <summary>
    /// Registers OnHover Event - ChartJsEvent type = ChartJsLegendHoverEvent
    /// </summary>   
    public bool? OnHoverEvent { get; set; }
    /// <summary>
    /// Registers OnLeave Event - ChartJsEvent type = ChartJsLegendLeaveEvent
    /// </summary>       
    public bool? OnLeaveEvent { get; set; }
    public bool? Reverse { get; set; }
    public Labels? Labels { get; set; }
    /// <summary>
    /// true for rendering the legends from right to left.
    /// </summary>        
    public bool? Rtl { get; set; }
    /// <summary>
    /// This will force the text direction 'rtl' or 'ltr' on the canvas for rendering the legend, regardless of the css specified on the canvas
    /// </summary>       
    public string? TextDirection { get; set; }
    public Title? Title { get; set; }
}


