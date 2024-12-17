// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Reactive.Subjects;
using System.Windows.Threading;
using BetterClip.Controllers;
using BetterClip.Core.Config;
using BetterClip.Extension;
using BetterClip.Model.Monaco;
using BetterClip.Service;
using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace BetterClip.ViewModel.Editor;

public partial class MonacoEditorViewModel : ViewModel
{
    private MonacoController? _monacoController;
    [ObservableProperty]
    private string _text = "";
    private readonly LanguageService languageService;
    private readonly Subject<Action> Subject = new();
    private MonacoLanguage _language = MonacoLanguage.non;
    public MonacoEditorViewModel(LanguageService languageService)
    {
        this.languageService = languageService;
        Subject.Subscribe(action => action?.Invoke());
    }
    public void SetWebView(WebView2 webView)
    {
        webView.NavigationCompleted += OnWebViewNavigationCompleted;
        webView.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);
        webView.SetCurrentValue(WebView2.DefaultBackgroundColorProperty, System.Drawing.Color.Transparent);
        webView.SetCurrentValue(
            WebView2.SourceProperty,
            new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\Monaco\index.html"))
        );

        _monacoController = new MonacoController(webView);
    }

    [RelayCommand]
    public async Task OnMenuAction(string parameter)
    {
        if (_monacoController == null)
        {
            return;
        }

        var snakbar = App.GetService<ISnackbarService>();
        var content = await _monacoController.GetTextAsync();
        snakbar.Info(content, time: 5);
        return;
    }

    private async Task InitializeEditorAsync()
    {
        if (_monacoController == null)
        {
            return;
        }

        await _monacoController.CreateEditorAsync(Text);
        await _monacoController.SetThemeAsync(ApplicationThemeManager.GetAppTheme());
        _monacoController.ListenToContentChange();
        _monacoController.TextChanged += (s, e) =>
        {
            Text = e.NewText;
        };
    }

    private void OnWebViewNavigationCompleted(
        object? sender,
        Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e
    )
    {
        DispatchAsync(InitializeEditorAsync);
    }

    private static DispatcherOperation<TResult> DispatchAsync<TResult>(Func<TResult> callback)
    {
        return Application.Current.Dispatcher.InvokeAsync(callback);
    }
}
