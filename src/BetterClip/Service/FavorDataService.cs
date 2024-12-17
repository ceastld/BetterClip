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
