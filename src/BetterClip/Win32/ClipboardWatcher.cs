using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace BetterClip.Win32;

public class ClipboardChangedEventArgs(IntPtr foregroundWindow)
{
    public nint ForegroundWindow { get; } = foregroundWindow;
}

public delegate void ClipboardChangedEventHandler(object sender, ClipboardChangedEventArgs e);

public class ClipboardWatcher
{
    private static readonly nint WndProcSuccess = nint.Zero;

    public event ClipboardChangedEventHandler? ClipboardChanged;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddClipboardFormatListener(nint hwnd);

    private readonly HwndSource _hwndSource;

    public ClipboardWatcher()
    {
        // 创建隐藏窗口
        var parameters = new HwndSourceParameters("ClipboardManager")
        {
            Width = 0,
            Height = 0,
            PositionX = 0,
            PositionY = 0,
            ParentWindow = IntPtr.Zero,
            WindowStyle = unchecked((int)(0x80000000 | 0x10000000)), // WS_POPUP | WS_VISIBLE
            ExtendedWindowStyle = 0x00000080, // WS_EX_TOOLWINDOW
            HwndSourceHook = WndProc
        };

        _hwndSource = new HwndSource(parameters);
        _hwndSource.Disposed += (sender, args) => _hwndSource.RemoveHook(WndProc);

        if (!AddClipboardFormatListener(_hwndSource.Handle))
        {
            throw new InvalidOperationException("Failed to add clipboard format listener.");
        }
    }

    private void OnClipboardChanged()
    {
        ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(GetForegroundWindow()));
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetForegroundWindow();

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        const int WM_CLIPBOARDUPDATE = 0x031D; // 剪贴板更新消息
        if (msg == WM_CLIPBOARDUPDATE)
        {
            OnClipboardChanged();
            handled = true;
        }
        return WndProcSuccess;
    }

    public void Dispose()
    {
        _hwndSource.Dispose();
    }
}

