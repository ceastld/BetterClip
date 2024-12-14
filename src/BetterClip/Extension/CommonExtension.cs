using System.Reactive.Linq;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace BetterClip.Extension
{
    internal static class CommonExtension
    {
        public static void RunOnDispatcher(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public static void Info(this ISnackbarService snackbarService, string info, int time = 1)
        {
            RunOnDispatcher(() =>
            {
                var icon = new SymbolIcon(SymbolRegular.Info24);
                snackbarService.Show("Info", info, icon, timeout: TimeSpan.FromSeconds(time));
            });
        }

        public static void Error(this ISnackbarService snackbarService, string info, int time = 3)
        {
            RunOnDispatcher(() =>
            {
                var icon = new SymbolIcon(SymbolRegular.ErrorCircle24);
                snackbarService.Show("Error", info, icon, timeout: TimeSpan.FromSeconds(time));
            });
        }
        public static IObservable<TSource> ObserveOnDispatcher<TSource>(this IObservable<TSource> source)
        {
            return source.ObserveOn(SynchronizationContext.Current!); // UI 线程上运行
        }
        public static IObservable<TSource> SubscribeOnDispatcher<TSource>(this IObservable<TSource> source)
        {
            return source.SubscribeOn(SynchronizationContext.Current!);
        }
    }
}
