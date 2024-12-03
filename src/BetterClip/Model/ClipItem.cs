

using Wpf.Ui.Controls;

namespace BetterClip.Model
{
    public partial class ClipItem : ObservableObject
    {
        public ClipItem(string title, string text, string des)
        {
            _title = title;
            _text = text;
            _description = des;
        }

        public ClipItem() { }

        [ObservableProperty]
        private string? _title;

        [ObservableProperty]
        private string? _text;

        [ObservableProperty]
        public string? _description;
    }
}
