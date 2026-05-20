namespace pax.BlazorChartJs;

public record ChartJsFillOptions
{
    public string? Above { get; set; }
    public string? Below { get; set; }
    public ChartJsFillTarget? Target { get; set; }
}

public record ChartJsFillTarget
{
    public double? Value { get; set; }
}

