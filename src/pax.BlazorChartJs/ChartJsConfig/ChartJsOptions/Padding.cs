namespace pax.BlazorChartJs;

public record Padding
{
    public Padding()
    {

    }

    public Padding(double allSides)
    {
        Left = allSides;
        Top = allSides;
        Right = allSides;
        Bottom = allSides;
    }

    public double? Left { get; set; }
    public double? Top { get; set; }
    public double? Right { get; set; }
    public double? Bottom { get; set; }
    public double? Z { get; set; }
}


