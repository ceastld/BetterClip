using System.IO;
using BetterClip.Model.ClipData;


namespace BetterClip.Service;

public class CommonItemCache(string dir, int delayms = 200) : FileSourceCache<CommonItem, string>(dir, "*.json", delayms)
{
    protected override string File2Key(string file) => Path.GetFileNameWithoutExtension(file);

    protected override CommonItem? File2Object(string file)
    {
        try
        {
            return CommonItemHelper.FromJson(File.ReadAllText(file));
        }
        catch
        {
            return default;
        }
    }

    protected override string KeyToFile(string key) => Path.Combine(_dir, key + ".json");

    protected override string Object2Key(CommonItem obj) => obj.Id;

    protected override void SaveObject2File(CommonItem obj, string file) => File.WriteAllText(file, obj.ToJson());
}