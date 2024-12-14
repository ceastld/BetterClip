using System.Text.Json.Serialization;
using System.Text.Json;

namespace BetterClip.Model.ClipData;

public abstract class CommonItem
{
    public string Id { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ItemType ItemType => this.GetItemType();
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

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemType
{
    Text,
    File,
    Image
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
        
        ItemType itemType = Enum.Parse<ItemType>(itemTypeProperty.GetString()!, true);
        return itemType switch
        {
            ItemType.Text => JsonSerializer.Deserialize<TextItem>(root.GetRawText(), options)!,
            ItemType.File => JsonSerializer.Deserialize<MultiFileItem>(root.GetRawText(), options)!,
            ItemType.Image => JsonSerializer.Deserialize<ImageItem>(root.GetRawText(), options)!,
            _ => throw new JsonException("Unknown ItemType")
        };
    }

    public override void Write(Utf8JsonWriter writer, CommonItem value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
