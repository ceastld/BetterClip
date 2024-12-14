using BetterClip.Win32;

namespace BetterClip.Service.Interface;

public interface IClipboardService
{
    /// <summary>
    /// 调用后一段时间内，停止监听剪贴板
    /// </summary>
    void Handle();
    void MonitorOn();
    void MonitorOff();
    public IObservable<ClipboardChangedEventArgs> Changed { get; }
}
