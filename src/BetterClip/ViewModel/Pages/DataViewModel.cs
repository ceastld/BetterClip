using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Media;
using BetterClip.Core.Config;
using BetterClip.Model;
using BetterClip.Model.Metadata;
using BetterClip.Service.Interface;
using Wpf.Ui.Controls;

namespace BetterClip.ViewModel.Pages
{

    public partial class DataViewModel(IMetadataService metadataService) : ViewModel
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
    }
}
