namespace BetterClip.ViewModel.Pages
{
    public partial class GIToolsViewModel : ViewModel
    {
        private bool _isInitialized = false;

        public override void OnNavigatedTo()
        {
            if(!_isInitialized)
                InitializeViewModel();

        }
        private void InitializeViewModel()
        {

        }

        [RelayCommand]
        private void OnOpenStrategy(string name)
        {

        }

        private void Open3dmigoto(string gi_path)
        {
            string new_line = @"target = D:\game\hoyo\Genshin Impact game\GenshinImpact.exe";

        }
    }
}
