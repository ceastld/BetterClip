using System.Diagnostics;
using System.IO;
using System.Text.Json;
using BetterClip.Model.Monaco;
using BetterClip.View.Windows;
using BetterClip.ViewModel.Editor;

namespace BetterClip.Helpers;

public static class CommonHelper
{
    public static void SaveObjectToJsonFile<TObject>(string path, TObject obj, JsonSerializerOptions? options = null)
    {
        var jsonString = JsonSerializer.Serialize(obj, options);
        File.WriteAllText(path, jsonString);
    }
    public static TObject ObjectFromJsonFile<TObject>(string path, JsonSerializerOptions? options = null, Func<TObject>? defaultFunc = null)
        where TObject : class, new()
    {
        TObject? ret = default;
        if (File.Exists(path))
        {
            var jsonString = File.ReadAllText(path);
            try
            {
                ret = JsonSerializer.Deserialize<TObject>(jsonString, options);
            }
            catch { }
        }
        return ret ?? defaultFunc?.Invoke() ?? new TObject();
    }

    public static void OpenFileOrUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static string EditText(string inputText, object? owner = null)
    {
        var vm = new MonacoEditorViewModel
        {
            Text = inputText
        };
        var window = new MonacoWindow(vm)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        if (owner is DependencyObject d && Window.GetWindow(d) is Window ow)
        {
            window.Owner = ow;
        }
        window.ShowDialog();
        return vm.Text;
    }

    public static (string, string) CompareText(string text1, string text2, MonacoLanguage language = MonacoLanguage.None, object? owner = null)
    {
        var vm = new MonacoEditorViewModel
        {
            OriginalText = text1,
            ModifiedText = text2,
            Language = language,
            DiffMode = true,
        };
        var window = new MonacoWindow(vm)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        if (owner is DependencyObject d && Window.GetWindow(d) is Window ow)
        {
            window.Owner = ow;
        }
        window.ShowDialog();
        return (vm.OriginalText, vm.ModifiedText);
    }
}
