using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;

namespace BetterClip.ViewModel.Common;

public partial class ImageItemViewModel(ImageItem item, ICommonDataService dataService) : CommonItemViewModel(item, dataService)
{
    private readonly ImageItem _item = item;
    public string Path => _dataService.GetImagePath(_item.Path);
}
