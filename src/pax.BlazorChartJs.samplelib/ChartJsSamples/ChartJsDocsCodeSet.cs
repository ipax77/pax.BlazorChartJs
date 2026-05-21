namespace pax.BlazorChartJs.samplelib.ChartJsSamples;

public sealed record ChartJsDocsCodeSet
{
    public ChartJsDocsCodeSet(string config, string setup, string actions)
        : this(
            [
                new ChartJsDocsCodeTab("config", "Config", config),
                new ChartJsDocsCodeTab("setup", "Setup", setup),
                new ChartJsDocsCodeTab("actions", "Actions", actions),
            ])
    {
    }

    public ChartJsDocsCodeSet(IReadOnlyList<ChartJsDocsCodeTab> tabs)
    {
        Tabs = tabs;
    }

    public IReadOnlyList<ChartJsDocsCodeTab> Tabs { get; }

    public string Config => GetCode("config");

    public string Setup => GetCode("setup");

    public string Actions => GetCode("actions");

    public string GetCode(string tabId)
    {
        for (var i = 0; i < Tabs.Count; i++)
        {
            if (string.Equals(Tabs[i].Id, tabId, StringComparison.Ordinal))
            {
                return Tabs[i].Code;
            }
        }

        return Tabs.Count == 0 ? string.Empty : Tabs[0].Code;
    }
}

public sealed record ChartJsDocsCodeTab(string Id, string Label, string Code);
