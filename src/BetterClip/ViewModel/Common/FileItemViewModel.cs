using System.IO;

namespace BetterClip.ViewModel.Common;

public partial class FileItemViewModel(string path) : ViewModel
{
    public string FilePath => Path.GetFullPath(path);
}