using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using BetterClip.ViewModel.Common;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MenuItem = System.Windows.Controls.MenuItem;
using static BetterClip.Core.Menu.MenuFactory;
using System.Collections;
using BetterClip.Extension;
using System.Windows.Media.Imaging;
using DynamicData;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using System.Reactive.Linq;
using BetterClip.Helpers;

namespace BetterClip.ViewModel.Pages;

public partial class ClipViewModel : ViewModel, IDisposable
{
    [ObservableProperty]
    private ReadOnlyObservableCollection<CommonItemViewModel> _clipitems;

    [ObservableProperty]
    private bool _notify = true;

    [ObservableProperty]
    private IList? _selectedItems;

    [ObservableProperty]
    private CommonItemViewModel? _selectedItem;

    [ObservableProperty]
    private int _selectedIndex = -1;

    [ObservableProperty]
    private BitmapSource? _testImage;

    private readonly IClipDataService ClipDataService;
    private readonly ISnackbarService SnackbarService;
    public SearchHints SearchHints { get; }

    public ClipViewModel(IClipDataService clipDataService,
                         SearchHints searchHints,
                         ISnackbarService snackbarService)
    {
        ClipDataService = clipDataService;
        SnackbarService = snackbarService;
        SearchHints = searchHints;

        var filter = SearchHints.WhenValueChanged(t => t.SearchText)
            .Select(BuildFilter!);

        _cleanUp = clipDataService.ViewModels.Connect()
            //.BatchIf(this.WhenValueChanged(x=>x.SearchText))
            .Filter(filter)
            .SortAndBind(out _clipitems, SortExpressionComparer<CommonItemViewModel>.Descending(item => item.Item.UpdateTime))
            .Subscribe();
    }

    private readonly IDisposable _cleanUp;

    private Func<CommonItemViewModel, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText)) return item => true;

        return item => item.Item.Filter(searchText);
    }

    [RelayCommand]
    private void EditSelected(object sender)
    {
        if(SelectedItem is TextItemViewModel textItem)
        {
            textItem.Text = CommonHelper.EditText(textItem.Text, sender);
            textItem.SaveIfNeed();
        }
    }

    [RelayCommand]
    private void OnTest()
    {

        var data = CommonItemHelper.GetTestData();
        for (int i = 0; i < 0; i++)
        {
            data.AddRange(CommonItemHelper.GetTestData());
        }
        ClipDataService.Save(data);
    }

    [RelayCommand]
    private void OnSave()
    {
        int count = 0;
        foreach (var item in Clipitems)
        {
            if (item.SaveIfNeed()) count++;
        }
    }

    [RelayCommand]
    private void OnSaveSelected()
    {
        var data = GetSelectedItems();
        if (data.Count == 0) return;
        foreach (var item in data)
        {
            ClipDataService.Save(item.Item);
        }
    }

    private List<CommonItemViewModel> GetSelectedItems() => SelectedItems?.Cast<CommonItemViewModel>().ToList() ?? [];

    [RelayCommand]
    private void OnDeleteSelected()
    {
        var data = GetSelectedItems();
        if (data.Count == 0) return;
        var idx = SelectedIndex;
        ClipDataService.Remove(data.Select(x => x.Item));
        SelectedIndex = idx.Limit(0, Clipitems.Count - 1);
    }

    [RelayCommand]
    private void OnCopySelected()
    {
        SnackbarService.Info(SelectedItem?.Item.ToJson() ?? "");
    }

    [RelayCommand]
    private void OnPaste()
    {
        var item = ClipDataService.CreateFromClipboard();
        if (item != null)
        {
            ClipDataService.Save(item);
        }
    }

    [RelayCommand]
    private void OnOpenDataFolder() => CommonHelper.OpenFileOrUrl(ClipDataService.DataFolder);

    public IEnumerable<MenuItem> GenerateMenuItem()
    {
        yield return CreateMenuItem(SymbolRegular.Save16, "Save", OnSaveSelected);
        yield return CreateMenuItem(SymbolRegular.Copy16, "Copy", OnCopySelected);
        yield return CreateMenuItem(SymbolRegular.ClipboardPaste16, "Paste", OnPaste);
        yield return CreateMenuItem(SymbolRegular.Delete16, "Delete", OnDeleteSelected);
        yield break;
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
        GC.SuppressFinalize(this);
    }
}
