using BetterClip.ViewModel.Common;
using BetterClip.ViewModel.Pages;
using ReactiveUI;
using Wpf.Ui.Controls;

namespace BetterClip.View.Pages
{
    /// <summary>
    /// FavorPage.xaml 的交互逻辑
    /// </summary>
    public partial class FavorPage : INavigableView<FavorViewModel>
    {
        public FavorViewModel ViewModel { get; }
        public FavorPage(FavorViewModel vm)
        {
            ViewModel = vm;
            DataContext = this;

            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = e.NewValue as CommonItemViewModel;
            
        }
    }
}
