using System;
using System.IO;
using System.Text.Json;
using System.Windows.Threading;

namespace BetterClip.Helpers;

public static class CommonHelper
{
    public static TObject ObjectFromJsonFile<TObject>(string path, Func<TObject> defaultFunc, JsonSerializerOptions? options = null)
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
        return ret ?? defaultFunc.Invoke();
    }
}
