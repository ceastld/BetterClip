using BetterClip.Model;
using Wpf.Ui.Controls;

namespace BetterClip.ViewModel.Pages
{
    public partial class ClipViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private IEnumerable<ClipItem>? _clipitems;
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            // Initialize the view model here.
            var ClipCollection = new List<ClipItem>();
            for (int i = 0; i < 10; i++)
            {
                ClipCollection.Add(new ClipItem
                {
                    Title = $"Title {i}",
                    Text = $"Text {i}",
                });
            }

            Clipitems = ClipCollection;
            _isInitialized = true;
        }

        public void OnNavigatedFrom() { }
    }
}
