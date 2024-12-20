// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using BetterClip.Controllers;
using BetterClip.Extension;
using BetterClip.Model.Monaco;
using BetterClip.Service;
using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using static BetterClip.Controllers.MonacoController;

namespace BetterClip.ViewModel.Editor;

public partial class MonacoEditorViewModel : ViewModel
{
    private MonacoController? _monacoController;
    private readonly Subject<Action> Subject = new();

    [ObservableProperty]
    private string _text = "";


    [ObservableProperty]
    private string _originalText = "";

    [ObservableProperty]
    private string _modifiedText = "";
    [ObservableProperty]
    private MonacoLanguage _language = MonacoLanguage.None;

    [ObservableProperty]
    private bool _wordWrap = false;

    public IEnumerable<MonacoLanguage> MonacoLanguages { get; } = Enum.GetValues<MonacoLanguage>();

    public bool DiffMode { get; set; } = false;

    public MonacoEditorViewModel()
    {
        // TODO auto detect language
        Subject.Subscribe(action => action?.Invoke());
    }

    public void SetWebView(WebView2 webView)
    {
        _monacoController = new MonacoController(webView);
        webView.NavigationCompleted += async (s, e) =>
        {
            if (DiffMode)
            {
                await DispatchAsync(InitialzeDiffEditorAsync);
            }
            else
            {
                await DispatchAsync(InitializeEditorAsync);
            }
            await _monacoController.SetWordWrap(WordWrap);
        };
        webView.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);
        webView.SetCurrentValue(WebView2.DefaultBackgroundColorProperty, System.Drawing.Color.Transparent);
        webView.SetCurrentValue(
            WebView2.SourceProperty,
            new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\Monaco\index.html"))
        );



        MonitorProperty();
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
    
    public event TextChangedEventHandler? TextChanged;
    public event TextChangedEventHandler? OriginalTextChanged;
    public event TextChangedEventHandler? ModifiedTextChanged;
    private async Task InitializeEditorAsync()
    {
        if (_monacoController == null) return;
        await _monacoController.CreateEditorAsync(Text, Language);
        await _monacoController.SetThemeAsync(ApplicationThemeManager.GetAppTheme());
        _monacoController.ListenToContentChange();
        _monacoController.TextChanged += (s, e) => Text = e.NewText;
        Subject.OnNext(() => Language = DetectLanguage(Text));

    }

    private async Task InitialzeDiffEditorAsync()
    {
        if (_monacoController == null) return;
        await _monacoController.CreateDiffEditorAsync(OriginalText, ModifiedText, Language);
        await _monacoController.SetThemeAsync(ApplicationThemeManager.GetAppTheme());
        _monacoController.ListenToContentChange();
        _monacoController.OriginalTextChanged += (s, e) => OriginalText = e.NewText;
        _monacoController.ModifiedTextChanged += (s, e) => ModifiedText = e.NewText;
        Subject.OnNext(() => Language = DetectLanguage(OriginalText));
    }


    private static DispatcherOperation<TResult> DispatchAsync<TResult>(Func<TResult> callback)
    {
        return Application.Current.Dispatcher.InvokeAsync(callback);
    }


    private void MonitorProperty()
    {
        this.ObserveChange()
    .Subscribe(e => Subject.OnNext(() =>
    {
        switch (e.PropertyName)
        {
            case nameof(Language):
                _monacoController?.SetLanguageAsync(Language);
                break;
            case nameof(WordWrap):
                _monacoController?.SetWordWrap(WordWrap);
                break;
            //case nameof(Text):
            //    _monacoController?.SetTextAsync(Text);
            //    break;
            //case nameof(OriginalText):
            //    _monacoController?.SetOriginalTextAsync(OriginalText);
            //    break;
            //case nameof(ModifiedText):
            //    _monacoController?.SetModifiedTextAsync(ModifiedText);
            //    break;
            default:
                break;
        }
    }));
    }

    public static MonacoLanguage DetectLanguage(string input)
    {
        // 去除所有类型的注释
        input = RemoveComments(input);

        // 定义每种语言及其对应的正则表达式
        var patterns = new Dictionary<MonacoLanguage, string>
        {
            { MonacoLanguage.Json, @"^\{.*\}$|^\[.*\]$" },
            { MonacoLanguage.Csharp, @"\b(namespace|using|public|private|void|static)\b" },
            { MonacoLanguage.JavaScript, @"\b(function|var|let|const|if|else|return)\b" },
            { MonacoLanguage.Python, @"\b(def|import|print|class|if|else|elif|return)\b" }
        };

        // 使用 foreach 遍历每种语言的检测规则
        foreach (var pattern in patterns)
        {
            if (Regex.IsMatch(input, pattern.Value))
            {
                return pattern.Key;  // 返回匹配到的语言类型
            }
        }

        // 如果没有匹配到任何语言，返回 None
        return MonacoLanguage.None;
    }

    // 去除代码中的注释
    static string RemoveComments(string input)
    {
        // 去除 C# 和 JavaScript 的单行注释 (//)
        input = Regex.Replace(input, @"//.*$", "", RegexOptions.Multiline);

        // 去除 C# 和 JavaScript 的多行注释 (/* */)
        input = Regex.Replace(input, @"/\*.*?\*/", "", RegexOptions.Singleline);

        // 去除 Python 和其他语言的单行注释 (#)
        input = Regex.Replace(input, @"#.*$", "", RegexOptions.Multiline);

        return input;
    }
}
