namespace pax.BlazorChartJs;

public record ChartJsElementsOptions
{
    public ChartJsBarElementOptions? Bar { get; set; }
    public ChartJsLineElementOptions? Line { get; set; }
    public ChartJsPointElementOptions? Point { get; set; }
    public ChartJsArcElementOptions? Arc { get; set; }
}

public record ChartJsBarElementOptions
{
    public IndexableOption<string>? BackgroundColor { get; set; }
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<object>? BorderSkipped { get; set; }
    public IndexableOption<double>? BorderRadius { get; set; }
}

public record ChartJsLineElementOptions
{
    public IndexableOption<string>? BackgroundColor { get; set; }
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<object>? Fill { get; set; }
    public IndexableOption<double>? Tension { get; set; }
}

public record ChartJsPointElementOptions
{
    public IndexableOption<string>? BackgroundColor { get; set; }
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<string>? HoverBackgroundColor { get; set; }
    public IndexableOption<string>? HoverBorderColor { get; set; }
    public IndexableOption<double>? HoverBorderWidth { get; set; }
    public IndexableOption<double>? HoverRadius { get; set; }
    public IndexableOption<double>? Radius { get; set; }
    public IndexableOption<object>? PointStyle { get; set; }
}

public record ChartJsArcElementOptions
{
    public IndexableOption<string>? BackgroundColor { get; set; }
    public IndexableOption<string>? BorderColor { get; set; }
    public IndexableOption<double>? BorderWidth { get; set; }
    public IndexableOption<double>? HoverOffset { get; set; }
}
