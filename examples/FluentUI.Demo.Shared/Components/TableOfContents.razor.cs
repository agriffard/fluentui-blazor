using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.JSInterop;

namespace FluentUI.Demo.Shared.Components;

public partial class TableOfContents : IAsyncDisposable
{
    private record Anchor(string Level, string Text, string Href, Anchor[] Anchors);
    private Anchor[]? _anchors;
    private bool _expanded = true;

    private IJSObjectReference _jsModule = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/FluentUI.Demo.Shared/Components/TableOfContents.razor.js");
            await QueryDom();

            IJSObjectReference _jsModule2 = await JSRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/FluentUI.Demo.Shared/Shared/DemoMainLayout.razor.js");
            bool mobile = await _jsModule2!.InvokeAsync<bool>("isDevice");

            if (mobile)
            {
                _expanded = false;
            }
        }
    }

    private async Task BackToTop()
    {
        await _jsModule.InvokeAsync<Anchor[]?>("backToTop");
    }

    private async Task QueryDom()
    {
        _anchors = await _jsModule.InvokeAsync<Anchor[]?>("queryDomForTocEntries");
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        // Subscribe to the event
        NavigationManager.LocationChanged += LocationChanged;
        base.OnInitialized();
    }

    private async void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        await QueryDom();
    }
    
    private RenderFragment? GetTocItems(IEnumerable<Anchor>? items)
    {
        if (items is not null)
        {
            return new RenderFragment(builder =>
            {
                int i = 0;

                builder.OpenElement(i++, "ul");
                foreach (Anchor item in items)
                {
                    builder.OpenElement(i++, "li");
                    builder.OpenComponent<FluentAnchor>(i++);
                    builder.AddAttribute(i++, "Href", item.Href);
                    builder.AddAttribute(i++, "Appearance", Appearance.Hypertext);
                    builder.AddAttribute(i++, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(i++, item.Text);
                        
                    }));
                    builder.CloseComponent();
                    if (item.Anchors is not null)
                    {
                       builder.AddContent(i++, GetTocItems(item.Anchors));
                    }
                    builder.CloseElement();
                }
                builder.CloseElement();
            });
        }
        else
        {
            return new RenderFragment(builder =>
            {
                builder.AddContent(0, ChildContent);
            });
        }

    }
    
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        try
        {
            // Unsubscribe from the event when our component is disposed
            NavigationManager.LocationChanged -= LocationChanged;
            
            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // The JSRuntime side may routinely be gone already if the reason we're disposing is that
            // the client disconnected. This is not an error.
        }
    }
}