using System.Text.Json.Serialization;

namespace BetterClip.Core.Config;

[Serializable]
public partial class AllConfig : ObservableObject
{
    public GIToolConfig GIToolConfig { get; set; } = new();

    [ObservableProperty]
    private string? _test;

    /// <summary>
    /// 修改过后需要重启
    /// </summary>
    public string DataFolder { get; } = Global.UserDataPath();


    [JsonIgnore]
    public Action? OnAnyChangedAction { get; set; }

    public void InitEvent()
    {
        PropertyChanged += OnAnyPropertyChanged;
        GIToolConfig.PropertyChanged += OnAnyPropertyChanged;
    }

    public void OnAnyPropertyChanged(object? sender, EventArgs args)
    {
        OnAnyChangedAction?.Invoke();
    }
}

