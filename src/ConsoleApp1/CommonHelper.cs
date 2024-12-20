using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using DynamicData.Binding;
using ReactiveUI;

namespace ConsoleApp1;

public static class CommonHelper
{
    public static string GetMD5(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);
        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
    }

    public static string GenerateRandomString()
    {
        var random = new Random();
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                char ch = (char)random.Next(48, 123);
                while (!char.IsLetterOrDigit(ch))
                {
                    ch = (char)random.Next(48, 123);
                }
                stringBuilder.Append(ch);
            }
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public static IObservable<FileSystemEventArgs> CreateFileWatcherStream(this FileSystemWatcher _fileWatcher, FileSystemEventType eventTypes)
    {
        var streams = new List<IObservable<FileSystemEventArgs>>();

        // 检查是否包含 Create 事件
        if (eventTypes.HasFlag(FileSystemEventType.Create))
        {
            streams.Add(Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    handler => _fileWatcher.Created += handler,
                    handler => _fileWatcher.Created -= handler)
                .Select(e => e.EventArgs));
        }

        // 检查是否包含 Delete 事件
        if (eventTypes.HasFlag(FileSystemEventType.Delete))
        {
            streams.Add(Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    handler => _fileWatcher.Deleted += handler,
                    handler => _fileWatcher.Deleted -= handler)
                .Select(e => e.EventArgs));
        }

        // 检查是否包含 Rename 事件
        if (eventTypes.HasFlag(FileSystemEventType.Rename))
        {
            streams.Add(Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                    handler => _fileWatcher.Renamed += handler,
                    handler => _fileWatcher.Renamed -= handler)
                .Select(e => (FileSystemEventArgs)e.EventArgs));
        }

        // 检查是否包含 Change 事件
        if (eventTypes.HasFlag(FileSystemEventType.Change))
        {
            streams.Add(Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                    handler => _fileWatcher.Changed += handler,
                    handler => _fileWatcher.Changed -= handler)
                .Select(e => e.EventArgs));
        }

        // 使用 Merge 合并所有符合条件的事件流
        return streams.Aggregate((current, next) => current.Merge(next));
    }
}

[Flags]
public enum FileSystemEventType
{
    None = 0,
    Create = 1,
    Delete = 2,
    Rename = 4,
    Change = 8,
    All = 15,
}


public static class HkHGNJ61EIMz
{
    public static void tlCuwtGx4GWU()
    {
        // 使用 Connect 将 listA 和 listB 双向绑定
        //.Do(change => Console.WriteLine($"Change detected: {change}"))
        var obj = new
        {
            listA = new ObservableCollectionExtended<int>(),
            listB = new ObservableCollectionExtended<int>()
        };
        obj.listA.ObserveCollectionChanges()
            .BindTo(obj, x => x.listB);
        //listB.ObserveCollectionChanges().BindTo(listA, x => x);
        // 添加测试数据
        obj.listA.AddRange([1, 2, 3]);

        // 打印 listB 的内容以验证同步
        Console.WriteLine("ListB contents:");
        foreach (var item in obj.listB)
        {
            Console.WriteLine(item);
        }

        //修改 listB 的内容以验证双向同步
        obj.listB.Add(4);
        obj.listB.Remove(1);

        // 打印 listA 的内容以验证同步
        Console.WriteLine("ListA contents:");
        foreach (var item in obj.listA)
        {
            Console.WriteLine(item);
        }
    }
}