﻿using AirTransfer.Consts;
using AirTransfer.Interfaces;

using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Components;

public abstract class PageComponentBase : VisualBase
{
    #region Injects


    [Inject] protected ILoopWatchClipboardService LoopWatchClipboardService { get; set; } = null!;


    #endregion

    #region Fields

    private readonly string fromPage = nameof(fromPage);

    private readonly string data = nameof(data);

    #endregion



    protected override Task OnInitializedAsync()
    {
        StateManager.StateChanged += () => StateChanged();
        return ParseInitPageDataAsync();
    }

    private Task StateChanged()
    {
        return InvokeAsync(StateHasChanged);

    }

    private Task ParseInitPageDataAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        StateManager.SetState(ConstParams.StateManagerKeys.CurrentUriKey, NavigationManager.ToBaseRelativePath(NavigationManager.Uri));

        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        Dictionary<string, object>? data = null;
        string? fromUri = null;
        if (query.HasKeys())
        {
            if (query.AllKeys.Any(x => x == fromPage))
            {
                fromUri = query[fromPage];
            }

            if (query.AllKeys.Any(x => x == this.data))
            {
                var dataString = query[this.data];
                data = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataString);
            }
        }

        return OnPageInitializedAsync(fromUri, data);
    }

    protected virtual Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        return Task.CompletedTask;
    }

    protected void NavigateTo(string uri)
    {
        NavigationManager.NavigateTo(uri);
    }

    protected void NavigateTo(string uri, Dictionary<string, object> dataDictionary)
    {
        var currentPage = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var json = JsonConvert.SerializeObject(dataDictionary, new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        });
        NavigationManager.NavigateTo($"{uri}?{fromPage}={currentPage.AbsolutePath}&{data}={json}");
    }


}