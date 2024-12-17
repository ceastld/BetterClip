using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;

namespace BetterClip.ViewModel.Common;

public partial class TextItemViewModel(TextItem item, ICommonDataService dataService) : CommonItemViewModel(item, dataService)
{
    private readonly TextItem _item = item;

    [ObservableProperty]
    private string _text = item.Text;
    public override string? VisualContent => base.VisualContent ?? Text;
    protected override void OnSave()
    {
        base.OnSave();
        _item.Text = Text;
    }
}
