using System.IO;


namespace BetterClip.Service;

public abstract class JsonFileSourceCache<TObject>(string dir, int delayms = 200) : FileSourceCache<TObject, string>(dir, "*json", delayms) where TObject : notnull
{
    protected override string File2Key(string file) => Path.GetFileNameWithoutExtension(file);
    protected override TObject? File2Object(string file)
    {
        try
        {
            return Json2Object(File.ReadAllText(file));
        }
        catch
        {
            return default;
        }
    }
    protected abstract TObject? Json2Object(string json);
    protected override string KeyToFile(string key) => Path.Combine(_dir, key + ".json");
    protected override void SaveObject2File(TObject obj, string file) => File.WriteAllText(file, Object2Json(obj));
    protected abstract string Object2Json(TObject obj);
}
