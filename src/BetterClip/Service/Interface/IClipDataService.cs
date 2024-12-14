using BetterClip.Helpers;
using BetterClip.Model.ClipData;
using DynamicData;

namespace BetterClip.Service.Interface
{
    public interface IClipDataService
    {
        IObservableCache<CommonItem, string> All { get; }
        void Save(CommonItem item);
        void Save(IEnumerable<CommonItem> item);
        void Remove(CommonItem item);
        void Remove(IEnumerable<CommonItem> item);
        string GetImagePath(string path);
        CommonItem? CreateFromClipboard();
    }
}
