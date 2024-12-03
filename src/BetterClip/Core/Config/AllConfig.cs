using System.Text.Json.Serialization;

namespace BetterClip.Core.Config;

[Serializable]
public partial class AllConfig : ObservableObject
{
    public GIToolConfig GIToolConfig { get; set; } = new();

    [ObservableProperty]
    private string? _test;

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

