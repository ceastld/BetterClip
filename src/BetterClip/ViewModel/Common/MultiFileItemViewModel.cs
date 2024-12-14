using BetterClip.Model.ClipData;
using System.Collections.ObjectModel;

namespace BetterClip.ViewModel.Common;

public partial class MultiFileItemViewModel(MultiFileItem item) : ClipItemViewModel(item)
{
    private readonly MultiFileItem _item = item;
    [ObservableProperty]
    private ObservableCollection<FileItemViewModel> _files = new(item.Files.Select(x => new FileItemViewModel(x)));

    public override void OnSave()
    {
        base.OnSave();
        _item.Files = Files.Select(x => x.FilePath).ToList();
    }
}
