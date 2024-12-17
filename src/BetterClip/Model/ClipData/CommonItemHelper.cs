using System.Text.Json;
using System.Text.Json.Serialization;
using BetterClip.Helpers;
using BetterClip.Service.Interface;
using BetterClip.ViewModel.Common;

namespace BetterClip.Model.ClipData;

public static class CommonItemHelper
{
    public static CommonItem CloneInDataService(this CommonItem item, ICommonDataService dataService)
    {
        return CloneToDataService(item, dataService, dataService);
    }
    public static CommonItem CloneToDataService(this CommonItem oldItem, ICommonDataService from, ICommonDataService to)
    {
        var newItem = oldItem.Clone();
        newItem.NewId();
        to.Save(newItem);
        if (from != to)
        {
            if (oldItem is ImageItem oldImageItem && newItem is ImageItem newImageItem)
            {
                var ext = Path.GetExtension(oldImageItem.Path);
                var topath = to.GetImagePath(Guid.NewGuid().ToBase62() + ext);
                if (!File.Exists(topath))
                {
                    File.Copy(from.GetImagePath(oldImageItem.Path),
                        to.GetImagePath(newImageItem.Path));
                }
            }
        }
        return newItem;
    }
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

    public static FolderItem CreateFolder(IEnumerable<string> children, string? title = null, string? des = null)
    {
        var item = CreateItem<FolderItem>(title, des);
        item.Children = [.. children];
        return item;
    }
    public static FolderItemViewModel ToViewModel(this FolderItem item, ICommonDataService dataService) => new(item, dataService);
    public static TextItemViewModel ToViewModel(this TextItem item, ICommonDataService dataService) => new(item, dataService);
    public static ImageItemViewModel ToViewModel(this ImageItem item, ICommonDataService dataService) => new(item, dataService);
    public static MultiFileItemViewModel ToViewModel(this MultiFileItem item, ICommonDataService dataService) => new(item, dataService);
    public static CommonItemViewModel ToViewModel(this CommonItem item, ICommonDataService dataService) => item switch
    {
        TextItem textItem => new TextItemViewModel(textItem, dataService),
        ImageItem imageItem => new ImageItemViewModel(imageItem, dataService),
        MultiFileItem fileItem => new MultiFileItemViewModel(fileItem, dataService),
        FolderItem folderItem => new FolderItemViewModel(folderItem, dataService),
        _ => throw new NotImplementedException()
    };

    public static CommonItemViewModel ToViewModelAutoSave(this CommonItem item, ICommonDataService dataService)
    {
        var vm = item.ToViewModel(dataService);
        vm.OpenEdit();
        vm.ObserveSave();
        return vm;
    }
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

    public static string ToJson(this IList<CommonItem> item) => JsonSerializer.Serialize(item, JsonOptions);
    public static string ToJson(this CommonItem item) => JsonSerializer.Serialize(item, JsonOptions);
    public static T? FromJson<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonOptions);
    public static CommonItem? FromJson(string json) => JsonSerializer.Deserialize<CommonItem>(json, JsonOptions);
}


