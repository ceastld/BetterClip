using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using BetterClip.Helpers;

namespace BetterClip.Model.ClipData;

public abstract partial class CommonItem
{
    public string Id { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string ItemType => GetType().Name;
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public virtual bool Filter(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return true;
        }
        return Title?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true ||
               Description?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
    }

    public CommonItem Clone()
    {
        return (CommonItem)MemberwiseClone();
    }
    public void NewId()
    {
        Id = Guid.NewGuid().ToBase62();
    }
}

public class TextItem : CommonItem
{
    public string Text { get; set; } = default!;
    public override bool Filter(string filter)
    {
        return base.Filter(filter)
            || Text?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
    }
}

public class MultiFileItem : CommonItem
{
    public List<string> Files { get; set; } = [];
}

public class ImageItem : CommonItem
{
    public string Path { get; set; } = default!;
}

public partial class FolderItem : CommonItem
{
    public bool IsExpanded { get; set; }
    public List<string> Children { get; set; } = [];
}

public class ClipPageItem : CommonItem
{
    public List<string> ItemFilter { get; set; } = [];
    public string FolderId { get; set; } = default!; // FolderId => 找到 folder, 然后可以显示children
    //太复杂了，还是现把主要的功能实现吧
}

public class CommonItemJsonConverter : JsonConverter<CommonItem>
{
    public override CommonItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("itemType", out var itemTypeProperty)
            && !root.TryGetProperty("ItemType", out itemTypeProperty))
        {
            // Log or handle the missing ItemType property
            throw new JsonException("ItemType is missing.");
        }

        string itemType = itemTypeProperty.GetString()!;
        return itemType switch
        {
            "Text" or "TextItem" => JsonSerializer.Deserialize<TextItem>(root.GetRawText(), options)!,
            "File" or "FileItem" or "MultiFileItem" => JsonSerializer.Deserialize<MultiFileItem>(root.GetRawText(), options)!,
            "Image" or "ImageItem" => JsonSerializer.Deserialize<ImageItem>(root.GetRawText(), options)!,
            "Folder" or "FolderItem" => JsonSerializer.Deserialize<FolderItem>(root.GetRawText(), options)!,
            _ => throw new JsonException("Unknown ItemType")
        };
    }

    public override void Write(Utf8JsonWriter writer, CommonItem value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
