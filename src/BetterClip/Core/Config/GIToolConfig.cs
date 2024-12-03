
using System.Collections.ObjectModel;

namespace BetterClip.Core.Config;

/// <summary>
/// 原神工具配置
/// </summary>
[Serializable]
public partial class GIToolConfig : ObservableObject
{

    [ObservableProperty]
    private ObservableCollection<GIStartConfig> _gIStartConfigs = [];

    public void NotifyPropertyChanged(string name) => OnPropertyChanged(name);
}

