namespace pax.BlazorChartJs;

#pragma warning disable CA5394
public static class ChartUtils
{
    private static readonly List<string> Labels =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];
    private static readonly List<string> Colors =
    [
        "#000000",
        "#FFFFFF",
        "#FF0000",
        "#00FF00",
        "#0000FF",
        "#FFFF00",
        "#00FFFF",
        "#FF00FF",
        "#C0C0C0",
        "#808080",
        "#800000",
        "#808000",
        "#008000",
        "#800080",
        "#008080",
        "#000080",
    ];

    public static string GetRandomColor()
    {
        return Colors[Random.Shared.Next(Colors.Count)];
    }

    public static ICollection<List<object>> GetRandomData(int length, int count, int min = -100, int max = 100)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min, max);

        List<List<object>> data = [];

        for (int i = 0; i < length; i++)
        {
            data.Add(GetRandomNumbers(count, min, max));
        }
        return data;
    }

    public static ChartJsDataset GetRandomDataset(ChartType chartType, int id, int count, int min = -100, int max = 100)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min, max);

        return chartType switch
        {
            ChartType.bar => GetRandomBarDataset(id, count, min, max),
            ChartType.line => GetRandomLineDataset(id, count, min, max),
            ChartType.pie => GetRandomPieDataset(count, min, max),
            ChartType.radar => GetRandomRadarDataset(id, count, min, max),
            ChartType.scatter => GetRandomScatterDataset(id, count, min, max),
            ChartType.bubble => GetRandomBubbleDataset(id, count, min, max),
            _ => throw new NotImplementedException(nameof(chartType))
        };
    }

    private static BarDataset GetRandomBarDataset(int id, int count, int min, int max)
    {
        return new BarDataset()
        {
            Label = $"Dataset {id}",
            BackgroundColor = GetRandomColors(count),
            BorderColor = GetRandomColors(count),
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }
    private static LineDataset GetRandomLineDataset(int id, int count, int min, int max)
    {
        return new LineDataset()
        {
            Label = $"Dataset {id}",
            BackgroundColor = GetRandomColors(1).First(),
            BorderColor = GetRandomColors(1).First(),
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }

    private static PieDataset GetRandomPieDataset(int count, int min, int max)
    {
        return new PieDataset()
        {
            BackgroundColor = GetRandomColors(count),
            BorderColor = GetRandomColors(count),
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }

    private static RadarDataset GetRandomRadarDataset(int id, int count, int min, int max)
    {
        var borderColor = GetRandomColors(1).First();
        var backgroundColor = $"{borderColor}80";
        return new RadarDataset()
        {
            Label = $"Dataset {id}",
            BackgroundColor = backgroundColor,
            BorderColor = borderColor,
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }

    private static ScatterDataset GetRandomScatterDataset(int id, int count, int min, int max)
    {
        var backgroundColor = GetRandomColors(1).First();
        var data = new List<object>();

        for (int i = 0; i < count; i++)
        {
            data.Add(new DataPoint()
            {
                X = Random.Shared.Next(min, max),
                Y = Random.Shared.Next(min, max)
            });
        }

        return new ScatterDataset()
        {
            Label = $"Dataset {id}",
            BackgroundColor = backgroundColor,
            Data = data
        };
    }

    private static BubbleDataset GetRandomBubbleDataset(int id, int count, int min, int max)
    {
        var backgroundColor = GetRandomColors(1).First();

        List<object> data = [];
        for (int i = 0; i < count; i++)
        {
            data.Add(new BubbleDataPoint()
            {
                X = Random.Shared.Next(min, max),
                Y = Random.Shared.Next(min, max),
                R = Random.Shared.Next(1, 15)
            });
        }

        return new BubbleDataset()
        {
            Label = $"Dataset {id}",
            BackgroundColor = backgroundColor,
            Data = data
        };
    }

    public static DataAddEventArgs GetRandomData(int count, int min = -100, int max = 100)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min, max);

        var label = Labels[Random.Shared.Next(Labels.Count)];
        List<object> data = [];
        List<string> backgroundColors = [];
        List<string> borderColors = [];

        for (int i = 0; i < count; i++)
        {
            data.Add(Random.Shared.Next(min, max));
            backgroundColors.Add(Colors[Random.Shared.Next(Colors.Count)]);
            borderColors.Add(Colors[Random.Shared.Next(Colors.Count)]);
        }

        return new DataAddEventArgs(label, data, backgroundColors, borderColors, null);
    }

    private static List<string> GetRandomColors(int count)
    {
        var colors = new List<string>();
        for (int i = 0; i < count; i++)
        {
            colors.Add(Colors[Random.Shared.Next(Colors.Count)]);
        }
        return colors;
    }

    private static List<object> GetRandomNumbers(int count, int min, int max)
    {
        var numbers = new List<object>();
        for (int i = 0; i < count; i++)
        {
            numbers.Add(Random.Shared.Next(min, max));
        }
        return numbers;
    }
}
#pragma warning restore CA5394