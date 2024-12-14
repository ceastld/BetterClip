using DynamicData;
using System.Reactive.Linq;

namespace ConsoleApp1;

public abstract class FileSourceCache<TObject, TKey> : IDisposable where TObject : notnull where TKey : notnull
{
    public SourceCache<TObject, TKey> SourceCache => _cache;
    protected readonly string _dir;
    protected readonly string _filter;
    protected readonly FileSystemWatcher _fileWatcher;

    protected readonly SourceCache<TObject, TKey> _cache;
    private readonly Dictionary<string, DateTime> _fileUpdateTime = [];
    protected FileSourceCache(string dir, string filter, int delayms = 200)
    {
        _cache = new(Object2Key);
        var objects = new List<TObject>();
        foreach (var file in Directory.GetFiles(dir, filter))
        {
            objects.Add(File2Object(file));
        }
        _cache.AddOrUpdate(objects);

        _dir = dir;
        _filter = filter;
        _fileWatcher = new FileSystemWatcher(dir, filter)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        var watcher = new ObservableFileSystemWatcher(_fileWatcher);

        watcher.Created
            .Merge(watcher.Deleted)
            .Merge(watcher.Changed)
            .Merge(watcher.Renamed)
            .Delay(TimeSpan.FromMilliseconds(delayms))
            .Subscribe(change => UpdateObject(change.FullPath));

        watcher.Renamed
            .Delay(TimeSpan.FromMilliseconds(delayms))
            .Subscribe(change => UpdateObject(change.OldFullPath));

        watcher.Start();

        var changer = _cache.Connect().Skip(objects.Count > 0 ? 1 : 0);

        changer.WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
               .ForEachChange(change => AddOrUpdateFile(change.Current))
               .Subscribe();

        changer.WhereReasonsAre(ChangeReason.Remove)
               .ForEachChange(change => RemoveFile(change.Current))
               .Subscribe();
    }
    protected abstract TKey Object2Key(TObject obj);
    private string Object2File(TObject obj) => KeyToFile(Object2Key(obj));
    protected abstract TObject File2Object(string file);
    protected abstract string KeyToFile(TKey key);
    protected abstract TKey File2Key(string file);

    protected abstract void SaveObject2File(TObject obj, string file);
    private void UpdateObject(string file)
    {
        if (File.Exists(file))
        {
            if (!CheckSync(file, out var update_time))
            {
                _fileUpdateTime[file] = update_time;
                _cache.AddOrUpdate(File2Object(file));
            }
        }
        else
        {
            _cache.Remove(File2Key(file));
        }
    }
    private bool CheckSync(string file, out DateTime new_time)
    {
        new_time = new FileInfo(file).LastWriteTime;
        return _fileUpdateTime.TryGetValue(file, out var last_time) && last_time == new_time;
    }
    private void AddOrUpdateFile(TObject obj)
    {
        var file = Object2File(obj);
        if (!CheckSync(file, out _))
        {
            SaveObject2File(obj, file);
            _fileUpdateTime[file] = new FileInfo(file).LastWriteTime;
        }
    }
    private void RemoveFile(TObject obj)
    {
        var file = Object2File(obj);
        _fileUpdateTime.Remove(file);
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
        _cache?.Dispose();
    }
}
