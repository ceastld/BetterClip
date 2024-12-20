// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using BetterClip.Model.Monaco;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui.Appearance;

namespace BetterClip.Controllers;

public class MonacoController(WebView2 webView)
{
    private readonly WebView2 _webView = webView;

    public class TextChangedEventArgs(string newText) : EventArgs
    {
        public string NewText { get; } = newText;
    }

    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);


    /// <summary>
    /// Occurs when the text in the editor changes.
    /// Needs to run <see cref="ListenToContentChange"/> first.
    /// </summary>
    public event TextChangedEventHandler? TextChanged;
    public event TextChangedEventHandler? OriginalTextChanged;
    public event TextChangedEventHandler? ModifiedTextChanged;

    public async Task CreateEditorAsync(string content, MonacoLanguage language = MonacoLanguage.None)
    {
        await Call("createEditor", content, GetLanguageId(language));
    }
    public async Task CreateDiffEditorAsync(string originalText, string modifiedText, MonacoLanguage language = MonacoLanguage.None)
    {
        await Call("createDiffEditor", originalText, modifiedText, GetLanguageId(language));
    }
    public async Task SetWordWrap(bool wordWrap) => await Call("setWordWrap", wordWrap);
    public async Task SetThemeAsync(ApplicationTheme appApplicationTheme) => await Call("setTheme", appApplicationTheme);
    private static string GetLanguageId(MonacoLanguage monacoLanguage)
    {
        return monacoLanguage switch
        {
            MonacoLanguage.None => "",
            MonacoLanguage.ObjectiveC => "objective-c",
            _ => monacoLanguage.ToString().ToLower()
        };
    }
    public async Task SetLanguageAsync(MonacoLanguage monacoLanguage) => await Call("setLanguage", GetLanguageId(monacoLanguage));
    public async Task SetTextAsync(string content) => await Call("setText", content);
    public async Task SetOriginalTextAsync(string text) => await Call("setOriginalText", text);
    public async Task SetModifiedTextAsync(string text) => await Call("setModifiedText", text);
    public async Task<string> GetTextAsync() => JsonSerializer.Deserialize<string>(await Call("getText")) ?? "";
    public async Task<string> GetOriginalTextAsync() => JsonSerializer.Deserialize<string>(await Call("getOriginalText")) ?? "";
    public async Task<string> GetModifiedTextAsync() => JsonSerializer.Deserialize<string>(await Call("getModifiedText")) ?? "";

    private async Task<string> ExecuteScriptAsync(string script, bool onDispatcher = false)
    {
        string? res;
        if (onDispatcher)
        {
            res = await await Application.Current.Dispatcher.InvokeAsync(async () => await _webView.ExecuteScriptAsync(script));
        }
        res = await _webView.ExecuteScriptAsync(script);
        return res;
    }

    public void ListenToContentChange()
    {
        _webView.CoreWebView2.WebMessageReceived += (sender, e) =>
        {
            string messageJson = e.WebMessageAsJson;
            MessagePackage? message = JsonSerializer.Deserialize<MessagePackage>(messageJson);
            if (message == null)
            {
                return;
            }
            switch (message.Type)
            {
                case MessageTypes.TextChanged:
                    TextChanged?.Invoke(this, new TextChangedEventArgs(message.Data!));
                    break;
                case MessageTypes.OriginalTextChanged:
                    OriginalTextChanged?.Invoke(this, new TextChangedEventArgs(message.Data!));
                    break;
                case MessageTypes.ModifiedTextChanged:
                    ModifiedTextChanged?.Invoke(this, new TextChangedEventArgs(message.Data!));
                    break;
                default:
                    break;
            }
            _logger.LogInformation("Message received: {0}", messageJson);
        };
    }

    private ILogger _logger = App.GetLogger<MonacoController>();
    public enum MessageTypes
    {
        TextChanged = 0,
        OriginalTextChanged = 1,
        ModifiedTextChanged = 2
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public class MessagePackage
    {
        public MessageTypes Type { get; set; }
        public string? Data { get; set; }
    }

    public async Task<string> Call(string name, params object[] objects)
    {
        var @params = string.Join(',', objects.Select(o => JsonSerializer.Serialize(o, JsonOptions)));
        //name = name[0].ToString().ToLower() + name[1..].TrimEnd();
        return await ExecuteScriptAsync($"editor.{name}({@params})");
    }
}
