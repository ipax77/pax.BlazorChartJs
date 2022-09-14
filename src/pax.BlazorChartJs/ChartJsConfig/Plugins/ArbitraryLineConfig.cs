namespace pax.BlazorChartJs;

/// <summary>
/// ArbitraryLineConfig <see href="https://www.youtube.com/watch?v=7ZZ_XfaJQbM">GitHub</see>
/// </summary>
public record ArbitraryLineConfig
{
    public string ArbitraryLineColor { get; set; } = "blue";
    public int XPosition { get; set; }
    public int XWidth { get; set; } = 3;
    public string Text { get; set; } = "Sommer";
}
