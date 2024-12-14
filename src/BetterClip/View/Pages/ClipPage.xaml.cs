using BetterClip.Core.Menu;
using BetterClip.ViewModel.Pages;
using Wpf.Ui.Controls;

namespace BetterClip.View.Pages
{
    /// <summary>
    /// ClipPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClipPage : INavigableView<ClipViewModel>
    {
        public ClipViewModel ViewModel { get; }

        public ClipPage(ClipViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void TheListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.SelectedItems = TheListView.SelectedItems;
        }

        private void TheListView_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TheListView.ContextMenu.ResetChildren(ViewModel.GenerateMenuItem());
            TheListView.ContextMenu.IsOpen = true;
        }
    }
}
