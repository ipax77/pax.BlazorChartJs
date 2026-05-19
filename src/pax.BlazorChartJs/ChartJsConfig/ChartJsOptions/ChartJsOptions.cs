namespace pax.BlazorChartJs;

# pragma warning disable CA2227

/// <summary>
/// Configuration Options
/// </summary>
public record ChartJsOptions : ChartJsCoreOptions
{
    /// <summary>
    /// Enables the Blazor/C# hover event bridge - ChartJsEvent type = ChartJsLabelHoverEvent.
    /// Native Chart.js OnHover callbacks configured on this chart or inherited from ChartJsSetupOptions.Defaults are preserved and invoked before the C# event is emitted.
    /// </summary>   
    public bool? OnHoverEvent { get; set; }
    /// <summary>
    /// Enables the Blazor/C# click event bridge - ChartJsEvent type = ChartJsLabelClickEvent.
    /// Native Chart.js OnClick callbacks configured on this chart or inherited from ChartJsSetupOptions.Defaults are preserved and invoked before the C# event is emitted.
    /// </summary>   
    public bool? OnClickEvent { get; set; }
    /// <summary>
    /// Enables the Blazor/C# resize event bridge - ChartJsEvent type = ChartJsResizeEvent.
    /// Native Chart.js OnResize callbacks configured on this chart or inherited from ChartJsSetupOptions.Defaults are preserved and invoked before the C# event is emitted.
    /// </summary>   
    public bool? OnResizeEvent { get; set; }
}
