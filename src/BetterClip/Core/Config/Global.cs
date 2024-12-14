using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BetterClip.Helpers;

namespace BetterClip.Core.Config;

public class GlobalConfig
{
    public string UserDataPath { get; set; } = "";
}

public class Global
{
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

    public static string StartUpPath { get; set; } = AppContext.BaseDirectory;

    public static readonly JsonSerializerOptions ManifestJsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static GlobalConfig Config { get; } = CommonHelper.ObjectFromJsonFile<GlobalConfig>(
        Absolute("Config", "manifest.json"), () => new(), ManifestJsonOptions);

    public static string Absolute(params string[] relativePath)
    {
        return Path.Combine([StartUpPath, .. relativePath]);
    }

    public static string CreateSubFolder(string parentDir, params string[] subFolderName)
    {
        var subFolderPath = Path.Combine([parentDir, .. subFolderName]);
        Directory.CreateDirectory(subFolderPath);
        return subFolderPath;
    }

    public static string CreateSubDataFolder(params string[] subFolderName)
    {
        return CreateSubFolder(UserDataPath(), subFolderName);
    }
    public static string UserDataPath(params string[] relativePath)
    {
        var userdataPath = Config.UserDataPath;
        if (string.IsNullOrEmpty(userdataPath) || !Directory.Exists(userdataPath))
        {
            userdataPath = Path.Combine(StartUpPath, "UserData");
            Directory.CreateDirectory(userdataPath);
            Config.UserDataPath = userdataPath;
        }
        return Path.Combine([userdataPath, .. relativePath]);
    }

    public static string ScriptPath()
    {
        return Absolute("User\\JsScript");
    }

    public static string? ReadAllTextIfExist(string relativePath)
    {
        var path = Absolute(relativePath);
        if (File.Exists(path)) return File.ReadAllText(path);
        return null;
    }

    /// <summary>
    ///     新获取到的版本号与当前版本号比较，判断是否为新版本
    /// </summary>
    /// <param name="currentVersion">新获取到的版本</param>
    /// <returns></returns>
    public static bool IsNewVersion(string currentVersion)
    {
        return IsNewVersion(Version, currentVersion);
    }

    /// <summary>
    ///     新获取到的版本号与当前版本号比较，判断是否为新版本
    /// </summary>
    /// <param name="oldVersion">老版本</param>
    /// <param name="currentVersion">新获取到的版本</param>
    /// <returns>是否需要更新</returns>
    public static bool IsNewVersion(string oldVersion, string currentVersion)
    {
        try
        {
            Version oldVersionX = new(oldVersion);
            Version currentVersionX = new(currentVersion);

            if (currentVersionX > oldVersionX)
                // 需要更新
                return true;
        }
        catch
        {
            ///
        }

        // 不需要更新
        return false;
    }

    public static void WriteAllText(string relativePath, string blackListJson)
    {
        var path = Absolute(relativePath);
        File.WriteAllText(path, blackListJson);
    }
}
