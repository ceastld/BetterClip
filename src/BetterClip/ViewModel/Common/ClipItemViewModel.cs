using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;

namespace BetterClip.ViewModel.Common;

public partial class ClipItemViewModel(CommonItem item) : ObservableObject
{
    private readonly CommonItem _item = item;
    public CommonItem Item => _item;

    [ObservableProperty]
    private string? _title = item.Title;
    [ObservableProperty]
    private string? _description = item.Description;
    public virtual string VisualTitle => Title ?? "Untitled";
    public virtual string? VisualContent => Description;

    [ObservableProperty]
    public bool _needSave = false;
    public void OpenEdit() => PropertyChanged += ClipItemViewModel_PropertyChanged;
    public void CloseEdit() => PropertyChanged -= ClipItemViewModel_PropertyChanged;
    private void ClipItemViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        NeedSave = true;
    }

    protected static IClipDataService ClipDataService => App.GetService<IClipDataService>();

    public virtual void OnSave()
    {
        _item.Title = Title;
        _item.Description = Description;
    }
}

public partial class ImageItemViewModel(ImageItem item) : ClipItemViewModel(item)
{
    private readonly ImageItem _item = item;
    public string Path => ClipDataService.GetImagePath(_item.Path);
}

public partial class TextItemViewModel(TextItem item) : ClipItemViewModel(item)
{
    private readonly TextItem _item = item;

    [ObservableProperty]
    private string _text = item.Text;
    public override string? VisualContent => base.VisualContent ?? Text;
    public override void OnSave()
    {
        base.OnSave();
        _item.Text = Text;
    }
}