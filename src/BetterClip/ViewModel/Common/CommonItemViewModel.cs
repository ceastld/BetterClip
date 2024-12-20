using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BetterClip.Extension;
using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace BetterClip.ViewModel.Common;

public partial class CommonItemViewModel : ObservableObject
{
    private readonly CommonItem _item;
    public CommonItem Item => _item;

    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _description;
    protected readonly ICommonDataService _dataService;

    public virtual string VisualTitle => Title ?? "Untitled";
    public virtual string? VisualContent => Description;

    private static ILogger _logger = App.GetLogger<CommonItemViewModel>();
    public string? Parent { get; set; }

    public ReactiveProperty<bool> NeedSave { get; } = new ReactiveProperty<bool>(false);
    public CommonItemViewModel(CommonItem item, ICommonDataService dataService)
    {
        _item = item;
        _title = item.Title;
        _description = item.Description;
        _dataService = dataService;
    }
    public virtual void Refresh() { }
    //private IDisposable? _editSubscription;
    //public void OpenEdit() =>
    //    _editSubscription = this.ObserveChange()
    //        .Where(x => x.PropertyName != nameof(NeedSave))
    //        .Subscribe(_ => NeedSave = true);

    private IDisposable? _edit;
    public virtual IDisposable OpenEdit()
    {
        return _edit ??= this.ObserveChange()
            .Subscribe(e =>
            {
                NeedSave.Value = true;
                _logger.LogInformation("PropertyChanged {0}, Type {1}", e.PropertyName, GetType().Name);
            });
    }

    private IDisposable? _save;
    public virtual IDisposable ObserveSave(int ms = 50)
    {
        return _save ??= NeedSave.Where(x => x)
            .Throttle(TimeSpan.FromMilliseconds(ms))
            .Subscribe(c => SaveIfNeed());
    }

    public bool SaveIfNeed()
    {
        if (!NeedSave.Value) return false;
        OnSave();
        _dataService.Save(Item);
        _logger.LogInformation("Saved item with ID: {0}, Title: {1}", _item.Id, _item.Title);
        NeedSave.Value = false;
        return true;
    }

    protected static IClipDataService ClipDataService => App.GetService<IClipDataService>();

    protected virtual void OnSave()
    {
        _item.UpdateTime = DateTime.Now;
        _item.Title = Title;
        _item.Description = Description;
    }
}
