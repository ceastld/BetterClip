using System.Reflection;
using BetterClip.Core.Config;
using BetterClip.Helpers;
using BetterClip.Service;
using Wpf.Ui.Controls;

namespace BetterClip.ViewModel.Pages
{
    public partial class SettingsViewModel(ConfigService configService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private string _rootDataPath = String.Empty;

        public GlobalConfig GlobalConfig { get; } = configService.GlobalConfig;

        [RelayCommand]
        private void OpenUserDataPath()
        {
            CommonHelper.OpenFileOrUrl(configService.UserDataPath());
        }

        [RelayCommand]
        public async Task OnSelectUserDataPath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Description = "选择 UserData 文件夹的路径",
                ShowNewFolderButton = true,
                SelectedPath = configService.UserDataPath()
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await UpdateUserDataPath(dialog.SelectedPath);
                //App.GetService<ISnackbarService>().Info("重启应用后生效", 5);
            }
        }

        [RelayCommand]
        public async Task OnUseDefaultUserDataPath() => await UpdateUserDataPath("");

        public async Task UpdateUserDataPath(string path)
        {
            configService.UpdateUserDataPath(path);
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "提示消息",
                Content = "您已选择新的 UserData 文件夹路径。重启应用后更改将生效，是否立即重启？",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonAppearance = ControlAppearance.Info,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消"
            };
            var res = await uiMessageBox.ShowDialogAsync();
            if (res == Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                App.ReStart();
            }
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            AppVersion = $"BetterClip - {GetAssemblyVersion()}";
            _isInitialized = true;
        }

        private static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        }
    }
}
