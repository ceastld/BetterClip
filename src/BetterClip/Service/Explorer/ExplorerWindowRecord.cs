using SHDocVw;

namespace BetterClip.Service.Explorer;

public class ExplorerWindowRecord
{
    public ExplorerWindowRecord(IntPtr hwnd, InternetExplorer ie)
    {
        HWND = hwnd;
        Window = ie;
    }
    public IntPtr HWND { get; set; }
    public InternetExplorer Window { get; set; }
    public DateTime LastUseTime { get; set; } = DateTime.Now;
}