using BetterClip.Model.ClipData;
using BetterClip.Service.Interface;
using static BetterClip.Model.ClipData.CommonItemHelper;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using BetterClip.ViewModel.Common;
using ReactiveUI;

namespace BetterClip.Service;

public partial class ClipDataService : CommonDataService, IClipDataService
{
    private readonly Subject<Action> _saveOrderSubject = new();
    public Subject<CommonItem> ClipboardChanged { get; }

    public override IObservableCache<CommonItemViewModel, string> ViewModels { get; }
    public ClipDataService(IClipboardService clipboardService, ConfigService config) : base("Clip", config)
    {
        var saveorder = _saveOrderSubject.Throttle(TimeSpan.FromMilliseconds(200))
                                         .Subscribe(action => action.Invoke());
        
        ClipboardChanged = new Subject<CommonItem>();

        ViewModels = Source.Connect()
                           .Transform(c => c.ToViewModelAutoSave(this))
                           .AsObservableCache();

        var subclipchange = clipboardService.Changed
            .ObserveOnDispatcher()
            .Subscribe(e =>
            {
                var item = CreateFromClipboard();
                if (item != null)
                {
                    Save(item);
                    ClipboardChanged.OnNext(item);
                }
            });
    }
}
