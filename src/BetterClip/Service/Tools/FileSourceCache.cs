using DynamicData;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using ReactiveUI;
using System.Reactive.Subjects;

namespace BetterClip.Service;

public abstract class FileSourceCache<TObject, TKey> : IDisposable where TObject : notnull where TKey : notnull
{
    public SourceCache<TObject, TKey> SourceCache => _cache;
    protected readonly SourceCache<TObject, TKey> _cache;
    protected readonly FileSystemWatcher _fileWatcher;
    protected readonly string _dir;
    protected readonly string _filter;
    private readonly ConcurrentDictionary<string, DateTime> _lastFileUpdateTime = new();
    private readonly ConcurrentDictionary<TKey, DateTime> _lastObjectUpdateTime = new();
    protected FileSourceCache(string dir, string filter, int delayms = 200)
    {
        _cache = new(Object2Key);
        var objects = new List<TObject>();
        foreach (var file in Directory.GetFiles(dir, filter))
        {
            var obj = File2Object(file);
            if (obj != null)
            {
                objects.Add(obj);
                _lastObjectUpdateTime[Object2Key(obj)] = Object2UpdateTime(obj);
            }
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
            .ObserveOnDispatcher()
            .Subscribe(change => UpdateFile2Object(change.FullPath));

        watcher.Renamed
            .Delay(TimeSpan.FromMilliseconds(delayms))
            .ObserveOnDispatcher()
            .Subscribe(change => UpdateFile2Object(change.OldFullPath));

        watcher.Start();

        var changer = _cache.Connect().Skip(objects.Count > 0 ? 1 : 0);

        changer.WhereReasonsAre(ChangeReason.Add, ChangeReason.Update, ChangeReason.Remove)
               .ForEachChange(change => UpdateObject2File(change.Current))
               .Subscribe();

        //changer.WhereReasonsAre(ChangeReason.Remove).ForEachChange(change => RemoveFile(change.Current)).Subscribe();
    }
    protected abstract TKey Object2Key(TObject obj);
    private string Object2File(TObject obj) => KeyToFile(Object2Key(obj));
    protected abstract TObject? File2Object(string file);
    protected abstract string KeyToFile(TKey key);
    protected abstract TKey File2Key(string file);
    protected abstract void SaveObject2File(TObject obj, string file);
    protected abstract DateTime Object2UpdateTime(TObject obj);
    public Subject<TObject> UpdateFromFile { get; } = new();
    private void UpdateFile2Object(string file)
    {
        if (File.Exists(file)) // add or update
        {
            if (!CheckFileSync(file, out var update_time))
            {
                _lastFileUpdateTime[file] = update_time;
                var obj = File2Object(file);
                if (obj != null)
                {
                    _cache.AddOrUpdate(obj);
                    _lastObjectUpdateTime[Object2Key(obj)] = Object2UpdateTime(obj);
                    UpdateFromFile.OnNext(obj);
                }
            }
        }
        else // delete
        {
            var key = File2Key(file);
            _cache.Remove(key);
            _lastObjectUpdateTime.TryRemove(key, out _);
        }
    }
    private bool CheckFileSync(string file, out DateTime new_time)
    {
        new_time = new FileInfo(file).LastWriteTime;
        return _lastFileUpdateTime.TryGetValue(file, out var last_time) && last_time == new_time;
    }

    private void UpdateObject2File(TObject obj)
    {
        if (_cache.Lookup(Object2Key(obj)).HasValue) // add or update 
        {
            if (_lastObjectUpdateTime.TryGetValue(Object2Key(obj), out var last_time)
                && last_time == Object2UpdateTime(obj)) { return; }

            _lastObjectUpdateTime[Object2Key(obj)] = Object2UpdateTime(obj);

            { // write file
                var file = Object2File(obj);
                SaveObject2File(obj, file);
                _lastFileUpdateTime[file] = new FileInfo(file).LastWriteTime;
            }
        }
        else // delete
        {
            var file = Object2File(obj);
            _lastFileUpdateTime.TryRemove(file, out _);
            _lastObjectUpdateTime.TryRemove(Object2Key(obj), out _);
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    public virtual void Dispose()
    {
        _fileWatcher?.Dispose();
        _cache?.Dispose();
    }
}
