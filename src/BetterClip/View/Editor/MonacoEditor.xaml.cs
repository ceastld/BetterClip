using System.Windows.Controls;
using BetterClip.ViewModel.Editor;

namespace BetterClip.View.Editor
{
    /// <summary>
    /// MonacoEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MonacoEditor : UserControl
    {
        public MonacoEditorViewModel ViewModel { get; init; }

        public MonacoEditor(MonacoEditorViewModel vm)
        {
            ViewModel = vm;
            DataContext = this;

            InitializeComponent();

            ViewModel.SetWebView(WebView);
        }
    }
}
