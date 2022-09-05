
using System.Text.Json;

namespace pax.BlazorChartJs;

public enum ChartJsEventType
{
    None = 0,
    click = 1,
    hover = 2,
    leave = 3,
    progress = 4,
    complete = 5,
    resize = 6,
}

public enum ChartJsEventSource
{
    None = 0,
    legend = 1,
    animation = 2,
    label = 3,
    chart = 4,
}

public record ChartJsEvent
{
    public ChartJsEvent(Guid chartJsConfigGuid, ChartJsEventType type, ChartJsEventSource source, object? data)
    {
        ChartJsConfigGuid = chartJsConfigGuid;
        Type = type;
        Source = source;
        Data = data;

        var jsonElement = data == null ? new JsonElement() : (JsonElement)data;

        EventData = (source, type) switch
        {
            (ChartJsEventSource.label, ChartJsEventType.click) => JsonSerializer.Deserialize<LabelEventData>(jsonElement),
            (ChartJsEventSource.label, ChartJsEventType.hover) => JsonSerializer.Deserialize<LabelEventData>(jsonElement),
            (ChartJsEventSource.legend, _) => JsonSerializer.Deserialize<LegendEventData>(jsonElement),
            (ChartJsEventSource.animation, ChartJsEventType.progress) => JsonSerializer.Deserialize<AnimationProgressEventData>(jsonElement),
            (ChartJsEventSource.animation, ChartJsEventType.complete) => JsonSerializer.Deserialize<AnimationCompleteEventData>(jsonElement),
            (ChartJsEventSource.chart, ChartJsEventType.resize) => JsonSerializer.Deserialize<ResizeEventData>(jsonElement),
            _ => null
        };
    }

    public Guid ChartJsConfigGuid { get; init; }
    public ChartJsEventType Type { get; init; }
    public ChartJsEventSource Source { get; init; }
    public object? Data { get; init; }
    public ChartJsEventData? EventData { get; init; }
}

public record ChartJsEventData { }

#pragma warning disable CS8618 // json constructor
public record LabelEventData : ChartJsEventData
{
    public LabelEventData()
    {

    }

    public LabelEventData(string label, object? value, double dataX, double dataY)
    {
        Label = label;
        Value = value;
        DataX = dataX;
        DataY = dataY;
    }

    public string Label { get; set; }
    public object? Value { get; set; }
    public double DataX { get; init; }
    public double DataY { get; init; }
}

public record LegendEventData : ChartJsEventData
{
    public LegendEventData()
    {

    }

    public LegendEventData(string label)
    {
        Label = label;
    }
    public string Label { get; init; }
}


public record AnimationProgressEventData : ChartJsEventData
{
    public AnimationProgressEventData()
    {

    }

    public AnimationProgressEventData(double currentSteps, double numSteps)
    {
        CurrentStep = currentSteps;
        NumSteps = numSteps;
    }

    public double CurrentStep { get; init; }
    public double NumSteps { get; init; }

}

public record AnimationCompleteEventData : ChartJsEventData
{
    public AnimationCompleteEventData()
    {
    }
    public AnimationCompleteEventData(bool initial)
    {
        Initial = initial;
    }
    public bool Initial { get; init; }
}

public record ResizeEventData : ChartJsEventData
{
    public ResizeEventData()
    {
    }

    public ResizeEventData(double height, double width)
    {
        Height = height;
        Width = width;
    }

    public double Height { get; init; }
    public double Width { get; init; }
}

#pragma warning restore CS8618