using System.Diagnostics;
using System.Windows.Controls;
using BetterClip.Core.Config;
using BetterClip.Helpers;
using BetterClip.Service.Explorer;
using Serilog.Sinks.RichTextBox.Abstraction;

namespace BetterClip.ViewModel.Pages
{
    public partial class LoggingViewModel(IRichTextBox richTextBoxImpl, IExplorerService explorer) : ViewModel
    {
        private readonly IRichTextBox richTextBoxImpl = richTextBoxImpl;
        public RichTextBox RichTextBox { get => richTextBoxImpl.RichTextBox; set => richTextBoxImpl.RichTextBox = value; }

        [RelayCommand]
        private void OnOpenCurrentLog() => CommonHelper.OpenFileOrUrl(LogFilePath);
        public string LogFilePath => Global.GetLogFiles().FirstOrDefault()!;

        [RelayCommand]
        private void OnOpenLastClosedFolder()
        {
            var folder = explorer.GetRecords().FirstOrDefault();
            if (folder is not null)
                CommonHelper.OpenFileOrUrl(folder);
        }
    }
}
