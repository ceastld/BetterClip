using BetterClip.ViewModel.Pages;
using Wpf.Ui.Controls;

namespace BetterClip.View.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            
            InitializeComponent();
        }
    }
}
