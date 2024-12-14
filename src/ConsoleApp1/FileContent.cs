namespace ConsoleApp1;

public class FileContent
{
    public string FullPath { get; }
    public string Id { get; }
    public string Text { get; }

    public FileContent(string filePath)
    {
        FullPath = filePath;
        Id = Path.GetFileNameWithoutExtension(filePath);
        Text = File.ReadAllText(filePath);
    }
    public FileContent(string filePath, string text)
    {
        FullPath = filePath;
        Id = Path.GetFileNameWithoutExtension(filePath);
        Text = text;
    }
}
