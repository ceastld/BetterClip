using BetterClip.Service.Interface;
using DynamicData;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace BetterClip.ViewModel.Common;

public partial class SearchHints : ObservableObject, IDisposable
{
    private readonly ReadOnlyObservableCollection<string> _hints;

    private readonly IDisposable _cleanUp;

    [ObservableProperty]
    private string? _searchText;

    public SearchHints(IClipDataService clipDataService)
    {
        //build a predicate when SearchText changes
        var filter = this.WhenValueChanged(t => t.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .Select(BuildFilter!);

        //share the connection
        var shared = clipDataService.All.Connect().Publish();
        //distinct observable
        var title = shared.DistinctValues(item => item.Title ?? "");
        var description = shared.DistinctValues(item => item.Description ?? "");

        var loader = title.Or(description)
            .Filter(filter)     //filter strings
            .ObserveOn(RxApp.MainThreadScheduler)
            .SortAndBind(out _hints, SortExpressionComparer<string>.Ascending(str => str))
            .Subscribe();

        _cleanUp = new CompositeDisposable(loader, shared.Connect());
    }

    private Func<string, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText)) return item => true;
        return str => str.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    public ReadOnlyObservableCollection<string> Hints => _hints;

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}
