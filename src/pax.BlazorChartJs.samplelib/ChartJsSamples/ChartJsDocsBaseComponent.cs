using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace pax.BlazorChartJs.samplelib.ChartJsSamples;

public abstract class ChartJsDocsBaseComponent : ComponentBase
{
    protected const string ConfigTab = "config";
    protected const string SetupTab = "setup";
    protected const string ActionsTab = "actions";

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected string ActiveCodeTab { get; private set; } = ConfigTab;

    private bool highlightPending = true;

    protected Task SelectCodeTab(string codeTab)
    {
        if (codeTab is ConfigTab or SetupTab or ActionsTab && !string.Equals(ActiveCodeTab, codeTab, StringComparison.Ordinal))
        {
            ActiveCodeTab = codeTab;
            QueueCodeHighlight();
        }

        return Task.CompletedTask;
    }

    protected ChartJsDocsAction CreateAction(string id, string name, Action handler)
    {
        return new ChartJsDocsAction(id, name, EventCallback.Factory.Create(this, handler));
    }

    protected void QueueCodeHighlight()
    {
        highlightPending = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || highlightPending)
        {
            highlightPending = false;
            await JSRuntime.InvokeVoidAsync("highlightCode").ConfigureAwait(false);
        }
    }
}
