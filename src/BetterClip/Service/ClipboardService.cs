using System.Reactive.Linq;
using System.Reactive.Subjects;
using BetterClip.Service.Interface;
using BetterClip.Win32;
using Wpf.Ui;

namespace BetterClip.Service;

public partial class ClipboardService : ObservableObject, IClipboardService
{
    private readonly ISnackbarService Snackbar;

    private readonly ClipboardWatcher Watcher;

    private readonly Subject<Action> HandleChange;
    public IObservable<ClipboardChangedEventArgs> Changed { get; }

    public ClipboardService(ISnackbarService snackbarService)
    {
        Snackbar = snackbarService;
        Watcher = new();

        Changed = Observable
            .FromEventPattern<ClipboardChangedEventHandler, ClipboardChangedEventArgs>(
            h => Watcher.ClipboardChanged += h,
            h => Watcher.ClipboardChanged -= h)
            .Select(x => x.EventArgs)
            .Where(args => !_isPaused && !_isHandle)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOnDispatcher()
            .Publish()
            .RefCount();

        HandleChange = new();
        HandleChange.Throttle(TimeSpan.FromMilliseconds(100))
                           .Subscribe(action => action.Invoke());

    }

    private bool _isHandle = false;
    [ObservableProperty]
    private bool _isPaused = false;
    public void Handle()
    {
        _isHandle = true;
        HandleChange.OnNext(() => _isHandle = false);
    }
    public void MonitorOn() => IsPaused = false;
    public void MonitorOff() => IsPaused = true;
}
