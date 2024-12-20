using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using SHDocVw;
using Windows.Win32;


namespace BetterClip.Service.Explorer;

public class ExplorerChangedEventArgs(ExplorerChangedTypes reason, IntPtr hwnd, ExplorerRecord record) : EventArgs
{
    public ExplorerChangedTypes Reason { get; } = reason;
    public IntPtr HWND { get; } = hwnd;
    public ExplorerRecord Record { get; } = record;
}

public enum ExplorerChangedTypes
{
    Init,
    Create,
    Update,
    Remove,
}

public delegate void ExplorerChangedEventHandler(object sender, ExplorerChangedEventArgs e);

public partial class ExplorerWindowManager : IDisposable
{
    private readonly ILogger _logger = App.GetLogger<ExplorerWindowManager>();

    private readonly ShellWindows _shellWindows;
    private readonly SourceCache<ExplorerWindowRecord, IntPtr> cache;
    private readonly IList<ExplorerWindowRecord> windows = [];
    public event ExplorerChangedEventHandler? ExplorerChanged;

    public ExplorerWindowManager()
    {
        _shellWindows = new();
        _shellWindows.WindowRegistered += ShellWindows_WindowRegistered;
        _shellWindows.WindowRevoked += ShellWindows_WindowRevoked;
        cache = new(i => i.HWND);
        cache.Connect()
             .SortAndBind(windows, SortExpressionComparer<ExplorerWindowRecord>.Descending(w => w.LastUseTime))
             .Subscribe();

        foreach (var ie in GetAllExplorer())
        {
            Register(ie);
        }
    }

    public List<string> GetAllOpendPath()
    {
        List<string> paths = [];
        PInvoke.EnumWindows((hwnd, lParam) =>
        {
            var lookup = cache.Lookup(hwnd);
            if (lookup.HasValue)
            {
                paths.Add(GetLocalPath(lookup.Value.Window));
            }
            return true;
        }, IntPtr.Zero);
        return paths;
    }

    /// <summary>
    /// 已经获取不到ie对象了，所以啥也不用干
    /// </summary>
    /// <param name="lCookie">窗口句柄</param>
    private void ShellWindows_WindowRevoked(int lCookie) { }

    private void Register(InternetExplorer ie)
    {
        var hwnd = new IntPtr(ie.HWND);

        var path = GetLocalPath(ie);
        cache.AddOrUpdate(new ExplorerWindowRecord(hwnd, ie));
        ExplorerChanged?.Invoke(this, new(ExplorerChangedTypes.Create, hwnd, new ExplorerRecord(path)));
        _logger.LogInformation("Explorer opened {Path}", path);

        ie.NavigateComplete2 += delegate
        {
            var path = GetLocalPath(ie);
            ExplorerChanged?.Invoke(this, new(ExplorerChangedTypes.Update, hwnd, new ExplorerRecord(path)));
            _logger.LogInformation("Explorer navigate {Path}", path);
        };

        ie.OnQuit += delegate
        {
            var path = GetLocalPath(ie);
            ExplorerChanged?.Invoke(this, new(ExplorerChangedTypes.Remove, hwnd, new ExplorerRecord(path)));
            _logger.LogInformation("Explorer closed {Path}", path);
        };
    }

    private void ShellWindows_WindowRegistered(int lCookie)
    {
        foreach (var ie in GetAllExplorer())
        {
            var hwnd = new IntPtr(ie.HWND);
            if (!cache.Lookup(hwnd).HasValue)
            {
                Register(ie);
            }
        }
    }

    private bool IsExplorer(InternetExplorer ie) => Path.GetFileNameWithoutExtension(ie.FullName).ToLower().Equals("explorer");

    private IEnumerable<InternetExplorer> GetAllExplorer() => _shellWindows.OfType<InternetExplorer>().Where(IsExplorer);

    /// <summary>
    /// 获取本地路径
    /// 原始格式，没有 file/// 前缀，并且没有url编码
    /// </summary>
    /// <param name="ie"></param>
    /// <returns></returns>
    private string GetLocalPath(InternetExplorer ie)
    {
        var url = ie.LocationURL;
        if (string.IsNullOrEmpty(url))
        {
            switch (ie.LocationName)
            {
                case "此电脑":
                    return "shell:::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                case "主文件夹":
                    return "shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}";
                default:
                    _logger.LogInformation("获取资源管理器路径失败");
                    return string.Empty;
            }
        }
        _logger.LogInformation("Location url:{url}, name{name}", url, ie.LocationName);
        try
        {
            return new Uri(ie.LocationURL).LocalPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取资源管理器路径失败");
            return string.Empty;
        }
    }

    public void Dispose()
    {
        _shellWindows.WindowRegistered -= ShellWindows_WindowRegistered;
        _shellWindows.WindowRevoked -= ShellWindows_WindowRevoked;
        GC.SuppressFinalize(this);
    }
}
