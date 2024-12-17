using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using System.Collections.ObjectModel;

namespace BetterClip.ViewModel.Common;

public partial class MultiFileItemViewModel(MultiFileItem item, ICommonDataService dataService) : CommonItemViewModel(item, dataService)
{
    private readonly MultiFileItem _item = item;
    [ObservableProperty]
    private ObservableCollection<FileItemViewModel> _files = new(item.Files.Select(x => new FileItemViewModel(x)));

    protected override void OnSave()
    {
        base.OnSave();
        _item.Files = Files.Select(x => x.FilePath).ToList();
    }
}
