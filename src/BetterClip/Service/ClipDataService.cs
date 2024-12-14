using System.IO;
using BetterClip.Helpers;
using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using Clipboard = System.Windows.Forms.Clipboard;
using static BetterClip.Model.ClipData.CommonItemHelper;
using DynamicData;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;
using BetterClip.Core.Config;

namespace BetterClip.Service;

public class ClipDataService : IClipDataService, IDisposable
{
    private readonly string DataFolder;

    private readonly string ItemsFolder;
    private string OrderFile => Path.Combine(DataFolder, "order.txt");

    private readonly ILogger _logger = App.GetLogger<ClipDataService>();
    public IObservableCache<CommonItem, string> All { get; }

    private readonly Subject<Action> _saveOrderSubject = new();

    private readonly CommonItemCache _cache;
    public IObservable<CommonItem> ClipboardChanged { get; }

    public ClipDataService(IClipboardService clipboardService)
    {
        DataFolder = Global.CreateSubDataFolder("Clip");
        ItemsFolder = Global.CreateSubDataFolder("Clip","Items");

        _cache = new CommonItemCache(ItemsFolder);

        All = _cache.SourceCache.AsObservableCache();

        _logger.LogInformation($"SourceCache Count {_cache.SourceCache.Count}");
        _logger.LogInformation($"All Count {All.Count}");

        var saveorder = _saveOrderSubject.Throttle(TimeSpan.FromMilliseconds(200))
                                         .Subscribe(action => action.Invoke());

        var PublishSubject = new Subject<CommonItem>();
        ClipboardChanged = PublishSubject.Publish();

        var subclipchange = clipboardService.Changed.Subscribe(e =>
        {
            var item = CreateFromClipboard();
            if (item != null)
            {
                Save(item);
                PublishSubject.OnNext(item);
            }
        });
    }

    public void Save(CommonItem item) => _cache.SourceCache.AddOrUpdate(item);
    public void Save(IEnumerable<CommonItem> items) => _cache.SourceCache.AddOrUpdate(items);
    public void Remove(CommonItem item) => _cache.SourceCache.Remove(item);
    public void Remove(IEnumerable<CommonItem> items) => _cache.SourceCache.Remove(items);

    public CommonItem? CreateFromClipboard()
    {
        if (Clipboard.ContainsImage())
        {
            var image = Clipboard.GetImage();
            if (image is null) return null;
            var name = Guid.NewGuid().ToBase62() + ".jpg";
            image.Save(Path.Combine(ItemsFolder, name));
            return CreateImage(name);
        }
        else if (Clipboard.ContainsText())
        {
            return CreateText(Clipboard.GetText());
        }
        else if (Clipboard.ContainsFileDropList())
        {
            return CreateMultiFile(Clipboard.GetFileDropList().Cast<string>().ToList());
        }
        else
        {
            return CreateText("Unknown data type");
        }
    }

    public string GetImagePath(string path)
    {
        if (File.Exists(path)) return path;
        return Path.Combine(ItemsFolder, path);
    }

    public void Dispose()
    {
        _cache.Dispose();
        GC.SuppressFinalize(this);
    }

    public void SaveOrder()
    {
        _logger.LogInformation("SaveOrder");
        _saveOrderSubject.OnNext(() => File.WriteAllLines(OrderFile, All.Items.Select(x => x.Id)));
    }
}
