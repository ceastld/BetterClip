using System.Reactive.Linq;
using BetterClip.Core.Config;
using DynamicData;
using DynamicData.Binding;

namespace BetterClip.Service.Explorer;

public partial class ExplorerService : IExplorerService, IDisposable
{
    private readonly ExplorerWindowManager manager;
    private readonly ExplorerRecoredCache cache;
    private readonly IDisposable monitor;
    private readonly ReadOnlyObservableCollection<ExplorerRecord> records;
    private readonly string ItemsFolder;
    public ExplorerService(ConfigService configService)
    {
        manager = new();
        ItemsFolder = Global.CreateLocalFolder("Explorer");
        cache = new(ItemsFolder);
        var source = cache.SourceCache;
        monitor = ExplorerChanged
            .Where(e => _isMonitorOn)
            .Where(e => e.Reason == ExplorerChangedTypes.Remove)
            .Subscribe(e => source.AddOrUpdate(e.Record));
        source.Connect()
            .ObserveOnDispatcher()
            .SortAndBind(out records, SortExpressionComparer<ExplorerRecord>.Descending(x => x.LastUseTime))
            .Subscribe();
    }
    private IObservable<ExplorerChangedEventArgs> ExplorerChanged =>
        Observable.FromEventPattern<ExplorerChangedEventHandler, ExplorerChangedEventArgs>(
        h => manager.ExplorerChanged += h,
        h => manager.ExplorerChanged -= h)
        .Select(e => e.EventArgs);

    public void Dispose()
    {
        monitor.Dispose();
        GC.SuppressFinalize(this);
    }

    public IEnumerable<string> GetRecords()
    {
        return records.Select(x => x.Path)
            .Where(x => x.StartsWith("shell") || Directory.Exists(x));
    }

    private bool _isMonitorOn;
    public void MonitorOff() => _isMonitorOn = false;
    public void MonitorOn() => _isMonitorOn = true;
}
