using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using BetterClip.ViewModel.Common;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace BetterClip.Service;

public partial class FavorDataService : CommonDataService, IFavorDataService
{
    public FavorDataService(ConfigService config) : base("Favor", config)
    {
        GetRoot(); //创建根节点

        ViewModels = Source.Connect()
            .Transform(c => c.ToViewModelAutoSave(this))
            .AsObservableCache();

        // item update 过后，viewmodel 也被刷新，暂时不知道怎么处理
        // 这里是想实现，文件更新后导致 item 的更新，然后 viewmodel 也被刷新
        // 所以还是直接刷新 root 节点就解决了。。。
        //_cache.UpdateFromFile.Subscribe(item =>
        //{
        //    var op = ViewModels.Lookup(item.Id);
        //    if (op.HasValue)
        //    {
        //        var vm = op.Value;
        //        if (vm.Parent != null)
        //        {
        //            FindParent(vm)?.Refresh();
        //        }
        //    }
        //});
    }
    public override IObservableCache<CommonItemViewModel, string> ViewModels { get; }
    public FolderItemViewModel? FindParent(CommonItemViewModel item)
    {
        if (item.Parent == null)
        {
            var parent = ViewModels.Items.Where(x => x.Item is FolderItem)
                .Select(x => (FolderItemViewModel)x)
                .Where(x => x.Children.Contains(item))
                .FirstOrDefault();
            item.Parent = parent?.Item.Id;
            return parent;
        }
        return (FolderItemViewModel?)ViewModels.Lookup(item.Parent).Value;
    }
    public FolderItem GetRoot()
    {
        var root = GetItem("");
        if (root == null)
        {
            root = CommonItemHelper.CreateFolder([]);
            root.Id = "";
            Save(root);
        }
        return (FolderItem)root;
    }

}
