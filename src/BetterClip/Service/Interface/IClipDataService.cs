using System.Reactive.Subjects;
using BetterClip.Model.ClipData;
using BetterClip.ViewModel.Common;
using DynamicData;

namespace BetterClip.Service.Interface
{
    public interface ICommonDataService
    {
        string DataFolder { get; }
        IObservableCache<CommonItem, string> All { get; }
        CommonItem? GetItem(string key);
        void Save(CommonItem item);
        void Save(IEnumerable<CommonItem> item);
        void Remove(CommonItem item);
        void Remove(IEnumerable<CommonItem> item);
        IObservableCache<CommonItemViewModel, string> ViewModels { get; }
        IEnumerable<CommonItemViewModel> SelectViewModels(IEnumerable<CommonItem> items);
        IEnumerable<CommonItemViewModel> SelectViewModels(IEnumerable<string> items);
        CommonItemViewModel GetViewModel(string key);
        CommonItemViewModel GetViewModel(CommonItem key);
        CommonItem? CreateFromClipboard();
        string GetImagePath(string path);
    }

    public interface IClipDataService: ICommonDataService
    {
        Subject<CommonItem> ClipboardChanged { get; }
    }

    public interface IFavorDataService: ICommonDataService
    {
        FolderItemViewModel? FindParent(CommonItemViewModel selectedItem);
        FolderItem GetRoot();
    }
}
