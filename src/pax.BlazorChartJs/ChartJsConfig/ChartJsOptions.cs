using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;

# pragma warning disable CA2227

/// <summary>
/// Configuration Options
/// </summary>
public class ChartJsOptions
{
    /// <summary>
    /// Resizes the chart canvas when its container does (<see href="https://www.chartjs.org/docs/latest/configuration/responsive.html#important-note">important note...</see>).
    /// </summary>
    public bool? Responsive { get; set; }
    /// <summary>
    /// Maintain the original canvas aspect ratio (width / height) when resizing.
    /// </summary>    
    public bool? MaintainAspectRatio { get; set; }
    /// <summary>
    /// Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas). Note that this option is ignored if the height is explicitly defined either as attribute or via the style. The default value varies by chart type; Radial charts (doughnut, pie, polarArea, radar) default to 1 and others default to 2.
    /// </summary>    
    public int? AspectRatio { get; set; }
    /// <summary>
    /// Delay the resize update by the given amount of milliseconds. This can ease the resize process by debouncing the update of the elements.
    /// </summary>    
    public int? ResizeDelay { get; set; }

    // public Plugins? Plugins { get; set; }
    public ChartJsOptionsScales? Scales { get; set; }
}

public class ChartJsOptionsScales
{
    public ChartJsOptionsScalesX? X { get; set; }
    public ChartJsOptionsScalesY? Y { get; set; }
}

public class ChartJsOptionsScalesX
{
    public string? Axis { get; set; }
    public bool? Display { get; set; }
    public Title? Title { get; set; }
    public Ticks? Ticks { get; set; }
    public Grid? Grid { get; set; }
    public string? Type { get; set; }
    public bool? Offset { get; set; }
    public bool? Reverse { get; set; }
    public bool? BeginAtZero { get; set; }
    public string? Bounds { get; set; }
    public int? Grace { get; set; }
    public string? Id { get; set; }
    public string? Position { get; set; }

}

public class Ticks
{
    public string? Color { get; set; }
    public bool? BeginAtZero { get; set; }
    public int? MinRotation { get; set; }
    public int? MaxRotation { get; set; }
    public bool? Mirror { get; set; }
    public int? TextStrokeWidth { get; set; }
    public string? TextStrokeColor { get; set; }
    public int? Padding { get; set; }
    public bool? Display { get; set; }
    public bool? AutoSkip { get; set; }
    public int? AutoSkipPadding { get; set; }
    public int? LabelOffset { get; set; }
    public string? Align { get; set; }
    public string? CrossAlign { get; set; }
    public bool? ShowLabelBackdrop { get; set; }
    public string? BackdropColor { get; set; }
    public int? BackdropPadding { get; set; }
}

public class Grid
{
    public string? Color { get; set; }
    public bool? Display { get; set; }
    public int? LineWidth { get; set; }
    public bool? DrawBorder { get; set; }
    public bool? DrawOnChartArea { get; set; }
    public bool? DrawTicks { get; set; }
    public int? TickLength { get; set; }
    public int? TickWidth { get; set; }
    public string? TickColor { get; set; }
    public bool? Offset { get; set; }
    public ICollection<object>? BorderDash { get; set; }
    public int? BorderDashOffset { get; set; }
    public int? BorderWidth { get; set; }
    public string? BorderColor { get; set; }
}


public class Title
{
    public bool? Display { get; set; }
    public string? Text { get; set; }
    public string? Color { get; set; }
    public Font? Font { get; set; }
    public Padding? Padding { get; set; }

}

public class Font
{
    public int? Size { get; set; }
}

public class Padding
{
    public int Top { get; set; }
    public int Bottom { get; set; }
}

public class ChartJsOptionsScalesY
{
    public string? Axis { get; set; }
    public bool? Display { get; set; }
    public Title? Title { get; set; }
    public Ticks? Ticks { get; set; }
    public Grid? Grid { get; set; }
    public string? Type { get; set; }
    public bool? Offset { get; set; }
    public bool? Reverse { get; set; }
    public bool? BeginAtZero { get; set; }
    public string? Bounds { get; set; }
    public int? Grace { get; set; }
    public string? Id { get; set; }
    public string? Position { get; set; }
}
