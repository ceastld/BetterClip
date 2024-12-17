using System.Diagnostics;
using System.Windows.Controls;
using BetterClip.Core.Config;
using BetterClip.Helpers;
using Serilog.Sinks.RichTextBox.Abstraction;

namespace BetterClip.ViewModel.Pages
{
    public partial class LoggingViewModel(IRichTextBox richTextBoxImpl) : ViewModel
    {
        private readonly IRichTextBox richTextBoxImpl = richTextBoxImpl;
        public RichTextBox RichTextBox { get => richTextBoxImpl.RichTextBox; set => richTextBoxImpl.RichTextBox = value; }

        [RelayCommand]
        private void OnOpenCurrentLog() => CommonHelper.OpenFileOrUrl(LogFilePath);
        public string LogFilePath => Global.GetLogFiles().FirstOrDefault()!;
    }
}
