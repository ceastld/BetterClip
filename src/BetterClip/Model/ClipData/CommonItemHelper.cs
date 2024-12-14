using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;
using BetterClip.Helpers;
using BetterClip.Service;
using BetterClip.Service.Interface;
using BetterClip.ViewModel.Common;

namespace BetterClip.Model.ClipData;

public static class CommonItemHelper
{
    public static ItemType GetItemType(this CommonItem item)
    {

        return item switch
        {
            TextItem => ItemType.Text,
            MultiFileItem => ItemType.File,
            ImageItem => ItemType.Image,
            _ => throw new ArgumentException("Invalid item type"),
        };
    }

    public static CommonItem CreateItem(ItemType type) => type switch
    {
        ItemType.Text => CreateItem<TextItem>(),
        ItemType.File => CreateItem<MultiFileItem>(),
        ItemType.Image => CreateItem<ImageItem>(),
        _ => throw new ArgumentException("Invalid item type"),
    };

    private static T CreateItem<T>(string? title = null, string? des = null) where T : CommonItem, new() => new()
    {
        Title = title,
        Description = des,
        CreateTime = DateTime.Now,
        UpdateTime = DateTime.Now,
        Id = Guid.NewGuid().ToBase62()
    };

    public static TextItem CreateText(string text, string? title = null, string? des = null)
    {
        var item = CreateItem<TextItem>(title, des);
        item.Text = text;
        return item;
    }

    public static ImageItem CreateImage(string imageUrl, string? title = null, string? des = null)
    {
        var item = CreateItem<ImageItem>(title, des);
        item.Path = imageUrl;
        return item;
    }
    public static MultiFileItem CreateMultiFile(IList<string> files)
    {
        var item = CreateItem<MultiFileItem>();
        item.Files = [.. files];
        return item;
    }

    public static ClipItemViewModel ToViewModel(this CommonItem item) => item switch
    {
        TextItem textItem => new TextItemViewModel(textItem),
        ImageItem imageItem => new ImageItemViewModel(imageItem),
        MultiFileItem fileItem => new MultiFileItemViewModel(fileItem),
        _ => throw new NotImplementedException()
    };

    public static List<CommonItem> GetTestData()
    {
        return [
            CreateImage(@"D:\OneDrive\图片\1724387304107.jpg",title:"一张图片"),
            CreateText("Hello World", title: "Greeting", des: "A simple greeting text"),
            CreateText("Lorem Ipsum", title: "Placeholder", des: "Common placeholder text used in design"),
            CreateText("Quick brown fox", title: "Typing Test", des: "A sentence used to test typewriters"),
            CreateMultiFile(
            [
                new(".cache"),
                new("log"),
                new("BetterClip.exe")
            ])
        ];
    }



    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new CommonItemJsonConverter()
        },
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static string ToJson<T>(T item) => JsonSerializer.Serialize(item, JsonOptions);
    public static string ToJson(this IList<CommonItem> item) => JsonSerializer.Serialize(item, JsonOptions);
    public static string ToJson(this CommonItem item) => JsonSerializer.Serialize(item, JsonOptions);
    public static T? FromJson<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonOptions);
    public static CommonItem? FromJson(string json) => JsonSerializer.Deserialize<CommonItem>(json, JsonOptions);


}


