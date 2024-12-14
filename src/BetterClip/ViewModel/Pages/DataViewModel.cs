using System.Diagnostics;
using BetterClip.Model;
using BetterClip.Service.Interface;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace BetterClip.ViewModel.Pages
{

    public partial class DataViewModel(IMetadataService metadataService, ISnackbarService snackbarService) : ViewModel
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private IEnumerable<Item>? _avatars;

        [ObservableProperty]
        private Item? _selectedAvatar;

        public override void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Avatars = metadataService.GetAvatars()
                .Select(x => x.ToItem())
                .ToList();
            _isInitialized = true;
        }

        [RelayCommand]
        private void OnOpenStrategy(string page)
        {
            if (SelectedAvatar is Item avatar)
            {
                OpenAvatarPage(avatar, page);
            }
            else
            {
                Show("INFO", "请先选择一个角色");
            }
        }

        private void OpenAvatarPage(Item avatar, string page = "")
        {
            var url = $@"https://wiki.biligame.com/ys/{avatar.Name}/攻略";
            if (!string.IsNullOrEmpty(page)) url += "#" + page;
            TryOpenFileOrUrl(url);
        }

        [RelayCommand]
        private void OnOpenGameBanana()
        {
            if (SelectedAvatar is Item item)
            {
                var avatar = metadataService.GetAvatar(item.Name);
                var url = $"https://gamebanana.com/search?_sOrder=best_match&_idGameRow=8552&_sSearchString={avatar.NameEN}";
                TryOpenFileOrUrl(url);
            }
        }

        public void TryOpenFileOrUrl(string cmd, string args = "")
        {
            try
            {
                Process.Start(new ProcessStartInfo(cmd, args) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                Show("ERROR", $"TryOpenFileOrUrl Error: {e.Message}"); 
            }
        }
        private void Show(string title, string message)
        {
            snackbarService.Show(title, message, ControlAppearance.Secondary, new SymbolIcon(SymbolRegular.Fluent24), TimeSpan.FromSeconds(2));
        }
    }
}
