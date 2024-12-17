using System.IO;
using System.Reactive.Linq;
using BetterClip.Helpers;
using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using BetterClip.Service.Tools;
using BetterClip.ViewModel.Common;
using DynamicData;
using Microsoft.Extensions.Logging;
using static BetterClip.Model.ClipData.CommonItemHelper;

namespace BetterClip.Service;

public abstract class CommonDataService : ICommonDataService, IDisposable
{
    public string DataFolder { get; }

    public string ItemsFolder { get; }
    protected readonly CommonItemCache _cache;
    public SourceCache<CommonItem, string> Source => _cache.SourceCache;
    public IObservableCache<CommonItem, string> All { get; }

    public abstract IObservableCache<CommonItemViewModel, string> ViewModels { get; }

    private readonly ILogger _logger = App.GetLogger<CommonDataService>();
    public CommonItem? GetItem(string key)
    {
        try { return _cache.SourceCache.Lookup(key).Value; }
        catch { return default; }
    }

    protected CommonDataService(string dir, ConfigService config)
    {
        DataFolder = config.CreateSubDataFolder(dir);
        ItemsFolder = config.CreateSubDataFolder(dir, "Items");
        _cache = new CommonItemCache(ItemsFolder);
        All = _cache.SourceCache.AsObservableCache();
        _logger.LogInformation($"SourceCache Count {_cache.SourceCache.Count}");
        _logger.LogInformation($"All Count {All.Count}");
    }

    public virtual void Save(CommonItem item) => _cache.SourceCache.AddOrUpdate(item);
    public virtual void Save(IEnumerable<CommonItem> items) => _cache.SourceCache.AddOrUpdate(items);
    public virtual void Remove(CommonItem item) => _cache.SourceCache.Remove(item);
    public virtual void Remove(IEnumerable<CommonItem> items) => _cache.SourceCache.Remove(items);
    public void Dispose()
    {
        _cache.Dispose();
        GC.SuppressFinalize(this);
    }

    public IEnumerable<CommonItemViewModel> SelectViewModels(IEnumerable<CommonItem> items)
    {
        return items.Select(x => ViewModels.Lookup(x.Id))
            .Where(x => x.HasValue)
            .Select(x => x.Value);
    }

    public IEnumerable<CommonItemViewModel> SelectViewModels(IEnumerable<string> items)
    {
        return items.Select(ViewModels.Lookup).Where(x => x.HasValue).Select(x => x.Value);
    }

    public CommonItem? CreateFromClipboard()
    {
        ClipboardHelper.CanUseClipboardHistory();
        ClipboardHelper.CanIncludeInClipboardHistory();

        var image = ClipboardHelper.GetImage();
        if (image != null)
        {
            var name = Guid.NewGuid().ToBase62() + ".jpg";
            image.Save(Path.Combine(ItemsFolder, name));
            return CreateImage(name);
        }
        var text = ClipboardHelper.GetText();
        if (text != null)
        {
            return CreateText(text);
        }
        var files = ClipboardHelper.GetFileDropList();
        if (files != null)
        {
            return CreateMultiFile(files);
        }
        return CreateText("Unknown data type");
    }

    public string GetImagePath(string path)
    {
        if (File.Exists(path)) return path;
        return Path.Combine(ItemsFolder, path);
    }

    public CommonItemViewModel GetViewModel(string key) => ViewModels.Lookup(key).Value;

    public CommonItemViewModel GetViewModel(CommonItem key) => ViewModels.Lookup(key.Id).Value;
}
