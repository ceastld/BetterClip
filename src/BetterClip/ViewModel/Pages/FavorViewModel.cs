using System.Reactive.Linq;
using BetterClip.Helpers;
using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using BetterClip.ViewModel.Common;
using DynamicData;
using Microsoft.Extensions.Logging;

namespace BetterClip.ViewModel.Pages;
using static CommonItemHelper;

public partial class FavorViewModel : ViewModel
{
    [ObservableProperty]
    private FolderItemViewModel? _root;
    public FavorViewModel(IFavorDataService favorDataService, IClipDataService clipDataService)
    {
        this.favorDataService = favorDataService;
        this.clipDataService = clipDataService;
        OnRefresh();

        favorDataService.ViewModels.WatchValue("")
            .Where(x => x != null)
            .Subscribe(x =>
            {
                Root = (FolderItemViewModel?)x;
                _logger.LogInformation(x.Item.ToJson());
            });
    }

    private static ILogger _logger = App.GetLogger<FavorViewModel>();

    [ObservableProperty]
    private CommonItemViewModel? _selectedItem;
    private readonly IFavorDataService favorDataService;
    private readonly IClipDataService clipDataService;

    #region MonitorClipboard
    private IDisposable? _monitor;

    [ObservableProperty]
    private bool _isMonitorClipboard;

    partial void OnIsMonitorClipboardChanged(bool value)
    {
        if (value) OnOpenMonitorClipboard();
        else OnCloseMonitorClipboard();
    }

    [RelayCommand]
    private void OnOpenMonitorClipboard()
    {
        _monitor ??= clipDataService.ClipboardChanged
                .Subscribe(c =>
                {
                    _logger.LogInformation("Recive ClipboardData");
                    var item = c.CloneToDataService(clipDataService, favorDataService);
                    Root?.Children.Insert(0, favorDataService.GetViewModel(item));
                });
    }

    [RelayCommand]
    private void OnCloseMonitorClipboard()
    {
        _monitor?.Dispose();
        _monitor = null;
    }
    #endregion


    [RelayCommand]
    private void OnDeleteSelected()
    {
        if (SelectedItem == null || SelectedItem.Parent == null) return;
        if (SelectedItem == null) return;
        var parent = favorDataService.FindParent(SelectedItem);
        parent?.Children.Remove(SelectedItem!);
    }

    [RelayCommand]
    private void OnRefresh() => Root?.Refresh();

    [RelayCommand]
    private void OnOpenDataFolder() => CommonHelper.OpenFileOrUrl(favorDataService.DataFolder);

    [RelayCommand]
    private void OnPaste()
    {
        var item = favorDataService.CreateFromClipboard();
        if (item == null) return;
        favorDataService.Save(item);
        Root?.Children.Insert(0, favorDataService.GetViewModel(item));
    }

    [RelayCommand]
    private void OnAddTestItem()
    {
        var data = new List<CommonItem>()
        {
            CreateText("示例文本1", title: "标题1"),
            CreateText("示例文本2", title: "标题2"),
        };
        favorDataService.Save(data);

        var child = new List<CommonItem>()
        {
            CreateText("子文本1", title: "子标题1"),
            CreateText("子文本2", title: "子标题2"),
        };
        favorDataService.Save(child);

        var folder = CreateFolder(child.Select(x => x.Id), "收藏夹");
        folder.IsExpanded = true;
        favorDataService.Save(folder);

        data.Add(folder);

        Root?.Children.AddRange(favorDataService.SelectViewModels(data));
    }
}
