using BetterClip.ViewModel.Editor;

namespace BetterClip.View.Windows;

public partial class MonacoWindow
{

    public MonacoEditorViewModel ViewModel { get; init; }

    public MonacoWindow(MonacoEditorViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        ViewModel.SetWebView(WebView);
    }
}
