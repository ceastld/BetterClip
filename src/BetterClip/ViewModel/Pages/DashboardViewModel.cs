using System.Collections.ObjectModel;
using BetterClip.Model;
using BetterClip.Model.Metadata;
using BetterClip.Service.Interface;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace BetterClip.ViewModel.Pages
{
    public partial class DashboardViewModel(IMetadataService metadataService, ISnackbarService snackbarService) : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }

        private bool _isInitialized = false;

        [ObservableProperty]
        private ObservableCollection<Item> _avatars = [];

        [ObservableProperty]
        private ObservableCollection<Item> _selectedAvatars = [];

        [ObservableProperty]
        private int _maxAvatarCount = 8;

        [ObservableProperty]
        private int _avatarViewColumns = 4;

        private readonly Random random = new();

        [RelayCommand]
        private void OnChoseAvatar()
        {
            if (SelectedAvatars.Count >= MaxAvatarCount)
            {
                snackbarService.Show("提示消息", $"当前最多选择 {MaxAvatarCount} 个角色，已经选了 {SelectedAvatars.Count} 个",
                    TimeSpan.FromSeconds(2));
                return;
            }
            var selected_index = random.Next(Avatars.Count);
            if (!SelectedAvatars.Contains(Avatars[selected_index]))
                SelectedAvatars.Add(Avatars[selected_index]);
            Avatars.RemoveAt(selected_index);
        }

        [RelayCommand]
        private void OnRefreshAvatars()
        {
            InitializeViewModel();
            SelectedAvatars = [];
        }

        [RelayCommand]
        private void OnUnChoseAvatar()
        {
            if (SelectedAvatars.Count == 0)
                return;
            var idx = SelectedAvatars.Count - 1;
            Avatars.Add(SelectedAvatars[idx]);
            SelectedAvatars.RemoveAt(idx);
        }

        [RelayCommand]
        private void OnChose10()
        {
            AvatarViewColumns = 5;
            MaxAvatarCount = 10;
            QuickChose();
        }

        [RelayCommand]
        private void OnChose8()
        {
            AvatarViewColumns = 4;
            MaxAvatarCount = 8;
            QuickChose();
        }

        private void QuickChose()
        {
            if (SelectedAvatars.Count > 0) OnRefreshAvatars();
            for (var i = 0; i < MaxAvatarCount; i++)
            {
                var selected_index = random.Next(Avatars.Count);
                SelectedAvatars.Add(Avatars[selected_index]);
                Avatars.RemoveAt(selected_index);
            }
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Avatars = new ObservableCollection<Item>(metadataService.GetAvatars().Select(x => x.ToItem()));
            _isInitialized = true;
        }

        public void OnNavigatedFrom() { }
    }
}
