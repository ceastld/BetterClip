using DynamicData;

namespace ConsoleApp1;

public class JsonFileSystemCache(string dir) : FileSourceCache<FileContent, string>(dir, "*.json")
{
    protected override string File2Key(string file) => Path.GetFileNameWithoutExtension(file);
    protected override FileContent File2Object(string file) => new(file);
    protected override string KeyToFile(string key) => Path.Combine(_dir, key + ".json");
    protected override string Object2Key(FileContent obj) => obj.Id;
    protected override void SaveObject2File(FileContent obj, string file) => File.WriteAllText(file, obj.Text);
}

public class JsonFIleSystemCacheTest
{
    public static void Test()
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