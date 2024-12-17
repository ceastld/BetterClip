using System.IO;
using System.Reflection;
using Wpf.Ui.Appearance;

namespace BetterClip.Core.Config;

public partial class GlobalConfig : ObservableObject
{
    [ObservableProperty]
    private string _userDataPath = "";

    [ObservableProperty]
    private ApplicationTheme _ApplicationTheme;
}

public class Global
{
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
    public static string StartUpPath { get; set; } = AppContext.BaseDirectory;
    public static string AppPath { get; } = Assembly.GetExecutingAssembly().Location;
    public static string LogFolder { get; } = CreateSubFolder("log");
    public static string LogFile { get; } = Path.Combine(LogFolder, "better-clip.log");
    public static IList<string> GetLogFiles()
    {
        return Directory.GetFiles(LogFolder, "better-clip*.log").OrderByDescending(f => f).ToList();
    }
    public static string CreateSubFolder(params string[] subFolderName)
    {
        return Directory.CreateDirectory(Absolute(subFolderName)).FullName;
    }

    public static string Absolute(params string[] relativePath)
    {
        var path = Path.Combine(relativePath);
        if (Path.IsPathRooted(path))
            return path;
        return Path.GetFullPath(path, StartUpPath);
    }
    public static string Relative(string path)
    {
        if (!Path.IsPathRooted(path) || !path.StartsWith(StartUpPath))
            return path;
        if (path.StartsWith(".."))
            return Path.GetFullPath(path, StartUpPath);
        return Path.GetRelativePath(StartUpPath, path);
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
}
