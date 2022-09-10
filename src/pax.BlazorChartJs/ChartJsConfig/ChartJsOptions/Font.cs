namespace pax.BlazorChartJs;

public record Font
{
    public string? Family { get; set; }
    public double? Size { get; set; }
    /// <summary>
    /// Default font style. Does not apply to tooltip title or footer. Does not apply to chart title. Follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
    /// </summary>
    public string? Style { get; set; }
    /// <summary>
    /// Weight - css property
    /// </summary>    
    public string? Weight { get; set; }
    /// <summary>
    /// LineHeight - css property
    /// e.g.:
    /// normal
    /// 2.5
    /// 3em
    /// 150%
    /// 32px
    /// </summary>      
    public object? LineHeight { get; set; }

}


