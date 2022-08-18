using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pax.BlazorChartJs;
#pragma warning disable CA5394
public static class ChartUtils
{
    private static Random random = new Random();
    private static readonly List<string> Labels = new()
    {
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
    };
    private static readonly List<string> Colors = new()
    {
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
    };

    public static void RandomizeData(ChartJsConfig config, int min = -100, int max = 100)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (min >= max)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        var data = config.Data.Datasets.ToList().Cast<ChartJsDataset>();
        var count = config.Data.Labels.Count;

        foreach (var dataset in data)
        {
            dataset.Data = GetRandomNumbers(count, min, max);           
        }
    }

    public static object GetRandomDataset(ChartType chartType, int id, int count, int min = -100, int max = 100)
    {
        if (min >= max)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        return chartType switch
        {
            ChartType.bar => GetRandomBarDataset(id, count, min, max),
            ChartType.line => GetRandomLineDataset(id, count, min, max),
            ChartType.pie => GetRandomPieDataset(id, count, min, max),
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
            BackgroundColor = GetRandomColors(count),
            BorderColor = GetRandomColors(count),
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }
    private static PieDataset GetRandomPieDataset(int id, int count, int min, int max)
    {
        return new PieDataset()
        {
            BackgroundColor = GetRandomColors(count),
            BorderColor = GetRandomColors(count),
            BorderWidth = 1,
            Data = GetRandomNumbers(count, min, max)
        };
    }

    public static DataAddEventArgs GetRandomData(int count, int min = -100, int max = 100)
    {
        if (min >= max)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        var label = Labels[random.Next(Labels.Count)];
        List<object> data = new();
        List<string> backgroundColors = new();
        List<string> borderColors = new();

        for (int i = 0; i < count; i++)
        {
            data.Add(random.Next(min, max));
            backgroundColors.Add(Colors[random.Next(Colors.Count)]);
            borderColors.Add(Colors[random.Next(Colors.Count)]);
        }

        return new DataAddEventArgs(label, data, backgroundColors, borderColors, null);
    }

    private static List<string> GetRandomColors(int count)
    {
        var colors = new List<string>();
        for (int i = 0; i < count; i++)
        {
            colors.Add(Colors[random.Next(Colors.Count)]);
        }
        return colors;
    }

    private static List<object> GetRandomNumbers(int count, int min, int max)
    {
        var numbers = new List<object>();
        for (int i = 0; i < count; i++)
        {
            numbers.Add(random.Next(min, max));
        }
        return numbers;
    }
}
#pragma warning restore CA5394