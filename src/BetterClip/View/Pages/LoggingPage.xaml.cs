using System.Windows.Controls;
using System.Windows.Documents;
using BetterClip.ViewModel.Pages;
using Serilog.Sinks.RichTextBox.Abstraction;

namespace BetterClip.View.Pages
{
    /// <summary>
    /// LoggingPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoggingPage : Page
    {
        public LoggingViewModel ViewModel { get; }
        public LoggingPage(LoggingViewModel vm)
        {
            ViewModel = vm;
            DataContext = this;
            
            Loaded += (s, e) => vm.RichTextBox = LogTextBox;
            InitializeComponent();
            LogTextBox.TextChanged += LogTextBoxTextChanged;
        }

        private void LogTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textRange = new TextRange(LogTextBox.Document.ContentStart, LogTextBox.Document.ContentEnd);
            if (textRange.Text.Length > 10000)
            {
                LogTextBox.Document.Blocks.Clear();
            }

            LogTextBox.ScrollToEnd();
        }
    }
}
