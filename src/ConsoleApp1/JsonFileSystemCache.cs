namespace ConsoleApp1;

public class JsonFileSystemCache(string dir) : FileSourceCache<FileContent, string>(dir, "*.json")
{
    protected override string File2Key(string file) => Path.GetFileNameWithoutExtension(file);
    protected override FileContent File2Object(string file) => new(file);
    protected override string KeyToFile(string key) => Path.Combine(_dir, key + ".json");
    protected override string Object2Key(FileContent obj) => obj.Id;
    protected override void SaveObject2File(FileContent obj, string file) => File.WriteAllText(file, obj.Text);
}
