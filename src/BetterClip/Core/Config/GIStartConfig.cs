namespace BetterClip.Core.Config;

public partial class GIStartConfig : ObservableObject
{
    public GIStartConfig() { }

    [ObservableProperty]
    private string _genshinImpactPath = string.Empty;

    [ObservableProperty]
    private string _migotoPath = string.Empty;

    public static GIStartConfig Default => new()
    {
        GenshinImpactPath = @"D:\Genshin Impact\Genshin Impact Game\YuanShen.exe",
        MigotoPath = @"D:\game\Genshin\3dmigoto\3DMigoto Loader.exe"
    };
}

