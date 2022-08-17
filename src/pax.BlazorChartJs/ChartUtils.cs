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

    public static void AddRandomDataset(ChartJsConfig config, int min = -100, int max = 100)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (min >= max)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        int count = config.Data.Labels.Count;

        if (config.Type == ChartType.bar)
        {
            var dataset = new BarDataset()
            {
                Label = $"Dataset {config.Data.Datasets.Count + 1}",
                BackgroundColor = GetRandomColors(count),
                BorderColor = GetRandomColors(count),
                BorderWidth = 1,
                Data = GetRandomNumbers(count, min, max)
            };
            config.Data.Datasets.Add(dataset);
        } else if (config.Type == ChartType.line)
        {
            var dataset = new LineDataset()
            {
                Label = $"Dataset {config.Data.Datasets.Count + 1}",
                BackgroundColor = GetRandomColors(count),
                BorderColor = GetRandomColors(count),
                BorderWidth = 1,
                Data = GetRandomNumbers(count, min, max)
            };
            config.Data.Datasets.Add(dataset);
        } else if (config.Type == ChartType.pie)
        {
            var dataset = new PieDataset()
            {
                BackgroundColor = GetRandomColors(count),
                BorderColor = "red",
                BorderWidth = 1,
                Data = GetRandomNumbers(count, min, max)
            };
            config.Data.Datasets.Add(dataset);
        }
    }

    public static void AddRandomData(ChartJsConfig config, int min = -100, int max = 100)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (min >= max)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        var label = Labels[random.Next(Labels.Count)];
        config.Data.Labels.Add(label);

        if (config.Type == ChartType.bar)
        {
            foreach (BarDataset dataset in config.Data.Datasets.Cast<BarDataset>())
            {
                dataset.Data.Add(random.Next(min, max));
                if (dataset.BackgroundColor != null && dataset.BackgroundColor.GetType() == typeof(List<string>))
                {
                    ((List<string>)dataset.BackgroundColor).Add(Colors[random.Next(Colors.Count)]);
                }
                if (dataset.BorderColor != null && dataset.BorderColor.GetType() == typeof(List<string>))
                {
                    ((List<string>)dataset.BorderColor).Add(Colors[random.Next(Colors.Count)]);
                }
            }
        }
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