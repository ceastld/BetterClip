using BetterClip.Core.Config;
using BetterClip.Extension;
using BetterClip.Helpers;
using BetterClip.Service.Interface;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wpf.Ui.Appearance;

namespace BetterClip.Service;

public class ConfigService
{
    private static readonly string _configPath = Global.Absolute("manifest.json");
    private static ILogger _logger = App.GetLogger<Global>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public ConfigService()
    {
        GlobalConfig.ObserveChange().Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(c =>
            {
                _logger.LogInformation("Global Config Changed, Property: {0}", c.PropertyName);
                CommonHelper.SaveObjectToJsonFile(_configPath, GlobalConfig, JsonOptions);
            });
        GlobalConfig.ObserveChange()
            .Where(c => c.PropertyName == nameof(GlobalConfig.ApplicationTheme))
            .Subscribe(c => ApplyTheme());
        ApplicationThemeManager.Changed += (theme, color) => { GlobalConfig.ApplicationTheme = theme; };

    }
    public void ApplyTheme() => ApplicationThemeManager.Apply(GlobalConfig.ApplicationTheme);

    public GlobalConfig GlobalConfig { get; } = CommonHelper.ObjectFromJsonFile<GlobalConfig>(_configPath, JsonOptions);
    public void UpdateUserDataPath(string path)
    {
        GlobalConfig.UserDataPath = Global.Relative(path);
    }
    public static string CreateSubFolder(string parentDir, params string[] subFolderName)
    {
        var subFolderPath = Path.Combine([parentDir, .. subFolderName]);
        Directory.CreateDirectory(subFolderPath);
        return subFolderPath;
    }

    public string CreateSubDataFolder(params string[] subFolderName)
    {
        return CreateSubFolder(UserDataPath(), subFolderName);
    }
    public string UserDataPath(params string[] relativePath)
    {
        var userdataPath = GlobalConfig.UserDataPath.TrimEnd('\\', '/');
        _logger.LogInformation("UserDataPath: {0}", userdataPath);
        if (!Path.IsPathRooted(userdataPath))
        {
            userdataPath = Global.Absolute(userdataPath);
        }
        if (Path.GetFileName(userdataPath) != "UserData") // 最后一级目录不是 UserData
        {
            userdataPath = Path.Combine(userdataPath, "UserData");
        }
        Directory.CreateDirectory(userdataPath);
        return Path.Combine([userdataPath, .. relativePath]);
    }
}
