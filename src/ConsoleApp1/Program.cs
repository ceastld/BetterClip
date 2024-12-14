using DynamicData;
using System.Reactive.Linq;

namespace ConsoleApp1;

public class Program
{
    public static void Main(string[] args)
    {
        
        // 初始化 ObservableCache
        var folderPath = @"D:\work\dotnet\BetterClip\src\BetterClip\bin\Debug\net8.0-windows\ClipData\Items";
        var cache = new JsonFileSystemCache(folderPath);

        cache.SourceCache.Connect()
            .ForEachChange(change =>
            {
                if (change.Reason == ChangeReason.Add)
                    Console.WriteLine($"File added: {change.Key}");
                else if (change.Reason == ChangeReason.Remove)
                    Console.WriteLine($"File removed: {change.Key}");
            })
            .Subscribe();

        // 保持程序运行以监听文件变化
        Console.WriteLine("Press any key to exit...");
        while (true)
        {
            Console.ReadLine();
            var path = Path.Combine(folderPath, $"{Guid.NewGuid()}.json");
            cache.SourceCache.AddOrUpdate(new FileContent(path, CommonHelper.GenerateRandomString()));
        }
    }
}