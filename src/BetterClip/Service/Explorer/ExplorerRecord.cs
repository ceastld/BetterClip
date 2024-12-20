using BetterClip.Helpers;

namespace BetterClip.Service.Explorer;

public class ExplorerRecord
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public ExplorerRecord() { }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。

    public ExplorerRecord(string path)
    {
        Id = path.ToMD5(base62: true);
        Path = path;
        LastUseTime = DateTime.Now;
    }

    public string Id { get; set; }
    public string Path { get; set; }
    public DateTime LastUseTime { get; set; }
}
