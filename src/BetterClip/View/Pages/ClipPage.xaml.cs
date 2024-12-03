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
        public void Test()
        {
            var vm = App.GetService<ClipViewModel>();
            vm.OnNavigatedTo();
        }
    }
}
