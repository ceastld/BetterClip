using System.Windows.Controls;
using BetterClip.Model.Monaco;
using BetterClip.ViewModel.Editor;
using DependencyPropertyGenerator;

namespace BetterClip.View.Editor
{
    /// <summary>
    /// MonacoEditor.xaml 的交互逻辑
    /// </summary>
    [DependencyProperty("Text", typeof(string))]
    [DependencyProperty("OriginalText", typeof(string))]
    [DependencyProperty("ModifiedText", typeof(string))]
    [DependencyProperty("Language", typeof(MonacoLanguage))]
    [DependencyProperty("Theme", typeof(string))]
    public partial class MonacoEditor : UserControl
    {
        public MonacoEditorViewModel ViewModel { get; init; }

        public MonacoEditor(MonacoEditorViewModel vm)
        {
            ViewModel = vm;
            DataContext = this;
            InitializeComponent();

            SetBinding(TextProperty, nameof(ViewModel.Text));
            SetBinding(OriginalTextProperty, nameof(ViewModel.OriginalText));
            SetBinding(ModifiedTextProperty, nameof(ViewModel.ModifiedText));
            SetBinding(LanguageProperty, nameof(ViewModel.Language));

            ViewModel.SetWebView(WebView);
        }
        partial void OnTextChanged(string newValue)
        {
            ViewModel.Text = newValue;
        }
        partial void OnOriginalTextChanged(string newValue)
        {
            ViewModel.OriginalText = newValue;
            //property 是想双向绑定，看起来很垃圾。。。
        }
        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Text):
                    Text = ViewModel.Text;
                    break;
                case nameof(ViewModel.OriginalText):
                    OriginalText = ViewModel.OriginalText;
                    break;
                case nameof(ViewModel.ModifiedText):
                    ModifiedText = ViewModel.ModifiedText;
                    break;
                case nameof(ViewModel.Language):
                    Language = ViewModel.Language;
                    break;
                default:
                    break;
            }
        }
    }
}
