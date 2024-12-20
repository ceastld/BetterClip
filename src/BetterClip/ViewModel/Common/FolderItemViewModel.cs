using System.Collections.ObjectModel;
using System.Reactive.Linq;
using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using DynamicData;
using Microsoft.Extensions.Logging;

namespace BetterClip.ViewModel.Common;

public partial class FolderItemViewModel(FolderItem item, ICommonDataService dataService) : CommonItemViewModel(item, dataService)
{
    private readonly FolderItem _item = item;
    private ObservableCollection<CommonItemViewModel>? _children;
    public ObservableCollection<CommonItemViewModel> Children => _children ??= CreateChildren();

    public override void Refresh()
    {
        _children = CreateChildren();
        OnPropertyChanged(nameof(Children));
    }

    private ObservableCollection<CommonItemViewModel> CreateChildren()
    {
        _logger.LogInformation("{0}", _dataService.ViewModels);
        var vms = _dataService.SelectViewModels(_item.Children).ToList();
        vms.ForEach(vm => vm.Parent = Item.Id);
        var children = new ObservableCollection<CommonItemViewModel>(vms);
        children.CollectionChanged += (s, e) =>
        {
            e.NewItems?.Cast<CommonItemViewModel>()
            .ToList().ForEach(c => c.Parent = Item.Id);
            NeedSave.Value = true;
        };
        return children;
    }

    [ObservableProperty]
    private bool _isExpanded = item.IsExpanded;

    private static ILogger _logger = App.GetLogger<FolderItemViewModel>();
    protected override void OnSave()
    {
        base.OnSave();
        _item.IsExpanded = IsExpanded;
        _item.Children = Children.Select(c => c.Item.Id).ToList();
        _logger.LogInformation("FolderItemViewModel OnSave Children Count {0}", Children.Count);
    }
    //protected override IDisposable InternalObserveSave(Action<IEnumerable<CommonItem>> save, int ms = 200)
    //{
    //    var d1 = base.InternalObserveSave(save, ms);
    //    var d2 = Children.ToObservableChangeSet()
    //        .Where(c => c.Adds > 0 || c.Replaced > 0)
    //        .Buffer(TimeSpan.FromMilliseconds(ms))
    //        .FlattenBufferResult()
    //        .Subscribe(c => save(c.SelectMany(c => c.Range).Select(x => x.Item)));
    //    return new CompositeDisposable(d1, d2);
    //}
}