using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace BetterClip.Extension
{
    public static class ReactiveExtension
    {
        public static IObservable<PropertyChangedEventArgs> ObserveChange(this ObservableObject obj)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => obj.PropertyChanged += h,
                h => obj.PropertyChanged -= h)
                .Select(c => c.EventArgs);
        }

    }
}
