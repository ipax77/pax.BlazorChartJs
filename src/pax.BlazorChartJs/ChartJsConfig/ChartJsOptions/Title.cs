namespace pax.BlazorChartJs;

public record Title
{
    /// <summary>
    /// Align
    /// Alignment of the title. Options are:
    /// 
    /// 'start'
    /// 'center'
    /// 'end'    
    /// </summary>
    public string? Align { get; set; }
    public object? Color { get; set; }
    public bool? Display { get; set; }
    public bool? FullSize { get; set; }
    /// <summary>
    /// Position
    /// Possible title position values are:
    /// 
    /// 'top'
    /// 'left'
    /// 'bottom'
    /// 'right'
    /// </summary>    
    public string? Position { get; set; }
    public Font? Font { get; set; }
    public Padding? Padding { get; set; }
    public IndexableOption<string>? Text { get; set; }
}


